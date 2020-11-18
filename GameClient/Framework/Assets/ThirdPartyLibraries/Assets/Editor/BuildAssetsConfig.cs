using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using GameAssets;
using Common;
using Tool = Common.Tool;


namespace GameAssets
{
    /// <summary>
    /// _BuildAsset 文件夹下的资源,
    /// 1:必定会被打进 AB 包里面;
    /// 2:必定以此文件夹下的资源为 AB 包的 Root 根节点,进行查找依赖项打包;
    /// 3:AB 包的名字以此文件夹下的名字为准,采取 MD5 名称进行加载
    /// 4:_BuildAsset文件夹中的资源依赖于_EditorAsset文件夹中的资源,即 _BuildAsset的大型资源 组合 _EditorAsset中的小型资源
    /// 5:_BuildAsset文件夹中的资源尽量不要小于 100KB,小于 100KB 的资源尽量放在其他文件夹中,最后被引用打包,而不是单独打包
    /// </summary>
    public sealed class BuildAssetsConfig : ScriptableObject
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

        [Header("LocalFiles Path: Assets/BuildAssets/")] 
        [Tooltip("不可手动修改时,都要进行代码的修改")]
        public List<LocalFile> LocalFiles;
        
        [Tooltip("仅仅展示名字,不要修改,每次打包后会更新")]
        public string AssetBundleManifest;
        
        [Tooltip("展示 AB 包的名字,不要修改,每次打包后会更新")]
        public List<ABBuildInfo> AssetBundleBuildInfos;
        

        /// <summary>
        /// 获取当前BuildAssetsConfig的实体对象
        /// </summary>
        /// <returns></returns>
        public static BuildAssetsConfig QueryAssetsConfig()
        {
            string path = EitorTools.GetScriptPath(typeof(BuildAssetsConfig)) + "BuildAssetsConfig.asset";;
            BuildAssetsConfig assetsConfig = AssetDatabase.LoadAssetAtPath<BuildAssetsConfig>(path);
            if (null == assetsConfig)
            {
                assetsConfig = ScriptableObject.CreateInstance<BuildAssetsConfig>();
                assetsConfig.UpdateRules();
                AssetDatabase.CreateAsset(assetsConfig, path);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }

            return assetsConfig;
        }
        
