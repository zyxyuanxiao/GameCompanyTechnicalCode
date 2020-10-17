using System.IO;

namespace LogSystem
{
    public class rdtUdpMessageHello
    {
        public string m_deviceName;
        public string m_deviceType;
        public string m_devicePlatform;
        public string m_serverVersion;
        public int    m_serverPort;

        public void Write(BinaryWriter w)
        {
            w.Write(this.m_deviceName);
            w.Write(this.m_deviceType);
            w.Write(this.m_devicePlatform);
            w.Write(this.m_serverVersion);
            w.Write(this.m_serverPort);
        }

        public void Read(BinaryReader r)
        {
            this.m_deviceName     = r.ReadString();
            this.m_deviceType     = r.ReadString();
            this.m_devicePlatform = r.ReadString();
            this.m_serverVersion  = r.ReadString();
            this.m_serverPort     = r.ReadInt32();
        }
    }
}