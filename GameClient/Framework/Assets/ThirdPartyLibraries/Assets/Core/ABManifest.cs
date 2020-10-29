using System;
using UnityEngine;

namespace GameAssets
{
    [Serializable]
    public class AssetRef
    {
        public string name;
        public int bundle;
        public int dir;
    }

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