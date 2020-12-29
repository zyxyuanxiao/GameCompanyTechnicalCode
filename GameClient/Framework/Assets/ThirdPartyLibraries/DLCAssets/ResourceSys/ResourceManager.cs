using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Text;
using SBC = LuaInterface.StringBuilderCache;

/**
 * 例： prefabs.ui.archieve.archieve_panel_ui.gameobject.ab
 */
using AssetBundleName = System.String;
/**
 * 相对于Res_Best目录的路径, Prefabs/UI/achieve/achieve_panel_ui.prefab"
 */
using EditorAssetURI = System.String; //
/**
 * 运行期资源URI(打包成AB后的资源名称) 
 * 可能对应于一个Assetbundle的名字, 如 prefabs.ui.archieve.archieve_panel_ui.gameobject.ab
 * 也可能是对应于AssetBundle中的一个资源。 如cc_effect.commontextures.gameobject.ab@texture1
 * 
 * GCTODO:目前第一版, RuntimeAssetName 完全等价于 AssetBundleName
 */
using RuntimeAssetName = System.String;
using UnityObject = UnityEngine.Object;

/**
 * GCTODO: 这个前提是错误的，后续会增加对单个asset加载的支持
 * 
 * 基于AssetBundle无法单独卸载其中的asset的前提。
 * AssetBundle的打包粒度会直接影响runtime对资源管理的粒度。
 * 因此，从资源管理的角度出发，每个AssetBundle中应该最多只有一个资源被逻辑使用或者被其他资源依赖。
 */

namespace Best
{
    namespace ResourceSys
    {
        public partial class ResourceManager : MonoBehaviour
        {
            /*
             * client对于asset的释放分为两类：
             * 1. 隐式释放： 
             *          即asset的生命周期依赖于C#代码中对asset的引用.
             *          当引用数为0且调全局资源清理接口时，才可以被释放
             *          例：
             *              public class ImplicitlyRefRes：MonoBehaviour
             *              {
             *                  private UnityEngine.Object m_asset;
             *                  private const string AssetURI = "Prefabs/UI/achieve/achieve_panel_ui.prefab";
             *                  void Start()
             *                  {
             *                      ResourceManager.LoadSyncImplicitly(AssetURI, typeof(GameObject), out m_asset)
             *                      if(m_asset == null)
             *                      {
             *                          //error handling
             *                      }
             *                  }
             *                  
             *                  void OnDestroy()
             *                  {
             *                      m_asset = null; 
             *                      //注意，隐式释放的资源，必须在合理的时机将所有对资源的引用设置为null。 这样才能依靠全局资源清理接口，正确释放资源。
             *                  }
             *              }
             *              
             * 2. 显式释放：
             *          即asset的生命周期依赖于显式的释放。
             *          预加载，常驻内存等功能均可通过显式释放实现
             *          
             *          该类资源必须显式释放。否则就会造成泄露。 但是资源管理的精细度高，内存可控。
             *          
             *          例:
             *          public class ExplicitlyRefRes : MonoBehaviour
             *          {
             *              private UnityEngine.Object m_asset;
             *              private uint m_resUID;
             *              private const string AssetURI = "Prefabs/UI/achieve/achieve_panel_ui.prefab";
             *              void Start(){
             *                  this.m_resUID = ResourceManager.LoadSync(resPath,  typeof(GameObject),  out m_asset);
             *                  if(m_asset == null)
             *                  {
             *                      //error handling
             *                  }
             *              }
             *  
             *              void OnDestroy(){
             *                  ResourceManager.Unload(ref resUID);
             *              }
             *          }
             * 
             * 对于client写逻辑而言，显式释放与隐式释放复杂度没区别。
             * 但是隐式释放会带来资源在内存中生存时间过长的问题，带来内存压力。
             * 显式释放的资源生存周期可以被精细控制，所以内存压力更小。 
             *      但可能会存在频繁的加载释放同一资源的问题。可以由逻辑层使用缓存策略解决。根据不同情况选择缓存策略（如LRU, LRU-K, LFU或者objectpool等）.
             * 
             *  使用方法:
             *      同步加载见上文的隐式加载与显式加载的例子。
             *      异步加载:
             *          隐式
             *          public ImplicitlyAsyncLoad : MonoBehaviour
             *          {
             *              private UnityEngine.Object m_asset;
             *              private const string AssetURI = "Prefabs/UI/achieve/achieve_panel_ui.prefab";
             *                  
             *              void Start()
             *              {
             *                  ResourceManager.LoadAsyncImplicitly(AssetURI, typeof(GameObject),  (asset) =>
             *                  {
             *                      if(asset == null)
             *                      {
             *                          Debug.LogErrorFormat("AsyncLoad asset {0} failed", resPath); 
             *                          return;
             *                      }
             *                      if(!CheckContext())
             *                      {//检查异步加载结束，当前上下文是否符合发起加载时的要求.
             *                          return;
             *                      }
             *                      Debug.Log("AysncLoad asset {0} success", resPath);
             *                      m_asset = asset;
             *                  });
             *              }
             *              
             *              void OnDestroy()
             *              {
             *                  m_asset = null;
             *              }
             *          }
             *          
             *          显式. 
             *          在释放时，逻辑层不需要关心异步加载是否结束，只需要调用Unload(resUID)即可。 
             *          对于正在加载中被取消的情况，逻辑层传入的回调 会返回asset为null。
             *          
             *          public ExplicitlyAsyncLoad : MonoBehaviour
             *          {
             *              private UnityEngine.Object m_asset;
             *              private uint m_resUID;
             *              private const string AssetURI = "Prefabs/UI/achieve/achieve_panel_ui.prefab";
             *                  
             *              void Start()
             *              {
             *                  m_resUID = ResourceManager.LoadAsync(AssetURI, typeof(GameObject),  (resUID, asset) =>
             *                      {
             *                          if(asset == null)
             *                          {
             *                              Debug.LogErrorFormat("AsyncLoad asset {0} failed or canceld", resPath); 
             *                              return;
             *                          }
             *                          if(!CheckContext())
             *                          {//检查异步加载结束，当前上下文是否符合发起加载时的要求.
             *                              ResourceManager.Unload(resUID); 
             *                              return;
             *                          }
             *                          Debug.Log("AysncLoad asset {0} success", resPath);
             *                          m_asset = asset;
             *                      }
             *                  );
             *              }
             *              
             *              void OnDestroy()
             *              {
             *                  ResourceManager.Unload(ref resUID);
             *              }
             *          }
            */

            private static ResourceManager _inst;
            /// <summary>
            /// 逻辑别用这个Instance, 从GameKernel中拿ResourceMgr。
            /// 此处是为了绕过GameKernel的Initialize/Uninitialize所带来的问题，而从之前的代码中继承过来的设计。
            /// </summary>
            /// <returns></returns>
            [LuaInterface.NoToLua]
            public static ResourceManager Instance()
            {

                if (_applicationIsQuitting)
                {
                    return null;
                }
                if (_inst == null)
                {
                    GameObject go = new GameObject("ResourceManager");
                    _inst = go.AddComponent<ResourceManager>();
                    if (Application.isPlaying)
                    {
                        GameObject.DontDestroyOnLoad(go);
                    }
                }

                return _inst;
            }

            private ResourceManager()
            {

            }

            private bool m_Init = false;
            public bool IsABLoadRefRecord
            {
                get; private set;
            }
            public bool IsABLoadRecord
            {
                get; private set;
            }

            private string m_ABLoadRefRecordPath = null;
            private string m_ABLoadRecordPath = null;

            private bool isLimitAsyncComplete = true;

            struct BundleInfo
            {
                public int ExplicitRefCout; //来自于ExplicitRef引用的数量
                public bool ImplicitRefered; //被ImplicitRef引用
            }

            private Dictionary<RuntimeAssetName, AsyncLoadingProcess> m_asyncLoadingProcesses = new Dictionary<RuntimeAssetName, AsyncLoadingProcess>();
            private Dictionary<RuntimeAssetName, ImplicitRef> m_implicitRefs = new Dictionary<RuntimeAssetName, ImplicitRef>();
            private Dictionary<RuntimeAssetName, ExplicitRef> m_explicitRefs = new Dictionary<RuntimeAssetName, ExplicitRef>();
            private Dictionary<AssetBundle, BundleInfo> m_bundleInfoMap = new Dictionary<AssetBundle, BundleInfo>();
            private Dictionary<uint, RuntimeAssetName> m_clientRef = new Dictionary<uint, RuntimeAssetName>();

            private Queue<ExplicitRef> m_unuseExpPool = new Queue<ExplicitRef>();
            private Queue<ImplicitRef> m_unuseImpPool = new Queue<ImplicitRef>();
            private Queue<WaitFrameItem> m_unuseWaitFrameQueue = new Queue<WaitFrameItem>();
            private List<WaitFrameItem> m_currWaitNextFrameCallBack = new List<WaitFrameItem>();

            // 需要深度拷贝当前的引用信息，然后和UI关闭后的引用信息进行对比
            private Dictionary<RuntimeAssetName, int> m_snapshotExplicitRefsBegin = null;
            private Dictionary<RuntimeAssetName, int> m_snapshotExplicitRefsEnd = null;
            private Dictionary<RuntimeAssetName, int> m_singleSampleAssetLeakResult = null;
            private Dictionary<string, List<AssetLeakInfo>> m_assetLeakResuilt = null;
            private string sampleContext = "Default";
            private bool isSampling = false;
            private string leakLogFilePath;
            private bool logFileInitFlag = false;
            private FileStream leakLogFile = null;

            //所有asseturi之间的依赖关系. 
            //GCTODO: 还有没有更好的办法呢？
            private Dictionary<string, uint[]> m_assetsDependInfo;
            private string[] m_allBundleNames;

            private static GameObject DummySceneAsset;

            private HashSet<string> m_debugAssetName = new HashSet<AssetBundleName>();

            private static bool _applicationIsQuitting = false;

            #region 异步加载数量限制
            private Dictionary<uint, AsyncParams> mAsyncLoadingDic = new Dictionary<uint, AsyncParams>();
            private Queue<AsyncParams> mAsyncWaitingQueue = new Queue<AsyncParams>();
            private uint mMaxRequest = 3;
            private uint mConfigMaxLoadingCount = 3;

            #endregion

            public void AddDebugTarget(string assetName)
            {
                m_debugAssetName.Add(assetName);
            }

            public void ClearDebugTarget()
            {
                m_debugAssetName.Clear();
            }

            private bool IsDebugTarget(string assetName)
            {
                if (m_debugAssetName.Count == 0)
                    return false;

                if (assetName == null)
                    return false;
                return m_debugAssetName.Contains(assetName);
            }

            public void Awake()
            {
                leakLogFilePath = Application.persistentDataPath + "/AssetLeak.txt";

                this.Initialize();

            }

