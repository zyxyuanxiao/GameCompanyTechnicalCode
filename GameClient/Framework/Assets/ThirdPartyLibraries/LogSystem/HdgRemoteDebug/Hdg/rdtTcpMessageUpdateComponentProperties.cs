using System.Collections.Generic;
using System.IO;

namespace LogSystem
{
    public struct rdtTcpMessageUpdateComponentProperties : rdtTcpMessage
    {
        public int                                    m_gameObjectInstanceId;
        public int                                    m_componentInstanceId;
        public string                                 m_componentName;
        public bool                                   m_enabled;
        public int                                    m_arrayIndex;
        public List<rdtTcpMessageComponents.Property> m_properties;

        public void Write(BinaryWriter w)
        {
            w.Write(this.m_gameObjectInstanceId);
            w.Write(this.m_componentInstanceId);
            w.Write(this.m_componentName);
            w.Write(this.m_enabled);
            w.Write(this.m_arrayIndex);
            rdtTcpMessageComponents.Component.WriteProperties(w, this.m_properties);
        }

        public void Read(BinaryReader r)
        {
            this.m_gameObjectInstanceId = r.ReadInt32();
            this.m_componentInstanceId  = r.ReadInt32();
            this.m_componentName        = r.ReadString();
            this.m_enabled              = r.ReadBoolean();
            this.m_arrayIndex           = r.ReadInt32();
            this.m_properties           = rdtTcpMessageComponents.Component.ReadProperties(r);
        }
    }
}