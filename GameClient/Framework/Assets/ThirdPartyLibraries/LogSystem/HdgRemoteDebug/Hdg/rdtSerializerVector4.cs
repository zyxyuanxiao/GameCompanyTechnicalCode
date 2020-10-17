namespace LogSystem
{
    using System;
    using System.IO;
    using UnityEngine;

    public class rdtSerializerVector4 : rdtSerializerInterface
    {
        public float x;
        public float y;
        public float z;
        public float w;

        public rdtSerializerVector4()
        {
        }

        public rdtSerializerVector4(Vector4 v)
        {
            this.x = v.x;
            this.y = v.y;
            this.z = v.z;
            this.w = v.w;
        }

        public object Deserialize(rdtSerializerRegistry registry) => 
            this.ToUnityType();

        public void Read(BinaryReader r)
        {
            this.x = r.ReadSingle();
            this.y = r.ReadSingle();
            this.z = r.ReadSingle();
            this.w = r.ReadSingle();
        }

        public Vector4 ToUnityType() => 
            new Vector4(this.x, this.y, this.z, this.w);

        public void Write(BinaryWriter bw)
        {
            bw.Write(this.x);
            bw.Write(this.y);
            bw.Write(this.z);
            bw.Write(this.w);
        }
    }
}

