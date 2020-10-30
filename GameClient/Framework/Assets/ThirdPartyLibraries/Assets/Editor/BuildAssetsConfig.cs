using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using GameAssets;
using Common;


namespace GameAssets
{

    /// <summary>
    /// 文件夹的搜索模式,
    /// 搜索文件夹及其子文件夹的所有内容,根据文件搜索模式过滤不属于的文件
    /// 打包规则:搜索  _BuildAsset  文件夹下的所有类型的文件,对这些类型的文件进行打包
    /// 某些类型的文件,必定可以成为一个 AB 包,比如  Hdr Asset  TTF  Shader  Prefab  Spriteatlas  Unity
    /// 剩下类型的文件根据其大小进行 AB 包的融合
    /// 
    /// 所有的 Bytes Json XML (文本)文件将其压缩到一个压缩包,再将压缩包打进一个 AB 包里面,
    /// 使用时加载 AB 包并且解压到项目本地,使用解压过后的文件,直接使用 bytes json xml 为后缀的名字,不再使用压缩包
    /// 此类型在初始化的时候与 Shader 统一加载
    /// </summary> 
    public static class FileSearchPattern
    {
        public static string Zip = "*.zip";
        public static string Png = "*.png";
        public static string Hdr = "*.hdr"; //一个 Hdr(大型图片) 必定是一个 AB 包
        public static string Asset = "*.asset"; //一个 asset(配置文件) 必定是一个 AB 包
        public static string TTF = "*.ttf"; //一个 ttf 必定是一个 AB 包
        public static string Shader = "*.shader"; //所有的 shader 必定是一个 AB 包
        public static string Controller = "*.controller";
        public static string Material = "*.mat";
        public static string Prefab = "*.prefab"; //一个 prefab 必定是一个 AB 包
        public static string Spriteatlas = "*.spriteatlas"; //一个图集必定是一个 AB 包
        public static string Unity = "*.unity"; //一个scene(后缀名为.unity)场景必定是一个 AB 包


        public static string BuildAsset = "_BuildAsset"; //一个scene(后缀名为.unity)场景必定是一个 AB 包

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

        [Tooltip("搜索当前文件夹下的通配符")] public string SearchPattern;

        [Tooltip("在本地Assets/的路径,从其中进行查找文件资源进行打热更包,初始化后需要自行设置")]
        public string[] LocalFilePaths;

        //获取文件夹内的所有文件资源        
        public static string[] QueryFilePath(string searchPattern)
        {
            string path = Application.dataPath.Replace("\\", "/") + "/" + FileSearchPattern.BuildAsset + "/";
            string[] localFilePaths = Directory.GetFiles(path, searchPattern, SearchOption.AllDirectories);
            List<string> filePaths = new List<string>();
            foreach (string item in localFilePaths)
            {
                if (Directory.Exists(item)) continue; //如果是文件夹则跳出
                if (!FileSearchPattern.ValidateAsset(item)) continue; //不通过则跳出
                var ext = Path.GetExtension(item).ToLower();
                if ((ext == ".fbx" || ext == ".anim") && !item.Contains(ext)) continue; //fbx或者anim都不打进游戏内
                string temp = item.Replace(Application.dataPath, "");
                filePaths.Add(temp.Replace("\\", "/"));
            }

            return filePaths.ToArray();
        }

