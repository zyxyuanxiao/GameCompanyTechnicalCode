using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

namespace GameAssets
{
    public static class BuilderMenuItems
    {
        [MenuItem("Builder/2 Update BuildAssetsConfig", priority = 1001)]
        public static void UpdateAssetsConfig()
        {
            BuildAssetsConfig assetsConfig = BuildAssetsConfig.QueryAssetsConfig();
            assetsConfig.UpdateRules();
            Selection.activeObject = assetsConfig;
        }

        [MenuItem("Builder/3 Build Assets For Init", priority = 1003)]
        private static void BuildDefaultStreamingAssets()
        {
            BuildAssetsHelper.BuildDefaultStreamingAssets();
        }

        [MenuItem("Builder/4 Build Assets For HotUpdate", priority = 1004)]
        private static void BuildHotUpdateAsset()
        {
            BuildAssetsHelper.BuildHotUpdateAsset();
        }

        [MenuItem("Builder/5 Build All Asset", priority = 1005)]
        private static void BuildAllStreamingAssets()
        {
            BuildAssetsHelper.BuildAllStreamingAssets();
        }

        [MenuItem("Builder/Open App Download Files Path", false, 3001)]
        private static void OpenAppAssetBundlesPath()
        {
            EditorUtility.OpenWithDefaultApp(Application.persistentDataPath);
        }

        [MenuItem("Builder/Copy Download Files To Web Server", false, 3002)]
        private static void CopyAssetBundlesToWebServer()
        {
            //拷贝下载的文件到 web 服务器上面,web 服务器需要自己搭建,mac 上面自带 Apache 服务器.
            string destDir = "";
            if (Application.platform == RuntimePlatform.OSXEditor)
            {
                destDir = "/Library/WebServer/Documents/DownloadAssets/";
            }

            string sourceDir = Application.dataPath.Replace("Assets", "DownloadAssets/");
            Common.EitorTools.CopyDirAndFile(sourceDir, destDir);
            if (!string.IsNullOrEmpty(destDir)) EditorUtility.OpenWithDefaultApp(destDir);
        }
    }
}