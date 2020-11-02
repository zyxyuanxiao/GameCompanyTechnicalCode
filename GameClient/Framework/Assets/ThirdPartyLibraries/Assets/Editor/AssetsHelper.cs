using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
            if (Directory.Exists(AssetsHelper.AssetBundlesDirectory))
                Directory.Delete(AssetsHelper.AssetBundlesDirectory, true);
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
            
            //将所有的 AB 包的名字记录下来
            string[] abNames = assetBundleManifest.GetAllAssetBundles();
            Dictionary<string, int> abNameToIndex = new Dictionary<string, int>();
            for (var index = 0; index < abNames.Length; index++)
            {
                string abName = abNames[index];
                abNameToIndex[abName] = index;
            }
            
            
            List<ABInfo> abInfos = new List<ABInfo>();
            for (var index = 0; index < abNames.Length; index++)
            {
                string abName = abNames[index];
                string[] deps = assetBundleManifest.GetAllDependencies(abName);//获取资产包的所有依赖
                FileInfo fileInfo = new FileInfo(AssetsHelper.AssetBundlesDirectory + abName);
                abInfos.Add(new ABInfo()
                {
                    Name = abName,
                    Id = index,
                    Dependencies = deps,
                    Length = fileInfo.Length,
                    Hash = assetBundleManifest.GetAssetBundleHash(abName).ToString(),
                });
            }
            
            HashSet<string> localDirectorys = new HashSet<string>();
            List<LocalFileInfo> assets = new List<LocalFileInfo>();
            foreach (AssetBundleBuild item in assetsConfig.QueryAssetBundleBuilds())
            {
                foreach (string path in item.assetNames)//路径
                {
                    string dir = Path.GetDirectoryName(path).Replace("\\", "/");
                    localDirectorys.Add(dir);
                    assets.Add(new LocalFileInfo()
                    {
                        Name = Path.GetFileName(path),
                        ABName = item.assetBundleName,
                        LocalDirectory = dir
                    });
                }
            }
            
            //将单个资源的文件夹,本地单个文件的信息,AB 包的信息,展示出依赖关系
            ABManifest manifest = QueryAssetsConfig();
            manifest.LocalDirectorys = localDirectorys.ToArray();
            manifest.LocalFileInfos = assets.ToArray();
            manifest.ABInfos = abInfos.ToArray();
            EditorUtility.SetDirty(manifest);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Selection.activeObject = manifest;
            
            //将这个依赖关系打包到AB包里面,属于增量更新包体
            string manifestBundleName = "ABManifest".ToLower() + ABHelper.Extension;
            AssetBundleBuild[] mabb = new[]
            {
                new AssetBundleBuild
                {
                    assetNames = new[] {AssetDatabase.GetAssetPath(manifest),},
                    assetBundleName = manifestBundleName
                }
            };
            
            BuildPipeline.BuildAssetBundles(
                AssetsHelper.AssetBundlesDirectory, mabb, options, targetPlatform);
            ArrayUtility.Add(ref abNames, manifestBundleName);
            
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