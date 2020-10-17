using System.IO;

namespace LogSystem
{
    public class rdtSerializerSlider : rdtSerializerInterface
    {
        public float Value;
        public float LimitMin;
        public float LimitMax;

        public rdtSerializerSlider()
        {
        }

        public rdtSerializerSlider(float invalue, float inmin, float inmax)
        {
            this.Value    = invalue;
            this.LimitMin = inmin;
            this.LimitMax = inmax;
        }

        public object Deserialize(rdtSerializerRegistry registry)
        {
            return (object) this;
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as rdtSerializerSlider);
        }

        public bool Equals(rdtSerializerSlider p)
        {
            return p != null && (double) this.Value == (double) p.Value && (double) this.LimitMin == (double) p.LimitMin && (double) this.LimitMax == (double) p.LimitMax;
        }

        public override int GetHashCode()
        {
            return this.Value.GetHashCode() ^ this.LimitMin.GetHashCode() ^ this.LimitMax.GetHashCode();
        }

        public void Write(BinaryWriter w)
        {
            w.Write(this.Value);
            w.Write(this.LimitMin);
            w.Write(this.LimitMax);
        }

        public void Read(BinaryReader r)
        {
            this.Value    = r.ReadSingle();
            this.LimitMin = r.ReadSingle();
            this.LimitMax = r.ReadSingle();
        }
    }
}