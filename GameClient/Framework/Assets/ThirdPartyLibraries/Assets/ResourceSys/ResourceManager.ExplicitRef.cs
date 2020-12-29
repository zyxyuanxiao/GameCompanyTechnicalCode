using System.Collections.Generic;
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
            public class ExplicitRef : IAssetRef
            {
                public RuntimeAssetName AssetName { get; private set; }
                public AssetBundle Bundle { get; private set; }
                public UnityObject Asset { get; private set; }

                public ExplicitRef(RuntimeAssetName assetName, AssetBundle bundle, UnityObject asset, ResourceManager mgr)
                {
                    if (asset == null)
                    {
                        StringBuilder sb28 = SBC.Acquire();
                        sb28.Append("Create ExplicitRef :");
                        sb28.Append(assetName);
                        sb28.Append(" with null asset");
                        mgr.LogicError(SBC.GetStringAndRelease(sb28));
                    }
                    ReInit(assetName, bundle, asset, mgr);
                }
                public void ReInit(RuntimeAssetName assetName, AssetBundle bundle, UnityObject asset, ResourceManager mgr)
                {

                    AssetName = assetName;
                    Bundle = bundle;
                    Asset = asset;
                    m_refCount = 0;
                    m_resMgr = mgr;

                    if (m_resMgr.IsABLoadRefRecord)
                    {
                        m_resMgr.ABLoadtRefRecord(assetName);
                    }
                }


                public void IncreaseRef(int count = 1)
                {
                    m_refCount += count;

                    //if (!BuildManifestUtility.GetBuildManifest().IsReleaseVer && m_resMgr.IsDebugTarget(AssetName))
                    //{
                    //    StringBuilder sb29 = SBC.Acquire();
                    //    sb29.Append("ExplicitRef: ");
                    //    sb29.Append(AssetName);
                    //    sb29.Append(" increase ref: ");
                    //    sb29.Append(m_refCount);
                    //    LogModule.Instance.Trace(LogModule.LogModuleCode.ResRefStatus, SBC.GetStringAndRelease(sb29));
                    //}
                }

                public void IncreaseRefRecursively(int count = 1)
                {
                    IncreaseRef(count);

                    List<RuntimeAssetName> deps = m_resMgr.GetDepInfo(AssetName);
                    if (deps.Count > 0)
                    {
                        for (int i = 0; i < deps.Count; i++)
                        {
                            ExplicitRef childExpRef = m_resMgr.GetExplicitRef(deps[i]);
                            if (childExpRef == null)
                            {
                                ImplicitRef childImpRef = m_resMgr.GetImplicitRef(deps[i]);
                                if (childImpRef == null)
                                {
                                    StringBuilder sb30 = SBC.Acquire();
                                    sb30.Append("ExplicitRef : ");
                                    sb30.Append(AssetName);
                                    sb30.Append(" does not have child asset ref : ");
                                    sb30.Append(deps[i]);
                                    m_resMgr.LogicError(SBC.GetStringAndRelease(sb30));
                                }
                                else
                                {
                                    childExpRef = m_resMgr.CreateExplicitRef(deps[i], childImpRef.Asset, childImpRef.Bundle);
                                }
                            }

                            if (childExpRef != null)
                                childExpRef.IncreaseRefRecursively(count);
                        }
                    }
                    m_resMgr.m_depInfoPool.Return(deps);
                }

                /// <summary>
                /// 
                /// </summary>
                /// <param name="parent"></param>
                /// <param name="isRelease">异步加载的时候类还在，释放的话会导致ab被释放，所以等加载类回收的时候再做判断</param>
                public void DecreaseRef(string parent = "",bool isRelease = true)
                {
                    m_refCount--;

                    if (!BuildManifestUtility.GetBuildManifest().IsReleaseVer && m_resMgr.IsDebugTarget(AssetName))
                    {
                        StringBuilder sb31 = SBC.Acquire();
                        sb31.Append("ExplicitRef: ");
                        sb31.Append(parent);
                        sb31.Append(" + ");
                        sb31.Append(AssetName);
                        sb31.Append(" decrease ref: ");
                        sb31.Append(m_refCount);
                        LogModule.Instance.Trace(LogModule.LogModuleCode.ResRefStatus, SBC.GetStringAndRelease(sb31));
                    }
                    if (m_refCount == 0 && isRelease)
                    {
                        m_resMgr.RemoveExplicitRef(AssetName);
                    }
                    else if (m_refCount < 0)
                    {
                        StringBuilder sb32 = SBC.Acquire();
                        sb32.Append("Explicit Ref : ");
                        sb32.Append(AssetName);
                        sb32.Append(" got invalid reference count: ");
                        sb32.Append(m_refCount);
                        m_resMgr.LogicError(SBC.GetStringAndRelease(sb32));
                    }
                }

                /// <summary>
                /// 检查能不能回收
                /// </summary>
                public void CheckCanRelease()
                {
                    if (m_refCount == 0)
                    {
                        
                        m_resMgr.RemoveExplicitRef(AssetName);
                    }
                }

                public void DecreaseRefRecursively(string parent = ""/*debug purpose*/)
                {
                    List<RuntimeAssetName> deps = m_resMgr.GetDepInfo(AssetName);
                    if (deps.Count > 0)
                    {
                        for (int i = 0; i < deps.Count; i++)
                        {
                            ExplicitRef childExpRef = m_resMgr.GetExplicitRef(deps[i]);
                            if (childExpRef == null)
                            {
                                StringBuilder sb33 = SBC.Acquire();
                                sb33.Append("ExplicitRef : ");
                                sb33.Append(AssetName);
                                sb33.Append(" does not have child ref : ");
                                sb33.Append(deps[i]);
                                m_resMgr.LogicError(SBC.GetStringAndRelease(sb33));
                            }
                            else
                            {
                                StringBuilder sb34 = SBC.Acquire();
                                sb34.Append(parent);
                                sb34.Append(" + ");
                                sb34.Append(AssetName);
                                childExpRef.DecreaseRefRecursively(SBC.GetStringAndRelease(sb34));
                            }
                        }
                    }
                    m_resMgr.m_depInfoPool.Return(deps);
                    DecreaseRef(parent);
                }

                public void Reset()
                {

                    AssetName = null;
                    Bundle = null;
                    Asset = null;
                    m_refCount = 0;
                }

                private ResourceManager m_resMgr;
                private int m_refCount;

              

                public int RefCount
                {
                    get { return m_refCount; }
                }
            }
        }
    }
}