            [LuaInterface.NoToLua]
            public void Initialize()
            {
                if (m_Init)
                {
                    if (UseAssetBundle)
                    {
                        float startTime = Time.realtimeSinceStartup;
                        LoadManifest();
                        float endTime = Time.realtimeSinceStartup;
                        Debug.Log("启动时间第一次LoadManifest：" + (endTime - startTime) * 1.0f);
                    }
                    return;
                }
                DummySceneAsset = GameObject.Find("dummy_scene_asset");
                if (DummySceneAsset == null)
                {
                    DummySceneAsset = new GameObject("dummy_scene_asset");
                }
                
                UnityObject.DontDestroyOnLoad(DummySceneAsset);

                if (UseAssetBundle)
                {
                    float startTime = Time.realtimeSinceStartup;
                    this.SetURITranslater(BundleConfig.EditorAssetURI2RuntimeAssetName);
                    this.SetABFullPathFunc(BundleConfig.GetBundleUrlForLoad);
                    LoadManifest();
                    float endTime = Time.realtimeSinceStartup;
                    Debug.Log("启动时间第二次LoadManifest：" + (endTime - startTime) * 1.0f);
                }
                //LRUKCache<string, AssetCache<Texture2D>.Bucket> cacheAlgo = new LRUKCache<RuntimeAssetName, AssetCache<Texture2D>.Bucket>(2, 10, 5, 60);
                //AssetCache<Texture2D> globalTextureCache = new AssetCache<Texture2D>(cacheAlgo, "GlobalTextureCache");
                //GlobalAssetCacheMgr.Instance.SetGlobalTextureCache(globalTextureCache);

                UpdatableRunner.Init();

                if (!BuildManifestUtility.GetBuildManifest().isPlayingDownLoad && BuildManifestUtility.GetBuildManifest().IsPlayingDownLoadPause)
                {
                    m_ABLoadRefRecordPath = Path.Combine(BundleConfig.Instance.BundlesPathForPersist, "ABLoadRefRecord.txt");
                    m_ABLoadRecordPath = Path.Combine(BundleConfig.Instance.BundlesPathForPersist, "ABLoadRecord.txt");
                    IsABLoadRefRecord = false;
                    IsABLoadRecord = true;
                }
                else
                {
                    IsABLoadRefRecord = false;
                    IsABLoadRecord = false;
                }

                m_Init = true;

            }



            public void OnApplicationQuit()
            {
                _applicationIsQuitting = true;
            }

            [LuaInterface.NoToLua]
            public void UnInitialize()
            {
                //Messenger.RemoveListener(MSG_DEFINE.MSG_SCENE_LOAD_PRE, OnSceneLoadPre);
                //Messenger.RemoveListener<string>(MSG_DEFINE.MSG_SCENE_LOAD_COMPLETE, OnLevelRealWasLoaded);

                //GlobalAssetCacheMgr.Instance.TextureCache.Release();
                //GlobalAssetCacheMgr.Instance.SetGlobalTextureCache(null);


                //GameObject.Destroy(UpdatableRunner.Instance.gameObject);

                //if (UseAssetBundle)
                //{
                //    m_assetsDependInfo = null;
                //    //foreach (var kv in m_clientRef)
                //    //{
                //    //    uint uid = kv.Key;
                //    //    Unload(ref uid);
                //    //}

                //    UnloadUnusedAssets();

                //    foreach (var kv in m_bundleInfoMap)
                //    {
                //        LogModule.Instance.Trace(LogModule.LogModuleCode.AssetBundleStatus, string.Format("AssetBundle: {0} Unloaded ", kv.Key.name));
                //        kv.Key.Unload(true);
                //    }
                //}
                //_inst = null;
            }

            /**
             * 由于边玩边下的逻辑，资源可能是不全的。
             * 边玩边下是以资源包为单位。 每解压完一个包，我们就可以认为此包中的所有资源已存在。
             * 所以我们可以排除边玩边下的影响，准确的报告资源加载失败的情况。
             * 
             * 假定有2W个AB，每个AB名字长度为50个字符，hashset最多占用大约1M的内存。
             * 所有资源包都下载完之后，不需要为此hashset赋值。也就是边玩边下完成后，没有额外内存消耗.
             * 
             * 暂时先采用模糊检测，即只关心边玩边下有没有全部完成。在全部完成之前，认为找不到资源是正常的。

            public HashSet<string> WaitingForDownloadSet = null;

            public bool WaitingForDownload(string abName)
            {
                if (WaitingForDownloadSet == null)
                    return false;
                int backSlashIndex = abName.LastIndexOf("/");
                if (backSlashIndex > 0)
                    abName = abName.Substring(backSlashIndex + 1);
                return WaitingForDownloadSet.Contains(abName);
            }
             */

            //AllResReady之前，认为找不到资源是正常的。

            public bool AllResReady
            {
                get
                {
                    return true;
                }
            }


            #region C#-only interface
            //方便C#接口的逻辑
            [LuaInterface.NoToLua]
            /**
             * <remarks>
             * 这个接口不应该存在，泄露的地方太多改不过来，才又用上的. 隐式释放也总比永久泄露强
             * </remarks>
             */
            public T LoadSyncImplicitly<T>(EditorAssetURI assetURI) where T : UnityObject
            {
#if UNITY_PROFILER
                UnityEngine.Profiling.Profiler.BeginSample("ResourceManager LoadSyncImplicitly :" + assetURI + "  type:" + typeof(T));
#endif
#if USE_UWA
             UWAEngine.PushSample("ResourceManager LoadSyncImplicitly :" + assetURI + "  type:" + typeof(T));
#endif
                UnityObject asset = LoadSyncImplicitly(assetURI, typeof(T));
#if USE_UWA
            UWAEngine.PopSample();
#endif
#if UNITY_PROFILER
                UnityEngine.Profiling.Profiler.EndSample();
#endif
                return asset as T;
            }

            //方便C#接口的逻辑
            [LuaInterface.NoToLua]
            public uint LoadSync<T>(EditorAssetURI assetURI, out T obj) where T : UnityObject
            {
#if UNITY_PROFILER
                UnityEngine.Profiling.Profiler.BeginSample("ResourceManager LoadSync");
                UnityEngine.Profiling.Profiler.BeginSample("ResourceManager LoadSync :" + assetURI + "  type:" + typeof(T));
#endif
#if USE_UWA
             UWAEngine.PushSample("ResourceManager LoadSync :" + assetURI + "  type:" + typeof(T));
#endif
                UnityObject asset = null;
                uint resUID = LoadSync(assetURI, typeof(T), out asset);
                obj = asset as T;
#if USE_UWA
            UWAEngine.PopSample();
#endif
#if UNITY_PROFILER
                UnityEngine.Profiling.Profiler.EndSample();
                UnityEngine.Profiling.Profiler.EndSample();
#endif
                return resUID;
            }

            //方便C#接口的逻辑
            [LuaInterface.NoToLua]
            public void LoadAsyncImplicitly<T>(EditorAssetURI assetURI, Action<T> callback) where T : UnityObject
            {
#if UNITY_PROFILER
                UnityEngine.Profiling.Profiler.BeginSample("ResourceManager LoadAsyncImplicitly :" + assetURI + "  type:" + typeof(T));
#endif
#if USE_UWA
             UWAEngine.PushSample("ResourceManager LoadAsyncImplicitly :" + assetURI + "  type:" + typeof(T));
#endif
                LoadAsyncImplicitly(assetURI, typeof(T), (asset) =>
                {
                    if (callback != null)
                        callback(asset as T);
                });
#if USE_UWA
            UWAEngine.PopSample();
#endif
#if UNITY_PROFILER
                UnityEngine.Profiling.Profiler.EndSample();
#endif
            }
            #endregion

            private void DoNextAsyncLoading()
            {
                while (mAsyncLoadingDic.Keys.Count < mMaxRequest && mAsyncWaitingQueue.Count > 0)
                {
                    AsyncParams item = mAsyncWaitingQueue.Dequeue();
                    RequestStartLoad(item);
                }
            }

            private void RequestStartLoad(AsyncParams asyncParams, bool isImportant = false)
            {
                //判断这个物体有没有在加载中
                foreach (var item in mAsyncLoadingDic.Values)
                {
                    if (item.isEquals(asyncParams))
                    {
                        RealLoadAsync(asyncParams);
                        return;
                    }
                }

                if (mAsyncLoadingDic.Count >= mMaxRequest && isImportant == false)
                {
                    mAsyncWaitingQueue.Enqueue(asyncParams);
                }
                else
                {
                    mAsyncLoadingDic.Add(asyncParams.m_ResId, asyncParams);
                    RealLoadAsync(asyncParams);
                }
            }

            private void OnSceneLoadComplete(string arg1)
            {
                mMaxRequest = mConfigMaxLoadingCount;
                isLimitAsyncComplete = true;
            }

            private void OnSceneLoadPre()
            {
                mMaxRequest = 500;//无限制加载
                isLimitAsyncComplete = false;
            }


            //方便C#接口的逻辑
            [LuaInterface.NoToLua]
            public uint LoadAsync<T>(EditorAssetURI assetURI, Action<uint, T> callback) where T : UnityObject
            {

                return LoadAsync(assetURI, typeof(T), (uid, asset) =>
                {
                    if (callback != null)
                        callback(uid, asset as T);
                });
            }


            public UnityObject LoadSyncImplicitly(EditorAssetURI assetURI, Type type)
            {
#if UNITY_PROFILER
                UnityEngine.Profiling.Profiler.BeginSample("ResourceManager LoadSyncImplicitly :" + assetURI + "  type:" + type);
#endif
#if USE_UWA
             UWAEngine.PushSample("ResourceManager LoadSyncImplicitly :" + assetURI + "  type:" + type);
#endif
                UnityObject asset;
                LoadSyncImpl(assetURI, type, out asset, false);
#if USE_UWA
            UWAEngine.PopSample();
#endif
#if UNITY_PROFILER
                UnityEngine.Profiling.Profiler.EndSample();
#endif
                return asset;
            }

            /**
             * <remarks>
             * 
             * </remarks>
             * <returns>
             * 返回一个唯一的id，代表此次显式持有的资源引用。用于在Unload时释放此显式持有的资源引用
             * </returns>
             */
            //因为AssetDatabase.LoadAssetAtPath必须有个type参数，加一个type参数。但是这个参数在真机运行时毫无意义，是不是有点本末倒置了？
            public uint LoadSync(EditorAssetURI assetURI, Type type, out UnityObject asset)
            {

#if UNITY_PROFILER
                UnityEngine.Profiling.Profiler.BeginSample("ResourceManager LoadSync:" + assetURI);
#endif
#if USE_UWA
             UWAEngine.PushSample("ResourceManager LoadSync:" + assetURI);
#endif
                uint uid = LoadSyncImpl(assetURI, type, out asset, true);

#if USE_UWA
            UWAEngine.PopSample();
#endif

#if UNITY_PROFILER
                UnityEngine.Profiling.Profiler.EndSample();
#endif
                return uid;
            }


            public void LoadAsyncImplicitly(EditorAssetURI assetURI, Type type, Action<UnityObject> onLoadFinished)
            {
#if UNITY_PROFILER
                UnityEngine.Profiling.Profiler.BeginSample("ResourceManager LoadAsyncImplicitly :" + assetURI + "  type:" + type);
#endif
#if USE_UWA
             UWAEngine.PushSample("ResourceManager LoadAsyncImplicitly :" + assetURI + "  type:" + type);
#endif
                LoadAsyncImpl(assetURI, type, (uid, asset) =>
                {
                    if (onLoadFinished != null)
                        onLoadFinished(asset);
                }, false);
#if USE_UWA
            UWAEngine.PopSample();
#endif
#if UNITY_PROFILER
                UnityEngine.Profiling.Profiler.EndSample();
#endif
            }

            /**
             * <returns>
             * uid代表了逻辑端所持有的资源引用。
             * 对于正在加载中但是被unload的情况，onLoadFinished依然会被调用。返回的UnityObject为null。
             * 如果异步加载的资源已存在，回调不会被立即调用，而是在本帧结束时调用
             * </returns>
             * <remarks>
             * 1. 函数返回的uid 与 onLoadFinished回调中的uid是同一个uid。
             * </remarks>
             */
            public uint LoadAsync(EditorAssetURI assetURI, Type type, Action<uint, UnityObject> onLoadFinished)
            {
#if UNITY_PROFILER
                UnityEngine.Profiling.Profiler.BeginSample("ResourceManager LoadAsync");
                UnityEngine.Profiling.Profiler.BeginSample("ResourceManager LoadAsync:" + assetURI);
#endif
#if USE_UWA
             UWAEngine.PushSample("ResourceManager LoadAsync:" + assetURI);
#endif
                uint uid = LoadAsyncImpl(assetURI, type, onLoadFinished, true);
#if USE_UWA
            UWAEngine.PopSample();
#endif
#if UNITY_PROFILER
                UnityEngine.Profiling.Profiler.EndSample();
                UnityEngine.Profiling.Profiler.EndSample();
#endif
                return uid;
            }

