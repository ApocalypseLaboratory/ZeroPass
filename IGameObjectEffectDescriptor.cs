using System.Collections.Generic;
using UnityEngine;

namespace ZeroPass
{
    public interface IGameObjectEffectDescriptor
    {
        List<Descriptor> GetDescriptors(GameObject go);
    }
}
