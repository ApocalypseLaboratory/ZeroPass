using System;
using UnityEngine;

namespace ZeroPass
{
    public static class EventExtensions
    {
        public static int Subscribe(this GameObject go, int hash, Action<object> handler)
        {
            if (!Application.isPlaying)
                return 0;
            
            RMonoBehaviour component = go.GetComponent<RMonoBehaviour>();
            return component.Subscribe(hash, handler);
        }

        public static void Subscribe(this GameObject go, GameObject target, int hash, Action<object> handler)
        {
            if (!Application.isPlaying)
                return;
            
            RMonoBehaviour component = go.GetComponent<RMonoBehaviour>();
            component.Subscribe(target, hash, handler);
        }

        public static void Unsubscribe(this GameObject go, int hash, Action<object> handler)
        {
            if (!Application.isPlaying)
                return;
            
            RMonoBehaviour component = go.GetComponent<RMonoBehaviour>();
            if (component != null)
            {
                component.Unsubscribe(hash, handler);
            }
        }

        public static void Unsubscribe(this GameObject go, int id)
        {
            if (!Application.isPlaying)
                return;
            
            RMonoBehaviour component = go.GetComponent<RMonoBehaviour>();
            if (component != null)
            {
                component.Unsubscribe(id);
            }
        }

        public static void Unsubscribe(this GameObject go, GameObject target, int hash, Action<object> handler)
        {
            if (!Application.isPlaying)
                return;
            
            RMonoBehaviour component = go.GetComponent<RMonoBehaviour>();
            if (component != null)
            {
                component.Unsubscribe(target, hash, handler);
            }
        }

        public static void Trigger(this GameObject go, int hash, object data = null)
        {
            if (!Application.isPlaying)
                return;
            
            RObject kObject = RObjectManager.Instance.Get(go);
            if (kObject != null && kObject.hasEventSystem)
            {
                kObject.GetEventSystem().Trigger(go, hash, data);
            }
        }
    }
}