using UnityEngine;
using System.Collections.Generic;
using System;

namespace Best
{
    /**<summary>
     * LRU-K主要是为了解决LRU的缓存污染问题。
     * 当短时间内有大量只使用一次的缓存数据产生时，LRU的命中率会急剧下降。
     * 
     * LRU-K对于这个问题的解决办法是：
     * 双重缓存。 第一层缓存称为short-term缓存。 第二层缓存称为long-term缓存。
     * short-term缓存的数据有生命周期， 在生命周期内每被访问一次，访问次数加一且生命周期重置。 当访问次数达到K次时，移入long-term缓存。
     * long-term缓存的数据按照最简单的LRU规则管理。
     * </summary>
     * 
     * <remarks>
     * LRU-K相对于LRU提供了更稳定的缓存命中率。 但是缓存对象的数量大幅增加。
     * K = 0，退化为有TTL的LRU.(此时如果shortTermSurvialSeconds为0，则进一步退化为普通的LRU)
     * K = 1, 访问一次即加入long-term cache
     * K = 2, 访问两次加入long-term cache
     * 以此类推
     * </remarks>
     */
    public class LRUKCache<TKey, TValue> : ILateUpdate, IEliminateCache<TKey, TValue>
    {
        public event System.Action<TKey, TValue> OnEliminate = delegate { };
        string m_tag;
        /**
         * <param name="K">
         * 阈值K
         * </param>
         * <param name="longTermCapacity">
         * long-term缓存最大容量
         * </param>
         * <param name="shortTermCapacity">
         * short-term缓存最大容量
         * </param>
         * <param name="shortTermSurvialSeconds">
         * short-term中的缓存数据的生存周期, 为0表示不自动删除short-term中的cache项。(一般情况下不要用0)
         * </param>
         */
        public LRUKCache(string tag, uint K, uint shortTermCapacity = 30, uint longTermCapacity = 10, float shortTermSurvialSeconds = 120)
        {
            m_K = K;
            m_shortTermCapacity = shortTermCapacity;
            m_longTermCapacity = longTermCapacity;
            m_shortTermSurvialSeconds = shortTermSurvialSeconds;
            m_shorTermCache = new Dictionary<TKey, ShortTermEntry>();
            m_layer = new LinkedList<TKey>[m_K];
            for (int i = 0; i < m_K; i ++)
                m_layer[i] = new LinkedList<TKey>();

            m_longTermCache = new LRUCache<TKey, TValue>("level 2", longTermCapacity);
            m_tag = tag;
            UpdatableRunner.Instance.AddLateUpdate(this);
        }

        public TValue Add(TKey key, TValue value, bool updateExists = true)
        {
            TValue preValue;
            if (m_longTermCache.TryGet(key, out preValue))
            {
                if (updateExists)
                {
                    m_longTermCache.UpdateValue(key, value);
                    return value;
                }
                return preValue;
            }

            ShortTermEntry entry;
            if(m_shorTermCache.TryGetValue(key, out entry))
            {
                if (updateExists)
                {
                    AddShortTermCache(key, value, entry.Times);
                    return value;
                }
                else
                {
                    return entry.Value;
                }
            }

            if(m_shorTermCache.Count ==  m_shortTermCapacity)
            {
                SweepOneShortTermCache();
            }
            AddShortTermCache(key, value, 0);
            return value;
        }

        public bool TryGet(TKey key, out TValue value)
        {
            if(m_longTermCache.TryGet(key, out value))
            {
                return true;
            }

            ShortTermEntry entry;
            if (m_shorTermCache.TryGetValue(key, out entry))
            {
                if (EnableDebug)
                    m_debugTool.OnTryGet(key, true);
                value = entry.Value;
                uint times = entry.Times + 1;
                if(times < m_K || m_K == 0)
                {
                    AddShortTermCache(key, value, times);
                }
                else
                {
                    SweepOut(key, entry.Times, true);
                    m_longTermCache.Add(key, value);
                }
                return true;
            }

            if (EnableDebug)
                m_debugTool.OnTryGet(key, false);

            value = default(TValue);
            return false;
        }

        [LuaInterface.NoToLua]
        public bool TryGetNoUpdate(TKey key, out TValue value)
        {
            if (m_longTermCache.TryGetNoUpdate(key, out value))
            {
                return true;
            }

            ShortTermEntry entry;
            if (m_shorTermCache.TryGetValue(key, out entry))
            {
                if (EnableDebug)
                    m_debugTool.OnTryGet(key, true);
                value = entry.Value;           
                return true;
            }

            value = default(TValue);
            return false;
        }

        public void Release()
        {
            if (EnableDebug)
                DebugLog(m_tag);
            m_longTermCache.Release();
            foreach(var kv in m_shorTermCache)
            {
                OnEliminate(kv.Value.Key, kv.Value.Value);
            }

            m_longTermCache = null;
            m_K = 0;
            m_shortTermCapacity = 0;
            m_shorTermCache = null;
            m_layer = null;
            m_shortTermSurvialSeconds = 0.0f;
            OnEliminate = null;

            UpdatableRunner.Instance.RemoveLateUpdate(this);
        }
        [LuaInterface.NoToLua]
        public void Remove(TKey key)
        {
            ShortTermEntry val;
            if (m_shorTermCache.TryGetValue(key, out val))
            {
                m_shorTermCache.Remove(key);
                if (EnableDebug)
                    m_debugTool.OnCacheItemRemoved(key);

                if (m_layer[val.Times] != null)
                {
                    m_layer[val.Times].Remove(key);
                }
            }
            m_longTermCache.Remove(key);

            //OnEliminate(val.Key, val.Value);          
        }