            /// <summary>
            /// 如果是important,不受加载队列影响
            /// </summary>
            /// <param name="assetURI"></param>
            /// <param name="type"></param>
            /// <param name="onLoadFinished"></param>
            /// <param name="isImportant"></param>
            /// <returns></returns>
            public uint LoadAsyncImportant(EditorAssetURI assetURI, Type type, Action<uint, UnityObject> onLoadFinished, bool isImportant = true)
            {
#if UNITY_PROFILER
                UnityEngine.Profiling.Profiler.BeginSample("ResourceManager LoadAsync");
                UnityEngine.Profiling.Profiler.BeginSample("ResourceManager LoadAsync:" + assetURI);
#endif
#if USE_UWA
             UWAEngine.PushSample("ResourceManager LoadAsync:" + assetURI);
#endif
                uint uid = LoadAsyncImpl(assetURI, type, onLoadFinished, true, isImportant);
#if USE_UWA
            UWAEngine.PopSample();
#endif
#if UNITY_PROFILER
                UnityEngine.Profiling.Profiler.EndSample();
                UnityEngine.Profiling.Profiler.EndSample();
#endif
                return uid;
            }

            //方便C#接口的逻辑
            [LuaInterface.NoToLua]
            public uint LoadAsyncImportant<T>(EditorAssetURI assetURI, Action<uint, T> callback) where T : UnityObject
            {
                return LoadAsyncImportant(assetURI, typeof(T), (uid, asset) =>
                {
                    if (callback != null)
                        callback(uid, asset as T);
                });
            }

            public void Unload(ref uint resUID)
            {
                if (resUID == INVALID_UID)
                    return;
                uint uid = resUID;
                resUID = 0;

                if (!UseAssetBundle)
                {
                    UnloadInEditor(uid);
                    return;
                }

                RuntimeAssetName assetName;
                if (!m_clientRef.TryGetValue(uid, out assetName))
                {
                    //LogicError(string.Format("Unload with invalid resuid : {0}", uid));
                    return;
                }
                m_clientRef.Remove(uid);

                AsyncLoadingProcess alp = GetAsyncLoadingProcess(assetName);
                if (alp != null)
                {
                    alp.Cancel(uid);
                    return;
                }

                ExplicitRef expRef = GetExplicitRef(assetName);
                if (expRef == null)
                {
                    //可能是加载资源失败，没有expref
                    //LogicError(string.Format("Unload with invalid RuntimeAssetName : {0}", assetName));
                    return;
                }
                //print("资源管理器回收前 引用计数"+expRef.RefCount+"  path:"+expRef.AssetName);
                expRef.DecreaseRefRecursively();

            }

            /// <summary>
            /// 逻辑层不要使用Resources.UnloadUnusedAssets，使用此接口
            /// 一般而言 全局卸载资源只应该在场景跳转中使用。 因为卸载全局资源和全局gc都是非常卡的行为。
            /// </summary>
            public void UnloadUnusedAssets()
            {
                if (!BuildManifestUtility.GetBuildManifest().IsReleaseVer)
                {
                    LogModule.Instance.Trace(LogModule.LogModuleCode.ResourceSys, "UnloadUnusedAssets Begin!");
                }
#if UNITY_PROFILER
                UnityEngine.Profiling.Profiler.BeginSample("ResourceManager UnloadUnusedAssets");
#endif
                AsyncOperation op = Resources.UnloadUnusedAssets();
                op.completed += OnGlobalUnLoadAssetsDone;
#if UNITY_PROFILER
                UnityEngine.Profiling.Profiler.EndSample();
#endif

            }

            public void LoadScene(EditorAssetURI uri)
            {
                if (!UseAssetBundle)
                    return;

                if (m_sceneABMap.ContainsKey(uri))
                    return;

                RuntimeAssetName ran = m_EditorAssetURI2RuntimeAssetName(uri, typeof(UnityEngine.SceneManagement.Scene));
                if (IsABLoadRecord)
                {
                    ABLoadRecord(ran);
                }

                string fullPath = m_GetABFullPathAndCheckExists(RuntimeAssetName2AssetBundleName(ran));
                if (fullPath == null)
                    return;

                UnityObject dummyObj;
                uint resUID = LoadSyncImpl(uri, typeof(UnityEngine.SceneManagement.Scene), out dummyObj, true);
                if (ReferenceEquals(dummyObj, null))
                {
                    StringBuilder sb = SBC.Acquire();
                    sb.Append("Load Scene AssetBundle failed! Path: ");
                    sb.Append(uri);
                    sb.Append(", AssetBundleName: ");
                    sb.Append(ran);
                    AssetError(SBC.GetStringAndRelease(sb));
                    return;
                }

                m_sceneABMap.Add(uri, resUID);

            }

            /*
            public void LoadSceneAsync(EditorAssetURI uri)
            {

#if !UNITY_EDITOR
                return;
#endif
                if (m_sceneABMap.ContainsKey(uri))
                    return;

                UnityEngine.Profiling.Profiler.BeginSample("ResourceManager LoadSceneAsync :" + uri);
                RuntimeAssetName ran = m_EditorAssetURI2RuntimeAssetName(uri, typeof(UnityEngine.SceneManagement.Scene));
                string fullPath = m_GetABFullPathAndCheckExists(RuntimeAssetName2AssetBundleName(ran));
                ResourceUtil.LoadAssetBundle(fullPath, false, (ab) =>
                {
                    if (ab == null)
                    {
                        StringBuilder sb = SBC.Acquire();
                        sb.Append("Load Scene AssetBundle failed! Path: ");
                        sb.Append(uri);
                        sb.Append(", AssetBundleName: ");
                        sb.Append(ran);
                        AssetError(SBC.GetStringAndRelease(sb));
                        return;
                    }
                    m_sceneABMap.Add(uri, ab);
                });
                UnityEngine.Profiling.Profiler.EndSample();
            }
            */


            /// 注意： 对于scene 的加载没有做任何处理，没做引用计数，没做异步逻辑处理 
            /// 只适用于最简单的加载场景->卸载场景
            public void UnloadScene(EditorAssetURI uri)
            {
                if (!UseAssetBundle)
                    return;
#if UNITY_PROFILER
                UnityEngine.Profiling.Profiler.BeginSample("ResourceManager UnloadScene :" + uri);
#endif
                uint uid;
                if (m_sceneABMap.TryGetValue(uri, out uid))
                {
                    Unload(ref uid);
                    m_sceneABMap.Remove(uri);
                }
#if UNITY_PROFILER
                UnityEngine.Profiling.Profiler.EndSample();
#endif
            }

            private Dictionary<AssetBundleName, uint> m_sceneABMap = new Dictionary<RuntimeAssetName, uint>();

            static Stack<RuntimeAssetName> m_toLoad = new Stack<RuntimeAssetName>();
            static List<RuntimeAssetName> m_toVisit = new List<RuntimeAssetName>();
            static List<IAssetRef> m_tempRefs = new List<IAssetRef>();
            #region 加载回调分帧处理
            static PooledLinkedList<AsyncCallBackParams> WaitForCallBackList = new PooledLinkedList<AsyncCallBackParams>();
            static SelfObjectPool<AsyncCallBackParams> m_unuseCallBackPool = new SelfObjectPool<AsyncCallBackParams>(200);
            private float currCostTime = 0f;

            public float OneCostTime = 0.006f;//s


            #endregion

            public void AddCallBack(Action<UnityEngine.Object> action, UnityEngine.Object obj, uint resID)
            {

                AsyncCallBackParams callbakcItem = m_unuseCallBackPool.CreateObject<AsyncCallBackParams>();
                callbakcItem.Init(resID, action, obj);
                WaitForCallBackList.AddLast(callbakcItem);
            }



            private void CleanOnSyncLoadFailed()
            {
                foreach (IAssetRef assetRef in m_tempRefs)
                    assetRef.DecreaseRef();
                m_tempRefs.Clear();
            }
            /*
            private UnityObject SyncLoadSingleAsset(RuntimeAssetName targetName, bool exp)
            {
                IAssetRef assetRef = CheckExitsRef(targetName, exp);
                if (assetRef != null)
                    return assetRef.Asset;

                if (GetAsyncLoadingProcess(targetName) != null)
                {
                    LogicError(string.Format("Sync loding request conflict with async IO on : {0}", targetName));
                    CleanOnSyncLoadFailed();
                    return null;
                }

                AssetBundleName abName = RuntimeAssetName2AssetName(targetName);
                string fullPath = m_GetABFullPathAndCheckExists(abName);
                if (fullPath == null)
                {
                    if (AllResReady)
                        AssetError(string.Format("All ab downloaded but {0} cannot be found!", abName));

                    CleanOnSyncLoadFailed();
                    return null;
                }

                AssetBundle bundle = AssetBundle.LoadFromFile(fullPath);
                if (bundle == null)
                {
                    AssetError(string.Format("load ab {0} failed!", abName));
                    CleanOnSyncLoadFailed();
                    return null;
                }

                LogModule.Instance.Trace(LogModule.LogModuleCode.AssetBundleLoadUnload, string.Format("AssetBundle: {0} loaded ", abName));

                //m_newLoadedBundles.Add(bundle);

                UnityObject[] assets = bundle.LoadAllAssets();
                if (exp)
                {
                    assetRef = CreateExplicitRef(targetName, assets[0], bundle);
                    assetRef.IncreaseRef();
                }
                else
                {
                    assetRef = CreateImplicitRef(targetName, assets[0], bundle);
                }
                m_refs.Add(assetRef);
                return assets[0];
            }

            private UnityObject SyncLoadAssetsRecursive(RuntimeAssetName targetName, bool exp, string parentName = "")
            {
                RuntimeAssetName[] deps = GetDepInfo(targetName);
                if (deps != null && deps.Length > 0)
                {
                    for(int i = 0; i < deps.Length; i++)
                    {
                        SyncLoadAssetsRecursive(deps[i], exp);
                    }
                }

                return SyncLoadSingleAsset(targetName, exp);
            }
            */

            private void LogAsyncConflictWarning(string assetName)
            {
                //StringBuilder sb8 = LuaInterface.StringBuilderCache.Acquire();
                //sb8.Append("Sync loding request conflict with async IO on : ");
                //sb8.Append(assetName);
                //LogicWarning(LuaInterface.StringBuilderCache.GetStringAndRelease(sb8));
            }

            private void LogLoadAssetFailedError(string assetName)
            {
                StringBuilder sb9 = SBC.Acquire();
                sb9.Append("load ab ");
                sb9.Append(RuntimeAssetName2AssetBundleName(assetName));
                sb9.Append(" failed!");
                AssetError(SBC.GetStringAndRelease(sb9));
            }

            private void LogBundleNilError(string assetName)
            {
                StringBuilder sb9 = SBC.Acquire();
                sb9.Append("assetName ");
                sb9.Append(RuntimeAssetName2AssetBundleName(assetName));
                sb9.Append(" bundle nil!");
                AssetError(SBC.GetStringAndRelease(sb9));
            }

