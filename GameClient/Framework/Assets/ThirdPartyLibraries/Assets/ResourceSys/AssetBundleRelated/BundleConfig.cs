/********************************************************************
	created:	2015/12/20  15:44
	file base:	BundleConfig
	file ext:	csConfusedFolder
	author:		luke
	
	purpose:	资源包配置
*********************************************************************/

using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using System;

namespace Best
{
    public enum TargetPlatform
    {
        None = 0,
        Win = 5,
        iPhone = 9,
        Android = 13
    }


    public class BundleConfig
    {
        /// <summary>
        /// 是否是资源混淆模式
        /// </summary>
        public static bool IsConfuseModel = false;

        public static byte BundleEncryptKey = 0;//0的情况下，是不会进行加密。0是用来判断加密有没有开启

        /// <summary>
        /// 加密资源包后缀
        /// </summary>
        public static string EncrypFilePostfix = "ecp";

        public static string CompleteEncrypFilePostfix = ".ecp";

        public bool hadOpenGm
        {
            get
            {
                if (hadInitGm == false)
                {
                    VersionInfo versionInfo = FileUtils.GetCurrentVerNo();

                    if (versionInfo != null && versionInfo.HadGm == "true")
                    {
                        m_HadOpenGm = true;
                    }
                    hadInitGm = true;
                }
                return m_HadOpenGm;

            }
        }

        private bool hadInitGm = false;
        private bool m_HadOpenGm = false;


        private static readonly object flag = new object();

        private static BundleConfig instance;

        private static bool isAndroidMobile = false;

        public static BundleConfig Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (flag)
                    {
                        if (instance == null)
                        {
                            instance = new BundleConfig();
                        }
#if !UNITY_EDITOR && UNITY_ANDROID 
                        isAndroidMobile =true; //判断是不是安卓真机，为了适配安卓的AssetZip
#endif
                    }
                }

