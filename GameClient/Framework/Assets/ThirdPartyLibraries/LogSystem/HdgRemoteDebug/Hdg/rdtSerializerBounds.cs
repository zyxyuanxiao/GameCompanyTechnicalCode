using System.IO;
using UnityEngine;

namespace LogSystem
{
    public class rdtSerializerBounds : rdtSerializerInterface
    {
        private rdtSerializerVector3 centre;
        private rdtSerializerVector3 size;

        public rdtSerializerBounds()
        {
        }

        public rdtSerializerBounds(Bounds b)
        {
            this.centre = new rdtSerializerVector3(b.center);
            this.size   = new rdtSerializerVector3(b.size);
        }

        public Bounds ToUnityType()
        {
            return new Bounds(this.centre.ToUnityType(), this.size.ToUnityType());
        }

        public object Deserialize(rdtSerializerRegistry registry)
        {
            return (object) this.ToUnityType();
        }

        public void Write(BinaryWriter w)
        {
            this.centre.Write(w);
            this.size.Write(w);
        }

        public void Read(BinaryReader r)
        {
            this.centre = new rdtSerializerVector3();
            this.centre.Read(r);
            this.size = new rdtSerializerVector3();
            this.size.Read(r);
        }
    }
}