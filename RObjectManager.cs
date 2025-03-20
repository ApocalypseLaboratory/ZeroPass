using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ZeroPass
{
    public class RObjectManager : MonoBehaviour
    {
        private Dictionary<int, RObject> objects = new Dictionary<int, RObject>();

        private List<int> pendingDestroys = new List<int>();

        private static RObjectManager _instance;

        public static RObjectManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    var gameObject = new GameObject(nameof(RObjectManager));
                    DontDestroyOnLoad(gameObject);
                    _instance = gameObject.AddComponent<RObjectManager>();
                }

                return _instance;
            }
        }

        public static void DestroyInstance()
        {
            _instance = null;
        }
        

        private void OnDestroy()
        {
            Cleanup();
            _instance = null;
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