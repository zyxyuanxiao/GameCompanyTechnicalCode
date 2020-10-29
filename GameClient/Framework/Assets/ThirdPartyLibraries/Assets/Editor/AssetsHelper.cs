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
            AssetsConfig assetsConfig = AssetsConfig.QueryAssetsConfig();
            assetsConfig.CollectAllResources();

            // if (!Directory.Exists(AssetsHelper.AssetBundlesDirectory))
            //     Directory.CreateDirectory(AssetsHelper.AssetBundlesDirectory);
            // const BuildAssetBundleOptions options = BuildAssetBundleOptions.ChunkBasedCompression |
            //                                         BuildAssetBundleOptions.DisableWriteTypeTree |
            //                                         BuildAssetBundleOptions.DeterministicAssetBundle;
            // var targetPlatform = EditorUserBuildSettings.activeBuildTarget;
            // AssetsConfig assetsConfig = AssetsConfig.QueryAssetsConfig();
            // AssetBundleManifest assetBundleManifest = BuildPipeline.BuildAssetBundles(
            //     AssetsHelper.AssetBundlesDirectory, assetsConfig.QueryAssetBundleBuilds(), options, targetPlatform);
            // if (assetBundleManifest == null)
            // {
            //     Debug.LogError("打包失败");
            //     return;
            // }
            //
            // ABManifest manifest = QueryAssetsConfig();
            // List<string> dirs = new List<string>();
            // List<AssetRef> assets = new List<AssetRef>();
            // string[] bundles = assetBundleManifest.GetAllAssetBundles();
            // Dictionary<string, int> bundle2Ids = new Dictionary<string, int>();
            // for (var index = 0; index < bundles.Length; index++)
            // {
            //     var bundle = bundles[index];
            //     bundle2Ids[bundle] = index;
            // }
            // var bundleRefs = new List<BundleRef> ();

        }

        public static void BuildDefaultStreamingAssets()
        {
           
        }
        
        public static void BuildHotUpdateAsset()
        {
           
        }
        
    }
}