        //淘汰过期数据
        static List<ShortTermEntry> ToRemoveBuffer = new List<ShortTermEntry>(4);
        [LuaInterface.NoToLua]
        public void LateUpdate()
        {
            if (m_shortTermSurvialSeconds == 0)
                return;

            ToRemoveBuffer.Clear();
            Dictionary<TKey, ShortTermEntry>.Enumerator m_shorTermCacheIter = m_shorTermCache.GetEnumerator();
            while (m_shorTermCacheIter.MoveNext())
            {
                if (bTestMode)
                {
                    //测试 直接加入remove
                    ToRemoveBuffer.Add(m_shorTermCacheIter.Current.Value);
                }
                else
                {
                    //正常
                    if ((m_shorTermCacheIter.Current.Value.LastVisitTime + m_shortTermSurvialSeconds) < Time.time)
                    {
                        ToRemoveBuffer.Add(m_shorTermCacheIter.Current.Value);
                    }
                }                              
            }
            for (int i = 0; i < ToRemoveBuffer.Count; i++)
            {
                SweepOut(ToRemoveBuffer[i].Key, ToRemoveBuffer[i].Times);
                m_shorTermCache.Remove(ToRemoveBuffer[i].Key);
            }
        }

        [LuaInterface.NoToLua]
        public void RegiestEvent(Action<TKey, TValue> OnEliminated)
        {
            OnEliminate += OnEliminated;
            m_longTermCache.OnEliminate -= OnEliminated;
            m_longTermCache.OnEliminate += OnEliminated;
        }

        #region private
        struct ShortTermEntry
        {
            public TKey Key;
            public TValue Value;
            //访问次数
            public uint Times;
            //上次访问时间
            public float LastVisitTime;
        }

        private uint m_K;
        private uint m_shortTermCapacity;
        private Dictionary<TKey, ShortTermEntry> m_shorTermCache;
        private LinkedList<TKey>[] m_layer;
        private float m_shortTermSurvialSeconds;

        private uint m_longTermCapacity;
        private LRUCache<TKey, TValue> m_longTermCache;
        private bool bTestMode = false;
        #region Debug Interface
        public void DebugLog(string s)
        {
            if (EnableDebug)
            {
                string.Format("{0}level 1 capacity = {1} survive time = {2}, level 2 capacity = {3}.\n", s, m_shortTermCapacity, m_shortTermSurvialSeconds, m_longTermCapacity);
                m_longTermCache.DebugLog(m_debugTool.LogString(s));
            }
        }

        public bool EnableDebug
        {
            get
            {
                return m_enableDebug;
            }
            set
            {
                if (m_enableDebug == false && value)
                {
                    m_longTermCache.EnableDebug = true;
                    m_debugTool = new LRUKDebugTool<TKey>(CacheDebugTool<TKey>.CacheTypeEnum.LRUK, m_shortTermCapacity * KeyLogCapacityCoefficient);
                    m_enableDebug = true;
                }
            }
        }
        public uint KeyLogCapacityCoefficient = 10;
        private bool m_enableDebug = false;

        public LRUKDebugTool<TKey> m_debugTool;
        #endregion

        private void AddShortTermCache(TKey key, TValue value, uint times)
        {
            if (m_shorTermCache.ContainsKey(key))
            {
                m_shorTermCache[key] = new ShortTermEntry { Key = key, Value = value, Times = times, LastVisitTime = Time.time };
            }
            else
            {
                m_shorTermCache.Add(key, new ShortTermEntry { Key = key, Value = value, Times = times, LastVisitTime = Time.time });
            }
            if (EnableDebug)
                m_debugTool.AddToK(times, key);
            if (times > 0)
            {
                m_layer[times - 1].Remove(key);
            }
            m_layer[times].AddFirst(key);
        }

        private void SweepOneShortTermCache()
        {
            for(uint i = 0; i < m_K; i++)
            {
                if(m_layer[i].Count > 0)
                {
                    SweepOut(m_layer[i].Last.Value, i);
                    return;
                }
            }
        }

        private void SweepOut(TKey key, uint layer, bool moveToLongTerm = false)
        {
            ShortTermEntry val ;
            if(m_shorTermCache.TryGetValue(key, out val))
            {
                if (!moveToLongTerm)
                    OnEliminate(val.Key, val.Value);
                m_shorTermCache.Remove(key);
                if (EnableDebug)
                    m_debugTool.OnCacheItemRemoved(key);
            }
            else
            {
                return;
            }

            if (!m_layer[layer].Remove(key))
            {
                Debug.LogError("LRUKCache layer sweep out error!");
            }
        }
        #endregion

        #region 测试专用
        /// <summary>
        /// ****只给测试资源卸载,强制清理短时缓存********
        /// </summary>
        [LuaInterface.NoToLua]
        public void OpenTestMode()
        {
            bTestMode = true; 
        }

        public void LogAllAsset(string head = null)
        {
            string s = string.Format("{0}LRUKCache tag = {1},一级缓存资源列表\n", head, m_tag);
            Dictionary<TKey, ShortTermEntry>.Enumerator iter = m_shorTermCache.GetEnumerator();
            while (iter.MoveNext())
            {
                s = s + iter.Current.Key + "\n";
            }
            m_longTermCache.LogAllAsset(s + "二级缓存：");
        }

        public void ReleaseUnuseAsset()
        {
            m_longTermCache.ReleaseUnuseAsset();
            Dictionary<TKey, ShortTermEntry>.Enumerator iter = m_shorTermCache.GetEnumerator();
            while (iter.MoveNext())
            {
                OnEliminate(iter.Current.Key, iter.Current.Value.Value);
            }
            m_shorTermCache.Clear();
        }      
        #endregion
    }
}
