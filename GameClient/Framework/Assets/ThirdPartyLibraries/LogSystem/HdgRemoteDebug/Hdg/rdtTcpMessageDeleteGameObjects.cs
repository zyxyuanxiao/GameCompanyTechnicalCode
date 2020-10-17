using System.Collections.Generic;
using System.IO;

namespace LogSystem
{
    public struct rdtTcpMessageDeleteGameObjects : rdtTcpMessage
    {
        public List<int> m_instanceIds;

        public void Write(BinaryWriter w)
        {
            int count = this.m_instanceIds.Count;
            w.Write(count);
            for (int index = 0; index < count; ++index)
                w.Write(this.m_instanceIds[index]);
        }

        public void Read(BinaryReader r)
        {
            int capacity = r.ReadInt32();
            this.m_instanceIds = new List<int>(capacity);
            for (int index = 0; index < capacity; ++index)
                this.m_instanceIds.Add(r.ReadInt32());
        }
    }
}