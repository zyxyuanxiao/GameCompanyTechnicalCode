using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Common;
using UnityEngine;
using UnityEngine.Networking;

namespace DLCAssets
{
    public sealed class CheckAllFiles : IBusiness
    {
        private string zipPath = string.Empty;
        public int Progress { get; set; }
        public IEnumerator Work()
        {
            if (AssetsHelper.FileInfoConfigs==null)//第一次安装 app 是没有这个对象的
            {
                AssetsHelper.FileInfoConfigs = new Dictionary<string, FileInfoConfig>();
            }

            Progress = 0;
            yield return AssetsHelper.OneFrame;
            yield return WriteToLocal();
            Progress = 25;
            yield return AssetsHelper.OneFrame;
            yield return DecompressZip();
            Progress = 50;
            yield return AssetsHelper.OneFrame;
            yield return ComparisonConfig();
            Progress = 75;
            yield return AssetsHelper.OneFrame;
            yield return WriteConfig();
            Progress = 100;
            AssetsNotification.Broadcast(IAssetsNotificationType.Info,
                "<color=cyan>========================>CheckAllFile 结束<========================</color>");
        }

        /// <summary>
        /// 将所有资源从 streamingAssetsPath 路径里面拷贝到沙盒空间里面
        /// </summary>
        /// <returns></returns>
        private IEnumerator WriteToLocal()
        {
            AssetsNotification.Broadcast(IAssetsNotificationType.BeginCopyToLocal,
                "如果是第一次安装 APP,就把数据拷贝到沙盒空间下 ");
            foreach (var item in AssetsHelper.VersionConfig.FileInfos)
            {
                string path = AssetsHelper.QueryLocalFilePath(item.Key);
                if (Path.GetExtension(path).ToLower().Contains("zip")) //如果是 zip 则需要解压
                {
                    zipPath = path;
                }

                if (AssetsHelper.FileExists(path)) continue;

                //移动 AB 包文件
                UnityWebRequest unityWebRequest = UnityWebRequest.Get(AssetsHelper.QueryStreamingFilePath(item.Key));
                yield return unityWebRequest.SendWebRequest();
                if (unityWebRequest.isHttpError || unityWebRequest.isNetworkError)
                {
                    AssetsNotification.Broadcast(IAssetsNotificationType.CopyToLocalFailed,
                        "拷贝 " + path + " 失败了  说明此文件不在随包资源里面,需要从网络上面下载");
                }
                else
                {
                    //将整个文件写入到沙盒空间下
                    path = AssetsHelper.CSharpFilePath(path);
                    File.WriteAllBytes(path, unityWebRequest.downloadHandler.data);
                    FileInfo fileInfo = new FileInfo(path);
                    //从streamingAssetsPath路径拷贝到本地沙盒空间路径的,肯定不会出问题,直接写入到文件信息配置里面即可
                    AssetsHelper.FileInfoConfigs[item.Key] = new FileInfoConfig()
                    {
                        MD5Hash = item.Value.MD5Hash,
                        Length = fileInfo.Length,
                        LastWriteTime = fileInfo.LastWriteTime.ToString(),
                        Name = item.Key
                    };
                }
            }
            AssetsNotification.Broadcast(IAssetsNotificationType.BeginCopyToLocal,
                "如果是第一次安装 APP,拷贝成功 ");
            yield return AssetsHelper.OneFrame;
        }
        
        
        /// <summary>
        /// 不管怎么样,都将二进制压缩进行解压
        /// </summary>
        /// <returns></returns>
        private IEnumerator DecompressZip()
        {
            try
            {
                //每次都将压缩文件解压一遍,防止
                AssetsHelper.DecompressBinary(zipPath);
                AssetsNotification.Broadcast(IAssetsNotificationType.UnZipFilesSucceed,
                    "解压缩文件成功");
            }
            catch (Exception e)
            {
                AssetsNotification.Broadcast(IAssetsNotificationType.UnZipFilesSucceed,
                    "解压缩文件失败. " + e.ToString());
            }

            yield return AssetsHelper.OneFrame;
        }
        
        
        /// <summary>
        /// 将版本配置文件与文件信息配置文件进行对比
        /// 根据版本配置文件,将文件信息配置文件中多余的进行删除
        /// 如果版本配置文件中有,但是本地没有这个文件,则 2 个配置文件都要进行删除操作
        /// 如果本地文件与文件信息配置文件的长度最后一次写入不匹配,则 2 个配置文件都要进行删除,并且本地文件删除
        /// 如果2 个配置文件中的 MD5 hash 值不一致,则 2 个配置文件都要进行删除,并且本地文件删除
        /// 删除之后,版本配置文件与网络配置文件相匹配,将需要更新的,与本地有问题的文件重新从服务器上面进行下载
        /// 以服务器上的文件为准,本地文件可以人为修改考虑,做出如下操作
        /// </summary>
        /// <returns></returns>
        private IEnumerator ComparisonConfig()
        {
            //将文件信息拿出来进行对比,如果信息有误,则直接删除配置文件中保存的文件,否则跳过
            //将这整理好的配置文件,与远程配置文件对比,直接进行下载覆盖,这个原理是以服务器上的资源为准,本地资源会被认为修改而坐的
            
            List<string> names = new List<string>(AssetsHelper.FileInfoConfigs.Keys);
            foreach (string name in names)
            {
                if (!AssetsHelper.VersionConfig.FileInfos.TryGetValue(name, out File_V_MD5 fileVMd5))
                {
                    AssetsHelper.FileInfoConfigs.Remove(name);//根据配置版本文件进行删除 ,2 个文件保持高度一致性
                }
                else
                {
                    if (!AssetsHelper.FileExists(AssetsHelper.QueryLocalFilePath(name)))
                    {
                        //文件不存在沙盒空间内,也需要删除,保持高度一致性
                        AssetsHelper.FileInfoConfigs.Remove(name);
                        AssetsHelper.VersionConfig.FileInfos.Remove(name);
                        continue;
                    }
                    
                    FileInfoConfig fic = AssetsHelper.FileInfoConfigs[name];
                    string filePath = AssetsHelper.CSharpFilePath(AssetsHelper.QueryLocalFilePath(name));
                    FileInfo fileInfo = new FileInfo(filePath);
                    if (fic.MD5Hash != fileVMd5.MD5Hash || 
                        fileInfo.LastWriteTime.Equals(fic.LastWriteTime)||
                        fileInfo.Length != fic.Length)//需要删除,然后重新下载
                    {
                        AssetsHelper.FileDelete(AssetsHelper.QueryLocalFilePath(name));
                        AssetsHelper.VersionConfig.FileInfos.Remove(name);//当文件信息与配置文件中的东西没有匹配成功,就需要删除,重新下载
                        AssetsHelper.VersionConfig.FileInfos.Remove(name);
                    }
                }
            }
            AssetsNotification.Broadcast(IAssetsNotificationType.Info,
                "2 个配置文件对比完毕");
            yield return AssetsHelper.OneFrame;
        }
        
        private IEnumerator WriteConfig()
        {
            //沙盒空间不存在这个文件,则生成一个文件,下次从沙盒空间里面去拿
            //下载之后的版本配置文件,也是和这个配置文件进行对比
            if (!AssetsHelper.FileExists(AssetsHelper.QueryLocalFilePath(AssetsHelper.VersionConfigName)))
            {
                AssetsHelper.WriteVersionConfigToFile();
            }
            //必定保存一次文件信息的数据
            AssetsHelper.WriteFileInfoConfigsToFile();
            AssetsNotification.Broadcast(IAssetsNotificationType.Info,
                "将 2 个配置文件写入到本地");
            yield return AssetsHelper.OneFrame;
        }
        
    }
}