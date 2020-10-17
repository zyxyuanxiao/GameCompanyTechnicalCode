using System.IO;
using UnityEngine;

namespace LogSystem
{
    public class rdtSerializerRect : rdtSerializerInterface
    {
        private float x;
        private float y;
        private float width;
        private float height;

        public rdtSerializerRect()
        {
        }

        public rdtSerializerRect(Rect r)
        {
            this.x      = r.x;
            this.y      = r.y;
            this.width  = r.width;
            this.height = r.height;
        }

        public Rect ToUnityType()
        {
            return new Rect(this.x, this.y, this.width, this.height);
        }

        public object Deserialize(rdtSerializerRegistry registry)
        {
            return (object) this.ToUnityType();
        }

        public void Write(BinaryWriter w)
        {
            w.Write(this.x);
            w.Write(this.y);
            w.Write(this.width);
            w.Write(this.height);
        }

        public void Read(BinaryReader r)
        {
            this.x      = r.ReadSingle();
            this.y      = r.ReadSingle();
            this.width  = r.ReadSingle();
            this.height = r.ReadSingle();
        }
    }
}