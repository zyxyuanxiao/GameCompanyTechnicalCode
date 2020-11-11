using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using LitJson;
using UnityEditor;
using UnityEngine;

namespace GameAssets
{
    public static class AssetsHelper
    {
        public static string AssetBundlesDirectory
        {
            get
            {  
                string path = Application.dataPath.Replace("Assets", "AssetBundles/") + 
                              Common.Tool.GetPlatform() + "/";
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                return path;
            }
        }
        
        
        /// <summary>
        /// 打所有的包,最新的包,包括 ab 包与 app (执行程序)
        /// </summary>
        public static void BuildAllStreamingAssets()
        {
            //配置路径
            if (Directory.Exists(AssetsHelper.AssetBundlesDirectory))
                Directory.Delete(AssetsHelper.AssetBundlesDirectory, true);
            
            //根据配置文件进行初始化 AB 包的名字以及资源
            BuildAssetsConfig assetsConfig = BuildAssetsConfig.QueryAssetsConfig();
            assetsConfig.BuildAllAB();
            AssetDatabase.Refresh();
            
            //设置打包压缩格式
            const BuildAssetBundleOptions options = BuildAssetBundleOptions.ChunkBasedCompression |
                                                    BuildAssetBundleOptions.DisableWriteTypeTree |
                                                    BuildAssetBundleOptions.DeterministicAssetBundle;
            var targetPlatform = EditorUserBuildSettings.activeBuildTarget;
            //开始打包
            AssetBundleManifest assetBundleManifest = BuildPipeline.BuildAssetBundles(
                AssetsHelper.AssetBundlesDirectory, 
                assetsConfig.QueryAssetBundleBuilds(), 
                options, 
                targetPlatform);
            if (assetBundleManifest == null)
            {
                Debug.LogError("打包失败");
                return;
            }
            
            BuildVersionConfig(assetBundleManifest);
            
            // //将所有的 AB 包的名字记录下来
            // string[] abNames = assetBundleManifest.GetAllAssetBundles();
            // Dictionary<string, int> abNameToIndex = new Dictionary<string, int>();
            // for (var index = 0; index < abNames.Length; index++)
            // {
            //     string abName = abNames[index];
            //     abNameToIndex[abName] = index;
            // }
            //
            //
            // List<ABInfo> abInfos = new List<ABInfo>();
            // for (var index = 0; index < abNames.Length; index++)
            // {
            //     string abName = abNames[index];
            //     string[] deps = assetBundleManifest.GetAllDependencies(abName);//获取资产包的所有依赖
            //     FileInfo fileInfo = new FileInfo(AssetsHelper.AssetBundlesDirectory + abName);
            //     abInfos.Add(new ABInfo()
            //     {
            //         Name = abName,
            //         Id = index,
            //         Dependencies = deps,
            //         Length = fileInfo.Length,
            //         Hash = assetBundleManifest.GetAssetBundleHash(abName).ToString(),
            //     });
            // }
            //
            // HashSet<string> localDirectorys = new HashSet<string>();
            // List<LocalFileInfo> assets = new List<LocalFileInfo>();
            // foreach (AssetBundleBuild item in assetsConfig.QueryAssetBundleBuilds())
            // {
            //     foreach (string path in item.assetNames)//路径
            //     {
            //         string dir = Path.GetDirectoryName(path).Replace("\\", "/");
            //         localDirectorys.Add(dir);
            //         assets.Add(new LocalFileInfo()
            //         {
            //             Name = Path.GetFileName(path),
            //             ABName = item.assetBundleName,
            //             LocalDirectory = dir
            //         });
            //     }
            // }
            //
            // //将单个资源的文件夹,本地单个文件的信息,AB 包的信息,展示出依赖关系
            // ABManifest manifest = QueryAssetsConfig();
            // manifest.LocalDirectorys = localDirectorys.ToArray();
            // manifest.LocalFileInfos = assets.ToArray();
            // manifest.ABInfos = abInfos.ToArray();
            // EditorUtility.SetDirty(manifest);
            // AssetDatabase.SaveAssets();
            // AssetDatabase.Refresh();
            // Selection.activeObject = manifest;
            //
            // //将这个依赖关系打包到AB包里面,属于增量更新包体
            // string manifestBundleName = "ABManifest".ToLower() + AssetBundleConfig.Extension;
            // AssetBundleBuild[] mabb = new[]
            // {
            //     new AssetBundleBuild
            //     {
            //         assetNames = new[] {AssetDatabase.GetAssetPath(manifest),},
            //         assetBundleName = manifestBundleName
            //     }
            // };
            //
            // BuildPipeline.BuildAssetBundles(
            //     AssetsHelper.AssetBundlesDirectory, mabb, options, targetPlatform);
            // ArrayUtility.Add(ref abNames, manifestBundleName);
            
            EditorUtility.OpenWithDefaultApp(AssetsHelper.AssetBundlesDirectory);
        }

        public static void BuildDefaultStreamingAssets()
        {
            
        }

        public static void BuildHotUpdateAsset()
        {

        }

        /// <summary>
        /// 根据打AB包生成的AssetBundleManifest,设置 VersionConfig
        /// </summary>
        private static void BuildVersionConfig(AssetBundleManifest assetBundleManifest)
        {
            VersionConfig vc = new VersionConfig()
            {
                OS = Common.Tool.GetPlatform(),
                SVNVersion = Common.SVNHelper.GetSvnVersion(),
                AppVersion = Application.version,
            };
            vc.ABInfos = new Dictionary<string, AB_V_MD5>();

            string[] abNames = assetBundleManifest.GetAllAssetBundles();
            foreach (string name in abNames)
            {
                string abPath = AssetsHelper.AssetBundlesDirectory + name;
                Debug.Log(abPath);
                vc.ABInfos[name] = new AB_V_MD5()
                    {Version = Common.SVNHelper.GetSvnVersion(), Md5Hash = Common.SecurityTools.GetMD5Hash(abPath)};
            }

            string vcPath = AssetsHelper.AssetBundlesDirectory + AssetsConfig.VersionConfigName;
            if (!File.Exists(vcPath))
            {
                using (File.Create(vcPath)) ;
            }

            File.WriteAllText(vcPath, JsonMapper.ToJson(vc), Encoding.UTF8);
        }

    }
}