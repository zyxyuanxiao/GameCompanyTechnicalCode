using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using BestHTTP;
using Common;
using LitJson;
using UnityEngine;

namespace GameAssets
{
    /// <summary>
    /// 第一步 先下载远程配置文件 VersionConfig,
    /// 第二步 对比当前的配置文件与远程配置文件,那些包的 MD5 更新了,或者哪些包不存在,则传入下载队列.
    /// 循环下载队列,当下载完一个资源,将下载的这个资源生成MD5,再与远程配置文件的 MD5 对比,对比正常,则下载正确,否则重新传入下载队列.
    /// 每次下载正确时,将数据添加进本地的 VersionConfig 对象里面
    /// 第三步 全部下载结束时,将VersionConfig对象的数据,全部重新写入文件
    /// </summary>
    public sealed class DownloadAssets : IBusiness
    {
        internal class DownloadFileInfo
        {
            public string fileName;//存入的有 AB 包的名字,也有压缩包的名字
            public bool downloadFinished;
            public File_V_MD5 remoteFileVMd5;//记录远程的 File_V_MD5,当下载一个完毕之后,赋值给本地的版本配置文件对象
        }
        
        private FileStream downloadFileStream;

        private DownloadFileInfo downloadFileInfo;
        
        private Queue<DownloadFileInfo> downloadQueue = new Queue<DownloadFileInfo>(300);

        private VersionConfig remoteVersionConfig = null;//是否从网络上下载了配置文件
        
        public int Progress { get; set; }
        public IEnumerator Work()
        {
            yield return AssetsHelper.OneFrame;
            remoteVersionConfig = null;
            Progress = 1;
            yield return DownloadVersionConfig();
            Progress = 15;
            yield return AssetsHelper.OneFrame;
            yield return DownloadFiles();
            Progress = 95;
            yield return AssetsHelper.OneFrame;
            AssetsHelper.WriteVersionConfigToFile();
            Progress = 100;
            AssetsNotification.Broadcast(IAssetsNotificationType.Info,
                "<color=cyan>========================>DownloadAssets 结束<========================</color>");
        }
        
        /// <summary>
        /// 下载 资源服务器上面的版本配置文件
        /// </summary>
        /// <returns></returns>
        private IEnumerator DownloadVersionConfig()
        {
            AssetsNotification.Broadcast(IAssetsNotificationType.BeginRequestVersionConfig,
                "开始下载");
            yield return AssetsHelper.OneFrame;
            
            //目前使用的是本地的 URL,具体到项目上面部署的情况下,再重新编写此类
            string versionConfigURL = AssetsHelper.QueryDownloadFileURL(AssetsHelper.VersionConfigName);
            BestHttpHelper.GET(versionConfigURL, (b, s) =>
            {
                if (b && !string.IsNullOrEmpty(s))//请求成功,应该进行
                {
                    //资源服务器上面的版本配置文件
                    remoteVersionConfig = JsonMapper.ToObject<VersionConfig>(s);
                    AssetsNotification.Broadcast(IAssetsNotificationType.RequestVersionConfigSucceed,
                        "下载 VersionConfig.json 成功");
                }
                else
                {
                    
                    Debug.LogError("资源服务器版本配置文件下载失败");
                    AssetsNotification.Broadcast(IAssetsNotificationType.RequestVersionConfigFailed,
                        "下载 VersionConfig.json 失败");
                }
            });
            
            while (null == remoteVersionConfig)
            {
                yield return AssetsHelper.OneFrame;
            }
            Progress = 10;

            if (!AssetsHelper.VersionConfig.OS.Equals(remoteVersionConfig.OS) || 
                AssetsHelper.VersionConfig.SVNVersion.ToInt() > remoteVersionConfig.SVNVersion.ToInt()||
                AssetsHelper.VersionConfig.AppVersion.ToFloat() > remoteVersionConfig.AppVersion.ToFloat())
            {
                Debug.Log("会出现这个情况的原因,是因为本地没有正常的VersionConfig文件");
                yield break;//下载配置文件自身的平台,版本号,游戏二进制号 与本身的平台不匹配,不大于的情况下不给下载
            }
            else//有更新时,先将本地配置文件的版本数据重置
            {
                AssetsHelper.VersionConfig.SVNVersion = remoteVersionConfig.SVNVersion;
                AssetsHelper.VersionConfig.AppVersion = remoteVersionConfig.AppVersion;
            }
            
            //查找配置表中的所有不存在与 MD5 值不匹配的 ab/zip 文件,然后记录下来
            foreach (var remoteItem in remoteVersionConfig.FileInfos)
            {
                if (!AssetsHelper.VersionConfig.FileInfos.TryGetValue(remoteItem.Key,out File_V_MD5 localABVMd5))
                {
                    downloadQueue.Enqueue(new DownloadFileInfo()
                    {
                        fileName = remoteItem.Key,
                        downloadFinished = false,
                        remoteFileVMd5 = remoteItem.Value,
                    });//本地没查到这个 AB,需要添加进下载队列
                }
                else
                {
                    //本地和远程都查找到了,但是远程的版本大于本地的版本,并且 MD5 值不同,需要添加进下载队列
                    if (remoteItem.Value.Version.ToInt() > localABVMd5.Version.ToInt() || 
                        !remoteItem.Value.MD5Hash.Equals(localABVMd5.MD5Hash))
                    {
                        downloadQueue.Enqueue(new DownloadFileInfo()
                        {
                            fileName = remoteItem.Key,
                            downloadFinished = false,
                            remoteFileVMd5 = remoteItem.Value,
                        });
                    }   
                }
            }
            yield return AssetsHelper.OneFrame;
        }

