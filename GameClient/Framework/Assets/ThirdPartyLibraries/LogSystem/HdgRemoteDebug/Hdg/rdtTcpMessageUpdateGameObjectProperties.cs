using System.IO;

namespace LogSystem
{
    public struct rdtTcpMessageUpdateGameObjectProperties : rdtTcpMessage
    {
        public int                                           m_instanceId;
        public rdtTcpMessageUpdateGameObjectProperties.Flags m_flags;
        public bool                                          m_enabled;
        public string                                        m_tag;
        public int                                           m_layer;

        public void Write(BinaryWriter w)
        {
            w.Write(this.m_instanceId);
            w.Write(this.m_enabled);
            w.Write(this.m_tag);
            w.Write(this.m_layer);
            w.Write((int) this.m_flags);
        }

        public void Read(BinaryReader r)
        {
            this.m_instanceId = r.ReadInt32();
            this.m_enabled    = r.ReadBoolean();
            this.m_tag        = r.ReadString();
            this.m_layer      = r.ReadInt32();
            this.m_flags      = (rdtTcpMessageUpdateGameObjectProperties.Flags) r.ReadInt32();
        }

        public void SetFlag(rdtTcpMessageUpdateGameObjectProperties.Flags flag, bool enabled)
        {
            if (enabled)
                this.m_flags |= flag;
            else
                this.m_flags &= ~flag;
        }

        public bool HasFlag(rdtTcpMessageUpdateGameObjectProperties.Flags flag)
        {
            return (uint) (this.m_flags & flag) > 0U;
        }

        public enum Flags
        {
            UpdateEnabled = 1,
            UpdateTag     = 2,
            UpdateLayer   = 4,
        }
    }
}