            private void LogAssetNilError(string assetName)
            {
                StringBuilder sb9 = SBC.Acquire();
                sb9.Append("assetName ");
                sb9.Append(RuntimeAssetName2AssetBundleName(assetName));
                sb9.Append(" asset nil!");
                AssetError(SBC.GetStringAndRelease(sb9));
            }

            private void LogABPathNotExistError(string abName)
            {
                StringBuilder sb10 = SBC.Acquire();
                sb10.Append("All ab downloaded but ");
                sb10.Append(abName);
                sb10.Append(" cannot be found!");
                Debug.LogError("ab资源丢失"+ sb10.ToString());
                AssetError(SBC.GetStringAndRelease(sb10));
            }

            private void LogLoadABFailedError(string fullPath)
            {
                StringBuilder sb11 = SBC.Acquire();
                sb11.Append("load ab ");
                sb11.Append(fullPath);
                sb11.Append(" failed!");
                AssetError(SBC.GetStringAndRelease(sb11));
            }

            private void LogABLoadFinishedTrace(string abName)
            {
                StringBuilder sb12 = SBC.Acquire();
                sb12.Append("sync load assetbundle: ");
                sb12.Append(abName);
                sb12.Append(" finished");
                LogModule.Instance.Trace(LogModule.LogModuleCode.AssetBundleStatus, SBC.GetStringAndRelease(sb12));
            }

            private void CheckToLogError(AssetBundle _bundle, UnityObject _asset, string assetName)
            {
                if (_bundle == null)
                {
                    LogBundleNilError(assetName);
                }
                else if (_asset == null)
                {
                    LogAssetNilError(assetName);
                }
            }

            private void FillVisit()
            {
                while (m_toVisit.Count > 0)
                {
                    RuntimeAssetName target = m_toVisit[m_toVisit.Count - 1];
                    m_toVisit.RemoveAt(m_toVisit.Count - 1);
                    m_toLoad.Push(target);
                    List<RuntimeAssetName> deps = GetDepInfo(target);
                    if (deps.Count > 0)
                        m_toVisit.AddRange(deps);
                    m_depInfoPool.Return(deps);
                }
            }

            private UnityObject LoadAssetFromBundle(AssetBundle bundle)
            {
                UnityObject asset = null;
                if (bundle.isStreamedSceneAssetBundle)
                {
                    asset = DummySceneAsset;
                    if (asset == null)
                    {
                        DummySceneAsset = new GameObject("dummy_scene_asset");
                        asset = DummySceneAsset;

                    }
                }
                else
                {
                    asset = bundle.LoadAsset(bundle.GetAllAssetNames()[0]);
                }

                return asset;
            }

            private void HandleAssetReady(IAssetRef assetRef, string assetName, AssetBundle _bundle, UnityObject _asset, bool exp, ref UnityObject result)
            {
                CheckToLogError(_bundle, _asset, assetName);

                // Debug.Assert(_bundle != null && _asset != null);
                if (exp)
                {
                    assetRef = GetExplicitRef(assetName);
                    if (assetRef == null)
                        assetRef = CreateExplicitRef(assetName, _asset, _bundle);
                    assetRef.IncreaseRef();
                }
                else
                {
                    assetRef = CreateImplicitRef(assetName, _asset, _bundle);
                }
                m_tempRefs.Add(assetRef);

                if (m_toLoad.Count == 0)
                    result = _asset;
            }

            private bool HandleAsyncLoadProcess(ref UnityObject asset, ref AssetBundle bundle, AsyncLoadingProcess alp, string assetName)
            {
                LogAsyncConflictWarning(assetName);
                asset = alp.ForceSyncDone();
                bundle = alp.Bundle;

                if (asset == null || bundle == null)
                {
                    LogLoadAssetFailedError(assetName);
                    CleanOnSyncLoadFailed();
                    return false;
                }

                return true;

            }


            /// <summary>
            /// 同步加载
            /// </summary>
            /// <param name="targetName"></param>
            /// <returns></returns>
            private UnityObject SyncLoadAssets(RuntimeAssetName targetName, bool exp)
            {
                // 检测是否已经存在了资源，如果有，则直接返回
                IAssetRef assetRef = CheckExitsRef(targetName, exp, true);
                if (assetRef != null)
                    return assetRef.Asset;

                // 否则进行加载
                m_toLoad.Clear();
                m_toVisit.Clear();

                // 得到所有直接依赖和间接依赖的子Asset,放到m_toLoad中，这是一个栈
                m_toVisit.Add(targetName);
                FillVisit();

                UnityObject result = null;
                //loading deps 加载所有依赖的子Asset
                while (m_toLoad.Count > 0)
                {
                    RuntimeAssetName assetName = m_toLoad.Pop();
                    UnityObject asset = null;
                    AssetBundle bundle = null;

                    // 看下是否在进行异步加载，如果是的话，则直接转换成同步加载
                    AsyncLoadingProcess alp = GetAsyncLoadingProcess(assetName);
                    if (alp != null)
                    {
                        if (!HandleAsyncLoadProcess(ref asset, ref bundle, alp, assetName))
                        {
                            return null;
                        }

                        HandleAssetReady(assetRef, assetName, bundle, asset, exp, ref result);
                    }
                    else // 没有在进行异步加载，则直接进行加载
                    {
                        // 非递归进行引用检查
                        assetRef = PureGetExitsRef(assetName, exp);
                        if (assetRef != null)
                        {
                            HandleAssetReady(assetRef, assetName, assetRef.Bundle, assetRef.Asset, exp, ref result);
                            continue;
                        }

                        AssetBundleName abName = RuntimeAssetName2AssetName(assetName);
                        string fullPath = m_GetABFullPathAndCheckExists(abName);
                        if (fullPath == null)
                        {
                            if (AllResReady)
                            {
                                LogABPathNotExistError(abName);
                            }

                            CleanOnSyncLoadFailed();
                            return null;
                        }

                        bundle = AssetBundle.LoadFromFile(fullPath);
                        if (bundle == null)
                        {
                            LogLoadABFailedError(fullPath);
                            CleanOnSyncLoadFailed();
                            return null;
                        }

                        if (!BuildManifestUtility.GetBuildManifest().IsReleaseVer && IsDebugTarget(abName))
                        {
                            LogABLoadFinishedTrace(abName);
                        }

                        asset = LoadAssetFromBundle(bundle);

                        if (ReferenceEquals(asset, null))
                        {
                            bundle.Unload(true);
                            LogLoadABFailedError(abName);
                            CleanOnSyncLoadFailed();
                            return null;
                        }

                        HandleAssetReady(assetRef, assetName, bundle, asset, exp, ref result);
                    }
                }
                m_tempRefs.Clear();
                return result;
            }

            /*
           * Object obj;
           * uint uid = GameKernel.ResourceMgr.LoadSync<Object>(path, out obj);
           * if(uid != 0){
           * AssetBundle ab =  GetLoadedBundleFromExpRef(uid);
           * }
           */
            public AssetBundle GetLoadedBundleFromExpRef(uint uid)
            {
                if (!m_clientRef.ContainsKey(uid))
                    return null;

                RuntimeAssetName name = m_clientRef[uid];

                if (!m_explicitRefs.ContainsKey(name))
                    return null;

                return m_explicitRefs[name].Bundle;
            }
            private AsyncLoadingProcess GetAsyncLoadingProcess(RuntimeAssetName assetName)
            {
                AsyncLoadingProcess process;
                m_asyncLoadingProcesses.TryGetValue(assetName, out process);
                return process;
            }



            private void RemoveAsyncLoadingProcess(RuntimeAssetName assetName)
            {
                if (m_asyncLoadingProcesses.ContainsKey(assetName))
                {
                    m_asyncLoadingProcesses.Remove(assetName);
                }
            }

            private AsyncLoadingProcess CreateAsyncLoadingProcess(RuntimeAssetName assetName, AsyncLoadingProcess parentProcess)
            {
                AsyncLoadingProcess alp = null;
                if (m_asyncLoadingProcesses.TryGetValue(assetName, out alp))
                {
                    alp.AddParentProcess(parentProcess);
                }
                else
                {
                    alp = AsyncLoadingProcess.Dequeue(assetName, this, parentProcess);
                    m_asyncLoadingProcesses.Add(assetName, alp);
                    alp.Start();
                }

                return alp;
            }

            private AsyncLoadingProcess CreateAsyncLoadingProcess(RuntimeAssetName assetName, Action<UnityObject> onAssetLoaded)
            {
                AsyncLoadingProcess alp = null;
                if (m_asyncLoadingProcesses.TryGetValue(assetName, out alp))
                {
                    alp.AddImplicitResRef(onAssetLoaded);
                }
                else
                {
                    alp = AsyncLoadingProcess.Dequeue(assetName, this, onAssetLoaded);
                    m_asyncLoadingProcesses.Add(assetName, alp);
                    alp.Start();
                }

                return alp;
            }



            private AsyncLoadingProcess CreateAsyncLoadingProcess(RuntimeAssetName assetName, uint resUID, Action<UnityObject> onAssetLoaded)
            {
                AsyncLoadingProcess alp = null;
                if (m_asyncLoadingProcesses.TryGetValue(assetName, out alp))
                {
                    alp.AddExplicitlyResRef(resUID, onAssetLoaded);
                }
                else
                {
                    alp = AsyncLoadingProcess.Dequeue(assetName, this, resUID, onAssetLoaded);
                    m_asyncLoadingProcesses.Add(assetName, alp);
                    alp.Start();
                }

                return alp;
            }

            class AssetLeakInfo
            {
                public string name;
                public int refCount;
            }


            #region asset ref

            /// <summary>
            /// Bundle有效,则尝试reload asset ,否则直接清理卸载(不知道为何Asset被释放)
            /// </summary>
            /// <param name="assetName"></param>
            [LuaInterface.NoToLua]
            public bool RevertInvaildExplicitaAeest(RuntimeAssetName assetName, ref uint resID)
            {
                ExplicitRef expRef = null;
                m_explicitRefs.TryGetValue(assetName, out expRef);
                if (expRef == null)
                {
                    return false;
                }

                if (expRef.Bundle != null && expRef.Asset == null)
                {
                    UnityObject asset = LoadAssetFromBundle(expRef.Bundle);

                    if (ReferenceEquals(asset, null))
                    {
                        //卸载重新load asset 失败的bundle,不应该出现
                        if (!BuildManifestUtility.GetBuildManifest().IsReleaseVer)
                        {
                            Debug.LogFormat("卸载重新load asset 失败的bundle  资源{0}  引用次数{1}" + assetName + expRef.RefCount);
                        }

                        if (m_clientRef.ContainsKey(resID))
                        {
                            m_clientRef.Remove(resID);
                        }

                        AsyncLoadingProcess alp = GetAsyncLoadingProcess(assetName);
                        if (alp != null)
                        {
                            alp.Cancel(resID);
                        }

                        int refCount = expRef.RefCount;
                        for (int i = 0; i < refCount; i++)
                        {
                            expRef.DecreaseRefRecursively();
                        }
                        return false;
                    }
                    else
                    {
                        //重置
                        if (!BuildManifestUtility.GetBuildManifest().IsReleaseVer)
                        {
                            Debug.LogFormat("重置被异常释放的Asset  资源{0}  引用次数{1}" + assetName + expRef.RefCount);
                        }

                        int refCount = expRef.RefCount;
                        expRef.ReInit(expRef.AssetName, expRef.Bundle, asset, this);
                        expRef.IncreaseRef(refCount);
                        return true;
                    }

                }
                else if (expRef.Bundle == null)
                {
                    if (!BuildManifestUtility.GetBuildManifest().IsReleaseVer)
                    {
                        Debug.LogErrorFormat("AssetCache.GetSync获取了1个Asset==null的资源 资源:{0} 引用计数:{1}  resID{2}" + assetName + expRef.RefCount + resID);
                    }

                    if (m_clientRef.ContainsKey(resID))
                    {
                        m_clientRef.Remove(resID);
                    }

                    m_explicitRefs.Remove(assetName);
                    if (expRef != null)
                    {
                        expRef.Reset();
                        m_unuseExpPool.Enqueue(expRef);
                    }
                    return false;
                }
                else
                {
                    //正常啊
                    return true;
                }
            }


