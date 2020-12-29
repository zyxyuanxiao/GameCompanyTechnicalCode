using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Best_Pool;
using Best.ResourceSys;
using LuaInterface;
using UnityEngine;
using UnityObject = UnityEngine.Object;
/**
    * IEliminateCacheComboState 和IEliminateCache是同一个cache实例,因为是接口开放,目前需要优化战斗期间的特效卸载导致的帧率波动,所以用IEliminateCacheComboState来代替
    * IEliminateCacheComboState存取资源刷新的TTL时间相比IEliminateCache更短,根据配置CacheConfig.bytes   
    */
namespace Best
{
    /// <summary>
    /// 因为现在的资源管理器在加载已经加载的资源的时候，内存垃圾产生过多，并且每次去加载时，返回的资源id不一样，所以会有额外消耗，这里不得不自己单独维护已经加载的资源引用。
    /// </summary>
    /// <typeparam name="TObject"></typeparam>
    /// <typeparam name="TAsset"></typeparam>
    public class ObjectCache<TObject,TAsset> : ICacheForceEliminated where TObject : class, ObjectCache<TObject, TAsset>.ITOjectCache, new() where TAsset : UnityObject
    {
        /// <summary>
        /// 没想到其他更好的内部封装方法去判断TObject内部是否已有资源引用和关联资源，这里只好用接口约束的方式去实现。
        /// </summary>
        public interface ITOjectCache
        {
            /// <summary>
            /// 该接口实现TObject对象引用TAsset资源
            /// </summary>
            /// <param name="o"></param>
            void AssignAsset(TAsset o);
            /// <summary>
            /// 用来判断是否引用资源
            /// </summary>
            /// <returns></returns>
            bool HasAssign();
            /// <summary>
            /// 回收前调用
            /// </summary>
            void ReturnCache();
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

        public class Bucket
        {
            public bool AsyncLoading;
            public string Path;
            public int RefCount;
            public uint ResUID;
            public TAsset Asset;
            public List<WaitLoad> OnLoadeds;
            ObjectPool<TObject> Pool;
            public ObjectCache<TObject, TAsset> Cache;
            [NoToLua] public float StartNoRefTime;

            public struct WaitLoad
            {
                public Action<TObject> action;
                public TObject obj;

                public WaitLoad(Action<TObject> action, TObject obj)
                {
                    this.action = action;
                    this.obj = obj;
                }
            }

            public TObject GetObject()
            {
                if (Pool == null)
                {
                    Pool = Cache.m_ObjPool.AddPool(Path);
                }
                TObject obj = Pool.Spawn();
                if (obj.HasAssign()) return obj;
                if (Asset == null)
                {
                    // ResUID = GameKernel.ResourceMgr.LoadSync<TAsset>(Path, out Asset);
                    // if (Asset == null)
                    // {
                    //     if (!BuildManifestUtility.GetBuildManifest().IsReleaseVer)
                    //     {
                    //         Debug.LogErrorFormat("ObjectCache Tag: {0} 好像是资源不存在！要不您检查下？path = {1}", Cache.Tag, Path);
                    //         ConsoleMgr.Instance.ShowDialogLog(string.Format("AssetCache Tag: {0} 好像是资源不存在！要不您检查下？path = {1}", Cache.Tag, Path));
                    //     }
                    //     Debug.LogErrorFormat("ObjectCache Tag {0} : Asset is null, path = {1}!", Cache.Tag, Path);
                    // }
                    AsyncLoading = false;
                }
                TAsset asset = Cache.GetAsset(Asset);
                obj.AssignAsset(asset);
                return obj;
            }

