using System.IO;
using UnityEngine;

namespace LogSystem
{
    public struct rdtTcpMessageLog : rdtTcpMessage
    {
        public string  m_message;
        public string  m_stackTrace;
        public LogType m_logType;

        public rdtTcpMessageLog(string message, string stackTrace, LogType logType)
        {
            this.m_message    = message;
            this.m_stackTrace = stackTrace;
            this.m_logType    = logType;
        }

        public override string ToString()
        {
            string str = this.m_message;
            if (!string.IsNullOrEmpty(this.m_stackTrace))
                str = str + "\n" + this.m_stackTrace;
            return str;
        }

        public void Write(BinaryWriter w)
        {
            w.Write(this.m_message);
            w.Write(this.m_stackTrace);
            w.Write((int) this.m_logType);
        }

        public void Read(BinaryReader r)
        {
            this.m_message    = r.ReadString();
            this.m_stackTrace = r.ReadString();
            this.m_logType    = (LogType) r.ReadInt32();
        }
    }
}