            private ExplicitRef GetExplicitRef(RuntimeAssetName assetName)
            {
                ExplicitRef expRef = null;
                m_explicitRefs.TryGetValue(assetName, out expRef);
                return expRef;
            }

            private ImplicitRef GetImplicitRef(RuntimeAssetName assetName)
            {
                ImplicitRef impRef = null;
                m_implicitRefs.TryGetValue(assetName, out impRef);
                //GCTODO: 没搞明白为什么会有被释放的资源还存在于m_implicitRefs中, 只能在这里打个补丁了
                if (impRef != null && impRef.Asset == null)
                {
                    BundleInfo info = m_bundleInfoMap[impRef.Bundle];
                    if (info.ExplicitRefCout == 0)
                    {
                        RemoveBundle(impRef.Bundle);
                    }

                    m_implicitRefs.Remove(assetName);
                    if (impRef != null)
                    {
                        impRef.Reset();
                        m_unuseImpPool.Enqueue(impRef);
                    }
                    return null;
                }
                else
                {
                    return impRef;
                }
            }


            private ExplicitRef CreateExplicitRef(RuntimeAssetName assetName, UnityObject asset, AssetBundle bundle)
            {
                if (!BuildManifestUtility.GetBuildManifest().IsReleaseVer && IsDebugTarget(assetName))
                {
                    StringBuilder sb35 = SBC.Acquire();
                    sb35.Append("Create Explicit Ref: ");
                    sb35.Append(assetName);
                    LogModule.Instance.Trace(LogModule.LogModuleCode.ResRefStatus, SBC.GetStringAndRelease(sb35));
                }

                ExplicitRef expRef = GetExplicitRef(assetName);
                if (expRef != null)
                {
                    StringBuilder sb36 = SBC.Acquire();
                    sb36.Append("Try to create explicit ref with same explicit ref exits! RuntimeAssetName : ");
                    sb36.Append(assetName);
                    LogicError(SBC.GetStringAndRelease(sb36));
                    return expRef;
                }
                if (m_unuseExpPool.Count > 0)
                {
                    expRef = m_unuseExpPool.Dequeue();
                    expRef.ReInit(assetName, bundle, asset, this);
                }
                else
                {
                    expRef = new ExplicitRef(assetName, bundle, asset, this);
                }

                m_explicitRefs.Add(assetName, expRef);

                BundleInfo info;
                if (m_bundleInfoMap.TryGetValue(bundle, out info))
                {
                    info.ExplicitRefCout++;
                    m_bundleInfoMap[bundle] = info;
                }
                else
                {
                    m_bundleInfoMap.Add(bundle, new BundleInfo { ExplicitRefCout = 1, ImplicitRefered = false });
                }

                return expRef;
            }

            private ImplicitRef CreateImplicitRef(RuntimeAssetName assetName, UnityObject asset, AssetBundle bundle)
            {
                ImplicitRef impRef = GetImplicitRef(assetName);
                if (impRef != null)
                {
                    //LogicError(string.Format("Try to create implicit ref with same implicit ref exits! RuntimeAssetName : {0}", assetName));
                    return impRef;
                }
                if (m_unuseImpPool.Count > 0)
                {
                    impRef = m_unuseImpPool.Dequeue();
                    impRef.ReInit(assetName, bundle, asset, this);
                }
                else
                {
                    impRef = new ImplicitRef(assetName, bundle, asset, this);
                }

                m_implicitRefs.Add(assetName, impRef);

                BundleInfo info;
                if (m_bundleInfoMap.TryGetValue(bundle, out info))
                {
                    info.ImplicitRefered = true;
                    m_bundleInfoMap[bundle] = info;
                }
                else
                {
                    m_bundleInfoMap.Add(bundle, new BundleInfo { ExplicitRefCout = 0, ImplicitRefered = true });
                }

                if (!BuildManifestUtility.GetBuildManifest().IsReleaseVer && IsDebugTarget(assetName))
                {
                    StringBuilder sb37 = SBC.Acquire();
                    sb37.Append("CreateImplicitRef for asset: ");
                    sb37.Append(asset.name);
                    LogModule.Instance.Trace(LogModule.LogModuleCode.ResRefStatus, SBC.GetStringAndRelease(sb37));
                }

                return impRef;
            }

            private IAssetRef GetExistAsset(RuntimeAssetName assetName)
            {
                ExplicitRef expRef = GetExplicitRef(assetName);
                if (expRef != null)
                {
                    return expRef;
                }
                ImplicitRef impRef = GetImplicitRef(assetName);
                if (impRef != null)
                {
                    return impRef;
                }

                return null;
            }

            private void RemoveExplicitRef(RuntimeAssetName assetName)
            {
                if (!BuildManifestUtility.GetBuildManifest().IsReleaseVer && IsDebugTarget(assetName))
                {
                    StringBuilder sb38 = SBC.Acquire();
                    sb38.Append("ExplitRef : ");
                    sb38.Append(assetName);
                    sb38.Append(" be removed");
                    LogModule.Instance.Trace(LogModule.LogModuleCode.ResRefStatus, SBC.GetStringAndRelease(sb38));
                }
                ExplicitRef expRef = m_explicitRefs[assetName];
                m_explicitRefs.Remove(assetName);

                if (expRef != null)
                {
                    BundleInfo info = m_bundleInfoMap[expRef.Bundle];
                    info.ExplicitRefCout--;
                    m_bundleInfoMap[expRef.Bundle] = info;

                    if (info.ExplicitRefCout == 0)
                    {
                        if (!info.ImplicitRefered)
                        {
                            RemoveBundle(expRef.Bundle);
                        }
                    }
                    else if (info.ExplicitRefCout < 0)
                    {
                        StringBuilder sb39 = SBC.Acquire();
                        sb39.Append("Invalid bundle explicit ref count for : ");
                        sb39.Append(assetName);
                        LogicError(SBC.GetStringAndRelease(sb39));
                    }
                    expRef.Reset();
                    m_unuseExpPool.Enqueue(expRef);
                }
                else Debug.LogWarning("expRef is null");
            }

            /// <summary>
            /// 只得到引用，不进行增加计数
            /// </summary>
            private IAssetRef PureGetExitsRef(RuntimeAssetName assetName, bool exp)
            {
                // 如果是显式加载
                if (exp)
                {
                    // 得到显式加载的引用
                    ExplicitRef expRef = GetExplicitRef(assetName);
                    if (expRef != null) // 不为空，则进行资源引用的递归增加
                    {
                        return expRef;
                    }

                    ImplicitRef impRef = GetImplicitRef(assetName);
                    if (impRef != null)
                    {
                        expRef = CreateExplicitRef(assetName, impRef.Asset, impRef.Bundle);
                        return expRef;
                    }
                }
                else
                {
                    ImplicitRef impRef = GetImplicitRef(assetName);
                    if (impRef != null)
                        return impRef;

                    ExplicitRef expRef = GetExplicitRef(assetName);
                    if (expRef != null)
                    {
                        return CreateImplicitRef(assetName, expRef.Asset, expRef.Bundle);
                    }
                }

                return null;
            }

            private IAssetRef CheckExitsRef(RuntimeAssetName assetName, bool exp, bool expRecursive = false)
            {
                // 如果是显式加载
                if (exp)
                {
                    // 得到显式加载的引用
                    ExplicitRef expRef = GetExplicitRef(assetName);
                    if (expRef != null) // 不为空，则进行资源引用的递归增加
                    {
                        if (expRecursive) // 什么时候会用到非递归呢？
                            expRef.IncreaseRefRecursively();
                        else
                            expRef.IncreaseRef();
                        return expRef;
                    }

                    ImplicitRef impRef = GetImplicitRef(assetName);
                    if (impRef != null)
                    {
                        expRef = CreateExplicitRef(assetName, impRef.Asset, impRef.Bundle);
                        if (expRecursive)
                            expRef.IncreaseRefRecursively();
                        else
                            expRef.IncreaseRef();
                        return expRef;
                    }
                }
                else
                {
                    ImplicitRef impRef = GetImplicitRef(assetName);
                    if (impRef != null)
                        return impRef;

                    ExplicitRef expRef = GetExplicitRef(assetName);
                    if (expRef != null)
                    {
                        return CreateImplicitRef(assetName, expRef.Asset, expRef.Bundle);
                    }
                }

                return null;
            }


            #endregion


            private void RemoveBundle(AssetBundle bundle)
            {
                m_bundleInfoMap.Remove(bundle);
                if (!BuildManifestUtility.GetBuildManifest().IsReleaseVer && IsDebugTarget(bundle.name))
                {
                    StringBuilder sb40 = SBC.Acquire();
                    sb40.Append("AssetBundle: ");
                    sb40.Append(bundle.name);
                    sb40.Append(" Unloaded");
                    LogModule.Instance.Trace(LogModule.LogModuleCode.AssetBundleStatus, SBC.GetStringAndRelease(sb40));
                }
                bundle.Unload(true);
            }


            private uint LoadSyncImpl(EditorAssetURI assetURI, Type type, out UnityObject asset, bool explicitly)
            {
                if (assetURI == null || assetURI.Length == 0)
                {
                    Debug.LogWarning("Load Resource with empty path!!");
                    asset = null;
                    return 0;
                }

                //float loadStartTime = Time.realtimeSinceStartup;
                uint resUID = explicitly ? GenUID() : INVALID_UID;
                if (!UseAssetBundle)
                {
                    asset = LoadInEditor(assetURI, type, resUID);
                }
                else
                {
                    RuntimeAssetName rtName = m_EditorAssetURI2RuntimeAssetName(assetURI, type);
                    if (IsABLoadRecord)
                    {
                        ABLoadRecord(rtName);
                    }

                    asset = SyncLoadAssets(rtName, explicitly);
                    if (explicitly)
                    {
                        m_clientRef.Add(resUID, rtName);
                    }
                }
                //float usedTime = Time.realtimeSinceStartup - loadStartTime;
                return resUID;

            }

            private uint LoadAsyncImpl(string assetURI, Type type, Action<uint, UnityObject> onLoadFinished, bool explicitly, bool isImportant = false)
            {
                if (assetURI == null || assetURI.Length == 0)
                {
                    Debug.LogWarning("Load Resource with empty path!!");
                    onLoadFinished(0, null);
                    return 0;
                }

                //float loadStartTime = Time.realtimeSinceStartup;
                uint resUID = explicitly ? GenUID() : INVALID_UID;

                if (!UseAssetBundle)
                {
                    UpdatableRunner.Instance.StartCoroutine(SimuAsyncLoad(assetURI, type, resUID, onLoadFinished));
                }
                else
                {
                    if (explicitly)
                    {
                        AsyncParams asyncParams = AsyncParams.Dequeue(assetURI, type, onLoadFinished, resUID);
                        // Debug.LogError("assetURI:"+ assetURI+ " resUID:"+ resUID);
                        RequestStartLoad(asyncParams, isImportant);
                    }
                    else
                    {
                        RuntimeAssetName rtName = m_EditorAssetURI2RuntimeAssetName(assetURI, type);
                        if (IsABLoadRecord)
                        {
                            ABLoadRecord(rtName);
                        }
                        CreateAsyncLoadingProcess(rtName, (asset) =>
                        {
                            onLoadFinished(resUID, asset);
                        });
                    }
                }

                return resUID;
            }

