namespace DLCAssets
{
    public class ABRefrence : Reference
    {
        public string ABName { get; private set; }
        public UnityEngine.AssetBundle AssetBundle { get; private set; }
        public UnityEngine.Object UncompressAsset { get; private set; }
        // public UnityEngine.Object Obj { get; private set; }
        
        private int m_refCount;//引用计数
        public int RefCount
        {
            get
            {
                return m_refCount;
            }
        }

        /// <summary>
        /// 初始化记录引用的对象
        /// </summary>
        /// <param name="abName"></param>
        /// <param name="assetBundle"></param>
        /// <param name="uncompressAsset"></param>
        public ABRefrence(string abName, UnityEngine.AssetBundle assetBundle, UnityEngine.Object uncompressAsset)
        {
            Reset(abName,assetBundle,uncompressAsset);
        }
    
        /// <summary>
        /// 增加引用
        /// </summary>
        public void Retain()
        {
            m_refCount++;
        }
        
        /// <summary>
        /// 减小引用
        /// </summary>
        public void Release()
        {
            m_refCount--;
        }
        
        /// <summary>
        /// 重置引用的对象
        /// </summary>
        /// <param name="abName"></param>
        /// <param name="assetBundle"></param>
        /// <param name="uncompressAsset"></param>
        public void Reset(string abName = null, UnityEngine.AssetBundle assetBundle = null, UnityEngine.Object uncompressAsset = null)
        {
            ABName = abName;
            AssetBundle = assetBundle;
            UncompressAsset = uncompressAsset;
            m_refCount = 0;
        }

        public void CheckRelease()
        {
            if (m_refCount <= 0)
            {
                
            }
        }
        

    }
}