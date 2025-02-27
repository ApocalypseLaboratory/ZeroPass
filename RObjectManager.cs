using System.Collections.Generic;
using UnityEngine;

namespace ZeroPass
{
    public class RObjectManager : MonoBehaviour
    {
        private Dictionary<int, RObject> objects = new Dictionary<int, RObject>();

        private List<int> pendingDestroys = new List<int>();

        public static RObjectManager Instance
        {
            get;
            private set;
        }

        public static void DestroyInstance()
        {
            Instance = null;
        }

        private void Awake()
        {
            Debug.Assert((Object)Instance == (Object)null);
            Instance = this;
        }

        private void OnDestroy()
        {
            Debug.Assert((Object)Instance != (Object)null);
            Debug.Assert((Object)Instance == (Object)this);
            Cleanup();
            Instance = null;
        }

        public void Cleanup()
        {
            foreach (KeyValuePair<int, RObject> @object in objects)
            {
                @object.Value.OnCleanUp();
            }
            objects.Clear();
            pendingDestroys.Clear();
        }

        public RObject GetOrCreateObject(GameObject go)
        {
            int instanceID = go.GetInstanceID();
            RObject value = null;
            if (!objects.TryGetValue(instanceID, out value))
            {
                value = new RObject(go);
                objects[instanceID] = value;
            }
            return value;
        }

        public RObject Get(GameObject go)
        {
            RObject value = null;
            objects.TryGetValue(go.GetInstanceID(), out value);
            return value;
        }

        public void QueueDestroy(RObject obj)
        {
            int id = obj.id;
            if (!pendingDestroys.Contains(id))
            {
                pendingDestroys.Add(id);
            }
        }

        private void LateUpdate()
        {
            for (int i = 0; i < pendingDestroys.Count; i++)
            {
                int key = pendingDestroys[i];
                RObject value = null;
                if (objects.TryGetValue(key, out value))
                {
                    objects.Remove(key);
                    value.OnCleanUp();
                }
            }
            pendingDestroys.Clear();
        }

        public void DumpEventData()
        {
        }
    }
}