            public TObject GetObjectAsync(Action<TObject> callback)
            {
                if (Pool == null)
                {
                    Pool = Cache.m_ObjPool.AddPool(Path);
                }
                TObject obj = Pool.Spawn();
                if (obj.HasAssign())
                {
                    callback(obj);
                    return obj;
                }
                if (AsyncLoading)
                {
                    OnLoadeds.Add(new WaitLoad(callback, obj));
                    return obj;
                }
                if (Asset != null)
                {
                    TAsset asset = Cache.GetAsset(Asset);
                    obj.AssignAsset(asset);
                    callback(obj);
                    return obj;
                }
                AsyncLoading = true;
                OnLoadeds.Add(new WaitLoad(callback, obj));
                ResUID = ResourceManager.Instance().LoadAsync<TAsset>(Path, (uid, newAsset) =>
                {
                    if (newAsset == null)
                    {
                        if (!BuildManifestUtility.GetBuildManifest().IsReleaseVer)
                        {
                            Debug.LogErrorFormat("ObjectCache Tag: {0} 好像是资源不存在！要不您检查下？path = {1}", Cache.Tag, Path);
                        }
                        Debug.LogErrorFormat("ObjectCache Tag {0} : The asset of loaded is null! path = {1}", Cache.Tag, Path);
                    }
                    Asset = newAsset;
                    AsyncLoading = false;
                    int len = -1;
                    if (Cache == null)
                    {
                        while (len++ != OnLoadeds.Count - 1)
                        { 
                            Bucket.WaitLoad waitLoad = OnLoadeds[len];                           
                            if (waitLoad.action != null)
                            {
                                waitLoad.action(null);
                            }
                        }                                        
                    }
                    else
                    {
                        while (len++ != OnLoadeds.Count - 1)
                        {
                            TAsset asset = Cache.GetAsset(Asset);
                            Bucket.WaitLoad waitLoad = OnLoadeds[len];
                            if (waitLoad.obj == null)
                            {
                                Debug.LogErrorFormat("ObjectCache Tag {0} Path{1}:  waitLoad.obj is Null! len:{2}", Cache.Tag, Path, len);
                            }
                            else
                            {
                                waitLoad.obj.AssignAsset(asset);
                            }

                            if (waitLoad.action != null)
                            {
                                waitLoad.action(waitLoad.obj);
                            }
                        }
                    }
                    OnLoadeds.Clear();
                });
                return obj;
            }

            public void RecoverObj(TObject obj)
            {       
                obj.ReturnCache();
                Pool.Despawn(obj);
                for(int i = 0; i < OnLoadeds.Count; i++)
                {
                    if (OnLoadeds[i].obj == obj)
                    {
                        OnLoadeds[i].action(OnLoadeds[i].obj);
                        OnLoadeds.RemoveAt(i);
                        break;
                    }
                }
            }

            public void Destroy()
            {             
#if USE_UWA
                UWAEngine.PushSample("luca.ObjectCache.Destroy.Unload");
#endif
#if UNITY_PROFILER
                UnityEngine.Profiling.Profiler.BeginSample("luca.ObjectCache.Destroy.Unload");
#endif
                ResourceManager.Instance().Unload(ref ResUID);
#if UNITY_PROFILER
                UnityEngine.Profiling.Profiler.EndSample();
#endif
#if USE_UWA
             UWAEngine.PopSample();
#endif
                Pool = null;
                Cache = null;
                Path = null;
                ResUID = 0;
                Asset = null;
                RefCount = 0;
                StartNoRefTime = -1;
                if (OnLoadeds != null && OnLoadeds.Count > 0)
                {
                    int len = OnLoadeds.Count;
                    while(len-- != 0)
                    {
                        OnLoadeds[len].action(OnLoadeds[len].obj);
                    }
                    OnLoadeds.Clear();
                }
            }
        }

        public readonly string Tag;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cacheStrategy">缓存淘汰策略</param>
        /// <param name="tag">调试用tag</param>
        public ObjectCache(IEliminateCache<string, Bucket> cacheStrategy, GatherObjectPool<TObject> objPool, string tag, bool instantiable = true,float comboShortSurvialTime = 0)
        {
            m_initantiable = instantiable;
            m_keepInCombo = comboShortSurvialTime > 0;
            Tag = tag;
            m_cacheStrategy = cacheStrategy;
            m_cacheStrategy.OnEliminate += OnEliminated;
            m_clientRefBucket = new Dictionary<string, Bucket>();
            m_clientNoRefBucket = new Dictionary<string, float>();
            m_ObjPool = objPool;
        }

        public void ResetcacheStrategy(IEliminateCache<string, Bucket> cacheStrategy)
        {
            m_cacheStrategy.Release();
            m_cacheStrategy = cacheStrategy;
            m_cacheStrategy.OnEliminate += OnEliminated;
        }

        public TObject GetSync(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                Debug.LogErrorFormat("ObjectCache Tag {0} : The path is null!");
                return null;
            }
            
            Bucket bucket = GetBucket(path, true);
            if (bucket == null)
            {
                bucket = CreateNewBucket(path);
                bucket.AsyncLoading = false;
            }
            bucket.RefCount++;
            bucket.StartNoRefTime = -1;
            return bucket.GetObject();
        }

        public TObject GetAsync(string path, Action<TObject> callback)
        {
            if (string.IsNullOrEmpty(path))
            {
                Debug.LogErrorFormat("ObjectCache Tag {0} : The path is null!");
                callback(null);
                return null;
            }
            if (callback == null)
            {
                Debug.LogWarningFormat("ObjectCache Tag {0} : Parameter Callback of GetAsync is null!");
            }
            
            Bucket bucket = GetBucket(path, true);
            if (bucket == null)
            {
                bucket = CreateNewBucket(path);
            }
            bucket.RefCount++;
            bucket.StartNoRefTime = -1;
            return bucket.GetObjectAsync(callback);
        }

