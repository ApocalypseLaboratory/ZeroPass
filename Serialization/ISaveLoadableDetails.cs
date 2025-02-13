using System.IO;

namespace ZeroPass.Serialization
{
    public interface ISaveLoadableDetails
    {
        void Serialize(BinaryWriter writer);

        void Deserialize(IReader reader);
    }
}