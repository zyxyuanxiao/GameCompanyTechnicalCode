namespace LogSystem
{
    using System;
    using System.IO;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct rdtTcpMessageGetComponents : rdtTcpMessage
    {
        public int m_instanceId;
        public void Write(BinaryWriter w)
        {
            w.Write(this.m_instanceId);
        }

        public void Read(BinaryReader r)
        {
            this.m_instanceId = r.ReadInt32();
        }
    }
}

