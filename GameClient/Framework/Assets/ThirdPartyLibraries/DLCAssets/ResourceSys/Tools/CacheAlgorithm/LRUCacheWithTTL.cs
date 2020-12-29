using UnityEngine;
using System.Collections.Generic;
using System;

namespace Best
{
    /**
     * TTL: Time To Live
     * 有生存期限的LRU实现. 典型应用例如： 高频率生成和销毁的临时物体的短暂缓存。
     * 
     * TODO：
     * 1. LRUCacheWithTTL 与 LRUCache有非常多的冗余代码，但是暂时没想到合适的减少冗余代码的方式.
     * 2. 这种模式真的有必要吗, LRU-K提供了更稳定的TTL实现。
     */
    public class LRUCacheWithTTL<TKey, TValue> : ILateUpdate, IEliminateCache<TKey, TValue>, IEliminateCacheComboState<TKey, TValue>
    {
        public event Action<TKey, TValue> OnEliminate = delegate { };
        string m_tag;

        public LRUCacheWithTTL(string tag, uint capacity, float ttl,float comboTTL = 0)
        {
            m_data = new LinkedList<KeyValuePair>();
            m_index = new Dictionary<TKey, LinkedListNode<KeyValuePair>>();
            m_capacity = capacity;
            m_TTL = ttl;
            m_tag = tag;
            m_comboTTL = comboTTL;
            UpdatableRunner.Instance.AddLateUpdate(this);
        }

        public void Resize(uint newCapacity)
        {
            if (newCapacity < m_capacity)
            {
                for (uint i = newCapacity; i < m_capacity; i++)
                {
                    RemoveLast();
                }
            }

            m_capacity = newCapacity;
        }

        public bool TryGet(TKey key, out TValue value)
        {
           return TrtGetFromeDict(key, out value);
        }

        [LuaInterface.NoToLua]
        public bool TryGetNoUpdate(TKey key, out TValue value)
        {
            LinkedListNode<KeyValuePair> dataNode;
            if (m_index.TryGetValue(key, out dataNode))
            {
                value = dataNode.Value.Value;

                return true;
            }

            value = default(TValue);
            return false;
        }

        [LuaInterface.NoToLua]
        public bool ComboStateTryGet(TKey key, out TValue value)
        {
            return TrtGetFromeDict(key, out value, true);
        }

        public TValue Add(TKey key, TValue value, bool updateValue = true)
        {
            UpdateNodeValue(key, value, updateValue);
            return value;
        }

        [LuaInterface.NoToLua]
        public TValue ComboStateAdd(TKey key, TValue value, bool updateValue = true)
        {
            UpdateNodeValue(key, value, updateValue, true);
            return value;
        }

        private TValue UpdateNodeValue(TKey key, TValue value, bool updateValue, bool comboState = false)
        {
            if (m_index.ContainsKey(key))
            {
                if (updateValue)
                {
                    UpdateValue(key, value, comboState);
                    return value;
                }
                else
                {
                    return m_index[key].Value.Value;
                }
            }

            KeyValuePair newData = new KeyValuePair { Key = key, Value = value, TimeStamp = Time.time, TTL = comboState ? m_comboTTL: m_TTL };
           
            if (m_data.Count < m_capacity)
            {
                LinkedListNode<KeyValuePair> node = m_data.AddFirst(newData);

                m_index.Add(key, node);
            }
            else
            {
                var dataNode = RemoveLast();
                dataNode.Value = newData;
                m_index.Add(key, dataNode);
                m_data.AddFirst(dataNode);
            }
            return value;

        }

        private bool TrtGetFromeDict(TKey key, out TValue value,bool comboState = false)
        {
            LinkedListNode<KeyValuePair> dataNode;
            if (m_index.TryGetValue(key, out dataNode))
            {
                value = dataNode.Value.Value;
                dataNode.Value = new KeyValuePair { Key = key, Value = value, TimeStamp = Time.time, TTL = comboState ? m_comboTTL : m_TTL};              
                if (dataNode.Previous != null)
                {
                    m_data.Remove(dataNode);
                    m_data.AddFirst(dataNode);
                }
                if (EnableDebug)
                    m_debugTool.OnTryGet(key, true);
                return true;
            }
            if (EnableDebug)
                m_debugTool.OnTryGet(key, false);
            value = default(TValue);
            return false;
        }
     
