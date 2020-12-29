using System.Collections.Generic;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace Best_Pool
{
    /**
     * 用于对象集合
     * GatherInstancePool用于同类型实例对象集合
     * GatherObjectPool用于对象中包含资源或者实例对象引用的情况，objectpool也可替代使用。
     **/
    interface IGatherPool<TPool,TValue> : IPool
    {
        bool ContainKey(string key);
        bool TryGet(string key, out TPool pool);
        void Set(string k, TPool v);
        TValue Spawn(string k);
        void Despawn(TValue ins, string name);
        void DestroyPool(string k);
    }

    public abstract class GatherPoolParent<TPool,TValue> : IGatherPool<TPool, TValue> where TPool : PoolParent<TValue>
    {
        protected string poolName;
        protected string typeName;
        protected bool bLogMessage = false;

        Dictionary<string, TPool> m_poolDic;
        protected Dictionary<string, TPool> PoolDic
        {
            get
            {
                if (m_poolDic == null)
                    m_poolDic = new Dictionary<string, TPool>();
                return m_poolDic;
            }
            set
            {
                m_poolDic = value;
            }
        }

        protected LimitSetter m_limitSetter;
        protected CullSetter m_cullSetter;
        protected PreSetter m_preSetter;

        public GatherPoolParent(string tag, LimitSetter limitSetter = null, CullSetter cullSetter = null, PreSetter preSetter = null)
        {
            poolName = tag;
            typeName = typeof(TValue).Name;
            if (!BuildManifestUtility.GetBuildManifest().IsReleaseVer && pool_manager.IsDebugPool(tag))
                bLogMessage = true;
            m_limitSetter = limitSetter;
            m_preSetter = preSetter;
            m_cullSetter = cullSetter;
        }

        public virtual bool ContainKey(string name)
        {
            return PoolDic.ContainsKey(name);
        }

        public virtual bool TryGet(string name, out TPool pool)
        {
            if (PoolDic.TryGetValue(name, out pool))
            {
                return true;
            }
            return false;
        }

        public virtual void Set(string k, TPool v)
        {
            if (v == null)
            {
                Debug.LogError("GatherPool : The pool is null!");
                return;
            }
            if (PoolDic.ContainsKey(k))
            {
                Debug.LogErrorFormat("GatherPool : The pool name {0} is exist!", k);
                return;
            }
            else
                PoolDic.Add(k, v);
        }

        public virtual TValue Spawn(string name)
        {
            TPool pool;
            if (TryGet(name, out pool))
            {
                return pool.Spawn();
            }
            Debug.LogError(string.Format("GatherPool {0} have no pool name {1}", poolName, name));
            return default(TValue);
        }

        public abstract void Despawn(TValue ins, string name);
        public virtual void DestroyPool(string name)
        {
            TPool pool;
            if (PoolDic.TryGetValue(name, out pool))
            {
                pool.ClearAll();
                PoolDic.Remove(name);
            }
        }

        public TPool this[string key]
        {
            get
            {
                TPool pool;
                try
                {
                    TryGet(key, out pool);
                }
                catch (KeyNotFoundException)
                {
                    string msg = string.Format("A InstancePool with the name '{0}' not found. ", key);
                    Debug.LogError(new KeyNotFoundException(msg));
                    throw null;
                }
                return pool;
            }
            set
            {
                if (value == null)
                {
                    DestroyPool(key);
                }
                else
                {
                    Set(key, value);
                }
            }
        }

        Dictionary<string, TPool>.Enumerator iter;
        public virtual void LateUpdate()
        {
            iter = PoolDic.GetEnumerator();
            while (iter.MoveNext())
            {
                if (iter.Current.Value != null)
                    iter.Current.Value.LateUpdate();
            }
        }

        public virtual void ClearAll()
        {
            iter = PoolDic.GetEnumerator();
            while (iter.MoveNext())
            {
                if (iter.Current.Value != null)
                {
                    iter.Current.Value.ClearAll();
                }
            }
            PoolDic.Clear();
            PoolDic = null;
        }

        bool m_isDebug = false;
        public bool IsDebug
        {
            get { return m_isDebug; }
            set
            {
                m_isDebug = value;
                Dictionary<string, TPool>.Enumerator iter = PoolDic.GetEnumerator();
                while (iter.MoveNext())
                    iter.Current.Value.IsDebug = value;
            }
        }

        public void DebugLog()
        {
            Dictionary<string, TPool>.Enumerator iter = PoolDic.GetEnumerator();
            int count = 0;
            while (iter.MoveNext())
            {
                count++;
                iter.Current.Value.DebugLog();
                if (count == 10)
                    break;
            }
        }
    }

    public class GatherInstancePool<T> : GatherPoolParent<InstancePool<T>,T> where T : UnityObject
    {
        Transform m_parent;

        public Transform Parent
        {
            get
            {
                return m_parent;
            }
        }

        public GatherInstancePool(string tag, LimitSetter limitSetter = null, CullSetter cullSetter = null, PreSetter preSetter = null)
            : base(tag, limitSetter, cullSetter, preSetter)
        {
            if (typeName == "GameObject")
            {
                m_parent = new GameObject("GatherInstancePool_" + tag).transform;
                Transform.DontDestroyOnLoad(m_parent);
            }
        }

        public InstancePool<T> AddPool(string name, T asset = null, uint ResUID = 0, LimitSetter limitSetter = null, CullSetter cullSetter = null, PreSetter preSetter = null)
        {
            InstancePool<T> pool;
            if (TryGet(name, out pool))
            {
                Debug.LogErrorFormat("GatherInstancePool {0}: Exist the same pool name of {1}", poolName, name);
                return pool;
            }
            pool = new InstancePool<T>(name, asset, m_parent,
                    limitSetter == null ? m_limitSetter : limitSetter,
                    cullSetter == null ? m_cullSetter : cullSetter,
                    preSetter == null ? m_preSetter : preSetter);
            pool.IsDebug = IsDebug;
            pool.ResUID = ResUID;
            Set(name, pool);
            return pool;
        }

        public override T Spawn(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                Debug.LogError("GatherInstancePool.Spawn -> The key IsNullOrEmpty");
                return null;
            }

            InstancePool<T> pool;
            if (TryGet(name, out pool))
            {
                return pool.Spawn();
            }
            Debug.LogError(string.Format("GatherInstancePool : The parameter 'asset' is null!"));
            return null;
        }

        public override void Despawn(T ins, string name)
        {
            InstancePool<T> pool;
            if(TryGet(name, out pool))
            {
                pool.Despawn(ins);
                return;
            }
            if (bLogMessage)
                Debug.LogError(string.Format("InstanceGatherPool {0}: Has not find the pool name {1}", poolName, name));
            UnityObject.Destroy(ins);
            ins = null;
        }

        public override void ClearAll()
        {
            base.ClearAll();
            if (m_parent != null) GameObject.Destroy(m_parent.gameObject);
            m_parent = null;
        }
    }

    public class GatherObjectPool<T> : GatherPoolParent<ObjectPool<T>, T> where T : class, new()
    {
        public GatherObjectPool(string tag, LimitSetter limitSetter = null, CullSetter cullSetter = null, PreSetter preSetter = null) 
            : base(tag, limitSetter, cullSetter, preSetter)
        {
        }

        public ObjectPool<T> AddPool(string name, LimitSetter limitSetter = null, CullSetter cullSetter = null, PreSetter preSetter = null)
        {
            ObjectPool<T> pool;
            if (TryGet(name, out pool))
            {
                Debug.LogErrorFormat("GatherObjectPool {0}: Exist the pool name of {1}", poolName, name);
                return pool;
            }
            pool = new ObjectPool<T>(name,
                    limitSetter == null ? m_limitSetter : limitSetter,
                    cullSetter == null ? m_cullSetter : cullSetter,
                    preSetter == null ? m_preSetter : preSetter);
            pool.IsDebug = IsDebug;
            Set(name, pool);
            return pool;
        }

        public override T Spawn(string name)
        {
            ObjectPool<T> pool;
            if(TryGet(name, out pool))
            {
                return pool.Spawn();
            }
            Debug.LogErrorFormat("GatherObjectPool {0}: Exist the pool name of {1}", poolName, name);
            return null;
        }

        public override void Despawn(T ins, string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                Debug.LogErrorFormat("GatherObjectPool : The parameter pool 'name' is null!");
                return;
            }
            ObjectPool<T> pool;
            if(TryGet(name, out pool))
            {
                pool.Despawn(ins);
                return;
            }
            if (bLogMessage)
                Debug.LogError(string.Format("GatherObjectPool {0}: Has not find the pool name {1}", poolName, name));
        }
    }
}
