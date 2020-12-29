using System;
using UnityEngine;
using System.Text;
using SBC = LuaInterface.StringBuilderCache;


using AssetBundleName = System.String;
using EditorAssetURI = System.String; 
using RuntimeAssetName = System.String;
using UnityObject = UnityEngine.Object;

namespace Best
{
    namespace ResourceSys
    {
        public partial class ResourceManager : MonoBehaviour
        {
            public class ImplicitRef : IAssetRef
            {
                public RuntimeAssetName AssetName { get; private set; }
                public AssetBundle Bundle { get; private set; }
                public UnityObject Asset
                {
                    get
                    {
                        return m_assetWeakRef.Target as UnityObject;
                    }
                }

                public ImplicitRef(RuntimeAssetName assetName, AssetBundle bundle, UnityObject asset, ResourceManager mgr)
                {
                    if (asset == null)
                    {
                        StringBuilder sb25 = SBC.Acquire();
                        sb25.Append("Create ImplicitRef:");
                        sb25.Append(assetName);
                        sb25.Append(" with null asset");
                        mgr.LogicError(SBC.GetStringAndRelease(sb25));
                    }
                    ReInit(assetName, bundle, asset, mgr);
                }

                public void ReInit(RuntimeAssetName assetName, AssetBundle bundle, UnityObject asset, ResourceManager mgr)
                {
                    AssetName = assetName;
                    Bundle = bundle;
                    m_assetWeakRef = new WeakReference(asset);
                    m_resMgr = mgr;
                    if (m_resMgr.IsABLoadRefRecord)
                    {
                        m_resMgr.ABLoadtRefRecord(assetName);
                    }
                }

                public void IncreaseRef(int count = 1)
                {
                    //do nothing
                }

                public void DecreaseRef(string name = "",bool isRelease = true)
                {
                    //do nothing
                }

                public void Reset()
                {
                    AssetName = null;
                    Bundle = null;
                    m_assetWeakRef = null;
                }

                private WeakReference m_assetWeakRef;
                private ResourceManager m_resMgr;
            }
        }
    }
}