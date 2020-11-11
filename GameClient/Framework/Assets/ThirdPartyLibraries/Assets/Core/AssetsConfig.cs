using System;
using UnityEngine;

namespace GameAssets
{
    /// <summary>
    /// 路径,后缀,配置,辅助
    /// </summary>
    public class AssetsConfig
    {
        /// <summary>
        /// 一帧
        /// </summary>
        public static WaitForEndOfFrame OneFrame = new WaitForEndOfFrame();
        
        /// <summary>
        /// AssetBundle 的后缀名
        /// </summary>
        public static readonly string Extension = ".unity3d";
        
        /// <summary>
        /// 下载VersionConfig的 URL,下载完毕之后,有一系列数据,包括VersionConfig对象文本,需要下载的资源URL配置
        /// </summary>
        public static string DownloadURL = string.Empty;
        

        #region VersionConfig
                
        /// <summary>
        /// 从本地加载的VersionConfig数据,全局使用这一份即可
        /// </summary>
        public static VersionConfig VersionConfig;
        
        
        /// <summary>
        /// 本次打包的配置版本文件,
        ///
        /// 编辑器下运行,路径是:Assets/StreamingAssets/VersionConfig.json,
        ///
        /// 如果此文件随整包打出去,是需要解压到 APP 下的 Application.persistentDataPath 文件夹下,然后再进行读取
        /// 如果是热更包,是跟随热更情况,先下载此热更配置文件,然后根据热更配置文件进行下载 AB 包
        /// 
        /// </summary>
        public static readonly string VersionConfigName = "VersionConfig.json";

        //在 streamingAssetsPath 路径下,随包的版本文件
        public static readonly string VersionConfigStreamingAssetsPath =
            LOCAL_PATH_TO_URL(Application.streamingAssetsPath.Replace("\\","/") + "/VersionConfig.json");
        //在 persistentDataPath 路径下,由网络下载后的文件
        public static readonly string VersionConfigPersistentDataPath =
            LOCAL_PATH_TO_URL(Application.persistentDataPath.Replace("\\","/") + "/VersionConfig.json");
        
        public static string LOCAL_PATH_TO_URL(string path)
        {
            //#if (UNITY_ANDROID && !UNITY_EDITOR) || (UNITY_EDITOR_WIN && !UNITY_ANDROID)//未来win上测试android ab出错检查这里
            //        return path;
            //#else
            //        return "file:///" + path;
            //#endif
            string url =
#if UNITY_ANDROID && !UNITY_EDITOR
        path;
#elif UNITY_IPHONE && !UNITY_EDITOR
        "file://" + path;
#elif UNITY_STANDALONE||UNITY_EDITOR
                "file:///" + path;
#else
        string.Empty;
#endif
            return Uri.EscapeUriString(url);
        }
        #endregion
    }
}