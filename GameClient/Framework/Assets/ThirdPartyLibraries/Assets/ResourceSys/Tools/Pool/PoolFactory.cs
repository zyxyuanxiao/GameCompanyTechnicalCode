using System.Collections.Generic;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace Best_Pool
{
    public class PoolFactory
    {
        public static GatherInstancePool<T> CreateGatherInstancePool<T>(string tag, LimitSetter limitSetter = null, CullSetter cullSetter = null, PreSetter preSetter = null)
            where T : UnityObject
        {
            return new GatherInstancePool<T>(tag, limitSetter, cullSetter, preSetter);
        }

        public static GatherObjectPool<T> CreateGatherObjectPool<T>(string tag, LimitSetter limitSetter = null, CullSetter cullSetter = null, PreSetter preSetter = null)
            where T : class, new()
        {
            return new GatherObjectPool<T>(tag, limitSetter, cullSetter, preSetter);
        }

        public static InstancePool<T> CreateInstancePool<T>(string tag, T o, int max)
            where T : UnityObject
        {
            return CreateInstancePool<T>(tag, o, null, (ref bool blimit, ref int amount) => { });
        }

        public static InstancePool<T> CreateInstancePool<T>(string tag, T o, Transform parent = null, LimitSetter limitSetter = null, CullSetter cullSetter = null, PreSetter preSetter = null)
            where T : UnityObject
        {
            return new InstancePool<T>(tag, o, parent, limitSetter, cullSetter, preSetter);
        }

        public static ObjectPool<T> CreateObjectPool<T>(string tag, int max)
            where T : class, new()
        {
            return CreateObjectPool<T>(tag, (ref bool blimit, ref int amount) => { });
        }

        public static ObjectPool<T> CreateObjectPool<T>(string tag, LimitSetter limitSetter = null, CullSetter cullSetter = null, PreSetter preSetter = null)
            where T : class, new()
        {
            return new ObjectPool<T>(tag, limitSetter, cullSetter, preSetter);
        }
    }

    public class pool_manager
    {
        Dictionary<string, IPool> m_poolDic;
        Dictionary<string, IPool> PoolDic
        {
            get
            {
                if (m_poolDic == null)
                    m_poolDic = new Dictionary<string, IPool>();
                return m_poolDic;
            }
        }
        Dictionary<string, IPool>.Enumerator PoolDicIter;

        public GatherInstancePool<T> GetGatherInstancePool<T>(string tag, LimitSetter limitSetter = null, CullSetter cullSetter = null, PreSetter preSetter = null)
            where T : UnityObject
        {
            if (!PoolDic.ContainsKey(tag))
            {
                GatherInstancePool<T> pool = PoolFactory.CreateGatherInstancePool<T>(tag, limitSetter, cullSetter, preSetter);
                PoolDic.Add(tag, pool);
            }
            return PoolDic[tag] as GatherInstancePool<T>;
        }

        public GatherObjectPool<T> GetGatherObjectPool<T>(string tag, LimitSetter limitSetter = null, CullSetter cullSetter = null, PreSetter preSetter = null)
            where T : class, new()
        {
            if (!PoolDic.ContainsKey(tag))
            {
                GatherObjectPool<T> pool = PoolFactory.CreateGatherObjectPool<T>(tag, limitSetter, cullSetter, preSetter);
                PoolDic.Add(tag, pool);
            }
            return PoolDic[tag] as GatherObjectPool<T>;
        }

        public InstancePool<T> GetInstancePool<T>(T asset, Transform parent = null, LimitSetter limitSetter = null, CullSetter cullSetter = null, PreSetter preSetter = null)
            where T : UnityObject
        {
            if (!PoolDic.ContainsKey(asset.name))
            {
                InstancePool<T> pool = PoolFactory.CreateInstancePool<T>(asset.name, asset, parent, limitSetter, cullSetter, preSetter);
                PoolDic.Add(asset.name, pool);
            }
            return PoolDic[asset.name] as InstancePool<T>;
        }

        public ObjectPool<T> GetObjectPool<T>(string key, LimitSetter limitSetter = null, CullSetter cullSetter = null, PreSetter preSetter = null)
            where T : class, new()
        {
            if (!PoolDic.ContainsKey(key))
            {
                ObjectPool<T> pool = PoolFactory.CreateObjectPool<T>(key, limitSetter, cullSetter, preSetter);
                PoolDic.Add(key, pool);
            }
            return PoolDic[key] as ObjectPool<T>;
        }

        public void RemovePool(string key)
        {
            if (!PoolDic.ContainsKey(key)) return;
            PoolDic[key].ClearAll();
            PoolDic.Remove(key);
        }

        public void LateUpdate()
        {
            PoolDicIter = PoolDic.GetEnumerator();
            while (PoolDicIter.MoveNext())
            {
                PoolDicIter.Current.Value.LateUpdate();
            }
        }

        #region debug
        static HashSet<string> m_debugPoolSet;
        public static HashSet<string> DebugPoolSet
        {
            get
            {
                if (m_debugPoolSet == null)
                    m_debugPoolSet = new HashSet<string>();
                return m_debugPoolSet;
            }
        }

        public static bool IsDebugPool(string name)
        {
            return DebugPoolSet.Contains(name);
        }

        public static void AddDebugPool(string name)
        {
            DebugPoolSet.Add(name);
        }
        #endregion
    }

    public class PoolDebug<T>
    {
        public int DeadTimes = 0;                       //死亡次数
        public int ActivateTimes = 0;                   //激活次数

        public int SpawnTimes = 0;                      //spawn次数
        public int DespawnTimes = 0;                    //despawn次数
        public int CreateTimes = 0;                     //create次数
        public int DestroyTimes = 0;                    //destroy次数
        public int LimitTimes = 0;                      //超出限制次数
        public int CullTimes = 0;                       //Cull次数
        public int CullCount = 0;                       //Cull个数
        //public int SpawnCount = 0;                      //spawn存在数量
        //public int DespawnCount = 0;                    //despawn存在数量

        PoolParent<T> pool;

        public PoolDebug(PoolParent<T> pool)
        {
            this.pool = pool;
        }

        public void DebugLog()
        {
            System.Text.StringBuilder s = LuaInterface.StringBuilderCache.Acquire();
            s.AppendFormat("pool {0}, type {1} debug信息.\n", pool.poolName, pool.typeName);
            s.AppendFormat("Spawn次数:{0}\t\t\tDespawn次数:{1}\n", SpawnTimes, DespawnTimes);
            s.AppendFormat("CreateNew次数:{0}\t\t\tDestroy次数:{1}\n", CreateTimes, DestroyTimes);
            s.AppendFormat("Cull次数:{0}\t\t\t\tCull数量:{1}\n", CullTimes, CullCount);
            s.AppendFormat("Spawn列表数量:{0}\t\t\tDespawn列表数量:{1}\n", pool.SpawnedCount, pool.DespawnedCount);
            s.AppendFormat("Dead次数:{0}\t\t\tActivate次数:{1}\n", DeadTimes, ActivateTimes);
            s.AppendFormat("超出限制次数:{0}", LimitTimes);
            Debug.Log(s.ToString());
            LuaInterface.StringBuilderCache.Release(s);
        }
    }
}

