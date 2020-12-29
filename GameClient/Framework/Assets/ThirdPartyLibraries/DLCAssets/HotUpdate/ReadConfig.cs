using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Common;
using LitJson;
using UnityEngine;
using UnityEngine.Networking;

namespace GameAssets
{
    public sealed class ReadConfig : IBusiness
    {
        public int Progress { get; set; }
        /// <summary>
        /// 读本地数据生成一个 VersionConfig 对象
        /// </summary>
        public IEnumerator Work()
        {
            Progress = 0;
            yield return AssetsHelper.OneFrame;
            yield return ReadVersionConfig();
            Progress = 50;
            yield return AssetsHelper.OneFrame;
            yield return ReadFileInfoConfig();
            Progress = 100;
            yield return AssetsHelper.OneFrame;
            AssetsNotification.Broadcast(IAssetsNotificationType.Info,
                "<color=cyan>========================>ReadConfig 结束<========================</color>");
        }
        
        
        /// <summary>
        /// 读取配置文件
        /// </summary>
        /// <returns></returns>
        private IEnumerator ReadVersionConfig()
        {
            AssetsNotification.Broadcast(IAssetsNotificationType.BeginReadConfig,
                "开始读取 VersionConfig.json 并转成 VersionConfig 对象");
            
            //判断一下配置文件是否已经从网络上面下载到本地了,
            //如果没有从网络上面下载到本地,则需要先从 StreamingAssets 路径下进行读取
            //如果下载到本地了,则需要从persistentDataPath路径下进行读取
            string vcPath = AssetsHelper.QueryLocalFilePath(AssetsHelper.VersionConfigName);
            if (!AssetsHelper.FileExists(vcPath))
            {
                vcPath = AssetsHelper.QueryStreamingFilePath(AssetsHelper.VersionConfigName);
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
                AssetsHelper.VersionConfig = JsonMapper.ToObject<VersionConfig>(unityWebRequest.downloadHandler.text);
                AssetsNotification.Broadcast(IAssetsNotificationType.ReadConfigSucceed,
                    "读取 " + vcPath + " 成功了  ");
            }
            
            yield return AssetsHelper.OneFrame;
        }


        /// <summary>
        /// 读取文件信息配置文件,每一个文件都有配套的文件信息所对应
        /// </summary>
        /// <returns></returns>
        private IEnumerator ReadFileInfoConfig()
        {
            AssetsNotification.Broadcast(IAssetsNotificationType.BeginReadConfig,
                "开始读取 FileInfoConfig.json 并转成 Dictionary<string, FileInfoConfig> 对象");
            string ficPath = AssetsHelper.QueryLocalFilePath(AssetsHelper.FileInfoConfigName);
            if (!AssetsHelper.FileExists(ficPath)) yield break; //不存在则跳出,说明是第一次
            UnityWebRequest unityWebRequest = UnityWebRequest.Get(ficPath);
            yield return unityWebRequest.SendWebRequest();

            if (unityWebRequest.isHttpError || unityWebRequest.isNetworkError)
            {
                AssetsNotification.Broadcast(IAssetsNotificationType.ReadConfigFailed,
                    "读取 " + ficPath + " 失败了  ");
            }
            else
            {
                AssetsHelper.FileInfoConfigs =
                    JsonMapper.ToObject<Dictionary<string, FileInfoConfig>>(unityWebRequest.downloadHandler.text);
                AssetsNotification.Broadcast(IAssetsNotificationType.ReadConfigSucceed,
                    "读取 " + ficPath + " 成功了  ");
            }
            
            yield return AssetsHelper.OneFrame;
        }
    }
}