using System.IO;
using UnityEngine;

namespace LogSystem
{
    public class rdtSerializerQuaternion : rdtSerializerInterface
    {
        public float x;
        public float y;
        public float z;
        public float w;

        public rdtSerializerQuaternion()
        {
        }

        public rdtSerializerQuaternion(Quaternion v)
        {
            this.x = v.x;
            this.y = v.y;
            this.z = v.z;
            this.w = v.w;
        }

        public object ToUnityType()
        {
            return (object) new Quaternion(this.x, this.y, this.z, this.w);
        }

        public object Deserialize(rdtSerializerRegistry registry)
        {
            return this.ToUnityType();
        }

        public void Write(BinaryWriter bw)
        {
            bw.Write(this.x);
            bw.Write(this.y);
            bw.Write(this.z);
            bw.Write(this.w);
        }

        public void Read(BinaryReader r)
        {
            this.x = r.ReadSingle();
            this.y = r.ReadSingle();
            this.z = r.ReadSingle();
            this.w = r.ReadSingle();
        }
    }
}