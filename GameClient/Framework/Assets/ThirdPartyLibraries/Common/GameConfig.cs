using System.IO;
using UnityEditor;
using UnityEngine;

namespace Common
{
    public enum GameBuildType
    {
        Local = 0,//本地编辑器上面运行,会输出 Log,编辑器下也会运行此模式
        Debug,   //打包测试,打包成 apk,ipa,app.exe 等,都会输出 Log
        Release,//打包成 apk,ipa,app.exe 等进行发布,不会输出 Log
    }
    
    [ExecuteAlways]
    public class GameConfig : ScriptableObject
    {
        [Tooltip("当前游戏运行的方式")]
        public GameBuildType BuildConfig;
        
        //根据 svn 版本号获取
        [Tooltip("当前游戏的版本")] 
        public string version = "1";

        [Tooltip("是否在编辑器下开启加载AssetBundle的模式,开启后需要先打AssetBundle,否则使用编辑器模式加载")]
        public bool UseAssetBundles;
        
        [SerializeField][Tooltip("web 服务器的 URL 列表")]
        private string[] RemoteWebServerAddress;



        public string[] QueryAddress()
        {
            if (BuildConfig == GameBuildType.Local || RemoteWebServerAddress == null || RemoteWebServerAddress.Length <= 0)
            {//使用的是 HTTP ,本地搭建的 web 服务器,测试使用,默认:http://127.0.0.1:80/DownloadAssets/" 请本地搭建.
                RemoteWebServerAddress = new string[] {"http://127.0.0.1/DownloadAssets"};
            }
            return RemoteWebServerAddress;
        }

        public void Reset()
        {
            UseAssetBundles = false;
            RemoteWebServerAddress = null;
            version = "1";
        }

#if UNITY_EDITOR
        [MenuItem("Builder/1 Update GameConfig", priority = 1000)]
        public static GameConfig UpdateGameConfig()
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
            return buildConfig;
            // Resources.UnloadAsset(assetsConfig);//编辑器下无需卸载
        }
#endif
    }
}