        public static Dictionary<string, string> QueryRules()
        {
            Dictionary<string, string> assetTypes = new Dictionary<string, string>();
            assetTypes.Add("Zip", FileSearchPattern.Zip);
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

    /// <summary>
    /// _BuildAsset 文件夹下的资源,
    /// 1:必定会被打进 AB 包里面;
    /// 2:必定以此文件夹下的资源为 AB 包的 Root 根节点,进行查找依赖项打包;
    /// 3:AB 包的名字以此文件夹下的名字为准,采取 MD5 名称进行加载
    /// 4:_BuildAsset文件夹中的资源依赖于_EditorAsset文件夹中的资源,即 _BuildAsset的大型资源 组合 _EditorAsset中的小型资源
    /// 5:_BuildAsset文件夹中的资源尽量不要小于 100KB,小于 100KB 的资源尽量放在其他文件夹中,最后被引用打包,而不是单独打包
    /// </summary>
    public class BuildAssetsConfig : ScriptableObject
    {
        /// <summary>
        /// 将资源转成 AB 包的集合,多个资源路径对应一个 AB 包,也就是 Key 不同,但是 AB 包的名字相同
        /// </summary>
        private readonly Dictionary<string, string> _assetToAB = new Dictionary<string, string>();

        //资产之间的冲突
        private readonly Dictionary<string, string[]> _conflicted = new Dictionary<string, string[]>();

        /// <summary>
        /// 有很多个资源被引用了多次,记录其路径以及引用次数,然后对其进行优化组合打包
        /// </summary>
        private readonly Dictionary<string, int> _duplicated = new Dictionary<string, int>();

        /// <summary>
        /// 记录所有资源所对应的 AB 包
        /// </summary>
        private readonly Dictionary<string, HashSet<string>> _tracker = new Dictionary<string, HashSet<string>>();

        [Header("Serach Asset Rule")] public List<LocalFile> LocalFiles;

        [Header("Asset Bundle")] public List<ABInfo> AssetBundleBuilds;



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

        public void BuildAllAB()
        {
            this.ZipAllTextFile(); //压缩所有的文本文件
            this.CollectAllAssets(); //收集所有可以打成 AB 包的资源
            this.AnalysisAssetsDependencies(); //分析 AB 包的所有资源依赖关系
            this.OptimizeAssets(); //对依赖关系进行优化
            this.CombineABWithAssets(); //将所有资源组合成 AB 包
        }


        /// <summary>
        /// 压缩所有的文本文件
        /// </summary>
        private void ZipAllTextFile()
        {
            List<FileInfo> zipInfos = new List<FileInfo>();
            string[] texts = new[] {"*.json", "*.bytes", "*.xml"};
            foreach (string searchPattern in texts)
            {
                string[] filePaths = Directory.GetFiles(Application.dataPath + "/_BuildAsset/",
                    searchPattern,
                    SearchOption.AllDirectories);
                foreach (string filePath in filePaths) //把所有的文件抽出来进行压缩
                {
                    zipInfos.Add(new FileInfo(filePath));
                }
            }

            string zipPath = Application.dataPath + "/_BuildAsset/Text/";
            if (!LZ4Helper.CompressDirectory(zipPath, zipInfos.ToArray(), zipPath + "Text.zip"))
            {
                Debug.LogError("压缩包压缩失败");
            }
        }

        /// <summary>
        /// 收集所有可以被打包的资源
        /// 收集规则是 多个资源对应一个 AB 包
        /// 多对一
        /// </summary>
        private void CollectAllAssets()
        {
            foreach (LocalFile lf in LocalFiles)
            {
                foreach (string item in lf.LocalFilePaths)
                {
                    string fullPath = Application.dataPath + item;
                    //记录所有资源,多个资源对应一个 md5Hash,此时的颗粒度不是最小的,但是必定会被打进 AB 包里面,因为 AB 包是以此为基础创建每个 AB 包的
                    _assetToAB[item] = QueryABName(fullPath);
                    // Debug.Log(item + "    "  + _assetToAB[item]);
                }
            }
        }

        /// <summary>
        /// 根据收集资产来进行的处理
        /// 对应规则:是一个 AB 包的名字对应多个资源路径,一对多
        /// 也就是 AB 包名字作为 Key, 多个资源路径作为 Value
        /// </summary>
        /// <returns></returns>
        private Dictionary<string, List<string>> QueryABMap()
        {
            Dictionary<string, List<string>> abMap = new Dictionary<string, List<string>>();
            foreach (var item in _assetToAB)
            {
                string bundle = item.Value;
                if (!abMap.TryGetValue(bundle, out List<string> list))
                {
                    list = new List<string>();
                    abMap[item.Value] = list;
                }

                //因为AssetDatabase.GetDependencies方法必须以Assets开头.所以必须要添加Assets
                if (!list.Contains(item.Key)) list.Add("Assets" + item.Key);
            }

            return abMap;
        }

        /// <summary>
        /// 对所有需要进行打包的资源,进行分析;对依赖的资源进行分析,如果资源不在_assetToAB中,则记录在_duplicated中
        /// </summary>
        private void AnalysisAssetsDependencies()
        {
            Dictionary<string, List<string>> abMap = QueryABMap();
            foreach (var item in abMap)
            {
                string abName = item.Key;
                //获取当前abName对应的文件路径的所有依赖资产,
                //例如:一个场景可以包括很多材质,shader,NAV,贴图等等其他资产,这些其他资产被称为依赖资产
                string[] dependencies = AssetDatabase.GetDependencies(item.Value.ToArray(), true);
                if (dependencies.Length <= 0) continue;
                //这个AB包对应的资源类型 有其他资源依赖,记录单个资产被多个 AB 包引用
                foreach (string asset in dependencies) //asset 表示一种资源,asset可以是文件夹
                {
                    // Debug.Log(asset + " 依赖于 " + abName);
                    if (!_tracker.TryGetValue(asset, out HashSet<string> hashABName))
                    {
                        hashABName = new HashSet<string>();
                        _tracker.Add(asset, hashABName); //每个资源对应一个 HashSet(无序列表,插入元素快)
                    }

                    hashABName.Add(abName); //记录这个asset(资源),被多少个 ab 包引用了
                    if (hashABName.Count <= 1) continue;
                    _assetToAB.TryGetValue(asset.Replace("Assets", ""), out string md5Hash);
                    if (string.IsNullOrEmpty(md5Hash))
                    {
                        // Debug.Log(asset + " 被多个 AB 包依赖: " + abName + "    " + bundleName);
                        _duplicated[asset.Replace("Assets", "")] =
                            hashABName.Count; //这个资产并没有在 _BuildAsset 文件夹下面,是在其他文件夹下面的
                    }
                }
            }
        }

        /// <summary>
        /// 优化 AB 包的引用分布
        /// 比如当多个 AB 包引用了一份资源,则将这份资源单独打包成一个 AB 包
        /// 当多个 AB 包引用了一份小于 1KB 的资源,不会将这份资源单独打成多个 AB 包
        /// 当一个场景依赖一个 AB 包,AB 包又依赖于另外一的资源,则需要单独进行处理场景,场景不应该添加 prefab,需要自己设置一套干净的场景配置
        /// </summary>
        private void OptimizeAssets()
        {
            foreach (var item in _duplicated)
            {
                string fullPath = Application.dataPath + item.Key;
                FileInfo fileInfo = new FileInfo(fullPath);
                // 如果依赖次数小于 5 次,并且其本身size 小于 5KB ,则无需单独打一个包,让其跟随其他 AB 包一起打,即允许资源冗余存在
                if (item.Value < 5 && fileInfo.Length / 1024 < 5)
                    continue; 
                _assetToAB[item.Key] = QueryABName(fullPath);
                // Debug.Log(item.Key + " 被多个 AB 包依赖了 " + item.Value + " 次");
            }
        }

        /// <summary>
        /// 根据 Asset(资源)组合成一个 AB 包
        /// 资源已经分门别类的装载到_assetToAB里面了,最后就是组合一个 AB 包里面包含多少个资源了
        /// </summary>
        private void CombineABWithAssets()
        {
            AssetBundleBuilds?.Clear();
            if (AssetBundleBuilds == null) AssetBundleBuilds = new List<ABInfo>();
            Dictionary<string, List<string>> abMap = QueryABMap();
            foreach (var item in abMap)
            {
                AssetBundleBuilds.Add(new ABInfo()
                {
                    assetBundleName = item.Key,
                    assetNames = item.Value.ToArray() //必须是以 Asset 开头
                });
            }

            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        /// <summary>
        /// 根据文件类型进行编码,返回出一个 md5Hash
        /// </summary>
        /// <param name="filePath"></param>
        private string QueryABName(string fullPath)
        {
            string md5Hash = SecurityTools.GetMD5Hash(fullPath) + ABHelper.Extension; //目前选择 hash 来进行编码名字
            if (fullPath.EndsWith(".shader"))
            {
                md5Hash = SecurityTools.GetMD5Hash("shaderlibs") + ABHelper.Extension;
            }

            md5Hash = Regex.Split(fullPath,"Assets",RegexOptions.IgnoreCase).Last();
            md5Hash = md5Hash.Replace("/", "_");
            return md5Hash;
        }

        /// <summary>
        /// 所有需要打包的资源,准备打包前一步
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


        public static BuildAssetsConfig QueryAssetsConfig()
        {
            string path = "Assets/ThirdPartyLibraries/Assets/Editor/BuildAssetsConfig.asset";
            BuildAssetsConfig assetsConfig = AssetDatabase.LoadAssetAtPath<BuildAssetsConfig>(path);
            if (null == assetsConfig)
            {
                assetsConfig = ScriptableObject.CreateInstance<BuildAssetsConfig>();
                assetsConfig.InitRules();
                AssetDatabase.CreateAsset(assetsConfig, path);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }

            return assetsConfig;
        }

    }
}