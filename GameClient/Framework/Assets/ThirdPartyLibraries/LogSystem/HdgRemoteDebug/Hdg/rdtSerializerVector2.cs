namespace LogSystem
{
    using System;
    using System.IO;
    using UnityEngine;

    public class rdtSerializerVector2 : rdtSerializerInterface
    {
        public float x;
        public float y;

        public rdtSerializerVector2()
        {
        }

        public rdtSerializerVector2(Vector2 v)
        {
            this.x = v.x;
            this.y = v.y;
        }

        public object Deserialize(rdtSerializerRegistry registry) => 
            this.ToUnityType();

        public void Read(BinaryReader r)
        {
            this.x = r.ReadSingle();
            this.y = r.ReadSingle();
        }

        public Vector2 ToUnityType() => 
            new Vector2(this.x, this.y);

        public void Write(BinaryWriter w)
        {
            w.Write(this.x);
            w.Write(this.y);
        }
    }
}