        /// <summary>
        /// 初始化打包规则
        /// </summary>
        public void UpdateRules()
        {
            _tracker?.Clear();
            _duplicated?.Clear();
            _conflicted?.Clear();
            _assetToAB?.Clear();
            LocalFiles?.Clear();
            AssetBundleBuildInfos?.Clear();
            if (LocalFiles == null) LocalFiles = new List<LocalFile>();
            if (LocalFiles.Count <= 0)
            {
                var assetTypes = LocalFile.QueryRules();
                foreach (var item in assetTypes)
                {
                    LocalFiles.Add(new LocalFile()
                    {
                        FileType = item.Key,
                        LocalFilePaths = LocalFile.QueryFilePath(item.Value),
                        SearchPattern = item.Value
                    });
                }
            }

            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        #region Build All

        public void BuildAll()
        {
            UpdateRules();
            AssetBundleManifest = Common.Tool.QueryPlatform();
            CollectAllAssets(); //收集所有可以打成 AB 包的资源
            AnalysisAssetsDependencies(); //分析 AB 包的所有资源依赖关系
            OptimizeAssets(); //对依赖关系进行优化
            CombineABWithAssets(); //将所有资源组合成 AB 包
            ZipAllTextFile(); //压缩所有的文本文件
        }


        /// <summary>
        /// 压缩所有的文本文件
        /// </summary>
        public void ZipAllTextFile()
        {
            List<FileInfo> zipInfos = new List<FileInfo>();
            string[] texts = new[] {"*.json", "*.bytes", "*.xml"};
            foreach (string searchPattern in texts)
            {
                string[] filePaths = Directory.GetFiles(Application.dataPath + "/" + FileFilter.BuildAssets + "/",
                    searchPattern,
                    SearchOption.AllDirectories);
                foreach (string filePath in filePaths) //把所有的文件抽出来进行压缩
                {
                    zipInfos.Add(new FileInfo(filePath));
                }
            }

            string dirPath = Application.dataPath + "/" + FileFilter.BuildAssets + "/";
            string zipFileName = BuildAssetsHelper.DownloadAssetsDirectory + FileFilter.AllText;
            if (!LZ4Helper.CompressDirectory(dirPath, zipInfos.ToArray(), zipFileName))
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
            _assetToAB?.Clear();
            foreach (LocalFile lf in LocalFiles)
            {
                foreach (string path in lf.LocalFilePaths)
                {
                    if (!FileFilter.QueryFileToAB(path))continue;
                    string fullPath = Application.dataPath + path;
                    //记录所有资源,多个资源对应一个 md5Hash,此时的颗粒度不是最小的,但是必定会被打进 AB 包里面,因为 AB 包是以此为基础创建每个 AB 包的
                    if (!_assetToAB.TryGetValue(path,out string abname))
                    {
                        _assetToAB[path] = QueryABName(fullPath);
                    }
                    else
                    {
                        Debug.LogError("文件夹中已有一个相同类型,相同名字的资源,请先确定此资源是否正常,建议修改资源名字:" + abname);
                    }
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
                if (!list.Contains(item.Key)) list.Add(item.Key);
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
                foreach (string filePath in dependencies) //asset 表示一种资源,asset可以是文件夹
                {
                    if (!FileFilter.QueryFileToAB(filePath)) continue; //符合打包文件的设置
                    // Debug.Log(asset + " 依赖于 " + abName);
                    if (!_tracker.TryGetValue(filePath, out HashSet<string> hashABName))
                    {
                        hashABName = new HashSet<string>();
                        _tracker.Add(filePath, hashABName); //每个资源对应一个 HashSet(无序列表,插入元素快)
                    }

                    hashABName.Add(abName); //记录这个asset(资源),被多少个 ab 包引用了
                    if (hashABName.Count <= 1) continue;
                    _assetToAB.TryGetValue(filePath, out string md5Hash);
                    if (string.IsNullOrEmpty(md5Hash))
                    {
                        // Debug.Log(asset + " 被多个 AB 包依赖: " + abName + "    " + bundleName);
                        //这个资产并没有在 _BuildAsset 文件夹下面,是在其他文件夹下面的
                        _duplicated[filePath] = hashABName.Count;
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
                string fullPath = Application.dataPath.Replace("Assets","") + item.Key;
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
            AssetBundleBuildInfos?.Clear();
            if (AssetBundleBuildInfos == null) AssetBundleBuildInfos = new List<ABBuildInfo>();
            Dictionary<string, List<string>> abMap = QueryABMap();
            foreach (var item in abMap)
            {
                var abbi = new ABBuildInfo();
                abbi.assetBundleName = item.Key;
                foreach (string file in item.Value)
                {
                    var anad = new ABBuildInfo.PathAndDepPath();
                    anad.filePath = file;
                    var deps = new List<string>();
                    foreach (string depName in AssetDatabase.GetDependencies(file))
                    {
                        if (depName.Equals(anad.filePath))continue;
                        deps.Add(depName);
                    }
                    anad.depsPath = deps.ToArray();
                    abbi.assetPathAndDepPaths.Add(anad);
                }
                AssetBundleBuildInfos.Add(abbi);
            }
            AssetBundleBuildInfos.Sort((a,b)=>a.assetBundleName.CompareTo(b.assetBundleName));
            
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        /// <summary>
        /// 根据文件类型进行编码,返回出一个 AssetBundle 的名字
        /// </summary>
        /// <param name="fileType">文件类型</param>
        /// <param name="fullPath">文件的完整路径</param>
        /// <returns></returns>
        private string QueryABName(string fullPath)
        {
            //Path.GetExtension(fullPath).ToLower() + "_" + 
            string abName = Path.GetFileName(fullPath).ToLower() + AssetsHelper.Extension; 
            if (fullPath.EndsWith(".shader"))
            {
                return "shaderlibs" + AssetsHelper.Extension;
            }
            return abName;

            // string md5Hash = SecurityTools.GetMD5Hash(fullPath) + AssetBundleConfig.Extension; //选择 hash 来进行编码名字
            // if (fullPath.EndsWith(".shader"))
            // {
            //     md5Hash = SecurityTools.GetMD5Hash("shaderlibs") + AssetBundleConfig.Extension;
            // }
            // return md5Hash;
        }

        /// <summary>
        /// 所有需要打包的资源,准备打包前一步
        /// </summary>
        /// <returns></returns>
        public AssetBundleBuild[] QueryAssetBundleBuilds()
        {
            var builds = new List<AssetBundleBuild>();
            foreach (ABBuildInfo ab in AssetBundleBuildInfos)
            {
                builds.Add(ab.ToABB());
            }
            return builds.ToArray();
        }
        
        #endregion
    }

    #region BuildAssetsConfigEditor
    
    [CanEditMultipleObjects]
    [CustomEditor(typeof(BuildAssetsConfig))]
    public class BuildAssetsConfigEditor : Editor
    {
        
        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("更新打包配置文件"))
            {
                BuilderMenuItems.UpdateAssetsConfig();
            }
            GUI.enabled = false;
            base.DrawDefaultInspector();
            GUI.enabled = true;
        }
    }
    #endregion
}