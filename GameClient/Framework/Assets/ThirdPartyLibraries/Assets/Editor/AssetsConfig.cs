using System;
using System.Collections.Generic;
using System.IO;
using Common;
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
        public static string Txt = "*.txt";
        public static string Bytes = "*.bytes";
        public static string Json = "*.json";
        public static string CSV = "*.csv";
        public static string XML = "*.xml";
        public static string Png = "*.png";
        public static string Hdr = "*.hdr";
        public static string Asset = "*.asset";
        public static string TTF = "*.ttf";
        public static string Shader = "*.shader";
        public static string Controller = "*.controller";
        public static string Material = "*.mat";
        public static string Prefab = "*.prefab";
        public static string Spriteatlas = "*.spriteatlas";
        public static string Unity = "*.unity";
        
        //验证此资源不属于以下类型的资源,才可以被打进游戏包内
        public static bool ValidateAsset(string file)
        {
            var ext = Path.GetExtension(file).ToLower();
            return ext != ".dll" && ext != ".cs" && ext != ".meta" && ext != ".js" && ext != ".boo";
        }
    }
    
    [Serializable]
    public class ABInfo
    {
        //整个 AB 包的名字
        public string assetBundleName;
        //AB 包内部的资源的名字,多个资源可以合并一个 AB 包
        public string[] assetNames;
    }
    
    /// <summary>
    /// 在本地Assets/的路径之后,游戏文件夹内的一些文件资源,被统一抽象化为  LocalFile
    /// </summary>
    [Serializable]
    public class LocalFile
    {
        public string name = string.Empty;

        [Tooltip("搜索当前文件夹下的通配符，多个之间请用,(分号)隔开")] 
        public string SearchPattern;
        
        [Tooltip("在本地Assets/的路径,从其中进行查找文件资源进行打热更包")]
        public string[] LocalFilePaths;
        
        //获取文件夹内的所有文件资源        
        public static string[] QueryFilePath(string searchPattern)
        {
            string[] localFilePaths = Directory.GetFiles(Application.dataPath + "/_BuildAsset/",
                searchPattern,
                SearchOption.AllDirectories);
            List<string> filePaths = new List<string>();
            foreach (string item in localFilePaths)
            {
                if (Directory.Exists(item)) continue;//如果是文件夹则跳出
                if (!FileSearchPattern.ValidateAsset(item)) continue;//不通过则跳出
                var ext = Path.GetExtension(item).ToLower();
                if ((ext == ".fbx" || ext == ".anim") && !item.Contains(ext)) continue;//fbx或者anim都不打进游戏内
                string temp = item.Replace(Application.dataPath, "");
                filePaths.Add(temp.Replace("\\", "/"));
            }
                
            return filePaths.ToArray();
        }

        public static Dictionary<string, string> QueryRules()
        {
            Dictionary<string, string> assetTypes = new Dictionary<string, string>();
            assetTypes.Add("Txt", FileSearchPattern.Txt);
            assetTypes.Add("Bytes", FileSearchPattern.Bytes);
            assetTypes.Add("Json", FileSearchPattern.Json);
            assetTypes.Add("CSV", FileSearchPattern.CSV);
            assetTypes.Add("XML", FileSearchPattern.XML);
            assetTypes.Add("Png", FileSearchPattern.Png);
            assetTypes.Add("TTF", FileSearchPattern.TTF);
            assetTypes.Add("Asset", FileSearchPattern.Asset);
            assetTypes.Add("Hdr", FileSearchPattern.Hdr);
            assetTypes.Add("Shader", FileSearchPattern.Shader);
            assetTypes.Add("Controller", FileSearchPattern.Controller);
            assetTypes.Add("Mat", FileSearchPattern.Material);
            assetTypes.Add("Prefab", FileSearchPattern.Prefab);
            assetTypes.Add("Unity", FileSearchPattern.Unity);
            assetTypes.Add("Spriteatlas", FileSearchPattern.Spriteatlas);
            return assetTypes;
        }

    }
    
    public class AssetsConfig : ScriptableObject
    {
        //将资源转成 AB 包的集合
        private readonly Dictionary<string,string> _assetToAB = new Dictionary<string, string>();
        //资产之间的冲突
        private readonly Dictionary<string, string[]> _conflicted = new Dictionary<string, string[]>();
        //重复资源
        private readonly List<string> _duplicated = new List<string>();
        //记录所有的资源
        private readonly Dictionary<string, HashSet<string>> _tracker = new Dictionary<string, HashSet<string>>();
        
        [Header("Serach Asset Rule")]
        public List<LocalFile> LocalFiles;
        
        [Header("Asset Bundle")]
        public List<ABInfo> AssetBundleBuilds;



        public void InitRules()
        {
            _tracker?.Clear();
            _duplicated?.Clear();
            _conflicted?.Clear();
            _assetToAB?.Clear();
            LocalFiles?.Clear();
            AssetBundleBuilds?.Clear();
            if (LocalFiles == null) LocalFiles = new List<LocalFile>();
            if (LocalFiles.Count <= 0)
            {
                var assetTypes = LocalFile.QueryRules();
                foreach (var item in assetTypes)
                {
                    LocalFiles.Add(new LocalFile()
                    {
                        name = item.Key,
                        LocalFilePaths = LocalFile.QueryFilePath(item.Value),
                        SearchPattern = item.Value
                    });
                }
            }

            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        /// <summary>
        /// 收集所有可以被打包的资源
        /// </summary>
        public void CollectAllResources()
        {
            Debug.Log("开始收集所有可以被打包的资源");
            foreach (LocalFile lf in LocalFiles)
            {
                foreach (string item in lf.LocalFilePaths)
                {
                    string abName = SecurityTools.GetMD5Hash(item) + ABHelper.Extension;//目前选择 hash 来进行编码名字
                    string abName_1 = item.Replace("\\", "/").ToLower() + ABHelper.Extension;//不采用路径进行编码
                    Debug.Log(abName_1);
                    Debug.Log(abName);
                    _assetToAB[item] = abName;
                }
            }
        }
        
        /// <summary>
        /// 所有需要打包的资源
        /// </summary>
        /// <returns></returns>
        public AssetBundleBuild[] QueryAssetBundleBuilds()
        {
            var builds = new List<AssetBundleBuild>();
            foreach (ABInfo ab in AssetBundleBuilds)
            {
                builds.Add(new AssetBundleBuild
                {
                    assetBundleName = ab.assetBundleName,
                    assetNames = ab.assetNames
                });
            }

            return builds.ToArray();
        }

        
        public static AssetsConfig QueryAssetsConfig()
        {
            string path = "Assets/ThirdPartyLibraries/Assets/Editor/AssetsConfig.asset";
            AssetsConfig assetsConfig = AssetDatabase.LoadAssetAtPath<AssetsConfig>(path);
            if (null == assetsConfig)
            {
                assetsConfig = ScriptableObject.CreateInstance<AssetsConfig>();
                assetsConfig.InitRules();
                AssetDatabase.CreateAsset(assetsConfig, path);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
            return assetsConfig;
        }
        
    }
}

