using UnityEngine;

namespace GameAssets
{
    public class AssetConfig
    {
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
        
        /// <summary>
        /// AssetBundle 的后缀名
        /// </summary>
        public static readonly string Extension = ".unity3d";

        
        public static VersionConfig VersionConfig = new VersionConfig();
        
        
        public static void Test()
        {
            
        }
    }
}