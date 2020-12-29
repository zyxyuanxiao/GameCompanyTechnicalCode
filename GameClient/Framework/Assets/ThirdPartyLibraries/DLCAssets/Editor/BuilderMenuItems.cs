using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

namespace DLCAssets
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
        
        [MenuItem("Builder/3 Build All Asset", priority = 1002)]
        private static void BuildAllAssets()
        {
            BuildAssetsHelper.BuildAllAssets();
        }
        
        [MenuItem("Builder/4 Build Assets For Default", priority = 1003)]
        private static void BuildDefaultStreamingAssets()
        {
            BuildAssetsHelper.BuildDefaultStreamingAssets();
        }

        [MenuItem("Builder/5 Build Assets For HotUpdate", priority = 1004)]
        private static void BuildHotUpdateAsset()
        {
            BuildAssetsHelper.BuildHotUpdateAsset();
        }
        
        [MenuItem("Builder/6 Copy DLC To StreamingAssets", false, 1005)]
        private static void CopyAssetBundlesToStreamingAssets()
        {
            string destDir = Application.streamingAssetsPath + "/";
            string sourceDir = Application.dataPath.Replace("Assets", "DLC/");
            if (!Directory.Exists(destDir)) Directory.CreateDirectory(destDir);
            Common.EitorTools.CopyDirAndFile(sourceDir, destDir);
            if (!string.IsNullOrEmpty(destDir)) 
                EditorUtility.OpenWithDefaultApp(destDir + Common.Tool.QueryPlatform());
            AssetDatabase.Refresh();
        }
        
        [MenuItem("Builder/7 Copy DLC To Web Server", false, 1006)]
        public static void CopyAssetBundlesToWebServer()
        {
            //拷贝下载的文件到 web 服务器上面,web 服务器需要自己搭建,mac 上面自带 Apache 服务器.
            string destDir = "";
            if (Application.platform == RuntimePlatform.OSXEditor)
            {
                destDir = "/Library/WebServer/Documents/DLC/";
            }
            if (!Directory.Exists(destDir)) Directory.CreateDirectory(destDir);
            string sourceDir = Application.dataPath.Replace("Assets", "DLC/");
            Common.EitorTools.CopyDirAndFile(sourceDir, destDir);
            if (!string.IsNullOrEmpty(destDir))                 
                EditorUtility.OpenWithDefaultApp(destDir + Common.Tool.QueryPlatform());
        }

        [MenuItem("Builder/8 Open App Download Files Path", false, 1007)]
        private static void OpenAppAssetBundlesPath()
        {
            EditorUtility.OpenWithDefaultApp(Application.persistentDataPath);
        }


    }
}