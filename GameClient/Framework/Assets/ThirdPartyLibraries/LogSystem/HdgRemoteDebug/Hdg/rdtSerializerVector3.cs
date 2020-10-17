namespace LogSystem
{
    using System;
    using System.IO;
    using UnityEngine;

    public class rdtSerializerVector3 : rdtSerializerInterface
    {
        public float x;
        public float y;
        public float z;

        public rdtSerializerVector3()
        {
        }

        public rdtSerializerVector3(Vector3 v)
        {
            this.x = v.x;
            this.y = v.y;
            this.z = v.z;
        }

        public object Deserialize(rdtSerializerRegistry registry) => 
            this.ToUnityType();

        public void Read(BinaryReader r)
        {
            this.x = r.ReadSingle();
            this.y = r.ReadSingle();
            this.z = r.ReadSingle();
        }

        public Vector3 ToUnityType() => 
            new Vector3(this.x, this.y, this.z);

        public void Write(BinaryWriter w)
        {
            w.Write(this.x);
            w.Write(this.y);
            w.Write(this.z);
        }
    }
}

