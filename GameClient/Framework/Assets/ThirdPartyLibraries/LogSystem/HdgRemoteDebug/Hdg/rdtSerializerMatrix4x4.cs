using System.IO;
using UnityEngine;

namespace LogSystem
{
    public class rdtSerializerMatrix4x4 : rdtSerializerInterface
    {
        private rdtSerializerVector4 col0;
        private rdtSerializerVector4 col1;
        private rdtSerializerVector4 col2;
        private rdtSerializerVector4 col3;

        public rdtSerializerMatrix4x4()
        {
        }

        public rdtSerializerMatrix4x4(Matrix4x4 m)
        {
            this.col0 = new rdtSerializerVector4(m.GetColumn(0));
            this.col1 = new rdtSerializerVector4(m.GetColumn(1));
            this.col2 = new rdtSerializerVector4(m.GetColumn(2));
            this.col3 = new rdtSerializerVector4(m.GetColumn(3));
        }

        public Matrix4x4 ToUnityType()
        {
            Matrix4x4 matrix4x4 = new Matrix4x4();
            matrix4x4.SetColumn(0, this.col0.ToUnityType());
            matrix4x4.SetColumn(1, this.col1.ToUnityType());
            matrix4x4.SetColumn(2, this.col2.ToUnityType());
            matrix4x4.SetColumn(3, this.col3.ToUnityType());
            return matrix4x4;
        }

        public object Deserialize(rdtSerializerRegistry registry)
        {
            return (object) this.ToUnityType();
        }

        public void Write(BinaryWriter w)
        {
            this.col0.Write(w);
            this.col1.Write(w);
            this.col2.Write(w);
            this.col3.Write(w);
        }

        public void Read(BinaryReader r)
        {
            this.col0 = new rdtSerializerVector4();
            this.col0.Read(r);
            this.col1 = new rdtSerializerVector4();
            this.col1.Read(r);
            this.col2 = new rdtSerializerVector4();
            this.col2.Read(r);
            this.col3 = new rdtSerializerVector4();
            this.col3.Read(r);
        }
    }
}