using System;
using UnityEngine;

namespace GameAssets
{
    /// <summary>
    /// 资源引用
    /// </summary>
    [Serializable]
    public class LocalFileInfo
    {
        public string Name;
        public string ABName;
        public string LocalDirectory;
    }
    
    /// <summary>
    /// AB 包引用
    /// </summary>
    [Serializable]
    public class ABInfo
    {
        public string Name;
        public int Id;
        public string[] Dependencies;//此 AB 包的依赖关系,如果有,就是依赖其他包
        public long Length;
        public string Hash;
    }

    public class ABManifest : ScriptableObject
    {
        public string[] ActiveVariants;
        public string[] LocalDirectorys;
        public LocalFileInfo[] LocalFileInfos;
        public ABInfo[] ABInfos;
    }
}