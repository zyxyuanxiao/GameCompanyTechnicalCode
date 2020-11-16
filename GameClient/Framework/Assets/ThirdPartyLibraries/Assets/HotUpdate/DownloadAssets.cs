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
    public class DownloadAssets : IBusiness
    {
        protected struct DownloadFileInfo
        {
            public string fileName;//存入的有 AB 包的名字,也有压缩包的名字
            public bool downloadFinished;
            public File_V_MD5 remoteFileVMd5;//记录远程的 File_V_MD5,当下载一个完毕之后,赋值给本地的版本配置文件对象
        }
        
        private static Queue<DownloadFileInfo> downloadQueue = new Queue<DownloadFileInfo>(300);

        private static FileStream downloadFileStream;
        
        private static VersionConfig remoteVersionConfig = null;//是否从网络上下载了配置文件

        public int Progress { get; set; }
        public IEnumerator Work()
        {
            yield return AssetsConfig.OneFrame;
            remoteVersionConfig = null;
            Progress = 1;
            yield return DownloadVersionConfig();
            Progress = 15;
            yield return DownloadFiles();
            Progress = 95;
            yield return AssetsConfig.OneFrame;
            AssetsConfig.WriteVersionConfigToFile();
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
            yield return AssetsConfig.OneFrame;
   
            //目前使用的是本地的 URL,具体到项目上面部署的情况下,再重新编写此类
            string versionConfigURL = AssetsConfig.QueryDownloadFileURL(AssetsConfig.VersionConfigName);
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
                yield return AssetsConfig.OneFrame;
            }
            Progress = 10;

            if (!AssetsConfig.VersionConfig.OS.Equals(remoteVersionConfig.OS) || 
                AssetsConfig.VersionConfig.SVNVersion.ToInt() < remoteVersionConfig.SVNVersion.ToInt()||
                AssetsConfig.VersionConfig.AppVersion.ToFloat() < remoteVersionConfig.AppVersion.ToFloat())
            {
                Debug.Log("会出现这个情况的原因,是因为本地没有正常的VersionConfig文件");
                yield break;//下载配置文件自身的平台,版本号,游戏二进制号 与本身的平台不匹配,不大于的情况下不给下载
            }
            else//有更新时,先将本地配置文件的版本数据重置
            {
                AssetsConfig.VersionConfig.SVNVersion = remoteVersionConfig.SVNVersion;
                AssetsConfig.VersionConfig.AppVersion = remoteVersionConfig.AppVersion;
            }
            
            //查找配置表中的所有不存在与 MD5 值不匹配的 ab/zip 文件,然后记录下来
            foreach (var remoteItem in remoteVersionConfig.FileInfos)
            {
                if (!AssetsConfig.VersionConfig.FileInfos.TryGetValue(remoteItem.Key,out File_V_MD5 localABVMd5))
                {
                    downloadQueue.Enqueue(new DownloadFileInfo()
                    {
                        fileName = remoteItem.Key,
                        downloadFinished = false,
                        remoteFileVMd5 = remoteItem.Value,
                    });//本地没查到这个 AB,需要添加进下载队列
                }
                //本地和远程都查找到了,但是远程的版本大于本地的版本,并且 MD5 值不同,需要添加进下载队列
                if (remoteItem.Value.Version.ToInt() > localABVMd5.Version.ToInt() && 
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
            yield return AssetsConfig.OneFrame;
        }

        /// <summary>
        /// 通过下载队列下载 AB 包
        /// </summary>
        /// <returns></returns>
        private IEnumerator DownloadFiles()
        {
            if (downloadQueue.Count <= 0)yield break;//没有下载列表
            
            AssetsNotification.Broadcast(IAssetsNotificationType.BeginDownloadFile,
                "开始下载所有 AB 包");
            
            while (downloadQueue.Count <= 0)//循环下载队列,下载一个移除一个
            {
                yield return AssetsConfig.OneFrame;
                Progress = Progress + (int)(downloadQueue.Count/80);
                DownloadFileInfo info = downloadQueue.Peek();
                string url = AssetsConfig.QueryDownloadFileURL(info.fileName);
                string path = AssetsConfig.QueryDownloadFilePath(info.fileName);
                AssetsConfig.FileDelete(path);//在这一步先进行删除,然后再下载
                downloadFileStream = new FileStream(AssetsConfig.CSharpFilePath(path),
                    FileMode.OpenOrCreate, FileAccess.Write);
                BestHttpHelper.Download(url,DownloadFiles);
                while (!info.downloadFinished)
                {
                    yield return AssetsConfig.OneFrame;
                }
            }
            
            AssetsNotification.Broadcast(IAssetsNotificationType.DownloadFileSucceed,
                "下载所有 AB 包成功");
            yield return AssetsConfig.OneFrame;
        }
        
        private void DownloadFiles(bool isError, byte[] data, int dataLength)
        {
            if (-100 == dataLength && null == data)
            {
                //下载完毕,并且正常
                downloadFileStream.Flush();
                downloadFileStream.Close();
                //下载正常之后,需要对这个包进行 MD5 校验
                DownloadFileInfo info = downloadQueue.Dequeue();
                string path = AssetsConfig.QueryDownloadFilePath(info.fileName);
                string localMD5 = SecurityTools.GetMD5Hash(path);
                if (SecurityTools.VerifyMd5Hash(localMD5,info.remoteFileVMd5.MD5Hash))//下载的 MD5 值与网络上的 MD5 值正常
                {
                    //将文件从下载路径移动到 AssetBundles 路径下
                    AssetsConfig.FileMove(path,AssetsConfig.QueryLocalFilePath());
                    AssetsConfig.VersionConfig.FileInfos[info.fileName] = info.remoteFileVMd5;
                }
                else
                {
                    Debug.LogError("下载到本地的 MD5 与网络上面的 MD5 不匹配:" + 
                                   info.fileName +"\n" + 
                                   info.remoteFileVMd5 + "\n" + 
                                   localMD5);
                    AssetsNotification.Broadcast(IAssetsNotificationType.DownloadFileFailed,
                        info.fileName + "的MD5 值,下载之后,本地与资源服务器上面的不匹配");
                }
                info.downloadFinished = true;
            }
            else if (isError && -200 == dataLength && null == data)
            {
                DownloadFileInfo info = downloadQueue.Peek();
                //下载过程中报错,需要删除之后,再重新添加进下载队列里面,再次重新下载
                downloadQueue.Dequeue();
                downloadQueue.Enqueue(info);
                downloadFileStream.Flush();
                downloadFileStream.Close();
                AssetsConfig.FileDelete(AssetsConfig.QueryDownloadFilePath(info.fileName));
                
                AssetsNotification.Broadcast(IAssetsNotificationType.DownloadFileFailed,
                    "下载 AB 包失败一次" + info.fileName);
                
                info.downloadFinished = true;
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