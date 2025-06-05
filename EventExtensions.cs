using System;
using UnityEngine;

namespace ZeroPass
{
    public static class EventExtensions
    {
        public static int Subscribe(this GameObject go, int hash, Action<object> handler)
        {
            RMonoBehaviour component = go.GetComponent<RMonoBehaviour>();
            return component.Subscribe(hash, handler);
        }

        public static void Subscribe(this GameObject go, GameObject target, int hash, Action<object> handler)
        {
            RMonoBehaviour component = go.GetComponent<RMonoBehaviour>();
            component.Subscribe(target, hash, handler);
        }

        public static void Unsubscribe(this GameObject go, int hash, Action<object> handler)
        {
            RMonoBehaviour component = go.GetComponent<RMonoBehaviour>();
            if (component != null)
            {
                component.Unsubscribe(hash, handler);
            }
        }

        public static void Unsubscribe(this GameObject go, int id)
        {
            RMonoBehaviour component = go.GetComponent<RMonoBehaviour>();
            if (component != null)
            {
                component.Unsubscribe(id);
            }
        }

        public static void Unsubscribe(this GameObject go, GameObject target, int hash, Action<object> handler)
        {
            RMonoBehaviour component = go.GetComponent<RMonoBehaviour>();
            if (component != null)
            {
                component.Unsubscribe(target, hash, handler);
            }
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