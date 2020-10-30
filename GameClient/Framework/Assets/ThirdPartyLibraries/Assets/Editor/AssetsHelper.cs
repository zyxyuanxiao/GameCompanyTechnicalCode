using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace GameAssets
{
    public static class AssetsHelper
    {
        public static string AssetBundlesDirectory = Application.dataPath.Replace("Assets", "AssetBundles/");

        /// <summary>
        /// 获取所有 AB 文件的配置
        /// </summary>
        /// <returns></returns>
        public static ABManifest QueryAssetsConfig()
        {
            string path = "Assets/_BuildAsset/Config/ABManifest.asset";
            ABManifest abManifest = AssetDatabase.LoadAssetAtPath<ABManifest>(path);
            if (null == abManifest)
            {
                abManifest = ScriptableObject.CreateInstance<ABManifest>();
                AssetDatabase.CreateAsset(abManifest, path);
                AssetDatabase.SaveAssets();
            }

            return abManifest;
        }

        /// <summary>
        /// 打所有的包,最新的包,包括 ab 包与 app (执行程序)
        /// </summary>
        public static void BuildAllStreamingAssets()
        {
            //根据配置文件进行初始化 AB 包的名字以及资源
            BuildAssetsConfig assetsConfig = BuildAssetsConfig.QueryAssetsConfig();
            assetsConfig.BuildAllAB();
            AssetDatabase.Refresh();

            if (!Directory.Exists(AssetsHelper.AssetBundlesDirectory))
                Directory.CreateDirectory(AssetsHelper.AssetBundlesDirectory);
            const BuildAssetBundleOptions options = BuildAssetBundleOptions.ChunkBasedCompression |
                                                    BuildAssetBundleOptions.DisableWriteTypeTree |
                                                    BuildAssetBundleOptions.DeterministicAssetBundle;
            var targetPlatform = EditorUserBuildSettings.activeBuildTarget;
            AssetBundleManifest assetBundleManifest = BuildPipeline.BuildAssetBundles(
                AssetsHelper.AssetBundlesDirectory, assetsConfig.QueryAssetBundleBuilds(), options, targetPlatform);
            if (assetBundleManifest == null)
            {
                Debug.LogError("打包失败");
                return;
            }

            ABManifest manifest = QueryAssetsConfig();
            List<string> dirs = new List<string>();
            List<AssetRef> assets = new List<AssetRef>();
            string[] assetBundles = assetBundleManifest.GetAllAssetBundles();
            Dictionary<string, int> abToRef = new Dictionary<string, int>();
            for (var index = 0; index < assetBundles.Length; index++)
            {
                string abName = assetBundles[index];
                abToRef[abName] = index;
                // Debug.Log(abName + "    index:" + index);
            }

            var bundleRefs = new List<BundleRef>();

        }

        public static void BuildDefaultStreamingAssets()
        {

        }

        public static void BuildHotUpdateAsset()
        {

        }

        public static string GetPlatform(BuildTarget target)
        {
            if (target == BuildTarget.Android)
            {
                return "Android";
            }
            else if (target == BuildTarget.iOS)
            {
                return "iOS";
            }
            else if (target == BuildTarget.WebGL)
            {
                return "WebGL";
            }
            else if (target == BuildTarget.StandaloneWindows || target == BuildTarget.StandaloneWindows64)
            {
                return "Windows";
            }
            else if (target == BuildTarget.StandaloneOSX || target == BuildTarget.StandaloneOSXIntel64)
            {
                return "OSX";
            }

            Debug.LogError("没有这个平台的设置");
            return "UnKnow";
        }

    }
}