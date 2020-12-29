using System.Collections.Generic;
using System;

namespace Best
{
    /**
     * Least Recently Used Cache.最近最少使用的缓存。
     */
    public class LRUCache<TKey, TValue> : IEliminateCache<TKey, TValue>
    {
        public event Action<TKey, TValue> OnEliminate = delegate { };
        string m_tag;

        public LRUCache(string tag, uint capacity)
        {
            m_data = new LinkedList<KeyValuePair>();
            m_index = new Dictionary<TKey, LinkedListNode<KeyValuePair>>();
            m_capacity = capacity;
            m_tag = tag;
        }

        /// <summary>
        /// 重新设置容量
        /// </summary>
        /// <param name="newCapacity"></param>
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
            LinkedListNode<KeyValuePair> dataNode;
            if (m_index.TryGetValue(key, out dataNode))
            {
                if (dataNode.Previous != null)
                {
                    m_data.Remove(dataNode);
                    m_data.AddFirst(dataNode);
                }
                value = dataNode.Value.Value;
                if (EnableDebug)
                    m_debugTool.OnTryGet(key, true);

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
           LinkedListNode<KeyValuePair> dataNode;
            if (m_index.TryGetValue(key, out dataNode))
            {              
                value = dataNode.Value.Value;              
                return true;
            }        
            value = default(TValue);
            return false;
        }

        public TValue Add(TKey key, TValue value, bool updateExists = true)
        {
            if (m_index.ContainsKey(key))
            {
                if (updateExists)
                {
                    UpdateValue(key, value);
                    return value;
                }
                else
                {
                    return m_index[key].Value.Value;
                }
            }

            KeyValuePair newData = new KeyValuePair { Key = key, Value = value };
            if (m_data.Count < m_capacity)
            {
                m_index.Add(key, m_data.AddFirst(newData));
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
    
        public bool UpdateValue(TKey key, TValue value)
        {
            LinkedListNode<KeyValuePair> dataNode;
            if (m_index.TryGetValue(key, out dataNode))
            {
                if (dataNode.Previous != null)
                {
                    m_data.Remove(dataNode.Value);
                    m_data.AddFirst(dataNode.Value);
                }
                dataNode.Value = new KeyValuePair { Key = key, Value = value };
            }

            return false;
        }

        public void Release()
        {
            if (EnableDebug)
                DebugLog(m_tag);
            while (m_data.Count > 0)
                RemoveLast();
            m_data = null;
            m_index = null;
            m_capacity = 0;
            OnEliminate = null;
        }
        [LuaInterface.NoToLua]
        public void Remove(TKey key)
        {
            if (m_index.ContainsKey(key))
            {
                LinkedListNode<KeyValuePair> dataNode = m_index[key];
                m_data.Remove(dataNode.Value);
                m_index.Remove(key);
            }
        }

        [LuaInterface.NoToLua]
        public void RegiestEvent(Action<TKey, TValue> OnEliminated)
        {

        }
        #region private
        struct KeyValuePair
        {
            public TKey Key;
            public TValue Value;
        }
        private LinkedList<KeyValuePair> m_data;
        private Dictionary<TKey, LinkedListNode<KeyValuePair>> m_index;
        private uint m_capacity;

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

        #endregion

        #region Debug Interface
        public void DebugLog(string s)
        {
            if (EnableDebug)
            {
                string str = string.Format("{0} {1} capacity = {2}.", s, m_tag, m_capacity);
                m_debugTool.Log(str);
            }
        }

        public void LogAllAsset(string head = null)
        {
            string s = string.Format("{0}LRUCache, tag = {1} 资源列表：\n", head, m_tag);
            Dictionary<TKey, LinkedListNode<KeyValuePair>>.Enumerator iter = m_index.GetEnumerator();
            while (iter.MoveNext())
            {
                s = s + iter.Current.Key + "\n";
            }
            UnityEngine.Debug.Log(s);
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
                    m_debugTool = new LRUDebugTool<TKey>(CacheDebugTool<TKey>.CacheTypeEnum.LRU, m_capacity * KeyLogCapacityCoefficient);
                    m_enableDebug = true;
                }
            }
        }
        public uint KeyLogCapacityCoefficient = 10;
        private bool m_enableDebug = false;

        public LRUDebugTool<TKey> m_debugTool;
        #endregion
    }
}
