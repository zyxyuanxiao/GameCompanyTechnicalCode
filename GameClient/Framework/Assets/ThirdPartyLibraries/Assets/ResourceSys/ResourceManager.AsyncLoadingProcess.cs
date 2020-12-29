using System;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using SBC = LuaInterface.StringBuilderCache;

using AssetBundleName = System.String;
using EditorAssetURI = System.String; //
using RuntimeAssetName = System.String;
using UnityObject = UnityEngine.Object;


namespace Best
{
    namespace ResourceSys
    {
        public partial class ResourceManager : MonoBehaviour
        {
            public enum AsyncLoadingState
            {
                IDLE,
                LOADING_DEPENDENT_ASSET,
                LOADING_AB,
                LOADING_ASSET,
                SUCCESS,
                FAILED,
            }

           
            public class AsyncLoadingProcess
            {
                public RuntimeAssetName TargetAssetName;
                public AsyncLoadingState State { get; private set; }
                public bool Finished { get { return State == AsyncLoadingState.FAILED || State == AsyncLoadingState.SUCCESS; } }
                public AssetBundle Bundle { get { return m_bundle; } }



                public UnityObject ForceSyncDone()
                {
                    AssetBundle ab;
                    UnityObject asset;
                    switch (State)
                    {
                        case AsyncLoadingState.FAILED:
                            return null;
                        case AsyncLoadingState.IDLE:
                            Debug.Assert(false, "AsyncLoadingProcess. ForceSyncDone illegal status");
                            return null;
                        case AsyncLoadingState.LOADING_AB:
                            m_loadingABRequest.completed -= OnAssetBundleLoaded;
                            ab = m_loadingABRequest.assetBundle;
                            if (ab == null)
                            {
                                StringBuilder sb18 = SBC.Acquire();
                                sb18.Append("ForceSyncDone LOADING_AB m_loadingABRequest.assetBundle nil : ");
                                sb18.Append(TargetAssetName);
                                m_resMgr.AssetError(SBC.GetStringAndRelease(sb18));
                                OnProcessEnd(null);
                                return null;
                            }
                            m_bundle = ab;
                            asset = ab.LoadAsset(ab.GetAllAssetNames()[0]);
                            if (asset == null)
                            {
                                StringBuilder sb18 = SBC.Acquire();
                                sb18.Append("ForceSyncDone LOADING_AB ab.LoadAsset nil : ");
                                sb18.Append(TargetAssetName);
                                m_resMgr.AssetError(SBC.GetStringAndRelease(sb18));
                            }
                           
                            OnProcessEnd(asset);
                            return asset;
                        case AsyncLoadingState.LOADING_ASSET:
                            m_loadingAssetRequest.completed -= OnAssetLoaded;
                            asset = m_loadingAssetRequest.asset;
                            if (asset == null)
                            {
                                StringBuilder sb18 = SBC.Acquire();
                                sb18.Append("ForceSyncDone LOADING_ASSET nil : ");
                                sb18.Append(TargetAssetName);
                                m_resMgr.AssetError(SBC.GetStringAndRelease(sb18));
                            }
                           
                            OnProcessEnd(asset);
                            return asset;
                        case AsyncLoadingState.LOADING_DEPENDENT_ASSET:
                            List<RuntimeAssetName> deps = m_resMgr.GetDepInfo(TargetAssetName);
                            for (int i = 0; i < deps.Count; i++)
                            {
                                RuntimeAssetName ran = deps[i];
                                AsyncLoadingProcess alp = m_resMgr.GetAsyncLoadingProcess(ran);
                                if (alp != null)
                                {
                                    UnityObject depAsset = alp.ForceSyncDone();
                                    if (depAsset == null)
                                    {
                                        StringBuilder sb18 = SBC.Acquire();
                                        sb18.Append("ForceSyncDone LOADING_DEPENDENT_ASSET nil : ");
                                        sb18.Append(TargetAssetName);
                                        m_resMgr.AssetError(SBC.GetStringAndRelease(sb18));
                                        OnProcessEnd(null);
                                        return null;
                                    }
                                }
                            }
                            ab = AssetBundle.LoadFromFile(RuntimeAssetName2AssetBundleName(TargetAssetName));
                            if (ab == null)
                            {
                                StringBuilder sb18 = SBC.Acquire();
                                sb18.Append("ForceSyncDone  LOADING_DEPENDENT_ASSET AssetBundle.LoadFromFile nil : ");
                                sb18.Append(TargetAssetName);
                                m_resMgr.AssetError(SBC.GetStringAndRelease(sb18));
                                OnProcessEnd(null);
                                return null;
                            }
                            m_bundle = ab;
                            asset = ab.LoadAsset(ab.GetAllAssetNames()[0]);
                            if (asset == null)
                            {
                                StringBuilder sb18 = SBC.Acquire();
                                sb18.Append("ForceSyncDone  LOADING_DEPENDENT_ASSET ab.LoadAsset nil : ");
                                sb18.Append(TargetAssetName);
                                m_resMgr.AssetError(SBC.GetStringAndRelease(sb18));
                            }
                          
                            OnProcessEnd(asset);
                            return asset;
                        case AsyncLoadingState.SUCCESS:
                           
                            return m_asset;
                    }

                    Debug.Assert(false);
                    return null;
                }

