using System.IO;

namespace LogSystem
{
    public class rdtSerializerButton : rdtSerializerInterface
    {
        public bool Pressed;

        public rdtSerializerButton()
        {
        }

        public rdtSerializerButton(bool inpressed)
        {
            this.Pressed = inpressed;
        }

        public object Deserialize(rdtSerializerRegistry registry)
        {
            return (object) this;
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as rdtSerializerButton);
        }

        public bool Equals(rdtSerializerButton p)
        {
            return p != null && this.Pressed == p.Pressed;
        }

        public override int GetHashCode()
        {
            return this.Pressed.GetHashCode();
        }

        public void Write(BinaryWriter w)
        {
            w.Write(this.Pressed);
        }

        public void Read(BinaryReader r)
        {
            this.Pressed = r.ReadBoolean();
        }
    }
}