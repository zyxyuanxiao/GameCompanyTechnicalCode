using System.IO;

namespace LogSystem
{
    public interface rdtSerializerInterface
    {
        object Deserialize(rdtSerializerRegistry registry);

        void Write(BinaryWriter w);

        void Read(BinaryReader r);
    }
}