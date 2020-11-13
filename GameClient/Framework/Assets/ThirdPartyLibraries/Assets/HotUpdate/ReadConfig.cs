using System.Collections;
using System.IO;
using System.Text;
using Common;
using LitJson;
using UnityEngine;
using UnityEngine.Networking;

namespace GameAssets
{
    public class ReadConfig : IBusiness
    {
        public int Progress { get; set; }
        /// <summary>
        /// 读本地数据生成一个 VersionConfig 对象
        /// </summary>
        public IEnumerator Work()
        {
            Progress = 0;

            AssetsNotification.Broadcast(IAssetsNotificationType.BeginReadConfig,
                "开始读取 VersionConfig.json 并转成 VersionConfig 对象");
            
            yield return AssetsConfig.OneFrame;
            Progress = 1;
            //判断一下配置文件是否已经从网络上面下载到本地了,
            //如果没有从网络上面下载到本地,则需要先从 StreamingAssets 路径下进行读取
            //如果下载到本地了,则需要从persistentDataPath路径下进行读取
            string vcPath = AssetsConfig.QueryLocalFilePath(AssetsConfig.VersionConfigName);
            if (!AssetsConfig.FileExists(vcPath))
            {
                vcPath = AssetsConfig.QueryStreamingFilePath(AssetsConfig.VersionConfigName);
            }
            UnityWebRequest unityWebRequest = UnityWebRequest.Get(vcPath);
            yield return unityWebRequest.SendWebRequest();
            if (unityWebRequest.isHttpError||unityWebRequest.isNetworkError)
            {
                AssetsNotification.Broadcast(IAssetsNotificationType.ReadConfigFailed,
                    "读取 " + vcPath + " 失败了  ");
            }
            else
            {
                AssetsConfig.VersionConfig = JsonMapper.ToObject<VersionConfig>(unityWebRequest.downloadHandler.text);
                AssetsNotification.Broadcast(IAssetsNotificationType.ReadConfigSucceed,
                    "读取 " + vcPath + " 成功了  ");
            }
            Progress = 50;
            yield return AssetsConfig.OneFrame;
            
            //检查文件是否改动了.如果改动了需要删除文件夹里面的所有文件,
            string path = AssetsConfig.CSharpFilePath(AssetsConfig.QueryLocalFilePath());
            string s = Tool.QueryAppendDirectoryLastWriteTime(path);
            string sha1 = Tool.QuerySHA1HashOfString(s);
            
            string record = PlayerPrefs.GetString(AssetsConfig.ASSETROOT_LASTWRITETIME_KEY, "");
            if (!sha1.Equals(record)) //资源非法发生修改，清理
            {
                Directory.Delete(path,true);
            }
            
            Progress = 75;
            
            //沙盒空间不存在这个文件,则生成一个文件,下次从沙盒空间里面去拿
            //下载之后的版本配置文件,也是和这个配置文件进行对比
            if (!AssetsConfig.FileExists(AssetsConfig.QueryLocalFilePath(AssetsConfig.VersionConfigName)))
            {
                AssetsConfig.WriteVersionConfigToFile();
            }
            yield return AssetsConfig.OneFrame;
            
            Progress = 100;
            yield return AssetsConfig.OneFrame;
            AssetsNotification.Broadcast(IAssetsNotificationType.Info,
                "<color=cyan>========================>ReadConfig 结束<========================</color>");
        }
    }
}