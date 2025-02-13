using System.IO;

namespace ZeroPass.Serialize
{
    public interface ISaveLoadableDetails
    {
        void Serialize(BinaryWriter writer);

        void Deserialize(IReader reader);
    }
}