                /// <summary>
                /// 为process增加显式加载回调和显式加载creator
                /// </summary>
                /// <param name="resUID"></param>
                /// <param name="onAssetLoaded"></param>
                /// <param name="expCreator"></param>
                public void AddExplicitlyResRef(uint resUID, Action<UnityObject> onAssetLoaded)
                {
                    //if (m_canceled)
                    //    m_canceled = false;

                    if (onAssetLoaded == null)
                    {
                        Debug.LogError("  添加了空回调");
                        return;
                    }
                    m_expUIDs.Add(resUID);
                    IncreaseRefCountRecursively(1);

                    m_onExpAssetLoaded.Add(resUID, onAssetLoaded);
                }
                /// <summary>
                /// 为process增加隐式加载回调和隐式加载creator
                /// </summary>
                /// <param name="onAssetLoaded"></param>
                /// <param name="impCreator"></param>
                public void AddImplicitResRef(Action<UnityObject> onAssetLoaded)
                {
                    //if (m_canceled)
                    //    m_canceled = false;
                    if (onAssetLoaded == null)
                    {
                        Debug.LogError("  隐式添加了空回调");
                        return;
                    }
                    m_onImpAssetLoaded += onAssetLoaded;
                    m_impRef = true;
                }
                #region 得到AsyncLoadingProcess类
                /// <summary>
                /// 创建显式持有加载的process
                /// </summary>
                /// <param name="assetName"></param>
                /// <param name="mgr"></param>
                /// <param name="expCreator"></param>
                /// <param name="resUID"></param>
                /// <param name="onAssetLoaded"></param>
                public AsyncLoadingProcess(RuntimeAssetName assetName, ResourceManager mgr, uint resUID, Action<UnityObject> onAssetLoaded)
                {
                    InitParams(assetName, mgr, resUID, onAssetLoaded);
                }

                private void InitParams(RuntimeAssetName assetName, ResourceManager mgr, uint resUID, Action<UnityObject> onAssetLoaded)
                {

                    Init(assetName, mgr);
                    AddExplicitlyResRef(resUID, onAssetLoaded);
                }

                public static AsyncLoadingProcess Dequeue(RuntimeAssetName assetName, ResourceManager mgr, uint resUID, Action<UnityObject> onAssetLoaded)
                {
                    if (mProcessPool.Count == 0)
                    {
                        return new AsyncLoadingProcess(assetName, mgr, resUID, onAssetLoaded);
                    }
                    else
                    {
                        AsyncLoadingProcess process = mProcessPool.Dequeue();
                        process.InitParams(assetName, mgr, resUID, onAssetLoaded);
                        return process;
                    }
                }

                /// <summary>
                /// 创建隐式持有加载的process
                /// </summary>
                /// <param name="assetName"></param>
                /// <param name="mgr"></param>
                /// <param name="impCreator"></param>
                /// <param name="onAssetLoaded"></param>
                public AsyncLoadingProcess(RuntimeAssetName assetName, ResourceManager mgr, Action<UnityObject> onAssetLoaded)
                {

                    InitParams(assetName, mgr, onAssetLoaded);
                }

                private void InitParams(RuntimeAssetName assetName, ResourceManager mgr, Action<UnityObject> onAssetLoaded)
                {

                    Init(assetName, mgr);
                    AddImplicitResRef(onAssetLoaded);
                }

