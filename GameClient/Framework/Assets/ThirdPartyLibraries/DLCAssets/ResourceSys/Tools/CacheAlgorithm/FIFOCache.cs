using System.Collections.Generic;
using System;


namespace Best
{
    public class FIFOCache<TKey, TValue> : IEliminateCache<TKey, TValue>
    {
        public event Action<TKey, TValue> OnEliminate = delegate { };

        public FIFOCache(uint capacity)
        {
            m_capacity = capacity;
            m_keyQueue = new Queue<TKey>();
            m_dict = new Dictionary<TKey, TValue>();
        }


        public bool TryGet(TKey key, out TValue value)
        {
            return m_dict.TryGetValue(key, out value);
        }

        public TValue Add(TKey key, TValue value, bool updateExists = true)
        {
            TValue prev;
            if(m_dict.TryGetValue(key, out prev))
            {
                if (updateExists)
                {
                    m_dict[key] = value;
                    return value;
                }
                return prev;
            }

            if(m_dict.Count >= m_capacity)
            {
                RemoveFirst();
            }

            m_dict.Add(key, value);
            m_keyQueue.Enqueue(key);
            return value;
        }

        public void Release()
        {
            while (m_dict.Count > 0)
            {
                RemoveFirst();
            }

            m_keyQueue = null;
            m_dict = null;
            m_capacity = 0;
        }


        private void RemoveFirst()
        {
            TKey k = m_keyQueue.Dequeue();
            TValue v = m_dict[k];
            m_dict.Remove(k);
            OnEliminate(k, v);
        }

        public void DebugLog(string s)
        {
            throw new NotImplementedException();
        }

        public void LogAllAsset(string head = null)
        {
        }

        public void ReleaseUnuseAsset()
        {
        }

        public void Remove(TKey key)
        {
            
        }

        public void RegiestEvent(Action<TKey, TValue> OnEliminated)
        {
            
        }

        public bool TryGetNoUpdate(TKey key, out TValue value)
        {
            value = default(TValue);
            return false;
        }

        public bool EnableDebug
        {
            get
            {
                return m_enableDebug;
            }
            set
            {
                m_enableDebug = value;
            }
        }

        public uint KeyLogCapacityCoefficient = 10;
        private bool m_enableDebug = false;

        public LRUKDebugTool<TKey> m_debugTool;

        private uint m_capacity;
        private Queue<TKey> m_keyQueue;
        private Dictionary<TKey, TValue> m_dict;
    }

}
