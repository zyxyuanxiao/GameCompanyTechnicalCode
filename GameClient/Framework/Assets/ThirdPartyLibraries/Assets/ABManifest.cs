using System;
using UnityEngine;

namespace GameAssets
{
    /// <summary>
    /// 资源引用
    /// </summary>
    [Serializable]
    public class AssetRef
    {
        public string name;
        public int bundle;
        public int dir;
    }
    
    /// <summary>
    /// AB 包引用
    /// </summary>
    [Serializable]
    public class BundleRef
    {
        public string name;
        public int id;
        public int[] deps;
        public long len;
        public string hash;
    }

    public class ABManifest : ScriptableObject
    {
        public string[] activeVariants;
        public string[] dirs;
        public AssetRef[] assets;
        public BundleRef[] bundles;
    }
}