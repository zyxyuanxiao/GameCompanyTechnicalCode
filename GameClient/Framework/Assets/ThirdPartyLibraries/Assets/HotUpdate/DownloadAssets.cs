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
        
        public int Progress { get; set; }
        public IEnumerator Work()
        {
            yield return AssetsConfig.OneFrame;
            Progress = 1;
            yield return DownloadVersionConfig();
            Progress = 15;
            yield return DownloadFiles();
            Progress = 95;
            yield return WriteToLocal();
            Progress = 100;
        }
        
        /// <summary>
        /// 下载 资源服务器上面的版本配置文件
        /// </summary>
        /// <returns></returns>
        public IEnumerator DownloadVersionConfig()
        {
            
            AssetsNotification.Broadcast(IAssetsNotificationType.BeginRequestVersionConfig,
                "开始下载 VersionConfig.json 并转成 VersionConfig 对象");
            
            //目前使用的是本地的 URL,具体到项目上面部署的情况下,再重新编写此类
            string versionConfigURL = AssetsConfig.DownloadURL + "/" + AssetsConfig.VersionConfigName;
            VersionConfig remoteVersionConfig = null;//网络请求是否回包
            BestHttpHelper.GET(versionConfigURL,(b,s) =>
            {
                Progress = 10;
                if (b)//请求成功,应该进行
                {
                    Debug.Log("下载版本配置文件成功:" + s);
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
            
            while (remoteVersionConfig == null)
            {
                yield return AssetsConfig.OneFrame;
            }
            //网络请求回包后,进行数据对比
            if (AssetsConfig.VersionConfig.Md5Hash.Equals(remoteVersionConfig.Md5Hash))
            {
                yield break;//配置文件自身的 hash 值相同,则无版本更新
            }

            if (!AssetsConfig.VersionConfig.OS.Equals(remoteVersionConfig.OS) || 
                AssetsConfig.VersionConfig.SVNVersion.ToInt() < remoteVersionConfig.SVNVersion.ToInt()||
                AssetsConfig.VersionConfig.AppVersion.ToInt() < remoteVersionConfig.AppVersion.ToInt())
            {
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
                var localItem = AssetsConfig.VersionConfig.FileInfos;
                if (!localItem.TryGetValue(remoteItem.Key,out File_V_MD5 localABVMd5))
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
                    !remoteItem.Value.Md5Hash.Equals(localABVMd5.Md5Hash))
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
        public IEnumerator DownloadFiles()
        {
            if (downloadQueue.Count <= 0)yield break;//没有下载列表
            
            AssetsNotification.Broadcast(IAssetsNotificationType.BeginDownloadAB,
                "开始下载所有 AB 包");
            
            while (downloadQueue.Count <= 0)//循环下载队列,下载一个移除一个
            {
                yield return AssetsConfig.OneFrame;
                Progress = Progress + (int)(downloadQueue.Count/80);
                DownloadFileInfo info = downloadQueue.Peek();
                string url = AssetsConfig.QueryRemoteABURL(info.fileName);
                string path = AssetsConfig.QueryDownloadABPath(info.fileName);
                if (File.Exists(path)) File.Delete(path);//在这一步先进行删除,然后再下载
                downloadFileStream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write);
                BestHttpHelper.Download(url,DownloadFiles);
                while (!info.downloadFinished)
                {
                    yield return AssetsConfig.OneFrame;
                }
            }
            
            AssetsNotification.Broadcast(IAssetsNotificationType.DownloadABSucceed,
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
                string path = AssetsConfig.QueryDownloadABPath(info.fileName);
                string localMD5 = SecurityTools.GetMD5Hash(path);
                if (SecurityTools.VerifyMd5Hash(localMD5,info.remoteFileVMd5.Md5Hash))
                {
                    AssetsConfig.VersionConfig.FileInfos[info.fileName] = info.remoteFileVMd5;
                }
                else
                {
                    Debug.LogError("下载到本地的 MD5 与网络上面的 MD5 不匹配:" + 
                                   info.fileName +"\n" + 
                                   info.remoteFileVMd5 + "\n" + 
                                   localMD5);
                    AssetsNotification.Broadcast(IAssetsNotificationType.DownloadABSucceed,
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
                File.Delete(AssetsConfig.QueryDownloadABPath(info.fileName));
                
                AssetsNotification.Broadcast(IAssetsNotificationType.DownloadABFailed,
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

        /// <summary>
        /// 将更新过后的配置所有数据写入到本地
        /// </summary>
        /// <returns></returns>
        private IEnumerator WriteToLocal()
        {
            //将所有数据,写入文件中.
            using (FileStream fileStream = new FileStream(AssetsConfig.VersionConfigPersistentDataPath,
                FileMode.Open,FileAccess.Write))
            {
                fileStream.SetLength(0);
                fileStream.Flush();
                byte[] data = Encoding.UTF8.GetBytes(JsonMapper.ToJson(AssetsConfig.VersionConfig));
                fileStream.Write(data,0,data.Length);
                fileStream.Flush();
            }
            yield return AssetsConfig.OneFrame;
        }

    }
}