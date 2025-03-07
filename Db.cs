using System;
using System.Collections.Generic;
using System.Linq;
using Database;

namespace ZeroPass
{
    public class Db : EntityModifierSet
    {
        [Serializable]
        public class SlotInfo : Resource
        {
        }

        private static Db _Instance;
        
        public AssignableSlots AssignableSlots;

        public static Db Get()
        {
            if ((UnityEngine.Object)_Instance == (UnityEngine.Object)null)
            {
                // Load Db Assets
                //_Instance = Resources.Load<Db>("Db");
                _Instance.Initialize();
            }
            return _Instance;
        }

        public override void Initialize()
        {
            base.Initialize();
            // Load assets
            CollectResources(Root, ResourceTable);
        }

        private void CollectResources(Resource resource, List<Resource> resource_table)
        {
            if (resource.Guid != (ResourceGuid)null)
            {
                resource_table.Add(resource);
            }
            ResourceSet resourceSet = resource as ResourceSet;
            if (resourceSet != null)
            {
                for (int i = 0; i < resourceSet.Count; i++)
                {
                    CollectResources(resourceSet.GetResource(i), resource_table);
                }
            }
        }

        public ResourceType GetResource<ResourceType>(ResourceGuid guid) where ResourceType : Resource
        {
            Resource resource = ResourceTable.FirstOrDefault((Resource s) => s.Guid == guid);
            if (resource == null)
            {
                Debug.LogWarning("Could not find resource: " + guid);
                return (ResourceType)null;
            }
            ResourceType val = (ResourceType)resource;
            if (val == null)
            {
                Debug.LogError("Resource type mismatch for resource: " + resource.Id + "\nExpecting Type: " + typeof(ResourceType).Name + "\nGot Type: " + resource.GetType().Name);
                return (ResourceType)null;
            }
            return val;
        }
    }
}
