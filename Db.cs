using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ZeroPass
{
    public class Db : EntityModifierSet
    {
        [Serializable]
        public class SlotInfo : Resource
        {
        }

        private static Db _Instance;
        
        public Database.AssignableSlots AssignableSlots;
        public Database.Amounts Amounts;
        public Database.AmountSets AmountSets;
        public Database.GameAbilityAssets GameAbilityAssets;
        public Database.GameEffectAssets GameEffectAssets;
        
        
        public static Db Get()
        {
            if (_Instance == null)
            {
                _Instance = CreateInstance<Db>();
                _Instance.Initialize();
            }
            return _Instance;
        }

        public override void Initialize()
        {
            base.Initialize();
            // Load assets
            AssignableSlots = new Database.AssignableSlots();
            Amounts = new Database.Amounts(Root);
            AmountSets = new Database.AmountSets(Root);
            GameAbilityAssets = new Database.GameAbilityAssets(Root);
            GameEffectAssets = new Database.GameEffectAssets(Root);
            
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
            Resource resource = ResourceTable.FirstOrDefault(s => s.Guid == guid);
            if (resource == null)
            {
                Debug.LogWarning("Could not find resource: " + guid);
                return null;
            }
            ResourceType val = (ResourceType)resource;
            return val;
        }
    }
}