        /// <summary>
        /// 回收Object
        /// </summary>
        /// <param name="path"></param>
        /// <param name="asset"></param>
        /// <returns></returns>
        public bool Return(string name, TObject o)
        {
            if (string.IsNullOrEmpty(name))
            {
                Debug.LogErrorFormat("ObjectCache Tag : {0}, Return with invalid name={1}!", Tag, name);
                return false;
            }
            if (o == null)
            {
                Debug.LogErrorFormat("ObjectCache Tag : {0}, Must be returned with {1} instance!", Tag, typeof(TObject).Name);
                return false;
            }

            Bucket bucket;
            if (!m_clientRefBucket.TryGetValue(name, out bucket))
            {
                Debug.LogErrorFormat("ObjectCache Tag: {0}. No ref info exists for path {1}. May caused by unpaired Get-Return calls", Tag, name);
                return false;
            }

            bucket.RefCount--;
            bucket.RecoverObj(o);
            if (bucket.RefCount == 0)
            {
                bucket.StartNoRefTime = Time.realtimeSinceStartup;
                MoveToPhase2(bucket);
            }
            else if (bucket.RefCount < 0)
            {
                Debug.LogErrorFormat("ObjectCache Tag: {0}. Wrong ref count for path {1}.", Tag, name);
            }
            return true;
        }

        public void Release()
        {
            Dictionary<string, Bucket>.Enumerator m_bucketIter = m_clientRefBucket.GetEnumerator();
            while (m_bucketIter.MoveNext())
            {
                DestroyBucket(m_bucketIter.Current.Value);
            }
            m_clientRefBucket.Clear();
            m_clientRefBucket = null;

            m_clientNoRefBucket.Clear();
            m_clientNoRefBucket = null;

            m_cacheStrategy.Release();
            m_cacheStrategy = null;

            m_bucketPool.ClearAll();
            m_ObjPool.ClearAll();
        }


        #region private
        /// <summary>
        /// 调用此接口会增加bucket的引用计数
        /// </summary>
        /// <param name="bucket"></param>
        /// <returns></returns>
        TAsset GetAsset(TAsset resAsset)
        {
            if (resAsset == null)
            {
                return null;
            }
            TAsset asset = null;
            //bucket.RefCount++;
            if (m_initantiable)
            {
                asset = UnityObject.Instantiate<TAsset>(resAsset);
            }
            else
            {
                asset = resAsset;
            }

            return asset;
        }

        private void MoveToPhase2(Bucket bucket)
        {
            m_clientRefBucket.Remove(bucket.Path);
            if (!m_clientNoRefBucket.ContainsKey(bucket.Path))
            {
                m_clientNoRefBucket.Add(bucket.Path, bucket.StartNoRefTime);
            }

            // if (m_keepInCombo)
            // {
            //     var dataCenter = GameKernel.GetDataCenter();
            //     if (dataCenter != null)
            //     {
            //         NS_Player.IPlayer player = dataCenter.GetHero();
            //         if (player != null && player.IsReadyFight())
            //         {
            //             IEliminateCacheComboState<string, Bucket> comboCacheStrategy = m_cacheStrategy as IEliminateCacheComboState<string, Bucket>;
            //             comboCacheStrategy.ComboStateAdd(bucket.Path, bucket);
            //             return;
            //         }
            //     }
            // }
          
            m_cacheStrategy.Add(bucket.Path, bucket);       
        }

        private void ComboStateMoveToPhase2(Bucket bucket)
        {
            m_clientRefBucket.Remove(bucket.Path);

            IEliminateCacheComboState<string, Bucket> comboCacheStrategy = m_cacheStrategy as IEliminateCacheComboState<string, Bucket>;
            if (comboCacheStrategy != null)
            {
                comboCacheStrategy.ComboStateAdd(bucket.Path, bucket);
            }
            else
            {
                m_cacheStrategy.Add(bucket.Path, bucket);
            }

            if (!m_clientNoRefBucket.ContainsKey(bucket.Path))
            {
                m_clientNoRefBucket.Add(bucket.Path, bucket.StartNoRefTime);
            }
        }

        private void BackToPhase1(Bucket bucket)
        {
            if (m_clientRefBucket.ContainsKey(bucket.Path))
            {
                Debug.LogErrorFormat("ObjectCache Tag : {0} Bucket : {1} back to phase 1, find conflict res ", Tag, bucket.Path);
                return;
            }

            m_clientRefBucket.Add(bucket.Path, bucket);
          
            if (m_clientNoRefBucket.ContainsKey(bucket.Path))
            {
                m_clientNoRefBucket.Remove(bucket.Path);
            }
        }