        private LinkedListNode<KeyValuePair> RemoveLast()
        {
            LinkedListNode<KeyValuePair> last = m_data.Last;
            m_data.RemoveLast();
            m_index.Remove(last.Value.Key);
            OnEliminate(last.Value.Key, last.Value.Value);
            if (EnableDebug)
                m_debugTool.OnCacheItemRemoved(last.Value.Key);
            return last;
        }

        private bool UpdateValue(TKey key, TValue value, bool comboState = false)
        {
            LinkedListNode<KeyValuePair> dataNode;
            if (m_index.TryGetValue(key, out dataNode))
            {
                if (dataNode.Previous != null)
                {
                    m_data.Remove(dataNode);
                    m_data.AddFirst(dataNode);
                }
                dataNode.Value = new KeyValuePair { Key = key, Value = value, TimeStamp = Time.time, TTL = comboState ? m_comboTTL : m_TTL };
            }
            return false;
        }

        [LuaInterface.NoToLua]
        public void LateUpdate()
        {
            float curTime = Time.time;

            while(m_data.Last != null && m_data.Last.Value.TimeStamp + m_data.Last.Value.TTL < curTime)
            {
                if (EnableDebug)
                    m_debugTool.OutTimeTimes++;

                RemoveLast();
            }
        }

        public void Release()
        {
            if (EnableDebug)
                DebugLog(m_tag);
            UpdatableRunner.Instance.RemoveLateUpdate(this);
            LinkedListNode<KeyValuePair> first = m_data.First;
            while(first != null)
            {
                OnEliminate(first.Value.Key, first.Value.Value);
                first = first.Next;
            }
            m_index = null;
            m_data = null;
            m_capacity = 0;
            OnEliminate = null;
        }

        #region debug
        public void DebugLog(string s)
        {
            if (EnableDebug)
            {
                string str = string.Format("{0} {1} capacity = {2} survive time = {3}.", s, m_tag, m_capacity, m_TTL);
                m_debugTool.Log(str);
            }
        }

        public void LogAllAsset(string head = null)
        {
            string s = string.Format("{0}LRUCacheWithTTL, tag = {1} 资源列表：\n", head, m_tag);
            Dictionary<TKey, LinkedListNode<KeyValuePair>>.Enumerator iter = m_index.GetEnumerator();
            while (iter.MoveNext())
            {
                s = s + iter.Current.Key + "\n";
            }
            Debug.Log(s);
        }

        public void ReleaseUnuseAsset()
        {
            LinkedListNode<KeyValuePair> first = m_data.First;
            while (first != null)
            {
                OnEliminate(first.Value.Key, first.Value.Value);
                first = first.Next;
            }
            m_data.Clear();
            m_index.Clear();
        }

        public void Remove(TKey key)
        {
            if (m_index.ContainsKey(key))
            {
                LinkedListNode<KeyValuePair> curNode = m_index[key];
                m_index.Remove(key);
                m_data.Remove(curNode);
            }
        }

        [LuaInterface.NoToLua]
        public void RegiestEvent(Action<TKey, TValue> OnEliminate)
        {
           
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
                    m_debugTool = new LRUTTLDebugTool<TKey>(CacheDebugTool<TKey>.CacheTypeEnum.LRUTTL, m_capacity * KeyLogCapacityCoefficient);
                    m_enableDebug = true;
                }
            }
        }
        public uint KeyLogCapacityCoefficient = 10;
        private bool m_enableDebug = false;

        public LRUTTLDebugTool<TKey> m_debugTool;
        #endregion

        #region private
        struct KeyValuePair
        {
            public TKey Key;
            public TValue Value;
            public float TimeStamp; //second
            public float TTL; //second
            public bool PVPAdd;
        }
        private LinkedList<KeyValuePair> m_data;
        private Dictionary<TKey, LinkedListNode<KeyValuePair>> m_index;
        private uint m_capacity;
        private float m_TTL;
        private float m_comboTTL;
        #endregion
    }
}
