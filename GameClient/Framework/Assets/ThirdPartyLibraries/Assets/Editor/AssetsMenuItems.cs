using System;
using System.IO;
using Common;
using UnityEngine;
using UnityEditor;

namespace GameAssets
{
    public class BuilderMenuItems
    {
        [MenuItem("Builder/2 Init AssetsConfig", priority = 1001)]
        private static void InitAssetsConfig()
        {
            AssetsConfig assetsConfig = AssetsConfig.QueryAssetsConfig();
            assetsConfig.Reset();
            assetsConfig.InitRules();
            Selection.activeObject = assetsConfig;
        }

        [MenuItem("Builder/3 Build Assets For App", priority = 1003)]
        private static void BuildDefaultStreamingAssets()
        {
           
        }
        
        [MenuItem("Builder/4 Build Assets For HotUpdate", priority = 1004)]
        private static void BuildHotUpdateAsset()
        {
           
        }
        
        [MenuItem("Builder/5 Build All Asset", priority = 1005)]
        private static void BuildAllStreamingAssets()
        {
            if (!Directory.Exists(AssetsHelper.AssetBundlesDirectory))
                Directory.CreateDirectory(AssetsHelper.AssetBundlesDirectory);
            const BuildAssetBundleOptions options = BuildAssetBundleOptions.ChunkBasedCompression | 
                                                    BuildAssetBundleOptions.DisableWriteTypeTree  |
                                                    BuildAssetBundleOptions.DeterministicAssetBundle;
            var targetPlatform = EditorUserBuildSettings.activeBuildTarget;
            AssetsConfig assetsConfig = AssetsConfig.QueryAssetsConfig();

        }
        
        [MenuItem("Builder/Add Asset To Rule", false, 3000)]
        private static void AddAssetToRule()
        {
            string assetsConfigPath = "Assets/ThirdPartyLibraries/Assets/Editor/AssetsConfig.asset";
            AssetsConfig assetsConfig = AssetDatabase.LoadAssetAtPath<AssetsConfig>(assetsConfigPath);
            foreach (var item in Selection.objects)
            {
                string path = AssetDatabase.GetAssetPath(item);
                string suffix = Path.GetExtension(path);//文件后缀
                if (string.IsNullOrEmpty(suffix)) continue; //表示这个是路径,不加入资源规则里面
                
                Debug.Log(path + "    " + suffix);
                // assetsConfig.Rules.Add(new LocalFile()
                // {
                //     LocalPath = AssetDatabase.GetAssetPath(Selection.activeObject),
                //     SearchPattern = suffix
                // });
            }
            
            EditorUtility.SetDirty(assetsConfig);
            AssetDatabase.SaveAssets();
            Selection.activeObject = assetsConfig;
        }


        [MenuItem("Builder/Open App AssetBundles", false, 3001)]
        private static void OpenAppAssetBundles()
        {
            EditorUtility.OpenWithDefaultApp(Application.persistentDataPath);
        }
















        
    }
}