                public static AsyncLoadingProcess Dequeue(RuntimeAssetName assetName, ResourceManager mgr, Action<UnityObject> onAssetLoaded)
                {
                    if (mProcessPool.Count == 0)
                    {
                        return new AsyncLoadingProcess(assetName, mgr, onAssetLoaded);
                    }
                    else
                    {
                        AsyncLoadingProcess process = mProcessPool.Dequeue();
                        process.InitParams(assetName, mgr, onAssetLoaded);
                        return process;
                    }
                }

                /// <summary>
                /// 创建子Process
                /// </summary>
                /// <param name="assetName"></param>
                /// <param name="mgr"></param>
                /// <param name="parentProcess"></param>
                public AsyncLoadingProcess(RuntimeAssetName assetName, ResourceManager mgr, AsyncLoadingProcess parentProcess)
                {
                    InitParams(assetName, mgr, parentProcess);
                }
                private void InitParams(RuntimeAssetName assetName, ResourceManager mgr, AsyncLoadingProcess parentProcess)
                {

                    Init(assetName, mgr);
                    AddParentProcess(parentProcess);
                }
                public static AsyncLoadingProcess Dequeue(RuntimeAssetName assetName, ResourceManager mgr, AsyncLoadingProcess parentProcess)
                {
                    if (mProcessPool.Count == 0)
                    {
                        return new AsyncLoadingProcess(assetName, mgr, parentProcess);
                    }
                    else
                    {
                        AsyncLoadingProcess process = mProcessPool.Dequeue();
                        process.InitParams(assetName, mgr, parentProcess);
                        return process;
                    }
                }

                public void AddParentProcess(AsyncLoadingProcess parent)
                {
                    if (parent == null)
                    {
                        StringBuilder sb1 = LuaInterface.StringBuilderCache.Acquire();
                        sb1.Append("AsyncLoadingProcess : add null parent process for : ");
                        sb1.Append(TargetAssetName);
                        m_resMgr.LogicError(LuaInterface.StringBuilderCache.GetStringAndRelease(sb1));
                        return;
                    }

                    if (m_parentProcesses.Contains(parent))
                    {
                        StringBuilder sb2 = LuaInterface.StringBuilderCache.Acquire();
                        sb2.Append("AsyncLoadingProcess : child process ");
                        sb2.Append(TargetAssetName);
                        sb2.Append(" already have parent process ");
                        sb2.Append(parent.TargetAssetName);
                        m_resMgr.LogicError(LuaInterface.StringBuilderCache.GetStringAndRelease(sb2));
                        return;
                    }

                    if (!BuildManifestUtility.GetBuildManifest().IsReleaseVer && m_resMgr.IsDebugTarget(TargetAssetName))
                    {
                        StringBuilder sb3 = LuaInterface.StringBuilderCache.Acquire();
                        sb3.Append("async loading AssetBundle: ");
                        sb3.Append(NameWithAncestor());
                        sb3.Append(" add parent process: ");
                        sb3.Append(parent.NameWithAncestor());
                        LogModule.Instance.Trace(LogModule.LogModuleCode.AsyncLoadingProcessState, LuaInterface.StringBuilderCache.GetStringAndRelease(sb3));
                    }
                    //Debug.Assert(!Finished);

                    m_parentProcesses.Add(parent);
                    parent.AddChildProcess(this);

                    IncreaseRefCountRecursively(parent.m_expRefCount);
                    if (!m_impRef && parent.m_impRef)
                        m_impRef = true;
                }

                #endregion

