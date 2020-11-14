using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Common;
using LitJson;
using UnityEditor;
using UnityEngine;
using Tool = Common.Tool;

namespace GameAssets
{
    public static class AssetsHelper
    {
        public static string DownloadAssetsDirectory
        {
            get
            {  
                string path = Application.dataPath.Replace("Assets", "DownloadAssets/") + 
                              Common.Tool.QueryPlatform() + "/";
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
            if (Directory.Exists(AssetsHelper.DownloadAssetsDirectory))
                Directory.Delete(AssetsHelper.DownloadAssetsDirectory, true);
            
            //根据配置文件进行初始化 AB 包的名字以及资源
            BuildAssetsConfig assetsConfig = BuildAssetsConfig.QueryAssetsConfig();
            assetsConfig.BuildAll();
            AssetDatabase.Refresh();
            
            //设置打包压缩格式
            const BuildAssetBundleOptions options = BuildAssetBundleOptions.ChunkBasedCompression |
                                                    BuildAssetBundleOptions.DisableWriteTypeTree |
                                                    BuildAssetBundleOptions.DeterministicAssetBundle;
            var targetPlatform = EditorUserBuildSettings.activeBuildTarget;
            //开始打包
            AssetBundleManifest assetBundleManifest = BuildPipeline.BuildAssetBundles(
                AssetsHelper.DownloadAssetsDirectory, 
                assetsConfig.QueryAssetBundleBuilds(), 
                options, 
                targetPlatform);
            if (assetBundleManifest == null)
            {
                Debug.LogError("打包失败");
                return;
            }
            
            BuildVersionConfig(assetBundleManifest);

            EditorUtility.OpenWithDefaultApp(AssetsHelper.DownloadAssetsDirectory);
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
                OS = Common.Tool.QueryPlatform(),
                SVNVersion = Common.SVNHelper.GetSvnVersion(),
                AppVersion = Application.version,
            };
            vc.FileInfos = new Dictionary<string, File_V_MD5>();

            string[] abNames = assetBundleManifest.GetAllAssetBundles();
            foreach (string name in abNames)
            {
                string abPath = AssetsHelper.DownloadAssetsDirectory + name;
                Debug.Log(abPath);
                vc.FileInfos[name] = new File_V_MD5()
                    {Version = Common.SVNHelper.GetSvnVersion(), MD5Hash = Common.SecurityTools.GetMD5Hash(abPath)};
            }

            //将 assetBundleManifest 文件也装载进配置文件中
            string abm = AssetsHelper.DownloadAssetsDirectory + Tool.QueryPlatform();
            vc.FileInfos[Tool.QueryPlatform()] = new File_V_MD5()
                {Version = Common.SVNHelper.GetSvnVersion(), MD5Hash = Common.SecurityTools.GetMD5Hash(abm)};

            string vcPath = AssetsHelper.DownloadAssetsDirectory + AssetsConfig.VersionConfigName;
            if (!File.Exists(vcPath))
            {
                using (File.Create(vcPath)) ;
            }
            // 这个后面不能加 UTF8,因为 Litjson 解析时报错
            File.WriteAllText(vcPath, JsonMapper.ToJson(vc));
        }



    }
}