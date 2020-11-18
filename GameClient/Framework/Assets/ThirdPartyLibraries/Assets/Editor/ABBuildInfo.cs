using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace GameAssets
{
    /// <summary>
    /// 一个此类,一个 AB 包
    /// </summary>
    [Serializable]
    public sealed class ABBuildInfo
    {
        //整个 AB 包的名字
        public string assetBundleName;
        
        [Serializable]
        public sealed class PathAndDepPath
        {
            public string   filePath;//本身资源路径
            public string[] depsPath;//资源所依赖的资源路径
        }
        //AB 包内部的资源的名字,多个资源可以合并一个 AB 包
        public List<PathAndDepPath> assetPathAndDepPaths = new List<PathAndDepPath>();
        
        /// <summary>
        /// 将所有的资源组合成一个个的 AB 包,然后组装到 AssetBundleBuild 里面
        /// 即:一个 AB 包里面,包含哪些资源?
        /// 这些资源路径都是以 Assets/ 文件夹开头的
        /// </summary>
        /// <returns></returns>
        public AssetBundleBuild ToABB()
        {
            var assetNames = new List<string>();
            foreach (PathAndDepPath item in assetPathAndDepPaths)
            {
                assetNames.Add(item.filePath);
            }
            AssetBundleBuild abb = new AssetBundleBuild();
            abb.assetBundleName = assetBundleName;
            abb.assetNames = assetNames.ToArray();
            return abb;
        }
    }
}