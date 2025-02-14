using System.Collections.Generic;

namespace ZeroPass
{
    public class RPrefabIDTracker
    {
        private static RPrefabIDTracker Instance;

        private Dictionary<int, RPrefabID> prefabIdMap = new Dictionary<int, RPrefabID>();

        public static void DestroyInstance()
        {
            Instance = null;
        }

        public static RPrefabIDTracker Get()
        {
            if (Instance == null)
            {
                Instance = new RPrefabIDTracker();
            }
            return Instance;
        }

        public void Register(RPrefabID instance)
        {
            if (instance.InstanceID != -1)
            {
                if (prefabIdMap.ContainsKey(instance.InstanceID))
                {
                    Debug.LogWarningFormat(instance.gameObject, "RPID instance id {0} was previously used by {1} but we're trying to add it from {2}. Conflict!", instance.InstanceID, prefabIdMap[instance.InstanceID].gameObject, instance.name);
                }
                prefabIdMap[instance.InstanceID] = instance;
            }
        }

        public void Unregister(RPrefabID instance)
        {
            prefabIdMap.Remove(instance.InstanceID);
        }

        public RPrefabID GetInstance(int instance_id)
        {
            RPrefabID value = null;
            prefabIdMap.TryGetValue(instance_id, out value);
            return value;
        }
    }
}
