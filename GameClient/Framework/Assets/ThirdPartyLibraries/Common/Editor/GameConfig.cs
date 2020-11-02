using System.IO;
using UnityEditor;
using UnityEngine;

namespace Common
{
    public class GameConfig : ScriptableObject
    {
        //根据 svn 版本号获取
        [SerializeField] [Tooltip("当前游戏的版本")] 
        private string version = string.Empty;

        [Tooltip("是否在编辑器下开启加载AssetBundle的模式,开启后需要先打AssetBundle,否则使用编辑器模式加载")]
        public bool UseAssetBundles;

        [Tooltip("使用本地服务器,Mac 下使用的是 Apache,windows 下需要自己搭建")]
        public bool UseLocalServer;

        [Tooltip("使用的是 HTTP ,本地搭建的 web 服务器,测试使用,默认:http://127.0.0.1:80/AssetBundles/")]
        public string LocalWebServerAddress = "http://127.0.0.1:80/AssetBundles/";

        [Tooltip("使用的是 HTTP ,正式搭建的 web 服务器")] 
        public string[] RemoteWebServerAddress;



        public string QueryVersion()
        {
            version = SVNHelper.GetSvnVersion();
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            return version;
        }

        public void Reset()
        {
            UseAssetBundles = false;
            UseLocalServer = false;
            LocalWebServerAddress = "http://127.0.0.1:80/AssetBundles/";
            RemoteWebServerAddress = null;
            QueryVersion();
        }


        [MenuItem("Builder/1 Init GameConfig", priority = 1000)]
        private static void CreateGameConfig()
        {
            GameConfig buildConfig = Resources.Load<GameConfig>("Configs/GameConfig");
            if (null == buildConfig)
            {
                buildConfig = ScriptableObject.CreateInstance<GameConfig>();
                string path = Application.dataPath + "/Resources/Configs/";
                if (!Directory.Exists(path)) Directory.CreateDirectory(path);
                AssetDatabase.CreateAsset(buildConfig, "Assets/Resources/Configs/GameConfig.asset");
                AssetDatabase.SaveAssets();
            }

            buildConfig.Reset();
            Selection.activeObject = buildConfig;
            // Resources.UnloadAsset(assetsConfig);//编辑器下无需卸载
        }

    }
}