            private void RealLoadAsync(AsyncParams asyncParams)
            {
                RuntimeAssetName rtName = m_EditorAssetURI2RuntimeAssetName(asyncParams.m_Url, asyncParams.m_Type);
                if (IsABLoadRecord)
                {
                    ABLoadRecord(rtName);
                }

                m_clientRef.Add(asyncParams.m_ResId, rtName);
                CreateAsyncLoadingProcess(rtName, asyncParams.m_ResId, (asset) =>
                {
                    if (asyncParams.m_OnLoadFinished == null)
                    {
                        Debug.LogWarning(rtName + "asyncParams.m_OnLoadFinished==null");
                        return;
                    }

                    if (asyncParams.m_ResId != 0)
                    {
                        if (mAsyncLoadingDic.ContainsKey(asyncParams.m_ResId))
                        {
                            mAsyncLoadingDic.Remove(asyncParams.m_ResId);
                        }

                    }
                    DoNextAsyncLoading();
                    asyncParams.m_OnLoadFinished(asyncParams.m_ResId, asset);
                    AsyncParams.Enqueue(asyncParams);

                });

            }


            #region 运行期数据设置接口

            //Editor模式下资源路径转化为运行时的资源名称
            private void SetURITranslater(Func<EditorAssetURI, Type, RuntimeAssetName> EditorAssetURI2RuntimeAssetName)
            {
                m_EditorAssetURI2RuntimeAssetName = EditorAssetURI2RuntimeAssetName;
            }

            //设置获得运行期AssetBundle的绝对路径的接口。
            //接口必须同时检测目标文件是否存在。 不存在返回null
            private void SetABFullPathFunc(Func<AssetBundleName, string> GetAbFullPathAndCheckExists)
            {
                m_GetABFullPathAndCheckExists = GetAbFullPathAndCheckExists;
            }

            private Func<EditorAssetURI, Type, RuntimeAssetName> m_EditorAssetURI2RuntimeAssetName;
            private Func<AssetBundleName, string> m_GetABFullPathAndCheckExists;
            #endregion

            private static AssetBundleName RuntimeAssetName2AssetBundleName(RuntimeAssetName name)
            {
                return name;
            }

            //获取AB内asset资源名
            private static AssetBundleName RuntimeAssetName2AssetName(RuntimeAssetName name)
            {
                return name;
            }

            /*
           * 当全局卸载完全结束时，调用此接口
           * 理想情况下，所有的资源应该都是显式获取，因此此处不应该出现一次卸载大量Ab的情况
           */
            static List<RuntimeAssetName> toRemoveList = new List<RuntimeAssetName>();
            private void OnGlobalUnLoadAssetsDone(AsyncOperation _)
            {
                toRemoveList.Clear();

                foreach (var item in m_implicitRefs)
                {
                    if (item.Value.Asset == null)
                    {
                        BundleInfo info = m_bundleInfoMap[item.Value.Bundle];
                        if (info.ExplicitRefCout == 0)
                        {
                            RemoveBundle(item.Value.Bundle);
                        }
                        toRemoveList.Add(item.Key);
                    }
                }

                foreach (RuntimeAssetName name in toRemoveList)
                {
                    ImplicitRef impEntity = m_implicitRefs[name];
                    if (impEntity != null)
                    {
                        impEntity.Reset();
                        m_unuseImpPool.Enqueue(impEntity);
                    }
                    m_implicitRefs.Remove(name);
                }
            }

            private ListPool<RuntimeAssetName> m_depInfoPool = new ListPool<RuntimeAssetName>(64);
            public List<RuntimeAssetName> GetDepInfo(RuntimeAssetName assetURI)
            {

                List<RuntimeAssetName> depInfo = m_depInfoPool.Get();
                uint[] deps = null;

                if (true)
                {
                    if (m_assetsDependInfo.ContainsKey(assetURI))
                    {
                        deps = m_assetsDependInfo[assetURI];
                    }
                    else
                    {
                        TrySetBundleDepInfo(assetURI);
                        m_assetsDependInfo.TryGetValue(assetURI, out deps);
                    }

                    if (deps != null)
                    {
                        foreach (uint nameIndex in deps)
                            depInfo.Add(m_allBundleNames[nameIndex]);
                    }
                    return depInfo;
                }
                else //热更流程没走完，走这里。因为manifest文件还要重新加载
                {
                    if (Manifest == null)
                    {
#if UNITY_EDITOR
                        Debug.LogError("查询依赖文件为空 请检查是否是ab环境，并且本地缺少ab资源");
#endif
                    }
                    string[] depArray = Manifest.GetDirectDependencies(assetURI);
                    depInfo.AddRange(depArray);
                    return depInfo;
                }
            }

            public bool UseAssetBundle
            {
                get
                {
#if UNITY_EDITOR
                    return UseAssetBundleInEditor;
#else
                    return true;
#endif
                }
            }

            private static int _flagNumUseAssetBundle = -1;
            private const string _kUseAssetBundle = "UseAssetBundleInEditor";

            [LuaInterface.NoToLuaAttribute]
            public static bool UseAssetBundleInEditor
            {
                get
                {
#if UNITY_EDITOR
                    if (_flagNumUseAssetBundle == -1)
                    {
                        _flagNumUseAssetBundle = UnityEditor.EditorPrefs.GetBool(_kUseAssetBundle, false) ? 1 : 0;
                    }

                    return _flagNumUseAssetBundle == 1;
#else
                    return false;
#endif
                }

                set
                {
#if UNITY_EDITOR
                    int newValue = value ? 1 : 0;

                    if (newValue != _flagNumUseAssetBundle)
                    {
                        _flagNumUseAssetBundle = newValue;
                        UnityEditor.EditorPrefs.SetBool(_kUseAssetBundle, value);
                    }
#else
#endif
                }
            }

            private const uint INVALID_UID = 0;
            private uint UID = INVALID_UID + 1;
            private uint GenUID()
            {
                return UID++;
            }

            #region Debug 
            //将资源类错误作为error报告
            public bool TreatAssetWaringAsError { get; set; }


            private bool m_debugMode = false;
            public void SetDebugMode(bool enable)
            {
                m_debugMode = enable;
            }

            private void LogicError(string msg)
            {
                StringBuilder sb = SBC.Acquire();
                sb.Append("ResourceManager Logic Error: ");
                sb.Append(msg);
                Debug.LogErrorFormat(SBC.GetStringAndRelease(sb));
            }

            private void LogicWarning(string msg)
            {
                StringBuilder sb = SBC.Acquire();
                sb.Append("ResourceManager Logic Warning: ");
                sb.Append(msg);
                Debug.LogWarningFormat(SBC.GetStringAndRelease(sb));
            }

            private void AssetError(string msg, bool forceErroe = false)
            {
                StringBuilder sb = SBC.Acquire();
                if (TreatAssetWaringAsError || forceErroe)
                {
                    sb.Append("ResourceManager Asset Error: ");
                    sb.Append(msg);
                    Debug.LogErrorFormat(SBC.GetStringAndRelease(sb));
                }
                else
                {
                    sb.Append("ResourceManager Asset Warning: ");
                    sb.Append(msg);
                    Debug.LogWarningFormat(SBC.GetStringAndRelease(sb));
                }
            }
            #endregion

            #region Editor
            private const string RES_PATH_EDITOR = "Assets/Res_Best/";

            private Dictionary<uint, UnityObject> m_editorExplicitlyRefMap = new Dictionary<uint, UnityObject>();

            private static Dictionary<string, List<string>> dicExtensionInfo = new Dictionary<string, List<string>>()
        {
            {"GameObject",new List<string>() { ".prefab" } },
            {"Object",new List<string>() { ".prefab", ".mat" } },
            {"AudioClip",new List<string>() { ".mp3", ".ogg" } },
            {"RuntimeAnimatorController",new List<string>() {".controller"} },
            {"ExternalBehaviorTree",new List<string>() { ".asset"} },
            {"Texture2D",new List<string>() { ".png", ".tga" } },
            {"Mesh",new List<string>() { ".mesh" } },
            {"AnimationClip",new List<string>() { ".anim" } },
            {"TimeLineData",new List<string>() { ".asset"} },
			 // add Lonely
			{"AnimatorOverrideController",new List<string>() { ".overrideController" } },
            {"StateMapMotionInfoDatas",new List<string>() { ".asset" } },
            {"Material",new List<string>() { ".mat" } },
            {"ShaderVariantCollection",new List<string>() { ".shadervariants" } },
            {"PostProcessResources",new List<string>() { ".asset" } },
            {"PostProcessProfile", new List<string>()  { ".asset" } },
        };

            [LuaInterface.NoToLua]
            public UnityObject LoadInEditor(string path, Type type, uint resUID)
            {
                UnityObject asset = LoadInEditorImpl(path, type);
                if (resUID != INVALID_UID && asset != null)
                {
                    m_editorExplicitlyRefMap.Add(resUID, asset);
                }
                return asset;
            }

            private void UnloadInEditor(uint resUID)
            {
                UnityObject asset = null;
                if (m_editorExplicitlyRefMap.TryGetValue(resUID, out asset))
                {
                    m_editorExplicitlyRefMap.Remove(resUID);
                    if (asset is GameObject || asset is Component || asset is AssetBundle)
                    {
                        //UpdatableRunner.Instance.StartCoroutine(RunUnload());
                    }
                    else
                    {
                        Resources.UnloadAsset(asset);
                    }
                }
            }

            private IEnumerator RunUnload()
            {
                yield return new WaitForEndOfFrame();
                Resources.UnloadUnusedAssets();
            }

            private UnityObject LoadInEditorImpl(string path, Type type)
            {
#if UNITY_EDITOR
                string fileName = Path.GetFileName(path);
                string rootPath = Path.GetDirectoryName(path);
                string resPath = string.Format("{0}{1}", RES_PATH_EDITOR, rootPath);

                string[] tpath = { resPath };
                if (!Directory.Exists(tpath[0]))
                {
                    /*
                     * 
                     * LogicError(string.Format("Load asset failed! asset path: {0}", tpath[0]));
                     */
                    return null;
                }

                List<string> listExtension;
                if (dicExtensionInfo.TryGetValue(type.Name, out listExtension))
                {
                    for (int i = 0; i < listExtension.Count; i++)
                    {
                        StringBuilder sb = SBC.Acquire();
                        sb.Append(resPath);
                        sb.Append("/");
                        sb.Append(fileName);
                        sb.Append(listExtension[i]);
                        string resFullPath = SBC.GetStringAndRelease(sb);
                        UnityObject obj = UnityEditor.AssetDatabase.LoadAssetAtPath(resFullPath, type);
                        if (obj != null && obj.name == fileName)
                        {
                            //Debug.LogError("resFullPath:"+ resFullPath);
                            return obj;
                        }
                    }
                }

                /* 这种模式弊远大于利，不再支持。
                //2:查找GUIID获取资源
                string[] guids = UnityEditor.AssetDatabase.FindAssets(fileName, tpath);
                if (guids != null && guids.Length > 0)
                {
                    for (int i = 0; i < guids.Length; ++i)
                    {
                        string fpath = UnityEditor.AssetDatabase.GUIDToAssetPath(guids[i]);
                        UnityObject obj = UnityEditor.AssetDatabase.LoadAssetAtPath(fpath, type);
                        Debug.LogWarning("未找到该类型的后缀，为加快加载速度请在dicExtensionInfo中添加:" + type.Name + "   GetExtension:" + Path.GetExtension(fpath));
                        if (obj != null && obj.name == fileName)
                            return obj;
                    }
                }
                */
                StringBuilder sb1 = SBC.Acquire();
                sb1.Append("资源路径错误或 资源类型需添加至dicExtensionInfo中, path: ");
                sb1.Append(path);
                sb1.Append(", type: ");
                sb1.Append(type);
                LogicWarning(SBC.GetStringAndRelease(sb1));
#endif
                return null;
            }