                public void Cancel(uint resUID)
                {
                    if (Finished)
                        return;

                    if (!m_expUIDs.Contains(resUID))
                    {
                        StringBuilder sb4 = LuaInterface.StringBuilderCache.Acquire();
                        sb4.Append("AsyncLoadingProcess: Target asset name: ");
                        sb4.Append(TargetAssetName);
                        sb4.Append(" try to Cancel with invalid resUID: ");
                        sb4.Append(resUID);
                        m_resMgr.LogicError(LuaInterface.StringBuilderCache.GetStringAndRelease(sb4));
                        return;
                    }

                    m_expUIDs.Remove(resUID);

                    DecreaseRefCountRecursively(1);

                    Action<UnityObject> onAssetLoaded;
                    if (m_onExpAssetLoaded.TryGetValue(resUID, out onAssetLoaded))
                    {
                        onAssetLoaded(null);
                        m_onExpAssetLoaded.Remove(resUID);
                    }
                    else
                    {
                        StringBuilder sb5 = LuaInterface.StringBuilderCache.Acquire();
                        sb5.Append("AsyncLoadingProcess: Target asset name: ");
                        sb5.Append(TargetAssetName);
                        sb5.Append(" cannot find explicitly onAssetLoaded callback for resUID ");
                        sb5.Append(resUID);
                        m_resMgr.LogicError(LuaInterface.StringBuilderCache.GetStringAndRelease(sb5));
                    }
                }

               
                public void Start()
                {
                    
                    if (!BuildManifestUtility.GetBuildManifest().IsReleaseVer && m_resMgr.IsDebugTarget(TargetAssetName))
                    {
                        StringBuilder sb6 = LuaInterface.StringBuilderCache.Acquire();
                        sb6.Append("async loading AssetBundle: ");
                        sb6.Append(NameWithAncestor());
                        sb6.Append(" start");
                        LogModule.Instance.Trace(LogModule.LogModuleCode.AsyncLoadingProcessState, LuaInterface.StringBuilderCache.GetStringAndRelease(sb6));
                    }

                    IAssetRef assetRef = m_resMgr.GetExistAsset(TargetAssetName);
                    if (assetRef != null)
                    {
                        m_bundle = assetRef.Bundle;
                        m_assetAlreadyExist = true;
                        if (assetRef.Asset == null)
                        {
                            StringBuilder sb18 = SBC.Acquire();
                            sb18.Append("Start assetRef.Asset nil");
                            sb18.Append(TargetAssetName);
                            m_resMgr.AssetError(SBC.GetStringAndRelease(sb18));
                        }
                        OnProcessEnd(assetRef.Asset);
                        return;
                    }

                    LoadDependents();
                }

                private void IncreaseRefCountRecursively(uint count)
                {
                    m_expRefCount += count;

                    if (Finished)
                    {
                        ExplicitRef expRef = m_resMgr.GetExplicitRef(TargetAssetName);
                        if (expRef == null)
                        {
                            ImplicitRef impRef = m_resMgr.GetImplicitRef(TargetAssetName);
                            if (impRef == null)
                            {
                                StringBuilder sb7 = LuaInterface.StringBuilderCache.Acquire();
                                sb7.Append("AsyncLoadingProcess : ");
                                sb7.Append(NameWithAncestor());
                                sb7.Append(" finished but neither explicitref nor implicitref was created!");
                                m_resMgr.LogicError(LuaInterface.StringBuilderCache.GetStringAndRelease(sb7));
                            }
                            else
                            {
                                expRef = m_resMgr.CreateExplicitRef(TargetAssetName, impRef.Asset, impRef.Bundle);
                            }
                        }
                        if (expRef != null) expRef.IncreaseRefRecursively((int)count);
                    }

                    if (!BuildManifestUtility.GetBuildManifest().IsReleaseVer && m_resMgr.IsDebugTarget(TargetAssetName))
                    {
                        StringBuilder sb8 = LuaInterface.StringBuilderCache.Acquire();
                        sb8.Append("AsyncLoadingProcess: ");
                        sb8.Append(NameWithAncestor());
                        sb8.Append(" IncreaseRefCountRecursively: ");
                        sb8.Append(m_expRefCount);
                        LogModule.Instance.Trace(LogModule.LogModuleCode.ResRefStatus, LuaInterface.StringBuilderCache.GetStringAndRelease(sb8));
                    }

                    foreach (var child in m_childrenProcesses)
                        child.IncreaseRefCountRecursively(count);
                }

                private void DecreaseRefCountRecursively(uint count)
                {
                    m_expRefCount -= count;
                    if (State == AsyncLoadingState.SUCCESS)
                    {
                        ExplicitRef expRef = m_resMgr.GetExplicitRef(TargetAssetName);
                        if (expRef == null)
                        {
                            StringBuilder sb14 = SBC.Acquire();
                            sb14.Append("AsyncLoadingProcess: ");
                            sb14.Append(TargetAssetName);
                            sb14.Append(" try to decrease refcount while no ExplicitRef found");
                            m_resMgr.LogicError(SBC.GetStringAndRelease(sb14));
                        }
                        else
                        {
                            for (int i = 0; i < count; i++)
                            {
                                expRef.DecreaseRef(isRelease:false);//异步加载的时候类还在，释放的话会导致ab被释放，所以等加载类回收的时候再做判断
                            }
                        }
                    }

                    foreach (var child in m_childrenProcesses)
                        child.DecreaseRefCountRecursively(count);
                }

