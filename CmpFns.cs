using System;
using System.Reflection;
using UnityEngine;

namespace ZeroPass
{
    public class CmpFns
    {
        public Func<RMonoBehaviour, Component> mFindOrAddFn;

        public Func<RMonoBehaviour, Component> mFindFn;

        public Func<RMonoBehaviour, Component> mRequireFn;

        public CmpFns(Type type)
        {
            Type[] type_array = new Type[1]
            {
                type
            };
            mFindOrAddFn = GetMethod("FindOrAddComponent", type_array);
            mFindFn = GetMethod("FindComponent", type_array);
            mRequireFn = GetMethod("RequireComponent", type_array);
        }

        public static Component FindComponent<T>(MonoBehaviour c) where T : Component
        {
            return c.FindComponent<T>();
        }

        public static Component RequireComponent<T>(MonoBehaviour c) where T : Component
        {
            return c.RequireComponent<T>();
        }

        public static Component FindOrAddComponent<T>(MonoBehaviour c) where T : Component
        {
            return c.FindOrAddComponent<T>();
        }

        private Func<RMonoBehaviour, Component> GetMethod(string name, Type[] type_array)
        {
            MethodInfo method = typeof(CmpFns).GetMethod(name);
            MethodInfo method2 = null;
            try
            {
                method2 = method.MakeGenericMethod(type_array);
            }
            catch (Exception obj)
            {
                Debug.LogError(obj);
                foreach (Type obj2 in type_array)
                {
                    Debug.Log(obj2);
                }
            }
            return (Func<RMonoBehaviour, Component>)Delegate.CreateDelegate(typeof(Func<RMonoBehaviour, Component>), method2);
        }
    }
}