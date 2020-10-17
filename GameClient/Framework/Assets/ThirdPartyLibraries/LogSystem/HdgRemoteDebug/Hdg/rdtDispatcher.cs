using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;


namespace LogSystem
{
    public class rdtDispatcher
    {
        // private ConcurrentQueue<Action> m_callbacks = new ConcurrentQueue<Action>();
        private Queue<Action> m_callbacks = new Queue<Action>();

        public void Clear()
        {
            lock (this.m_callbacks)
                this.m_callbacks.Clear();
        }

        public void Enqueue(Action action)
        {
            lock (this.m_callbacks)
                this.m_callbacks.Enqueue(action);
        }

        public void Update()
        {
            lock (this.m_callbacks)
            {
                while (this.m_callbacks.Count > 0)
                    this.m_callbacks.Dequeue()();
            }
        }
    }
}