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

        
        public ABRefrence(string abName, UnityEngine.AssetBundle assetBundle, UnityEngine.Object uncompressAsset)
        {
            ReInit(abName,assetBundle,uncompressAsset);
        }

        public void ReInit(string abName, UnityEngine.AssetBundle assetBundle, UnityEngine.Object uncompressAsset)
        {
            this.ABName = abName;
            this.AssetBundle = assetBundle;
            this.UncompressAsset = uncompressAsset;
            m_refCount = 0;
        }
        
        
        public void Retain()
        {
            m_refCount++;
        }

        public void Release()
        {
            m_refCount--;
        }

        public void Reset()
        {

            ABName = null;
            AssetBundle = null;
            UncompressAsset = null;
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