                static Queue<AsyncLoadingProcess> NameWithAncestorBuffer = new Queue<AsyncLoadingProcess>();
                private string NameWithAncestor()
                {
                    NameWithAncestorBuffer.Clear();
                    System.Text.StringBuilder sb = LuaInterface.StringBuilderCache.Acquire();
                    sb.Append(TargetAssetName);
                    sb.Append("\n");
                    foreach (AsyncLoadingProcess p in m_parentProcesses)
                        NameWithAncestorBuffer.Enqueue(p);

                    while (NameWithAncestorBuffer.Count > 0)
                    {
                        AsyncLoadingProcess p = NameWithAncestorBuffer.Dequeue();
                        sb.Append(p.TargetAssetName);
                        sb.Append("\n");
                        foreach (var pp in p.m_parentProcesses)
                            NameWithAncestorBuffer.Enqueue(pp);
                    }

                    return LuaInterface.StringBuilderCache.GetStringAndRelease(sb);
                }

                private void AddChildProcess(AsyncLoadingProcess child)
                {
                    if (m_childrenProcesses.Contains(child))
                    {
                        StringBuilder sb15 = SBC.Acquire();
                        sb15.Append("AsyncLoadingProcess : ");
                        sb15.Append(TargetAssetName);
                        sb15.Append(" try to add duplicate child ");
                        sb15.Append(child.TargetAssetName);
                        m_resMgr.LogicError(SBC.GetStringAndRelease(sb15));
                        return;
                    }
                    m_childrenProcesses.Add(child);
                }

                private void LoadDependents()
                {
                    List<RuntimeAssetName> depAssets = m_resMgr.GetDepInfo(TargetAssetName);
                    if (depAssets.Count > 0)
                    {
                        State = AsyncLoadingState.LOADING_DEPENDENT_ASSET;
                        m_depLoadingProcess = depAssets.Count;

                        for (int i = 0; i < depAssets.Count; i++)
                        {
                            if (State == AsyncLoadingState.FAILED)
                            {
                                return;
                            }
                            string depname = depAssets[i];
                            m_resMgr.CreateAsyncLoadingProcess(depname, this);
                        }
                    }
                    else
                    {
                        LoadAssetBundle();
                    }
                    m_resMgr.m_depInfoPool.Return(depAssets);
                }

                private void LoadAssetBundle()
                {
                    if (!BuildManifestUtility.GetBuildManifest().IsReleaseVer && m_resMgr.IsDebugTarget(TargetAssetName))
                    {
                        StringBuilder sb16 = SBC.Acquire();
                        sb16.Append("async loading AssetBundle: ");
                        sb16.Append(NameWithAncestor());
                        sb16.Append(" loading assetbundle");
                        LogModule.Instance.Trace(LogModule.LogModuleCode.AsyncLoadingProcessState, SBC.GetStringAndRelease(sb16));
                        StringBuilder sb17 = SBC.Acquire();
                        sb17.Append("async loading AssetBundle {");
                        sb17.Append(TargetAssetName);
                        sb17.Append(" begin");
                        LogModule.Instance.Trace(LogModule.LogModuleCode.AssetBundleStatus, SBC.GetStringAndRelease(sb17));
                    }
                    State = AsyncLoadingState.LOADING_AB;

                    AssetBundleName abName = RuntimeAssetName2AssetBundleName(TargetAssetName);
                    string fullPath = m_resMgr.m_GetABFullPathAndCheckExists(abName);
                    if (fullPath == null)
                    {
                        if (m_resMgr.AllResReady)
                        {
                            StringBuilder sb18 = SBC.Acquire();
                            sb18.Append("All ab downloaded but ");
                            sb18.Append(abName);
                            sb18.Append(" cannot be found");
                            Debug.LogError("ab资源丢失");
                            m_resMgr.AssetError(SBC.GetStringAndRelease(sb18));
                        }

                        OnProcessEnd(null);
                        return;
                    }
                    m_loadingABRequest = AssetBundle.LoadFromFileAsync(fullPath);
                    m_loadingABRequest.completed += OnAssetBundleLoaded;
                }