        private Bucket CreateNewBucket(string path)
        {
            Bucket newBucket = BucketPool.Spawn();
            newBucket.Cache = this;
            newBucket.Path = path;
            newBucket.RefCount = 0;
            newBucket.AsyncLoading = false;
            if (newBucket.OnLoadeds == null)
                newBucket.OnLoadeds = new List<Bucket.WaitLoad>();
            m_clientRefBucket.Add(path, newBucket);
            if (m_clientNoRefBucket.ContainsKey(path))
            {
                m_clientNoRefBucket.Remove(path);
            }
            return newBucket;
        }

        private Bucket GetBucket(string name, bool backToClientRef = false)
        {
            Bucket bucket;
            if (m_clientRefBucket.TryGetValue(name, out bucket))
                return bucket;

            // if (m_keepInCombo)
            // {
            //     NS_Player.IPlayer player = GameKernel.GetDataCenter().GetHero();
            //     if (player != null && player.IsReadyFight())
            //     {
            //         IEliminateCacheComboState<string, Bucket> comboCacheStrategy = m_cacheStrategy as IEliminateCacheComboState<string, Bucket>;
            //         if (comboCacheStrategy != null && comboCacheStrategy.ComboStateTryGet(name,out bucket))
            //         {
            //             if (backToClientRef)
            //             {
            //                 BackToPhase1(bucket);
            //             }
            //             return bucket;
            //         }
            //     }
            // }

            if (m_cacheStrategy.TryGet(name, out bucket))
            {
                if (backToClientRef)
                {
                    BackToPhase1(bucket);
                }

                return bucket;
            }

            return null;
        }

        private void OnEliminated(string path, Bucket bucket)
        {
            if (bucket == null) return;
            if (bucket.RefCount == 0)
            {
                // if (m_keepInCombo && !SceneMgr.Instance.IsLoading)
                // {
                //     var dataCenter = GameKernel.GetDataCenter();
                //     if (dataCenter != null)
                //     {
                //         NS_Player.IPlayer player = dataCenter.GetHero();
                //         if (player != null && player.IsReadyFight())
                //         {
                //             ComboStateMoveToPhase2(bucket);//战斗状态重新加入LRU,不走销毁卸载
                //             return;
                //         }
                //     }
                // }

                DestroyBucket(bucket);
            }
            else if (bucket.RefCount > 0)
            {
                if (!m_clientRefBucket.ContainsKey(bucket.Path))
                    Debug.LogErrorFormat("Logic Error : Please Check it! path = {0}", path);
            }
            else
            {
                Debug.LogErrorFormat("ObjectCache Tag : {0}, Bucket : {1} be eliminated with wrong ref count : {2}", Tag, bucket.Path, bucket.RefCount);
            }
        }

        private void DestroyBucket(Bucket bucket)
        {
            if (m_clientNoRefBucket.ContainsKey(bucket.Path))
            {
                m_clientNoRefBucket.Remove(bucket.Path);
            }
            m_ObjPool.DestroyPool(bucket.Path);
            bucket.Destroy();
            BucketPool.Despawn(bucket);
           
        }

        public void DebugLog(bool debugpool)
        {
            if (EnableDebug)
            {
                m_cacheStrategy.DebugLog(string.Format("{0} AssetCache : Current assign count = {1}.\r\n", Tag, m_clientRefBucket.Count));
                if (debugpool)
                {
                    m_ObjPool.DebugLog();
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

        public void ReleaseUnuseAsset()
        {
            m_cacheStrategy.ReleaseUnuseAsset();
        }

        public void LogAllAsset(string head=null)
        {
            string s = string.Format("{0}ObjectCache={1}所有资源如下：\n", head, Tag);
            Dictionary<string, Bucket>.Enumerator iter = m_clientRefBucket.GetEnumerator();
            s = s + "正在使用的资源";
            while (iter.MoveNext())
            {
                s = s + iter.Current.Value.Path + "\n";
            }
            m_cacheStrategy.LogAllAsset(s);
        }

        [LuaInterface.NoToLua]
        public Dictionary<string, float> GetClientNoRefBucket()
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

        bool m_initantiable;
        private bool m_keepInCombo;
        private IEliminateCache<string, Bucket> m_cacheStrategy;
        private Dictionary<string, Bucket> m_clientRefBucket;
        private Dictionary<string, float> m_clientNoRefBucket;
        private GatherObjectPool<TObject> m_ObjPool;
        #endregion
    }
}
