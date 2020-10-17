using System.IO;
using UnityEngine;

namespace LogSystem
{
    public class rdtSerializerColor32 : rdtSerializerInterface
    {
        public byte r;
        public byte g;
        public byte b;
        public byte a;

        public rdtSerializerColor32()
        {
        }

        public rdtSerializerColor32(Color32 c)
        {
            this.r = c.r;
            this.g = c.g;
            this.b = c.b;
            this.a = c.a;
        }

        public Color32 ToUnityType()
        {
            return new Color32(this.r, this.g, this.b, this.a);
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
            this.r = br.ReadByte();
            this.g = br.ReadByte();
            this.b = br.ReadByte();
            this.a = br.ReadByte();
        }
    }
}