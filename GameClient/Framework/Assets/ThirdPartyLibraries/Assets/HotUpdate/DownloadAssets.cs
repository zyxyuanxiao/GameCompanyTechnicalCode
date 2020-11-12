using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
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
        protected struct DownloadABInfo
        {
            public string abName;
            public bool downloadFinished;
        }
        
        private static Queue<DownloadABInfo> downloadQueue = new Queue<DownloadABInfo>(300);

        private static FileStream downloadFileStream;
        
        public int Progress { get; set; }
        public IEnumerator Work()
        {
            yield return AssetsConfig.OneFrame;
            Progress = 1;
            yield return DownloadVersionConfig();
            Progress = 15;
            yield return DownloadAB();
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
            //查找配置表中的所有不存在.MD5 值不匹配的 ab 包,然后记录下来
            foreach (var remoteItem in remoteVersionConfig.ABInfos)
            {
                var localItem = AssetsConfig.VersionConfig.ABInfos;
                if (!localItem.TryGetValue(remoteItem.Key,out AB_V_MD5 localABVMd5))
                {
                    downloadQueue.Enqueue(new DownloadABInfo()
                    {
                        abName = remoteItem.Key,
                        downloadFinished = false,
                    });//本地没查到这个 AB,需要添加进下载队列
                }
                //本地和远程都查找到了,但是远程的版本大于本地的版本,并且 MD5 值不同,需要添加进下载队列
                if (remoteItem.Value.Version.ToInt() > localABVMd5.Version.ToInt() && 
                    !remoteItem.Value.Md5Hash.Equals(localABVMd5.Md5Hash))
                {
                    downloadQueue.Enqueue(new DownloadABInfo()
                    {
                        abName = remoteItem.Key,
                        downloadFinished = false,
                    });
                }
            }
            yield return AssetsConfig.OneFrame;
        }
        
        /// <summary>
        /// 通过下载队列下载 AB 包
        /// </summary>
        /// <returns></returns>
        public IEnumerator DownloadAB()
        {
            if (downloadQueue.Count <= 0)yield break;//没有下载列表
            
            AssetsNotification.Broadcast(IAssetsNotificationType.BeginDownloadAB,
                "开始下载所有 AB 包");
            
            while (downloadQueue.Count <= 0)//循环下载队列,下载一个移除一个
            {
                Progress = Progress + (int)(downloadQueue.Count/85);
                DownloadABInfo info = downloadQueue.Peek();
                string url = AssetsConfig.QueryRemoteABURL(info.abName);
                string path = AssetsConfig.QueryDownloadABPath(info.abName);
                File.Delete(path);//在这一步先进行删除,然后再下载
                downloadFileStream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write);
                BestHttpHelper.Download(url,DownloadAB);
                while (!info.downloadFinished)
                {
                    yield return AssetsConfig.OneFrame;
                }
            }
            
            AssetsNotification.Broadcast(IAssetsNotificationType.DownloadABSucceed,
                "下载所有 AB 包成功");
            yield return AssetsConfig.OneFrame;
        }
        
        private void DownloadAB(bool isError, byte[] data, int dataLength)
        {
            if (-100 == dataLength && null == data)
            {
                DownloadABInfo info = downloadQueue.Peek();
                //下载完毕,并且正常
                downloadQueue.Dequeue();
                downloadFileStream.Flush();
                downloadFileStream.Close();
                info.downloadFinished = true;
            }
            else if (isError && -200 == dataLength && null == data)
            {
                DownloadABInfo info = downloadQueue.Peek();
                //下载过程中报错,需要删除之后,再重新添加进下载队列里面,再次重新下载
                downloadQueue.Dequeue();
                downloadQueue.Enqueue(info);
                downloadFileStream.Flush();
                downloadFileStream.Close();
                File.Delete(AssetsConfig.QueryDownloadABPath(info.abName));
                
                AssetsNotification.Broadcast(IAssetsNotificationType.DownloadABFailed,
                    "下载 AB 包失败一次" + info.abName);
                
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