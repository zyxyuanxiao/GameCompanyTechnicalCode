using UnityEngine;
using System.Collections.Generic;

namespace Best
{
    /**
     * <summary>
     * 用于衡量cache性能的debug工具。
     * 主要指标是： 
     *      1.cache命中率. 既从Cache中成功获得缓存的比例。
     *      2.cache的曾经命中率。 既Cache中曾经存在过的key，现在已经被淘汰，然后又被尝试从缓存中获取的比例。
     * </summary>
     */
    public class CacheDebugTool<TKey>
    {
        public enum CacheTypeEnum
        {
            None,
            FIFO,
            LRU,
            LRUK,
            LRUTTL,
        }

        public CacheDebugTool(CacheTypeEnum type, uint logKeyCount)
        {
            this.CacheType = type;
            m_logKeyCount = logKeyCount;
        }

        public void OnTryGet(TKey key, bool success)
        {
            TotalGetTimes++;
            if (success)
            {
                CacheHitTimes++;
            }
            else
            {
                if (m_keyLog.Contains(key))
                    UsedHitTime++;
            }
        }

        public void OnCacheItemRemoved(TKey key)
        {
            if (m_keyLog.Contains(key))
                return;

            m_keyLog.Add(key);
            m_keyLogList.AddFirst(key);
            if (m_keyLogList.Count > m_logKeyCount)
            {
                LinkedListNode<TKey> lastKey = m_keyLogList.Last;
                m_keyLogList.RemoveLast();
                m_keyLog.Remove(lastKey.Value);
            }

        }

        public virtual void Log(string s) { }

        public virtual string LogString(string s) { return ""; }
        
        /**
         * 缓存命中次数
         * 缓存命中比例过低，说明cache没用。
         */
        public uint CacheHitTimes { get; private set; }
        /**
         * 缓存key曾经存在但被淘汰，然后又被tryget的次数 
         * 这个值过高，就需要考虑当前淘汰策略是否合适。是否要更换LRU-K以降低cache更新速度，或者是增加capacity)
        */
        public uint UsedHitTime { get; private set; }
        //总计tryget的次数
        public uint TotalGetTimes { get; private set; }

        public CacheTypeEnum CacheType;

        //记录cache中存在过的key， 最大记录数量为capacity * 10。
        public HashSet<TKey> m_keyLog = new HashSet<TKey>();
        public LinkedList<TKey> m_keyLogList = new LinkedList<TKey>();

        private uint m_logKeyCount = 0;
    }

    public class LRUDebugTool<TKey> : CacheDebugTool<TKey>
    {
        public LRUDebugTool(CacheTypeEnum type, uint logKeyCount) : base(type,logKeyCount)
        {

        }

        public override void Log(string s)
        {
            //Debug.LogFormat("{0} cache strategy : {1}", tag, CacheType.ToString());
            //Debug.LogFormat("tryget times = {0}, cache hit times = {1}, used hit times = {2}.", TotalGetTimes, CacheHitTimes, UsedHitTime);
            Debug.LogFormat(LogString(s));
        }

        public override string LogString(string s)
        {
            return string.Format("{0} cache strategy : {1}\n tryget times = {2}, cache hit times = {3}, used hit times = {4}.",
                s, CacheType.ToString(), TotalGetTimes, CacheHitTimes, UsedHitTime);
        }
    }

    public class LRUKDebugTool<TKey> : CacheDebugTool<TKey>
    {
        public LRUKDebugTool(CacheTypeEnum type, uint logKeyCount) : base(type, logKeyCount)
        {

        }

        public override void Log(string s)
        {
            //Debug.LogFormat("{0} cache strategy : {0}", tag, CacheType.ToString());
            //Debug.LogFormat("level 1 cache : tryget times {0}, cache hit times = {1}, use hit times = {2}.", TotalGetTimes, CacheHitTimes, UsedHitTime);
            Debug.LogFormat(LogString(s));
        }

        public override string LogString(string s)
        {
            string str =  string.Format("{0} cache strategy : {1}\n level 1 cache : tryget times {2}, cache hit times = {3}, use hit times = {4}."
                , s, CacheType.ToString(), TotalGetTimes, CacheHitTimes, UsedHitTime);
            foreach (var item in m_kTimesDic)
            {
                str = string.Format("{0}\n k = {1}, cache key times = {2}", str, item.Key, item.Value);
                //Debug.LogFormat("k = {0}, cache key times = {1}", item.Key, item.Value);
            }
            str = string.Format("{0}.\n", str);
            return str;
        }

        public void AddToK(uint k, TKey key)
        {
            if (!m_kTimesDic.ContainsKey(k))
                m_kTimesDic.Add(k, 0);
            m_kTimesDic[k]++;

            if (!m_kKeyDic.ContainsKey(k))
                m_kKeyDic.Add(k, new LinkedList<TKey>());
            m_kKeyDic[k].AddFirst(key);
        }

        Dictionary<uint, LinkedList<TKey>> m_kKeyDic = new Dictionary<uint, LinkedList<TKey>>();
        Dictionary<uint, uint> m_kTimesDic = new Dictionary<uint, uint>();
    }

    public class LRUTTLDebugTool<TKey> : CacheDebugTool<TKey>
    {
        public LRUTTLDebugTool(CacheTypeEnum type, uint logKeyCount) : base(type, logKeyCount)
        {

        }

        public override void Log(string s)
        {
            Debug.LogFormat(LogString(s));
            //Debug.LogFormat("{0} cache strategy : {0}", tag, CacheType.ToString());
            //Debug.LogFormat("tryget times = {0}, cache hit times = {1}, used hit times = {2}, out time times = {3}.", TotalGetTimes, CacheHitTimes, UsedHitTime, OutTimeTimes);
        }

        public override string LogString(string s)
        {
           return string.Format("{0} cache strategy : {1}.\n tryget times = {2}, cache hit times = {3}, used hit times = {4}, out time times = {5}.",
               s, CacheType.ToString(), TotalGetTimes, CacheHitTimes, UsedHitTime, OutTimeTimes);
        }

        public uint OutTimeTimes = 0;
    }
}
