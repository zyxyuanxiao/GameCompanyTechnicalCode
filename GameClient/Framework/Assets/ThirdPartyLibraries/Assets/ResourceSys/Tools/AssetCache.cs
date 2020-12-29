using Best_Pool;
using LuaInterface;
using System;
using System.Collections.Generic;
using Best.ResourceSys;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace Best
{
    public interface IAssetCache<TAsset>
    {
        void Get(string path);
        bool Return(string path, TAsset asset);
        void Release();
        void DebugLog();
    }
    public interface ICacheForceEliminated
    {
        void OnForceEliminated(string path);
        Dictionary<string,float> GetClientNoRefBucket();
    }

    /// <summary>
    /// 具有淘汰策略的AssetCache, 封装了相应的资源管理逻辑。
    /// 
    /// AssetCache中的资源生命周期分为两个阶段：
    /// 阶段一：
    ///     Get -> Return
    ///     由逻辑端控制。当逻辑端返回对资源的全部引用（Get-Return调用数相匹配）时，资源的生命周期进入阶段二。
    /// 阶段二:
    ///     Return -> Eliminated
    ///     由淘汰策略控制。被淘汰策略所淘汰的资源下一步生命周期根据引用数分为两种：
    ///         1.引用数为0：资源被销毁
    ///         2.引用数大于0：重新进入阶段一
    ///     见的淘汰策略如LRU, LRU-TTL, LRUK等。
    /// </summary>
    /// <typeparam name="TAsset">资源类型</typeparam>
    public class AssetCache<TAsset>: ICacheForceEliminated where TAsset : UnityObject 
    {
        public class Bucket
        {
            public bool AsyncLoading;
            public string Path;
            public int RefCount;
            public uint ResUID;
            public TAsset Asset;
            public List<Action<TAsset>> OnLoadeds;
            public InstancePool<TAsset> Pool;
            public AssetCache<TAsset> Cache;
            [NoToLua]public float StartNoRefTime;
            public TAsset GetAsset()
            {
                if (Asset == null)
                {
                    Debug.LogErrorFormat("Asset is null. path is {0}.", Path);
                    return null;
                }
                TAsset asset = null;
                RefCount++;
                StartNoRefTime = -1;
                if (Cache.m_initantiable)
                {
                    if (Pool == null)
                    {
                        Pool = Cache.m_pool.AddPool(Path, Asset);
                    }
                    asset = Pool.Spawn();
                }
                else
                {
                    asset = Asset;
                }
                return asset;
            }

            public void RecoverObj(TAsset asset)
            {
                RefCount--;
                if(RefCount == 0)
                    StartNoRefTime = Time.realtimeSinceStartup;
                if (Cache.m_initantiable)
                    Pool.Despawn(asset);
            }

            public void Invoke()
            {
                if (OnLoadeds != null && OnLoadeds.Count > 0)
                {
                    List<TAsset> list = new List<TAsset>();
                    int len = -1;
                    while (++len != OnLoadeds.Count)
                    {
                        list.Add(GetAsset());
                    }
                    len = -1;
                    while (++len != OnLoadeds.Count)
                    {
                        OnLoadeds[len](list[len]);
                    }
                    list.Clear();
                    OnLoadeds.Clear();
                }
            }

            public void Destroy()
            {
                Invoke();
                ResourceManager.Instance().Unload(ref ResUID);
                ResUID = 0;
                Asset = null;
                RefCount = 0;
                if (Cache.m_initantiable)
                {
                    Cache.m_pool.DestroyPool(Path);
                }
                Pool = null;
                Cache = null;
                Path = null;
                StartNoRefTime = -1;
            }
        }

        ObjectPool<Bucket> m_bucketPool;
        ObjectPool<Bucket> BucketPool
        {
            get
            {
                // if (m_bucketPool == null)
                //     m_bucketPool = GameKernel.GetPoolManager().GetObjectPool<Bucket>(Tag + "ObjectBucket");
                return m_bucketPool;
            }
        }

        public readonly string Tag;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cacheStrategy">缓存淘汰策略</param>
        /// <param name="tag">调试用tag</param>
        /// <param name="instantiable">所缓存资源是否Instantiable(例如prefab, material等 可instantiable， Texture2D不可instantiable)</param>
        /// <param name="maxInstantiateCount">对于Instantiable的资源，最大缓存的instantiate数量</param>
        public AssetCache(string tag, IEliminateCache<string, Bucket> cacheStrategy, bool instantiable = false, GatherInstancePool<TAsset> pool = null, uint maxInstantiateCount = 0)
        {
            Tag = tag;
            m_initantiable = instantiable;
            m_maxInitantiateCount = maxInstantiateCount;
            m_cacheStrategy = cacheStrategy;
            m_cacheStrategy.RegiestEvent(OnEliminated);
            m_clientRefBucket = new Dictionary<string, Bucket>();
            m_clientNoRefBucket = new Dictionary<string, float>();
             m_pool = pool;
        }

        public void ResetcacheStrategy(IEliminateCache<string, Bucket> cacheStrategy)
        {
            m_cacheStrategy.Release();
            m_cacheStrategy = cacheStrategy;
            m_cacheStrategy.OnEliminate += OnEliminated;
        }


        /// <summary>
        /// 同步获得资源
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public TAsset GetSync(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                Debug.LogErrorFormat("AssetCache Tag : {0}, 获取资源路径为空 path IsNullOrEmpty ", Tag);
                return null;
            }

            Bucket bucket = GetBucket(path, true);
            if (bucket != null)
            {

                if (bucket.Asset != null)
                {
                    TAsset asset2 = bucket.GetAsset();
                    //Debug.Log("获取  引用计数" + bucket.RefCount + "   path:" + bucket.Path);
                    return asset2;
                }
                else
                {
                    if (!GlobalAssetCacheMgr.Instance.OpenAssetCacheRevertMode)
                    {
                        Debug.LogErrorFormat("Asset rtName == null; 资源路径:{0} 当前引用次数:{1}", bucket.Path, bucket.RefCount);
                        return null;
                    }
                    string rtName = BundleConfig.EditorAssetURI2RuntimeAssetName(bucket.Path, typeof(TAsset));
                    if (string.IsNullOrEmpty(rtName))
                    {
                        Debug.LogErrorFormat("Asset rtName == null; 资源路径:{0} 当前引用次数:{1}", bucket.Path, bucket.RefCount);
                        return null;
                    }

                    Dictionary<string, ResourceSys.ResourceManager.ExplicitRef> expDick = ResourceManager.Instance().GetExpDic();
                    ResourceSys.ResourceManager.ExplicitRef explicitRef = null;
                    if (expDick.TryGetValue(rtName, out explicitRef))
                    {
                        if (explicitRef != null && explicitRef.Asset != null)
                        {
                            bucket.Asset = explicitRef.Asset as TAsset;
                            return bucket.GetAsset();
                        }
                        else
                        {
                            //两边都进行清理
                            bool bRest = ResourceManager.Instance().RevertInvaildExplicitaAeest(rtName, ref bucket.ResUID);

                            bucket.RefCount = 0;
                            m_cacheStrategy.Remove(bucket.Path);
                            if (!bRest)
                            {
                                OnEliminated(bucket.Path, bucket);
                            }
                        }
                    }
                    else
                    {
                        //单独清理Bucket
                        bucket.RefCount = 0;
                        m_cacheStrategy.Remove(bucket.Path);
                        OnEliminated(bucket.Path, bucket);
                    }

                    // Debug.LogErrorFormat("Asset has been destroyed. path is {0}  refCount:{1}  lua traceback:{2}", bucket.Path, bucket.RefCount, LuaClient.Instance.LuaTraceback());
                }
            }


            TAsset asset;
            uint resUID = ResourceManager.Instance().LoadSync(path, out asset);
            if (asset == null)
            {
                if (!BuildManifestUtility.GetBuildManifest().IsReleaseVer)
                {
                    Debug.LogError(string.Format("AssetCache Tag: {0} 好像是资源不存在！要不您检查下？path = {1}", Tag, path));
                }
                Debug.LogErrorFormat("AssetCache Tag: {0} 好像是资源不存在！要不您检查下？path = {1}", Tag, path);
                return null;
            }

            bucket = CreateNewBucket(resUID, asset, path);
            return bucket.GetAsset();
        }

        /// <summary>
        /// 缓存中有目标资源时，回调会同步调用
        /// 缓存中没有目标资源时，异步获得资源
        /// </summary>
        /// <param name="path"></param>
        /// <param name="callback"></param>
        public void GetAsync(string path, Action<TAsset> callback)
        {
            if (string.IsNullOrEmpty(path))
            {
                Debug.LogErrorFormat("AssetCache Tag : {0}, 获取路径为空的资源？这里没有! lua traceback:{1}", Tag);
                if (callback != null)
                    callback(null);
                return;
            }
            if (callback == null)
            {
                Debug.LogErrorFormat("您看是不是可以传个回调进来!");
                //Debug.LogErrorFormat("The call back is null!");
                return;
            }

            Bucket bucket = GetBucket(path, true);
            if (bucket != null)
            {
                if (bucket.AsyncLoading)
                {
                    bucket.OnLoadeds.Add(callback);
                }
                else
                {
                    TAsset asset = bucket.GetAsset();
                    if (asset == null)
                    {
                        Debug.LogErrorFormat("GetAssetFromBucket got null. path is {0}.", path);
                    }
                    callback(asset);
                }
                return;
            }

            uint resUID = ResourceManager.Instance().LoadAsync<TAsset>(path, (uid, newAsset) =>
            {
                if (newAsset == null)
                {
                    if (!BuildManifestUtility.GetBuildManifest().IsReleaseVer)
                    {
                        Debug.LogError(string.Format("AssetCache Tag: {0} 好像是资源不存在！要不您检查下？path = {1}", Tag, path));
                    }
                    Debug.LogErrorFormat("AssetCache Tag: {0} 好像是资源不存在！要不您检查下？path = {1}", Tag, path);
                }

                Bucket emptyBucket = GetBucket(path, true);
                if (emptyBucket == null) //bucket可能已经被释放
                {
                    return;
                }

                emptyBucket.AsyncLoading = false;

                if (emptyBucket.Asset != null)
                    Debug.LogErrorFormat("AssetCache Tag: {0} .GetAsync path : {1} logic error", Tag, path);

                //newAsset = emptyBucket.RefCount > 0 ? newAsset : null; //对于引用数的bucket，回调传null
                emptyBucket.Asset = newAsset;
                emptyBucket.Invoke();
            });

            CreateNewBucket(resUID, null, path, callback);
        }

        /// <summary>
        /// 为了绕开ToLua的限制，方便lua层写代码。
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public bool Return(string path)
        {
            return Return(path, null);
        }

        /// <summary>
        /// 返还所引用的资源
        /// </summary>
        /// <param name="path"></param>
        /// <param name="asset"></param>
        /// <returns></returns>
        public bool Return(string path, TAsset asset = null)
        {
            if (string.IsNullOrEmpty(path))
            {
                Debug.LogErrorFormat("AssetCache Tag : {0}, 回收传入的资源名为空，请检查您的代码！谢谢！", Tag);
                //Debug.LogErrorFormat("AssetCache Tag : {0}, Return with invalid path! lua tracback:{1}", Tag, LuaClient.Instance.LuaTraceback());
                return false;
            }
            if (m_initantiable && asset == null)
            {
                Debug.LogErrorFormat("AssetCache Tag : {0}, 传入的资源实例对象为空，请检查！ path = {1}!", Tag, path);
                //Debug.LogErrorFormat("AssetCache Tag : {0}, Must be returned with {1} instance! lua traceback:{2}", Tag, path, LuaClient.Instance.LuaTraceback());
                return false;
            }

            Bucket bucket;
            if (!m_clientRefBucket.TryGetValue(path, out bucket))
            {
                Debug.LogErrorFormat("AssetCache Tag: {0}. 不要缓存不是通过缓存获取的资源 ！{1}.", Tag, path);
                //Debug.LogErrorFormat("AssetCache Tag: {0}. No ref info exists for path {1}. May caused by unpaired Get-Return calls. lua traceback:{2}", Tag, path, LuaClient.Instance.LuaTraceback());
                return false;
            }

            bucket.RecoverObj(asset);
            //Debug.Log("回收后  引用计数"+ bucket.RefCount + "   path:" + bucket.Path);
            if (bucket.RefCount == 0)
            {
                MoveToPhase2(bucket);
            }
            else if (bucket.RefCount < 0)
            {
                Debug.LogErrorFormat("AssetCache Tag: {0}. Wrong ref count for path {1}.", Tag, path);
            }

            return true;
        }

        public void Release()
        {
            m_cacheStrategy.Release();
            m_cacheStrategy = null;
            foreach (var kv in m_clientRefBucket)
            {
                DestroyBucket(kv.Value);
            }
            m_clientRefBucket.Clear();
            m_clientRefBucket = null;

            m_clientNoRefBucket.Clear();
            m_clientNoRefBucket = null;

            if (m_pool != null) m_pool.ClearAll();
            m_bucketPool.ClearAll();
        }

        #region private
        private void MoveToPhase2(Bucket bucket)
        {
            m_clientRefBucket.Remove(bucket.Path);
            m_cacheStrategy.Add(bucket.Path, bucket);

            if (!m_clientNoRefBucket.ContainsKey(bucket.Path))
            {
                m_clientNoRefBucket.Add(bucket.Path, bucket.StartNoRefTime);
            } 
        }

        private void BackToPhase1(Bucket bucket)
        {
            if (m_clientRefBucket.ContainsKey(bucket.Path))
            {
                Debug.LogErrorFormat("AssetCache Tag : {0} Bucket : {1} back to phase 1, find conflict res ", Tag, bucket.Path);
                return;
            }

            m_clientRefBucket.Add(bucket.Path, bucket);
            if (m_clientNoRefBucket.ContainsKey(bucket.Path))
            {
                m_clientNoRefBucket.Remove(bucket.Path);
            }
        }

        private Bucket CreateNewBucket(uint resUID, TAsset asset, string path, Action<TAsset> callback = null)
        {
            Bucket newBucket = BucketPool.Spawn();
            newBucket.Path = path;
            newBucket.Asset = asset;
            newBucket.ResUID = resUID;
            newBucket.RefCount = 0;
            newBucket.AsyncLoading = asset == null;
            newBucket.Cache = this;
            if (callback != null)
            {
                if (newBucket.OnLoadeds == null)
                    newBucket.OnLoadeds = new List<Action<TAsset>>();
                newBucket.OnLoadeds.Add(callback);
            }
            if (m_clientRefBucket.ContainsKey(path))
            {
                m_clientRefBucket[path] = newBucket;
            }
            else
            {
                m_clientRefBucket.Add(path, newBucket);
            }
            return newBucket;
        }

        private TAsset GetCachedAsset(string path)
        {
            Bucket bucket = GetBucket(path, true);

            if (bucket != null)
                return bucket.GetAsset();

            return null;
        }

        private Bucket GetBucket(string path, bool backToClientRef = false)
        {
            Bucket bucket;

            if (m_clientRefBucket.TryGetValue(path, out bucket))
                return bucket;

            if (m_cacheStrategy.TryGet(path, out bucket))
            {
                if (backToClientRef)
                {
                    BackToPhase1(bucket);
                }

                return bucket;
            }

            return null;
        }
        [NoToLua]
        public void OnEliminate(string path, Bucket bucket)
        {
            OnEliminated(path, bucket);
        }

        private void OnEliminated(string path, Bucket bucket)
        {
            if (bucket.RefCount == 0)
            {
                DestroyBucket(bucket);
            }
            else if (bucket.RefCount > 0) //虽然被淘汰，但是仍然有引用, 在之前get时就已经回到clientref中，因此无需操作
            {
                //do nothing.
                if (!m_clientRefBucket.ContainsKey(bucket.Path))
                    Debug.LogErrorFormat("Logic Error : Please Check it! path = {0}", path);
            }
            else
            {
                Debug.LogErrorFormat("AssetCache Tag : {0}, Bucket : {1} be eliminated with wrong ref count : {2}", Tag, bucket.Path, bucket.RefCount);
            }
        }

        private void DestroyBucket(Bucket bucket)
        {
            if (string.IsNullOrEmpty(bucket.Path))
            {
                Debug.LogErrorFormat("Logic Error : The path cannot be null! Check your code!");
            }
            if (m_clientNoRefBucket.ContainsKey(bucket.Path))
            {
                m_clientNoRefBucket.Remove(bucket.Path);
            }
            bucket.Destroy();
            BucketPool.Despawn(bucket);         
        }

        public void DebugLog(bool debugpool)
        {
            if (EnableDebug)
            {
                m_cacheStrategy.DebugLog(string.Format("{0} AssetCache : Current assign count = {1}. \r\n ", Tag, m_clientRefBucket.Count));
                if (debugpool)
                {
                    if (m_pool != null) m_pool.DebugLog();
                }
            }
        }

        bool m_enableDebug = false;
        public bool EnableDebug
        {
            get { return m_enableDebug; }
            set
            {
                m_enableDebug = value;
                m_cacheStrategy.EnableDebug = value;
            }
        }

        public Dictionary<string, Bucket> GetClientRefBucket()
        {
            return m_clientRefBucket;
        }

        [LuaInterface.NoToLua]
        public Dictionary<string,float> GetClientNoRefBucket()
        {
            return m_clientNoRefBucket;
        }

        [LuaInterface.NoToLua]
        public void OnForceEliminated(string path)
        { 
            Bucket bucket = null;
            m_cacheStrategy.TryGetNoUpdate(path, out bucket);

            if (bucket == null)
            {
                return;
            }

            m_clientNoRefBucket.Remove(path);
            if (bucket.RefCount > 0 || bucket.ResUID == 0)
            {
                return;
            }

            m_cacheStrategy.Remove(path);
            OnEliminated(path, bucket);
        }
        private bool m_initantiable;
        private uint m_maxInitantiateCount;
        private IEliminateCache<string, Bucket> m_cacheStrategy;
        private Dictionary<string, Bucket> m_clientRefBucket;
        private Dictionary<string,float> m_clientNoRefBucket;
        GatherInstancePool<TAsset> m_pool;
        #endregion

        #region 测试
        public void ReleaseUnuseAsset()
        {
            m_cacheStrategy.ReleaseUnuseAsset();
        }

        public void LogAllAsset(string head = null)
        {
            string s = string.Format("{0}AssetCache={1}所有资源如下：\n", head, Tag);
            Dictionary<string, Bucket>.Enumerator iter = m_clientRefBucket.GetEnumerator();
            s = s + "正在使用的资源";
            while (iter.MoveNext())
            {
                s = s + iter.Current.Value.Path + "\n";
            }
            m_cacheStrategy.LogAllAsset(s);
        }

        /// <summary>
        /// 测试资源引用=0立马卸载,不走短时缓存,其它勿用
        /// </summary>
        /// <returns></returns>
        public void OpenTestMode()
        {
            LRUKCache<string, Bucket> lruk = m_cacheStrategy as LRUKCache<string, Bucket>;
            if (lruk != null)
            {
                lruk.OpenTestMode();
            }
            else
            {
                Debug.LogError("开启LRUK.OpenTestMode失败 lruk = =null");
            }
        }
        #endregion
    }
}