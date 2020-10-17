using System.IO;
using UnityEngine;

namespace LogSystem
{
    public class rdtSerializerColor : rdtSerializerInterface
    {
        public float r;
        public float g;
        public float b;
        public float a;

        public rdtSerializerColor()
        {
        }

        public rdtSerializerColor(Color c)
        {
            this.r = c.r;
            this.g = c.g;
            this.b = c.b;
            this.a = c.a;
        }

        public Color ToUnityType()
        {
            return new Color(this.r, this.g, this.b, this.a);
        }

        public object Deserialize(rdtSerializerRegistry registry)
        {
            return (object) this.ToUnityType();
        }

        public void Write(BinaryWriter w)
        {
            w.Write(this.r);
            w.Write(this.g);
            w.Write(this.b);
            w.Write(this.a);
        }

        public void Read(BinaryReader br)
        {
            this.r = br.ReadSingle();
            this.g = br.ReadSingle();
            this.b = br.ReadSingle();
            this.a = br.ReadSingle();
        }
    }
}