            private IEnumerator SimuAsyncLoad(string assetURI, Type type, uint resUID, Action<uint, UnityObject> onLoadFinished)
            {
                //yield return new WaitForSeconds(UnityEngine.Random.Range(0.001f, 0.002f));
                yield return null;                      //这里导致一些特效显示问题，先这么处理
                if (onLoadFinished != null)
                    onLoadFinished(resUID, LoadInEditor(assetURI, type, resUID));
            }
            #endregion

            AssetBundleManifest Manifest = null;
            AssetBundle mainAssetBundle = null;
            private void LoadManifest()
            {
                if (!BuildManifestUtility.GetBuildManifest().IsReleaseVer)
                    Debug.Log("[ResourceManager] Load Manifest!");

                float startTime = Time.realtimeSinceStartup;
                if (Manifest != null)
                {
                    Resources.UnloadAsset(Manifest);
                    Manifest = null;
                }
                if (mainAssetBundle != null)
                {
                    mainAssetBundle.Unload(true);
                    mainAssetBundle = null;
                }
                float endTime = Time.realtimeSinceStartup;
                Debug.Log("启动时间LoadManifest UnloadAsset：" + (endTime - startTime) * 1.0f);


                DirectoryInfo di = new DirectoryInfo(BundleConfig.Instance.BundlesPathForPersist);
                if (!di.Exists)
                {
                    di.Create();
                }

                string manifestFileUrl = string.Format("{0}{1}.ab",
                    BundleConfig.Instance.BundlesPathForPersist,
                    BundleConfig.Instance.BundlePlatformStr);




                if (File.Exists(manifestFileUrl))
                {
                    byte[] bytes = File.ReadAllBytes(manifestFileUrl);
                    mainAssetBundle = AssetBundle.LoadFromMemory(bytes);

                    Manifest = mainAssetBundle.LoadAsset("AssetBundleManifest") as AssetBundleManifest;
                }
                else
                {
                    manifestFileUrl = "AssetBundles/"
                                      + BundleConfig.Instance.BundlePlatformStr
                                      + "/" + BundleConfig.Instance.BundlePlatformStr + ".ab";

                    if (BundleConfig.IsConfuseModel)
                    {
                        manifestFileUrl = "AssetBundles/"
                            + BundleConfig.Instance.BundlePlatformStr
                            + "/" + BundleConfig.ConfusedFolder + "/" + BundleConfig.Instance.BundlePlatformStr + ".ab";
                    }


                    Stream stream = StreamingAssetLoad.GetFile(manifestFileUrl);
                    if (stream == null)
                    {
#if UNITY_EDITOR || UNITY_IOS

                        manifestFileUrl = BundleConfig.Instance.StreamingAssetPath + "/AssetBundles/"
                                          + BundleConfig.Instance.BundlePlatformStr
                                          + "/" + BundleConfig.Instance.BundlePlatformStr + ".ab" + "." + BundleConfig.EncrypFilePostfix;

                        if (BundleConfig.IsConfuseModel)
                        {

                            manifestFileUrl = BundleConfig.Instance.StreamingAssetPath + "/AssetBundles/"
                                + BundleConfig.Instance.BundlePlatformStr
                                + "/" + BundleConfig.ConfusedFolder + "/" + BundleConfig.Instance.BundlePlatformStr + ".ab" + "." + BundleConfig.EncrypFilePostfix;
                        }

                        if (File.Exists(manifestFileUrl))
                        {
                            mainAssetBundle = LoadEncryptAssetBundle(manifestFileUrl);
                            Manifest = mainAssetBundle.LoadAsset("AssetBundleManifest") as AssetBundleManifest;
                            return;
                        }
#endif
                        Debug.LogWarning("Manifest 文件不存在:" + manifestFileUrl);
                        return;
                    }
                    else
                    {

                        byte[] bytes = new byte[stream.Length];
                        stream.Read(bytes, 0, bytes.Length);
                        stream.Seek(0, SeekOrigin.Begin);

                        try
                        {
                            mainAssetBundle = AssetBundle.LoadFromMemory(bytes);

                            Manifest = mainAssetBundle.LoadAsset("AssetBundleManifest") as AssetBundleManifest;
                        }
                        catch (System.Exception ex)
                        {
                            if (!BuildManifestUtility.GetBuildManifest().IsReleaseVer) LuaInterface.Debugger.LogWarning("--sync-->" + ex.ToString() + "<--aburl-->" + manifestFileUrl);
                        }
                    }
                    stream.Close();
                }
                startTime = Time.realtimeSinceStartup;
                Debug.Log("启动时间LoadManifest realLoadMainFest：" + (startTime - endTime) * 1.0f);
                if (mainAssetBundle == null || Manifest == null)
                {
                    LogicError("ResourceManager Init failed!! load Manifest failed!!");
                    return;
                }

                if (Application.isPlaying == false)
                {
                    mainAssetBundle.Unload(false);
                    mainAssetBundle = null;
                    return;
                }
                endTime = Time.realtimeSinceStartup;
                Debug.Log("启动时间LoadManifest isStartGame Unload：" + (endTime - startTime) * 1.0f);

                /**
                 * GCTODO: 
                 *      依赖数据： 
                 *              占用内存： 6M
                 *              时间： 华为note 10: 1.4秒
                 */

                float timeBefore = Time.realtimeSinceStartup;

                Dictionary<string, uint[]> depInfo = new Dictionary<string, uint[]>();
                string[] tempAllAssetBundles = Manifest.GetAllAssetBundles();
                m_allBundleNames = new string[tempAllAssetBundles.Length];
                for (int i = 0; i < tempAllAssetBundles.Length; i++)
                {
                    m_allBundleNames[i] = string.Intern(tempAllAssetBundles[i]);
                }

                bundleName2Index.Clear();
                for (uint i = 0; i < m_allBundleNames.Length; i++)
                {
                    bundleName2Index[m_allBundleNames[i]] = i;
                }
                startTime = Time.realtimeSinceStartup;
                Debug.Log("启动时间LoadManifest GetAllAssetBundles：" + (startTime - endTime) * 1.0f);

                //List<uint> indexBuffer = new List<uint>();
                //foreach (string bundleName in m_allBundleNames)
                //{
                //    indexBuffer.Clear();
                //    string[] deps = Manifest.GetDirectDependencies(bundleName);
                //    if (deps != null)
                //    {
                //        foreach (string dep in deps)
                //            indexBuffer.Add(bundleName2Index[dep]);
                //        if (deps.Length > 0)
                //            depInfo.Add(bundleName, indexBuffer.ToArray());
                //    }

                //}
                endTime = Time.realtimeSinceStartup;
                Debug.Log("启动时间LoadManifest depInfo.Add：" + (endTime - startTime) * 1.0f);
                m_assetsDependInfo = depInfo;

                mainAssetBundle.Unload(false);
                mainAssetBundle = null;
                startTime = Time.realtimeSinceStartup;
                Debug.Log("启动时间LoadManifest assetBundle.Unload：" + (startTime - endTime) * 1.0f);
            }
            Dictionary<string, uint> bundleName2Index = new Dictionary<AssetBundleName, uint>();
            List<uint> indexBufferTemp = new List<uint>();
            private void TrySetBundleDepInfo(string bundleName)
            {
                UnityEngine.Profiling.Profiler.BeginSample("Manifest.GetDirectDependencies");
                if (!m_assetsDependInfo.ContainsKey(bundleName))
                {
                    indexBufferTemp.Clear();
                    string[] deps = Manifest.GetDirectDependencies(bundleName);
                    if (deps != null && deps.Length > 0)
                    {
                        foreach (string dep in deps)
                            indexBufferTemp.Add(bundleName2Index[dep]);

                        m_assetsDependInfo.Add(bundleName, indexBufferTemp.ToArray());
                    }
                    else
                    {
                        m_assetsDependInfo.Add(bundleName, null);
                    }
                }
                UnityEngine.Profiling.Profiler.EndSample();
            }

            public void SetAsyncLoadCountLimit()
            {
                // mConfigMaxLoadingCount = QualitySettingMgr.GetCurQualitySetInfo().AsyncCount;
            }

            public void SetAsyncLoadCountLimit(uint count)
            {
                mConfigMaxLoadingCount = count;
            }

            private AssetBundle LoadEncryptAssetBundle(string fileUrl)
            {
                byte[] temp = File.ReadAllBytes(fileUrl);
                // UtilTool.XorEncrypt(ref temp, BundleConfig.BundleEncryptKey);
                AssetBundle assetBundle = AssetBundle.LoadFromMemory(temp);
                return assetBundle;
            }

            private IEnumerator EndOfFrame(Action func)
            {
                yield return new WaitForEndOfFrame();
                func();
            }

            public class WaitFrameItem
            {
                public Action action;
                public int count = 0;

                public WaitFrameItem(Action action, int count)
                {
                    this.action = action;
                    this.count = count;
                }

                public void ReInit(Action action, int count)
                {
                    this.action = action;
                    this.count = count;
                }

                public void Reset()
                {
                    this.action = null;
                    this.count = 0;
                }
            }

            public void AddNextFrameCallBack(Action action, int count)
            {
                WaitFrameItem item = null;
                if (m_unuseWaitFrameQueue.Count > 0)
                {
                    item = m_unuseWaitFrameQueue.Dequeue();
                    item.ReInit(action, count);
                }
                else
                {
                    item = new WaitFrameItem(action, count);
                }
                m_currWaitNextFrameCallBack.Add(item);
            }

            class ListPool<T>
            {
                public ListPool(uint capacity)
                {
                    m_capacity = capacity;
                    m_cache = new Stack<List<T>>((int)capacity);
                }

                public List<T> Get()
                {
                    if (m_cache.Count > 0)
                        return m_cache.Pop();

                    return new List<T>();
                }

                public void Return(List<T> item)
                {
                    if (m_cache.Count < m_capacity)
                    {
                        item.Clear();
                        m_cache.Push(item);
                    }
                }

                private uint m_capacity;
                Stack<List<T>> m_cache;
            }

            /// <summary>
            /// 兼容旧代码，不应该再用了。WWW已经废弃了。
            /// </summary>
            /// <param name="path"></param>
            /// <returns></returns>
            public Texture2D SyncLoadTexture2dInStreamingAsset(string path)
            {

                string localPath = "";
                if (Application.platform == RuntimePlatform.Android)
                {
                    localPath = Application.streamingAssetsPath + path;
                }
                else
                {
                    localPath = "file:///" + Application.streamingAssetsPath + path;
                }

                WWW t_WWW = new WWW(localPath);

                if (t_WWW.error != null)
                {
                    Debug.LogError("error : " + localPath);
                    return null;          //读取文件出错
                }

                while (!t_WWW.isDone)
                {

                }

                return t_WWW.texture;
            }

            public void Update()
            {
                UpdateCompleteCallbackList();
            }