        /// <summary>
        /// 通过下载队列下载 AB 包
        /// </summary>
        /// <returns></returns>
        private IEnumerator DownloadFiles()
        {
            if (downloadQueue.Count <= 0)
            {
                AssetsNotification.Broadcast(IAssetsNotificationType.Info,
                    "没有 AB 包可下载");
                yield break;//没有下载列表
            }
            
            AssetsNotification.Broadcast(IAssetsNotificationType.BeginDownloadFile,
                "开始下载所有 AB 包");
            
            while (downloadQueue.Count > 0)//循环下载队列,下载一个移除一个
            {
                yield return AssetsHelper.OneFrame;
                Progress = Progress + (int)(downloadQueue.Count/80);
                downloadFileInfo = downloadQueue.Peek();//这个值是复制了一份内存,使用Dequeue与其得到的不是同一个对象
                string url = AssetsHelper.QueryDownloadFileURL(downloadFileInfo.fileName);
                string path = AssetsHelper.QueryDownloadFilePath(downloadFileInfo.fileName);
                AssetsHelper.FileDelete(path);//在这一步先进行删除,然后再下载
                downloadFileStream = new FileStream(AssetsHelper.CSharpFilePath(path),
                    FileMode.OpenOrCreate, FileAccess.Write);
                BestHttpHelper.Download(url,DownloadFiles);
                
                while (!downloadFileInfo.downloadFinished)
                {
                    yield return AssetsHelper.OneFrame;
                }
            }
            
            AssetsNotification.Broadcast(IAssetsNotificationType.DownloadFileSucceed,
                "下载所有 AB 包成功");
            yield return AssetsHelper.OneFrame;
        }
        
        private void DownloadFiles(bool isError, byte[] data, int dataLength)
        {
            if (-100 == dataLength && null == data)
            {
                //下载完毕,并且正常
                downloadFileStream.Flush();
                downloadFileStream.Close();
                
                //下载正常之后,需要对这个包进行 MD5 校验
                downloadFileInfo = downloadQueue.Dequeue();
                string path = AssetsHelper.QueryDownloadFilePath(downloadFileInfo.fileName);
                string destPath = AssetsHelper.QueryLocalFilePath(downloadFileInfo.fileName);
                AssetsHelper.FileMove(path,destPath);
                if (Path.GetExtension(destPath).ToLower().Contains("zip"))//如果是 zip 则需要解压
                {
                    AssetsHelper.DecompressBinary(destPath);//解压
                }
                
                //重新填充配置文件
                AssetsHelper.VersionConfig.FileInfos[downloadFileInfo.fileName] = downloadFileInfo.remoteFileVMd5;
                FileInfo fileInfo = new FileInfo(AssetsHelper.CSharpFilePath(destPath));
                AssetsHelper.FileInfoConfigs[downloadFileInfo.fileName] = new FileInfoConfig()
                {
                    MD5Hash = downloadFileInfo.remoteFileVMd5.MD5Hash,
                    Length = fileInfo.Length,
                    LastWriteTime = fileInfo.LastWriteTime.ToString(),
                    Name = downloadFileInfo.fileName
                };
                Debug.Log("下载成功了:" + downloadFileInfo.fileName);
                AssetsHelper.WriteVersionConfigToFile();
                AssetsHelper.WriteFileInfoConfigsToFile();
                downloadFileInfo.downloadFinished = true;
            }
            else if (isError && -200 == dataLength && null == data)
            {
                downloadFileInfo = downloadQueue.Peek();
                //下载过程中报错,需要删除之后,再重新添加进下载队列里面,再次重新下载
                downloadQueue.Dequeue();
                downloadQueue.Enqueue(downloadFileInfo);
                downloadFileStream.Flush();
                downloadFileStream.Close();
                AssetsHelper.FileDelete(AssetsHelper.QueryDownloadFilePath(downloadFileInfo.fileName));
                
                AssetsNotification.Broadcast(IAssetsNotificationType.DownloadFileFailed,
                    "下载 AB 包失败一次:  " + downloadFileInfo.fileName);
                
                downloadFileInfo.downloadFinished = true;
            }
            else if (dataLength > 0 && null != data)
            {
                //正在下载的数据,将数据存入下载的文件中
                if (downloadFileStream.CanWrite)
                {
                    downloadFileStream.Write(data, 0, data.Length);
                }
            }
        }
    }
}