using System.Collections;
using System.IO;
using System.Text;
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
            IAssetsNotificationType t = IAssetsNotificationType.BeginReadConfig;
            JsonData jsonData = new JsonData();
            jsonData["message"] = "开始读取 VersionConfig.json 并转成 VersionConfig 对象";
            AssetsNotification.Broadcast(t,jsonData.ToJson());
            yield return AssetConfig.OneFrame;
            Progress = 1;
            //判断一下配置文件是否已经从网络上面下载到本地了,
            //如果没有从网络上面下载到本地,则需要先从 StreamingAssets 路径下进行读取
            //如果下载到本地了,则需要从persistentDataPath路径下进行读取
            string vcPath = AssetConfig.VersionConfigPersistentDataPath;
            if (!File.Exists(AssetConfig.VersionConfigPersistentDataPath))
            {
                vcPath = AssetConfig.VersionConfigStreamingAssetsPath;
            }
            UnityWebRequest unityWebRequest = UnityWebRequest.Get(vcPath);
            yield return unityWebRequest.SendWebRequest();
            if (unityWebRequest.isHttpError||unityWebRequest.isNetworkError)
            {
                jsonData["message"] = "读取 " + vcPath + " 失败了  ";
                t = IAssetsNotificationType.ReadConfigFailed;
            }
            else
            {
                jsonData["message"] = "读取 " + vcPath + " 成功了";
                AssetConfig.VersionConfig = JsonMapper.ToObject<VersionConfig>(unityWebRequest.downloadHandler.text);
                t = IAssetsNotificationType.ReadConfigSucceed;
            }
            AssetsNotification.Broadcast(t,jsonData.ToJson());
            Progress = 100;
            yield return AssetConfig.OneFrame;
        }
    }
}