            /// <summary>
            /// 限制每帧异步加载回调数量
            /// </summary>
            private void UpdateCompleteCallbackList()
            {
                if (WaitForCallBackList.Count > 0)
                {
                    float startTime = Time.realtimeSinceStartup;
                    LinkedListNode<AsyncCallBackParams> next = null;
                    for (LinkedListNode<AsyncCallBackParams> curr = WaitForCallBackList.First; curr != null;)
                    {
                        next = curr.Next;
                        bool hadDelete = false;
                        if (curr.Value != null)
                        {
                            AsyncCallBackParams currItem = curr.Value;

                            if (currItem.callBack == null)
                            {
                                Debug.LogWarning("army: currItem.callBack == null");
                                hadDelete = true;
                                currItem.Reset();
                                m_unuseCallBackPool.RecoverObject(currItem);
                                WaitForCallBackList.RemoveFirst();
                                curr = next;
                                continue;
                            }

                            UnityEngine.Object asset = currItem.asset;

                            Delegate[] delegates = currItem.callBack.GetInvocationList();

                            if (delegates != null && delegates.Length > 0)
                            {
                                int count = delegates.Length;
                                for (int i = 0; i < count; i++)
                                {
                                    Action<UnityEngine.Object> callback = (Action<UnityEngine.Object>)delegates[i];
                                    if (callback != null && (currItem.resId == 0 || m_clientRef.ContainsKey(currItem.resId)))
                                    {
                                        currItem.callBack -= (Action<UnityEngine.Object>)delegates[i];
                                        if (currItem.callBack == null)
                                        {
                                            hadDelete = true;
                                            currItem.Reset();
                                            m_unuseCallBackPool.RecoverObject(currItem);
                                            WaitForCallBackList.RemoveFirst();
                                        }
                                        callback(asset);

                                        currCostTime = Time.realtimeSinceStartup - startTime;


                                        if (isLimitAsyncComplete && currCostTime > OneCostTime)
                                        {
                                            return;
                                        }
                                    }
                                }

                            }
                            else
                            {
                                currItem.Reset();
                                m_unuseCallBackPool.RecoverObject(currItem);
                            }


                        }
                        if (hadDelete == false)
                        {
                            WaitForCallBackList.RemoveFirst();
                        }

                        curr = next;

                    }

                }

                if (m_currWaitNextFrameCallBack.Count > 0)
                {
                    for (int i = 0; i < m_currWaitNextFrameCallBack.Count; i++)
                    {
                        WaitFrameItem item = m_currWaitNextFrameCallBack[i];
                        if (item.count > 0)
                        {
                            if (item.action != null)
                            {
                                item.action();
                            }

                            m_currWaitNextFrameCallBack.RemoveAt(i);
                            i--;
                            item.Reset();
                            m_unuseWaitFrameQueue.Enqueue(item);
                        }
                        else
                        {
                            m_currWaitNextFrameCallBack[i].count++;
                        }

                    }
                }
            }

            private bool isInit = false;
            private int packageIndex = 0;
            private readonly int packageCount = 15;
            private HashSet<string> m_ABLoadHashSet;
            private Dictionary<int, HashSet<string>> m_ABLoadSubDic = null;
            private int level = 0;
            private HashSet<string> m_ABLoadHashSet1 = new HashSet<string>();
            private void ABLoadtRefRecord(string assetName)
            {
                if (!isInit)
                {
                    ABLoadtRefRecordInit();
                }

                if (!m_ABLoadHashSet.Contains(assetName))
                {
                    // if (GameKernel.GetDataCenter() != null)
                    // {
                    //     level = GameKernel.GetDataCenter().GetPlayerData().GetIntValue((uint)cs.ActorAttrEnum.U_ACTOR_BASE_GRADE_ATTR);
                    // }
                    // else
                    {
                        level = 0;
                    }
                    m_ABLoadHashSet.Add(assetName);
                    m_ABLoadSubDic[packageIndex].Add(assetName);
                    OutPutRefRecordRealTime(assetName);
                }
            }

            private void ABLoadRecord(string assetName)
            {
                if (!m_ABLoadHashSet1.Contains(assetName))
                {
                    // if (GameKernel.GetDataCenter() != null)
                    // {
                    //     level = GameKernel.GetDataCenter().GetPlayerData().GetIntValue((uint)cs.ActorAttrEnum.U_ACTOR_BASE_GRADE_ATTR);
                    // }
                    // else
                    {
                        level = 0;
                    }
                    m_ABLoadHashSet1.Add(assetName);
                    OutPutRecordRealTime(assetName);
                }
            }

            private void ABLoadtRefRecordInit()
            {
                isInit = true;
                m_ABLoadHashSet = new HashSet<string>();
                m_ABLoadSubDic = new Dictionary<int, HashSet<string>>();
                for (int i = 0; i <= packageCount; i++)
                {
                    m_ABLoadSubDic.Add(i, new HashSet<string>());
                }
            }

            public void SetPackageIndex(int index)
            {
                if (index > packageCount || index < 0)
                {
                    return;
                }

                packageIndex = index;
            }

            public void GetPlayerLevel()
            {
                // Debug.Log(GameKernel.GetDataCenter().GetPlayerData().GetIntValue((uint)cs.ActorAttrEnum.U_ACTOR_BASE_GRADE_ATTR));
            }

            public void OutPutRecord()
            {
                using (StreamWriter sw = new StreamWriter(m_ABLoadRefRecordPath, true))
                {
                    for (int i = 0; i <= packageCount; i++)
                    {
                        sw.WriteLine(i);
                        foreach (string assetName in m_ABLoadSubDic[i])
                        {
                            sw.WriteLine(assetName);
                        }
                    }
                }
            }

            private void OutPutRefRecordRealTime(string assetName)
            {
                bool isReleaseVer = BuildManifestUtility.GetBuildManifest().IsReleaseVer;
                using (StreamWriter sw = new StreamWriter(m_ABLoadRefRecordPath, true))
                {
                    sw.WriteLine(assetName + "|" + level + "|" + isReleaseVer);
                }
            }

            private void OutPutRecordRealTime(string assetName)
            {
                bool isReleaseVer = BuildManifestUtility.GetBuildManifest().IsReleaseVer;
                using (StreamWriter sw = new StreamWriter(m_ABLoadRecordPath, true))
                {

                    sw.WriteLine(assetName + "|" + level + "|" + isReleaseVer);
                }
            }

            /// <summary>
            /// 保存一次引用数据的快照
            /// </summary>
            public void BeginAssetRefSample(string contex = "")
            {
                if (!isSampling)
                {
                    isSampling = true;
                }
                else
                {
                    Debug.LogError("In sampling, begin and end should be paired up!");
                    return;
                }

                sampleContext = contex;

                if (m_snapshotExplicitRefsBegin == null)
                {
                    m_snapshotExplicitRefsBegin = new Dictionary<string, int>();
                }
                m_snapshotExplicitRefsBegin.Clear();
                foreach (KeyValuePair<string, ExplicitRef> kvp in m_explicitRefs)
                {
                    m_snapshotExplicitRefsBegin.Add(kvp.Key, kvp.Value.RefCount);
                }
            }

            public void EndAssetRefSample()
            {

                UpdatableRunner.Instance.StartCoroutine(DoEndAssetRefSample());
            }

            private void AnalyzeAssetLeak()
            {
                if (m_singleSampleAssetLeakResult == null)
                {
                    m_singleSampleAssetLeakResult = new Dictionary<AssetBundleName, int>();
                }
                m_singleSampleAssetLeakResult.Clear();

                foreach (KeyValuePair<string, int> kvp in m_snapshotExplicitRefsEnd)
                {
                    int refCount;
                    if (!m_snapshotExplicitRefsBegin.TryGetValue(kvp.Key, out refCount))
                    {
                        m_singleSampleAssetLeakResult.Add(kvp.Key, kvp.Value);
                    }
                    else
                    {
                        if (refCount < kvp.Value)
                        {
                            m_singleSampleAssetLeakResult.Add(kvp.Key, (kvp.Value - refCount));
                        }
                    }
                }

                int resultFlag = 0;
                if (m_singleSampleAssetLeakResult.Count > 0)
                {
                    resultFlag = 1;
                }
                
                SaveLeakInfo();
            }

            private void SaveLeakInfo()
            {
                CheckLeakLogFile();
                using (StreamWriter sw = new StreamWriter(leakLogFile))
                {

                    if (m_singleSampleAssetLeakResult.Count > 0)
                    {
                        sw.WriteLine("---------------存在资源引用计数增加，有泄露的可能性！----------------------");
                        foreach (KeyValuePair<string, int> kvp in m_singleSampleAssetLeakResult)
                        {
                            ExplicitRef expRef = null;

                            int currentRefCount = 0;
                            if (m_explicitRefs.TryGetValue(kvp.Key, out expRef))
                            {
                                currentRefCount = expRef.RefCount;
                            }

                            string leakInfo = kvp.Key + ",  引用计数新增: " + kvp.Value + ", 当前引用计数总数: " + currentRefCount;
                            sw.WriteLine("[" + DateTime.Now + "]" + "[" + sampleContext + "]" + leakInfo);
                        }
                    }
                    else
                    {
                        sw.WriteLine("---------------不存在引用计数变化！----------------------");
                    }
                }
            }

            private void CheckLeakLogFile()
            {
                if (leakLogFile == null)
                {
                    leakLogFile = File.Open(leakLogFilePath, FileMode.OpenOrCreate);
                }
                else
                {
                    leakLogFile = File.Open(leakLogFilePath, FileMode.Append);
                }
            }

            private IEnumerator DoEndAssetRefSample()
            {
                yield return new UnityEngine.WaitForSeconds(0.2f);
                if (isSampling)
                {
                    isSampling = false;
                }
                else
                {
                    Debug.LogError("Not in sampling, begin and end should be paired up!");
                    yield return null;
                }

                if (m_snapshotExplicitRefsEnd == null)
                {
                    m_snapshotExplicitRefsEnd = new Dictionary<string, int>();
                }
                m_snapshotExplicitRefsEnd.Clear();

                foreach (KeyValuePair<string, ExplicitRef> kvp in m_explicitRefs)
                {
                    m_snapshotExplicitRefsEnd.Add(kvp.Key, kvp.Value.RefCount);
                }

                AnalyzeAssetLeak();
            }

            #region 重写Inspector面板专用
            /// <summary>
            /// 此方法很耗，只是用来界面查找泄露使用
            /// </summary>
            /// <param name="abName"></param>
            /// <returns></returns>
            public bool AssetBundleHadBeDepend(string abName)
            {
                int index = -1;

                for (int i = 0; i < m_allBundleNames.Length; i++)
                {
                    if (m_allBundleNames[i] == abName)
                    {
                        index = i;
                        break;
                    }
                }
                foreach (var item in m_assetsDependInfo)
                {
                    foreach (var i in item.Value)
                    {
                        if (i == index)
                        {
                            return true;
                        }
                    }
                }
                return false;
            }

            public Dictionary<string, ExplicitRef> GetExpDic()
            {
                return m_explicitRefs;
            }

            public Dictionary<string, ImplicitRef> GetImpDic()
            {
                return m_implicitRefs;
            }

            public bool CheckAbHadLoad(string abName, ref int count)
            {
                bool hadLoad = false;
                count = 0;
                foreach (var name in m_clientRef.Values)
                {
                    if (name == abName)
                    {
                        hadLoad = true;
                        count++;
                    }
                }
                return hadLoad;
            }

            public string[] GetAllABs()
            {
                return m_allBundleNames;
            }

            #endregion

        }

        public class AsyncCallBackParams
        {
            public uint resId = 0;
            public Action<UnityEngine.Object> callBack;
            public UnityEngine.Object asset;

            public AsyncCallBackParams()
            {

            }

            public void Init(uint id, Action<UnityEngine.Object> callback, UnityEngine.Object asset)
            {
                this.resId = id;
                this.callBack = callback;
                this.asset = asset;
            }

            public void Reset()
            {
                resId = 0;
                this.callBack = null;
                this.asset = null;
            }
        }
    }
}