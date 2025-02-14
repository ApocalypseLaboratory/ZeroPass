using System.Collections.Generic;
using UnityEngine;

namespace ZeroPass
{
    public class ModifierSet : ScriptableObject
    {
        public class ModifierInfo : Resource
        {
            public string Type;

            public string Attribute;

            public float Value;

        }


        public TextAsset modifiersFile;

        public ResourceSet Root;

        public List<Resource> ResourceTable;

        public virtual void Initialize()
        {
            ResourceTable = new List<Resource>();
            Root = new ResourceSet<Resource>("Root", null);
        }
    }
}
