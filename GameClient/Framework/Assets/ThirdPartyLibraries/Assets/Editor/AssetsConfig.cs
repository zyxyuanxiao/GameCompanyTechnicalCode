using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace GameAssets
{
    /// <summary>
    /// 文件夹的搜索模式,
    /// 搜索文件夹及其子文件夹的所有内容,根据文件搜索模式过滤不属于的文件
    /// </summary>
    public static class FileSearchPattern
    {
        public static string All = "*";
        public static string Asset = "*.asset";
        public static string Controller = "*.controller";
        public static string Material = "*.mat";
        public static string Png = "*.png";
        public static string Prefab = "*.prefab";
        public static string Scene = "*.unity";
        public static string Text = "*.txt,*.bytes,*.json,*.csv,*.xml,*htm,*.html,*.yaml,*.fnt";
        
        //验证此资源不属于以下类型的资源,才可以被打进游戏包内
        public static bool ValidateAsset(string file)
        {
            if (!file.StartsWith("Assets/")) return false;
            var ext = Path.GetExtension(file).ToLower();
            return ext != ".dll" && ext != ".cs" && ext != ".meta" && ext != ".js" && ext != ".boo";
        }
    }
    
    
    /// <summary>
    /// 在本地Assets/的路径之后,单个文件夹内的资源
    /// </summary>
    [Serializable]
    public class LocalFile
    {
        public string name = string.Empty;
        [Tooltip("在本地Assets/的路径,从其中进行查找文件资源进行打热更包")]
        public string LocalPath = string.Empty;
        
        [Tooltip("搜索当前文件夹下的通配符，多个之间请用,(分号)隔开")] 
        public string SearchPattern;
        
        //获取当前路径下可以被打包的资源
        public string[] GetAsset()
        {
            if (!Directory.Exists(LocalPath))
            {
                Debug.Log("本地文件夹为空:" + LocalPath);
                return null;
            }
            string[] patterns = SearchPattern.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries);
            List<string> getFiles = new List<string>();
            foreach (var item in patterns)
            {
                string[] files = Directory.GetFiles(LocalPath, item, SearchOption.AllDirectories);
                foreach (string file in files)
                {
                    if (Directory.Exists(file)) continue;//如果是文件夹则跳出
                    var ext = Path.GetExtension(file).ToLower();//获取后缀名
                    Debug.Log("获取文件的后缀名字:" + ext);
                    if ((ext == ".fbx" || ext == ".anim") && !item.Contains(ext)) continue;//fbx或者anim都不打进游戏内
                    if (!FileSearchPattern.ValidateAsset(file)) continue;
                    string asset = file.Replace("\\", "/");
                    getFiles.Add(asset);
                }
            }
            return getFiles.ToArray();
        }
    }
    
    public class AssetsConfig : ScriptableObject
    {
       
        [Header("Serach Asset Rule")]
        public List<LocalFile> Rules;
        
        [Header("AssetBundleBuild")]
        public List<AssetBundleBuild> AssetBundleBuilds;

        public void InitRules()
        {
            // string[] directory = {"ttf", "hdr", "mat", "fbx", "prefab", "unity", "shader/cginc", "asset","png"};
            // foreach (var item in directory)
            // {
            //     Rules.Add(new LocalFile()
            //     {
            //         name = item,
            //         LocalPath = "/_BuildAsset/" + item + "/",
            //         SearchPattern = "*"
            //     });
            // }
            
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
        }
        
        public void Reset()
        {
            if (null != Rules)Rules.Clear();
        }

        
        
        public static AssetsConfig QueryAssetsConfig()
        {
            string path = "Assets/ThirdPartyLibraries/Assets/Editor/AssetsConfig.asset";
            AssetsConfig assetsConfig = AssetDatabase.LoadAssetAtPath<AssetsConfig>(path);
            if (null == assetsConfig)
            {
                assetsConfig = ScriptableObject.CreateInstance<AssetsConfig>();
                AssetDatabase.CreateAsset(assetsConfig, "Assets/ThirdPartyLibraries/Assets/Editor/AssetsConfig.asset");
                AssetDatabase.SaveAssets();
            }
            return assetsConfig;
        }
        
    }
}

