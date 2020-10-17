namespace LogSystem
{
    using System;
    using System.IO;

    public interface rdtTcpMessage
    {
        void Read(BinaryReader r);
        void Write(BinaryWriter w);
    }
}