                return instance;
            }
        }

        //打包平台
        public TargetPlatform BundlePlatform
        {
            get
            {
                TargetPlatform tp = TargetPlatform.Win;
#if UNITY_EDITOR
                switch (UnityEditor.EditorUserBuildSettings.activeBuildTarget)
                {
                    case UnityEditor.BuildTarget.StandaloneOSXIntel64:
                    case UnityEditor.BuildTarget.StandaloneWindows:
                        tp = TargetPlatform.Win;
                        break;
                    case UnityEditor.BuildTarget.iOS:
                        tp = TargetPlatform.iPhone;
                        break;
                    case UnityEditor.BuildTarget.Android:
                        tp = TargetPlatform.Android;
                        break;
                }
#else
#if UNITY_STANDALONE
                        tp = TargetPlatform.Win;
#elif UNITY_IPHONE
                        tp = TargetPlatform.iPhone;
#elif UNITY_ANDROID
                        tp = TargetPlatform.Android;
#endif
#endif
                return tp;
            }
        }

        private string bundlePlatformStr = null;
        public string BundlePlatformStr
        {
            get
            {
                if (string.IsNullOrEmpty(bundlePlatformStr))
                {
                    bundlePlatformStr = BundlePlatform.ToString();
                }
                return bundlePlatformStr;
            }
        }

        public string soundWwisePlatfromStr = null;
        public string SoundWwisePlatfromStr
        {
            get
            {
                if (string.IsNullOrEmpty(soundWwisePlatfromStr))
                {
#if UNITY_STANDALONE
                    soundWwisePlatfromStr = "Windows";
#elif UNITY_IPHONE
                    soundWwisePlatfromStr = "iOS";
#elif UNITY_ANDROID
                    soundWwisePlatfromStr = "Android";
#endif

                }
                return soundWwisePlatfromStr;
            }
        }

        //资源包存放目录名
        public string AssetBundleDirectory { get { return "AssetBundles"; } }

        private string persistentDataPath = null;
        public string PersistentDataPath
        {
            get
            {
                if (string.IsNullOrEmpty(persistentDataPath))
                {
                    persistentDataPath = Application.persistentDataPath;
                }
                return persistentDataPath;
            }
        }

        //资源包存放路径persistentPath
        private string bundlesPathForPersist = null;
        public string BundlesPathForPersist
        {
            get
            {
                if (string.IsNullOrEmpty(bundlesPathForPersist))
                {
                    bundlesPathForPersist = PersistentDataPath + "/" + BundlePlatformStr + "/";

                    if (IsConfuseModel)
                    {
                        bundlesPathForPersist = bundlesPathForPersist + ConfusedFolder + "/";
                    }
                }
                return bundlesPathForPersist;
            }
        }

        public string ConfusedLuaResPath()
        {
            return string.Format("{0}Lua", bundlesPathForPersist);
        }

        private string persistPathForPatch = null;
        /// <summary>
        /// Gets the Patch path for persist.
        /// </summary>
        /// <value>The atch path for persist.</value>
        public string PersistPathForPatch
        {
            get
            {
                if (string.IsNullOrEmpty(persistPathForPatch))
                {
                    persistentDataPath = PersistentDataPath;
                }
                return persistentDataPath;
            }
        }

        //资源依赖关系表文件名
        public string DependFileName = "Depend.bytes";

        //版本文件存放文件名
        public string VersionFileName = "version.bytes";

        //渠道号存放文件名
        public string ChannelFileName = "channel.bytes";

        //资源包的后缀
        public string BundleSuffix = ".ab";

        //bsdiff文件信息文件
        public string  BsDiffConfigFileName = "bsdiffconfig.txt";

        public static string ConfusedFolder = "ojbf"; //混淆文件夹

        private static Dictionary<System.Type, string> _dictTypeStr = new Dictionary<System.Type, string>();

        private static Dictionary<string, bool> errorABStateDic = new Dictionary<string, bool>();//记录开启gm命令和ab记录文件下载失败后，使用file.exits判断的状态

        private static string AddToTypeStrDic(System.Type type, System.Type nameType)
        {
            string[] strType = nameType.ToString().Split('.');
            string str = strType[strType.Length - 1];
            if (type == typeof(RuntimeAnimatorController))
            {
                str = "rac";
            }
            _dictTypeStr.Add(type, str);
            return str;
        }
        private static string GetStrByType(System.Type type)
        {
            string str = null;
            if (!_dictTypeStr.TryGetValue(type, out str))
            {
                if (type == typeof(UnityEngine.Object) || type == typeof(GameObject))
                {
                    str = AddToTypeStrDic(typeof(UnityEngine.Object), typeof(GameObject));
                    str = AddToTypeStrDic(typeof(GameObject), typeof(GameObject));
                }
                else
                {
                    str = AddToTypeStrDic(type, type);
                }

                return str;
            }
            return str;
        }

        public const string SCENE_AB_SURFIX = ".unity.ab";

        //根据相对于Res_Best的路径和文件类型，生成运行时的资源uri（GCTODO：目前就是assetbundle的文件名，后续会扩展）
        public static string EditorAssetURI2RuntimeAssetName(string assetURI, Type type)
        {
            //string[] fileTypeArray = abParams.type.ToString().Split('.');

            System.Text.StringBuilder sb = LuaInterface.StringBuilderCache.Acquire();

            if (type == typeof(Shader) || type == typeof(ShaderVariantCollection))
            {
                sb.Append("shaderbundle.ab");
                return LuaInterface.StringBuilderCache.GetStringAndRelease(sb);
            }

            sb.Append(assetURI);

            if (type == typeof(UnityEngine.SceneManagement.Scene))
            {
                sb.Append(SCENE_AB_SURFIX);

                string bundleName = LuaInterface.StringBuilderCache.GetStringAndRelease(sb).ToLower();

                return bundleName;
            }

            sb.Append(".");
            if (assetURI.StartsWith("CC_Atlases"))
            {
                sb.Append("atlas");
            }
            else if (assetURI.StartsWith("CC_Fonts"))
            {
                sb.Append("font");
            }
            else
            {
                sb.Append(GetStrByType(type));//(fileTypeArray[fileTypeArray.Length - 1]);
            }
            sb.Replace(" ", "_");
            sb.Replace("/", ".");
            sb.Append(".ab");

            return LuaInterface.StringBuilderCache.GetStringAndRelease(sb).ToLower();
        }

        /// <summary>
        /// 判断ab是否存在
        /// </summary>
        /// <param name="bundleName"></param>
        /// <returns></returns>
        public static bool GetBundleHadExit(string bundleName, Type type)
        {
            if (string.IsNullOrEmpty(bundleName))
            {
                return false;
            }
            bundleName = EditorAssetURI2RuntimeAssetName(bundleName, type);
            string fileUrl;

            switch (AssetBundleExistsInfo.CheckFileType(bundleName))
            {
                case FileFixType.Normal:
                case FileFixType.Encryp:
                    return true;
            }

            if (BundleConfig.instance.hadOpenGm)
            {
                fileUrl = instance.GetBundlePathInPersistentPath(bundleName);
                if (!errorABStateDic.ContainsKey(fileUrl))
                {
                    if (File.Exists(fileUrl))
                    {
                        errorABStateDic[fileUrl] = true;
                        return true;
                    }
                    else
                    {
                        errorABStateDic[fileUrl] = false;
                    }
                }
                else if (errorABStateDic[fileUrl])
                {
                    return true;
                }
                if (BundleEncryptKey != 0) //0的情况下，是不会进行加密。0是用来判断加密有没有开启
                {
                    //获取加密资源
                    fileUrl = instance.GetEncryptBundlePathInPersistentPath(bundleName);
                    if (File.Exists(fileUrl))
                    {
                        return true;
                    }
                }
            }

            fileUrl = instance.GetBundleRelativePath(bundleName);
            // if (StreamingAssetLoad.HasStreamingFile(fileUrl))
            // {
            //     return true;
            // }

#if UNITY_EDITOR || UNITY_IOS
            //获取资源包在StreamingAssets目录下的加密资源uri
            fileUrl = instance.GetEncryptBundleRelativePath(bundleName);

            // if (StreamingAssetLoad.HasStreamingFile(fileUrl))
            // {
            //     return true;
            // }
#endif
            return false;
        }


        //生成运行时的assetbundle资源的绝对路径
        public static string GetBundleUrlForLoad(string bundleName)
        {
            string fileUrl = "";

            switch (AssetBundleExistsInfo.CheckFileType(bundleName))
            {
                case FileFixType.Normal:
                    return instance.GetBundlePathInPersistentPath(bundleName);
                case FileFixType.Encryp:
                    return instance.GetEncryptBundlePathInPersistentPath(bundleName);
            }
            string streamPath = instance.GetBundleRelativePath(bundleName);

            if (BundleConfig.instance.hadOpenGm)
            {
                fileUrl = instance.GetBundlePathInPersistentPath(bundleName);
                if (!errorABStateDic.ContainsKey(fileUrl))
                {
                    if (File.Exists(fileUrl))
                    {
                        errorABStateDic[fileUrl] = true;
                        return fileUrl;
                    }
                    else
                    {
                        errorABStateDic[fileUrl] = false;
                    }
                }
                else if (errorABStateDic[fileUrl])
                {
                    return fileUrl;
                }

                if (BundleEncryptKey != 0)
                {
                    //获取加密资源
                    fileUrl = instance.GetEncryptBundlePathInPersistentPath(bundleName);
                    if (File.Exists(fileUrl))
                    {
                        return fileUrl;
                    }
                }
            }


            return instance.GetPathInStreamingAssetPath(streamPath);
        }

        //获取资源包在persistentDataPath目录下的uri
        private string GetBundlePathInPersistentPath(string bundleName)
        {
            StringBuilder sb = LuaInterface.StringBuilderCache.Acquire();

            sb.Append(PersistentDataPath);
            sb.Append("/");
            sb.Append(BundlePlatformStr);
            sb.Append("/");
            if (IsConfuseModel)
            {
                sb.Append(ConfusedFolder);
                sb.Append('/');
            }
            sb.Append(bundleName);

            return LuaInterface.StringBuilderCache.GetStringAndRelease(sb);
        }

        //获取加密资源包在persistentDataPath目录下的uri
        private string GetEncryptBundlePathInPersistentPath(string bundleName)
        {
            StringBuilder sb = LuaInterface.StringBuilderCache.Acquire();

            sb.Append(PersistentDataPath);
            sb.Append("/");
            sb.Append(BundlePlatformStr);
            sb.Append("/");
            if (IsConfuseModel)
            {
                sb.Append(ConfusedFolder);
                sb.Append('/');
            }

            StringBuilder nameSb = LuaInterface.StringBuilderCache.Acquire();
            nameSb.Append(bundleName);
            nameSb.Append(".");
            nameSb.Append(EncrypFilePostfix);
            string encryptedBundleName = LuaInterface.StringBuilderCache.GetStringAndRelease(nameSb);

            sb.Append(encryptedBundleName);

            return LuaInterface.StringBuilderCache.GetStringAndRelease(sb);
        }

        //获取资源的相对路径
        private string GetBundleRelativePath(string bundleName)
        {
            StringBuilder sb = LuaInterface.StringBuilderCache.Acquire();
            if (!isAndroidMobile)
            {
                sb.Append(AssetBundleDirectory);
                sb.Append("/");
                sb.Append(BundlePlatformStr);
                sb.Append("/");
            }

            if (IsConfuseModel)
            {
                sb.Append(ConfusedFolder);
                sb.Append('/');
            }
            sb.Append(bundleName);
            return LuaInterface.StringBuilderCache.GetStringAndRelease(sb);
        }

        //获取加密资源的相对路径
        private string GetEncryptBundleRelativePath(string bundleName)
        {
            StringBuilder sb = LuaInterface.StringBuilderCache.Acquire();
            sb.Append(AssetBundleDirectory);
            sb.Append("/");
            sb.Append(BundlePlatformStr);
            sb.Append("/");
            if (IsConfuseModel)
            {
                sb.Append(ConfusedFolder);
                sb.Append('/');
            }
            sb.Append(bundleName);
            sb.Append(".");
            sb.Append(EncrypFilePostfix);
            return LuaInterface.StringBuilderCache.GetStringAndRelease(sb);
        }

        //获取在StreamingAsset下的路径
        private string GetPathInStreamingAssetPath(string uri)
        {
            StringBuilder sb = LuaInterface.StringBuilderCache.Acquire();
            sb.Append(StreamingAssetPath);
            sb.Append('/');
            if (isAndroidMobile)
            {
                sb.Append(AssetBundleDirectory);
                sb.Append("/");
                sb.Append(BundlePlatformStr);
                sb.Append("/");
            }
            sb.Append(uri);

            return LuaInterface.StringBuilderCache.GetStringAndRelease(sb);
        }


        //获取各平台下StreamingAsset目录路径
        private string streamingAssetPath = null;
        public string StreamingAssetPath
        {
            get
            {
                if (string.IsNullOrEmpty(streamingAssetPath))
                {
                    streamingAssetPath = Application.streamingAssetsPath;
                    /*
#if UNITY_EDITOR || UNITY_STANDALONE
                        streamingAssetPath = Application.streamingAssetsPath;
#elif UNITY_IPHONE
                        streamingAssetPath = "file://" + Application.dataPath + "/Raw/";
#elif UNITY_ANDROID
                        streamingAssetPath = "jar:file://" + Application.dataPath + "!/assets/";
#endif
                    */
                }

                return streamingAssetPath;
            }
        }

        /// <summary>
        /// 游戏压缩资源包名字
        /// </summary>
        public static string GameResourceZipName = "GameResource.zip";

        /// <summary>
        /// 游戏压缩资源包Streaming路径
        /// </summary>
        public static string GameResourceZipStreamingPath
        {
            get
            {
                return Application.streamingAssetsPath + "/" + GameResourceZipName;
            }
        }

        /// <summary>
        /// 获取Apk版本号
        /// </summary>
        public static string LocalApkVersion
        {
            get
            {
                string path = BundleConfig.Instance.BundlesPathForPersist + "ApkVersion.bytes";

                if (File.Exists(path))
                {
                    string text = File.ReadAllText(path);
                    LitJson.JsonData jd = LitJson.JsonMapper.ToObject(text);
                    return (string)jd["VersionNum"];
                }
                else
                {
                    return "0.0.0.0";
                }
            }
        }

        /// <summary>
        /// 对比新老版本号
        /// </summary>
        public static bool CompareVersion(string oldVersion, string newVersion)
        {
            Version oldVer = new Version(oldVersion);
            Version newVer = new Version(newVersion);

            return newVer > oldVer;
        }
    }
}
