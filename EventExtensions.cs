using UnityEngine;

namespace ZeroPass
{
    public static class EventExtensions
    {
        public static int Subscribe<ComponentType>(this GameObject go, int hash, EventSystem.IntraObjectHandler<ComponentType> handler)
        {
            return RObjectManager.Instance.GetOrCreateObject(go).GetEventSystem().Subscribe(hash, handler);
        }

        public static void Trigger(this GameObject go, int hash, object data = null)
        {
            RObject kObject = RObjectManager.Instance.Get(go);
            if (kObject != null && kObject.hasEventSystem)
            {
                kObject.GetEventSystem().Trigger(go, hash, data);
            }
        }
    }
}