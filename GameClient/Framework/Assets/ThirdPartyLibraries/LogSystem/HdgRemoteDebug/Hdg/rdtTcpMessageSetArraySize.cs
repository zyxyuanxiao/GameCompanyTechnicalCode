using System.Collections.Generic;
using System.IO;

namespace LogSystem
{
    public struct rdtTcpMessageSetArraySize : rdtTcpMessage
    {
        public int                                    m_gameObjectInstanceId;
        public int                                    m_componentInstanceId;
        public string                                 m_componentName;
        public int                                    m_size;
        public List<rdtTcpMessageComponents.Property> m_properties;

        public void Write(BinaryWriter w)
        {
            w.Write(this.m_gameObjectInstanceId);
            w.Write(this.m_componentInstanceId);
            w.Write(this.m_componentName);
            w.Write(this.m_size);
            rdtTcpMessageComponents.Component.WriteProperties(w, this.m_properties);
        }

        public void Read(BinaryReader r)
        {
            this.m_gameObjectInstanceId = r.ReadInt32();
            this.m_componentInstanceId  = r.ReadInt32();
            this.m_componentName        = r.ReadString();
            this.m_size                 = r.ReadInt32();
            this.m_properties           = rdtTcpMessageComponents.Component.ReadProperties(r);
        }
    }
}