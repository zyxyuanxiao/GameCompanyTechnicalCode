using System;
using System.Collections.Generic;
using UnityEditor;

namespace GameAssets
{
    [Serializable]
    public class ABBuildInfo
    {
        //整个 AB 包的名字
        public string assetBundleName;

        //AB 包内部的资源的名字,多个资源可以合并一个 AB 包
        public List<string> assetNames = new List<string>();
        
        /// <summary>
        /// 将所有的资源组合成一个个的 AB 包,然后组装到 AssetBundleBuild 里面
        /// 即:一个 AB 包里面,包含哪些资源?
        /// 这些资源路径都是以 Assets/ 文件夹开头的
        /// </summary>
        /// <returns></returns>
        public AssetBundleBuild ToABB()
        {
            return new AssetBundleBuild()
            {
                assetBundleName = assetBundleName,
                assetNames = assetNames.ToArray(),
            };
        }
    }
}