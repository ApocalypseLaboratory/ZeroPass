using System.IO;
using UnityEngine;

namespace ZeroPass.Serialization
{
    public interface ISerializableComponentManager : IComponentManager
    {
        void Serialize(GameObject go, BinaryWriter writer);

        void Deserialize(GameObject go, IReader reader);
    }

}