using System;
using UnityEngine;

namespace LogSystem
{
    internal class rdtProfiler : IDisposable
    {
        private DateTime m_start;
        private string   m_description;

        public rdtProfiler(string desc)
        {
            this.m_start       = DateTime.Now;
            this.m_description = desc;
        }

        public void Dispose()
        {
            Debug.Log((object) string.Format("{0} took {1}s", (object) this.m_description, (object) (DateTime.Now - this.m_start).TotalSeconds));
        }
    }
}