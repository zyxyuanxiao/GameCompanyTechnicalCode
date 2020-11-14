using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

namespace GameAssets
{
    public class BuilderMenuItems
    {
        [MenuItem("Builder/2 Init BuildAssetsConfig", priority = 1001)]
        private static void InitAssetsConfig()
        {
            BuildAssetsConfig assetsConfig = BuildAssetsConfig.QueryAssetsConfig();
            assetsConfig.InitRules();
            Selection.activeObject = assetsConfig;
        }

        [MenuItem("Builder/3 Build Assets For Init", priority = 1003)]
        private static void BuildDefaultStreamingAssets()
        {
            AssetsHelper.BuildDefaultStreamingAssets();
        }

        [MenuItem("Builder/4 Build Assets For HotUpdate", priority = 1004)]
        private static void BuildHotUpdateAsset()
        {
            AssetsHelper.BuildHotUpdateAsset();
        }

        [MenuItem("Builder/5 Build All Asset", priority = 1005)]
        private static void BuildAllStreamingAssets()
        {
            AssetsHelper.BuildAllStreamingAssets();
        }

        
        
        
        
        
        
        
        
        
        
        
        
        
        // [MenuItem("Builder/Add Asset To Rule", false, 3000)]
        // private static void AddAssetToRule()
        // {
        //     string assetsConfigPath = "Assets/ThirdPartyLibraries/Assets/Editor/AssetsConfig.asset";
        //     AssetsConfig assetsConfig = AssetDatabase.LoadAssetAtPath<AssetsConfig>(assetsConfigPath);
        //     foreach (var item in Selection.objects)
        //     {
        //         string path = AssetDatabase.GetAssetPath(item);
        //         string suffix = Path.GetExtension(path);//文件后缀
        //         if (string.IsNullOrEmpty(suffix)) continue; //表示这个是路径,不加入资源规则里面
        //         
        //         Debug.Log(path + "    " + suffix);
        //         // assetsConfig.Rules.Add(new LocalFile()
        //         // {
        //         //     LocalPath = AssetDatabase.GetAssetPath(Selection.activeObject),
        //         //     SearchPattern = suffix
        //         // });
        //     }
        //     
        //     EditorUtility.SetDirty(assetsConfig);
        //     AssetDatabase.SaveAssets();
        //     Selection.activeObject = assetsConfig;
        // }


        [MenuItem("Builder/Open App AssetBundles Path", false, 3001)]
        private static void OpenAppAssetBundlesPath()
        {
            EditorUtility.OpenWithDefaultApp(Application.persistentDataPath);
        }

        [MenuItem("Builder/Copy AssetBundles To Web Server", false, 3002)]
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