                private void OnAssetBundleLoaded(AsyncOperation op)
                {
                    if (!BuildManifestUtility.GetBuildManifest().IsReleaseVer && m_resMgr.IsDebugTarget(TargetAssetName))
                    {
                        StringBuilder sb19 = SBC.Acquire();
                        sb19.Append("async loading AssetBundle ");
                        sb19.Append(TargetAssetName);
                        sb19.Append(" end");
                        LogModule.Instance.Trace(LogModule.LogModuleCode.AssetBundleStatus, SBC.GetStringAndRelease(sb19));
                        StringBuilder sb20 = SBC.Acquire();
                        sb20.Append("async loading AssetBundle: ");
                        sb20.Append(NameWithAncestor());
                        sb20.Append(" loading asset");
                        LogModule.Instance.Trace(LogModule.LogModuleCode.AsyncLoadingProcessState, SBC.GetStringAndRelease(sb20));
                    }

                    
                    AssetBundle bundle = null;
                    if (op != null)
                    {
                        bundle = (op as AssetBundleCreateRequest).assetBundle;
                        m_bundle = bundle;
                    }
                    if (bundle != null)
                    {
                        State = AsyncLoadingState.LOADING_ASSET;
                        m_loadingAssetRequest = bundle.LoadAssetAsync(bundle.GetAllAssetNames()[0]);
                        m_loadingAssetRequest.completed += OnAssetLoaded;
                    }
                    else
                    {
                        StringBuilder sb18 = SBC.Acquire();
                        sb18.Append("OnAssetBundleLoaded bundle nil");
                        if (m_loadingABRequest != null)
                        {
                            sb18.Append(" isDone:");
                            sb18.Append(m_loadingABRequest.isDone);
                            sb18.Append(" progress:");
                            sb18.Append(m_loadingABRequest.progress);
                        }
                        sb18.Append(TargetAssetName);
                        m_resMgr.AssetError(SBC.GetStringAndRelease(sb18));
                        OnProcessEnd(null);
                    }
                    if (m_loadingABRequest != null)
                    {
                        m_loadingABRequest.completed -= OnAssetBundleLoaded;
                        m_loadingABRequest = null;
                    }
                }

                private void OnAssetLoaded(AsyncOperation op)
                {
                    UnityObject asset = (op as AssetBundleRequest).asset;
                    if (asset == null)
                    {
                        StringBuilder sb18 = SBC.Acquire();
                        sb18.Append("OnAssetLoaded asset nil ");
                        if (m_loadingAssetRequest != null)
                        {
                            sb18.Append("  isDone:");
                            sb18.Append(m_loadingAssetRequest.isDone);
                            sb18.Append("  progress:");
                            sb18.Append(m_loadingAssetRequest.progress);

                        }
                        sb18.Append(TargetAssetName);
                        m_resMgr.AssetError(SBC.GetStringAndRelease(sb18));
                    }
                    
                    m_loadingAssetRequest.completed -= OnAssetLoaded;
                    m_loadingAssetRequest = null;
                    OnProcessEnd(asset);
                }

              

                /// <summary>
                /// 4个入口：
                /// 1. 目标资源已存在时， process直接结束
                /// 2. bundle加载失败时，
                /// 3. asset正常加载结束时
                /// 4. 有依赖资源加载失败时， process直接结束
                /// </summary>
                /// <remarks>
                /// process逻辑必须保证一定可以走到OnProcessEnd
                /// </remarks>
                /// <param name="asset">
                /// 失败时asset为空
                /// </param>
                private void OnProcessEnd(UnityObject asset)
                {
                   
                    if (!BuildManifestUtility.GetBuildManifest().IsReleaseVer && m_resMgr.IsDebugTarget(TargetAssetName))
                    {
                        StringBuilder sb21 = SBC.Acquire();
                        sb21.Append("async loading AssetBundle: ");
                        sb21.Append(NameWithAncestor());
                        sb21.Append(" loading finished! asset : ");
                        sb21.Append(asset);
                        LogModule.Instance.Trace(LogModule.LogModuleCode.AsyncLoadingProcessState, SBC.GetStringAndRelease(sb21));
                    }
                    m_resMgr.RemoveAsyncLoadingProcess(TargetAssetName);
                  
                    State = asset == null ? AsyncLoadingState.FAILED : AsyncLoadingState.SUCCESS;
                  
                    m_asset = asset;

                    if (asset != null)
                    {
                        if (m_expRefCount >= 0)
                        {
                            ExplicitRef expRef = m_resMgr.GetExplicitRef(TargetAssetName); //可能是implicit ref，所以需要创建exp
                            if (expRef == null)
                            {
                                expRef = m_resMgr.CreateExplicitRef(TargetAssetName, m_asset, m_bundle);
                            }

                            for (int i = 0; i < m_expRefCount; i++)
                            {
                                if (m_assetAlreadyExist)
                                {
                                    expRef.IncreaseRefRecursively();
                                }
                                else
                                {
                                    expRef.IncreaseRef();
                                }
                            }
                        }

                        if (m_impRef)
                            m_resMgr.CreateImplicitRef(TargetAssetName, m_asset, m_bundle);
                    }


                    if ((m_onExpAssetLoaded != null && m_onExpAssetLoaded.Count > 0) || m_onImpAssetLoaded != null)
                    {
                        if (m_assetAlreadyExist)
                        {
                            m_resMgr.AddNextFrameCallBack(InvokeAssetLoadedCallBacks, 0);
                        }
                        else
                            InvokeAssetLoadedCallBacks();
                    }

                    if (State == AsyncLoadingState.FAILED)//父process会直接递归，所以子process不需要再处理这个
                    {
                        m_resMgr.AssetError(TargetAssetName + "资源加载失败");

                        DecreaseRefCountRecursively(m_expRefCount);

                        //防止失败节点被重复处理
                        foreach (var parent in m_parentProcesses)
                        {
                            parent.m_childrenProcesses.Remove(this);
                        }

                    }

                    //bundle加载完毕但是在bundleinfomap中不存在，说明并没有为bundle创建ExplicitRef或者ImplicitRef
                    //可能由于该process本身没有显式或隐式引用导致。 需要立即unload bundle。
                    if (m_bundle != null && !m_resMgr.m_bundleInfoMap.ContainsKey(m_bundle))
                    {
                        StringBuilder sb18 = SBC.Acquire();
                        sb18.Append("OnProcessEnd Unload bundle : ");
                        sb18.Append(m_bundle.name);
                        if (asset != null)
                        {
                            sb18.Append(" asset:");
                            sb18.Append(asset.name);
                        }
                       
                        m_resMgr.AssetError(SBC.GetStringAndRelease(sb18));

                        m_bundle.Unload(true);
                        m_bundle = null;
                    }
                   
                  
                    int parentCount = m_parentProcesses.Count;
                    if (parentCount > 0)
                    {
                        for (int i = 0; i < parentCount; i++)
                        {
                            m_parentProcesses[i].OnChildProcessEnd(this, asset);
                        }
                    }
                    else
                    {
                        m_resMgr.AddNextFrameCallBack(CheckChildProcess, -2);//防止m_assetAlreadyExist是这种情况，延迟2帧执行回收
                    }

                }


                private void DeleteParentProcess(AsyncLoadingProcess parentProcess)
                {
                    if (m_parentProcesses == null)
                    {
                        return;
                    }
                    int parentCount = m_parentProcesses.Count;
                    for (int i = parentCount - 1; i >= 0; i--)
                    {
                        if (m_parentProcesses[i] == parentProcess)
                        {
                            m_parentProcesses.RemoveAt(i);
                            break;
                        }
                    }
                    if (m_parentProcesses.Count == 0)
                    {
                        CheckChildProcess();
                    }
                }

                private void CheckChildProcess()
                {
                    if (State == AsyncLoadingState.FAILED || State == AsyncLoadingState.SUCCESS)
                    {
                        int childCount = m_childrenProcesses.Count;
                        for (int i = 0; i < childCount; i++)
                        {
                            m_childrenProcesses[i].DeleteParentProcess(this);
                        }
                    
                        ExplicitRef expRef = m_resMgr.GetExplicitRef(TargetAssetName);
                        if (expRef != null)
                        {
                            expRef.CheckCanRelease();
                        }
                        AsyncLoadingProcess.Recycle(this);
                    }
                 
                }


                private void InvokeAssetLoadedCallBacks()
                {
                    if (m_onImpAssetLoaded != null)
                    {
                        m_resMgr.AddCallBack(m_onImpAssetLoaded, m_asset, 0);
                    }

                    if (m_onExpAssetLoaded != null)
                    {
                        foreach (var kv in m_onExpAssetLoaded)
                        {
                            m_resMgr.AddCallBack(kv.Value, m_asset, kv.Key);
                        }
                    }

                }

                /// <summary>
                /// 子加载进程结束时，调用此接口。 
                /// </summary>
                /// <param name="asset"></param>
                private void OnChildProcessEnd(AsyncLoadingProcess child, UnityObject asset)
                {
                    if (!BuildManifestUtility.GetBuildManifest().IsReleaseVer && m_resMgr.IsDebugTarget(TargetAssetName))
                    {
                        StringBuilder sb22 = SBC.Acquire();
                        sb22.Append("async loading AssetBundle: ");
                        sb22.Append(NameWithAncestor());
                        sb22.Append(" OnChildProcessEnd ");
                        sb22.Append(child.NameWithAncestor());
                        sb22.Append(", asset: ");
                        sb22.Append(asset);
                        sb22.Append(", remain count : ");
                        sb22.Append(m_depLoadingProcess);
                        LogModule.Instance.Trace(LogModule.LogModuleCode.AsyncLoadingProcessState, SBC.GetStringAndRelease(sb22));
                    }
                   
                    //第一个加载失败的子资源，触发process结束。
                    if (asset == null && State == AsyncLoadingState.LOADING_DEPENDENT_ASSET)
                    {
                        StringBuilder sb18 = SBC.Acquire();
                        sb18.Append(" child fail: ");
                        sb18.Append(child.TargetAssetName);
                        sb18.Append(" self fail: ");
                        sb18.Append(TargetAssetName);
                        m_resMgr.AssetError(SBC.GetStringAndRelease(sb18));

                        OnProcessEnd(null);
                        return;
                    }

                    //已经失败的process，忽略来自于子process的回调
                    if (State == AsyncLoadingState.FAILED)
                    {
                        return;
                    }
                  
                    m_depLoadingProcess--;
                    if (m_depLoadingProcess == 0)
                    {
                        LoadAssetBundle();
                    }
                    else if (m_depLoadingProcess < 0)
                    {
                        StringBuilder sb23 = SBC.Acquire();
                        sb23.Append("Loading ");
                        sb23.Append(TargetAssetName);
                        sb23.Append(" dependent asset count error: ");
                        sb23.Append(m_depLoadingProcess);
                        m_resMgr.LogicError(SBC.GetStringAndRelease(sb23));
                    }
                }

                private void Init(RuntimeAssetName assetName, ResourceManager mgr)
                {
                    TargetAssetName = assetName;
                    m_resMgr = mgr;

                    State = AsyncLoadingState.IDLE;

                    m_childrenProcesses = new List<AsyncLoadingProcess>();
                    m_parentProcesses = new List<AsyncLoadingProcess>();

                    m_onExpAssetLoaded = new Dictionary<uint, Action<UnityObject>>();
                    m_impRef = false;
                    m_expRefCount = 0;
                    m_expUIDs = new HashSet<uint>();
                }



                private ResourceManager m_resMgr;
                private AssetBundle m_bundle;
                private UnityObject m_asset;
                private AssetBundleCreateRequest m_loadingABRequest;
                private AssetBundleRequest m_loadingAssetRequest;

                private Dictionary<uint, Action<UnityObject>> m_onExpAssetLoaded; //只有被逻辑直接显式引用的Process才有
                private uint m_expRefCount; // m_refCount = 逻辑端直接引用的数量 + 父process的m_refcount
                private HashSet<uint> m_expUIDs;//只有被逻辑直接显式引用的Process才有

                private Action<UnityObject> m_onImpAssetLoaded; //只有被逻辑直接隐式引用的Process才有
                private bool m_impRef;

                private List<AsyncLoadingProcess> m_childrenProcesses;
                private List<AsyncLoadingProcess> m_parentProcesses;

                private int m_depLoadingProcess;
                private bool m_assetAlreadyExist;

                private static Queue<AsyncLoadingProcess> mProcessPool = new Queue<AsyncLoadingProcess>();

                public void Reset()
                {
                    TargetAssetName = null;
                    State = AsyncLoadingState.IDLE;
                    m_bundle = null;
                    m_asset = null;
                    m_loadingABRequest = null;
                    m_loadingAssetRequest = null;
                    m_onExpAssetLoaded = null;
                    m_expRefCount = 0;
                    m_expUIDs = null;
                    m_onImpAssetLoaded = null;
                    m_impRef = false;
                    m_childrenProcesses = null;
                    m_parentProcesses = null;
                    m_depLoadingProcess = 0;
                    m_assetAlreadyExist = false;
                }

                public static void Recycle(AsyncLoadingProcess process) //回收池和加载没完成的时候会不会走这里 TODO
                {
                    if (process!=null)
                    {
                        process.Reset();
                        mProcessPool.Enqueue(process);
                    }
                }


            }
        }
    }
}