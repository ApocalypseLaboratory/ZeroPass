using System.Collections.Generic;
using System.IO;
using UnityEngine;
using ZeroPass.Serialization;

namespace ZeroPass
{
    public static class Util
    {
        private static HashSet<char> defaultInvalidUserInputChars = new HashSet<char>(Path.GetInvalidPathChars());

        private static HashSet<char> additionalInvalidUserInputChars = new HashSet<char>(new char[9]
        {
            '<',
            '>',
            ':',
            '"',
            '/',
            '?',
            '*',
            '\\',
            '!'
        });

        private static System.Random random = new System.Random();

        private static string defaultRootFolder = Application.persistentDataPath;

        public static void Swap<T>(ref T a, ref T b)
        {
            T val = a;
            a = b;
            b = val;
        }

        public static void InitializeComponent(Component cmp)
        {
            if ((UnityEngine.Object)cmp != (UnityEngine.Object)null)
            {
                RMonoBehaviour RMonoBehaviour = cmp as RMonoBehaviour;
                if ((UnityEngine.Object)RMonoBehaviour != (UnityEngine.Object)null)
                {
                    RMonoBehaviour.InitializeComponent();
                }
            }
        }

        public static void SpawnComponent(Component cmp)
        {
            if ((UnityEngine.Object)cmp != (UnityEngine.Object)null)
            {
                RMonoBehaviour RMonoBehaviour = cmp as RMonoBehaviour;
                if ((UnityEngine.Object)RMonoBehaviour != (UnityEngine.Object)null)
                {
                    RMonoBehaviour.Spawn();
                }
            }
        }

        public static Component FindComponent(this Component cmp, string targetName)
        {
            return cmp.gameObject.FindComponent(targetName);
        }

        public static Component FindComponent(this GameObject go, string targetName)
        {
            Component component = go.GetComponent(targetName);
            InitializeComponent(component);
            return component;
        }

        public static T FindComponent<T>(this Component c) where T : Component
        {
            return c.gameObject.FindComponent<T>();
        }

        public static T FindComponent<T>(this GameObject go) where T : Component
        {
            T component = go.GetComponent<T>();
            InitializeComponent(component);
            return component;
        }

        public static T FindOrAddUnityComponent<T>(this Component cmp) where T : Component
        {
            return cmp.gameObject.FindOrAddUnityComponent<T>();
        }

        public static T FindOrAddUnityComponent<T>(this GameObject go) where T : Component
        {
            T val = go.GetComponent<T>();
            if ((UnityEngine.Object)val == (UnityEngine.Object)null)
            {
                val = go.AddComponent<T>();
            }

            return val;
        }

        public static Component RequireComponent(this Component cmp, string name)
        {
            return cmp.gameObject.RequireComponent(name);
        }

        public static Component RequireComponent(this GameObject go, string name)
        {
            Component component = go.GetComponent(name);
            if ((UnityEngine.Object)component == (UnityEngine.Object)null)
            {
                Debug.LogErrorFormat(go, "{0} '{1}' requires a component of type {2}!", go.GetType().ToString(),
                    go.name, name);
                return null;
            }

            InitializeComponent(component);
            return component;
        }

        public static T RequireComponent<T>(this Component cmp) where T : Component
        {
            T component = cmp.gameObject.GetComponent<T>();
            if ((UnityEngine.Object)component == (UnityEngine.Object)null)
            {
                Debug.LogErrorFormat(cmp.gameObject, "{0} '{1}' requires a component of type {2} as requested by {3}!",
                    cmp.gameObject.GetType().ToString(), cmp.gameObject.name, typeof(T).ToString(),
                    cmp.GetType().ToString());
                return (T)null;
            }

            InitializeComponent(component);
            return component;
        }

        public static T RequireComponent<T>(this GameObject gameObject) where T : Component
        {
            T component = gameObject.GetComponent<T>();
            if ((UnityEngine.Object)component == (UnityEngine.Object)null)
            {
                Debug.LogErrorFormat(gameObject, "{0} '{1}' requires a component of type {2}!",
                    gameObject.GetType().ToString(), gameObject.name, typeof(T).ToString());
                return (T)null;
            }

            InitializeComponent(component);
            return component;
        }

        public static void SetLayerRecursively(this GameObject go, int layer)
        {
            SetLayer(go.transform, layer);
        }

        public static void SetLayer(Transform t, int layer)
        {
            t.gameObject.layer = layer;
            for (int i = 0; i < t.childCount; i++)
            {
                SetLayer(t.GetChild(i), layer);
            }
        }

        public static T FindOrAddComponent<T>(this Component cmp) where T : Component
        {
            return cmp.gameObject.FindOrAddComponent<T>();
        }

        public static T FindOrAddComponent<T>(this GameObject go) where T : Component
        {
            T val = go.GetComponent<T>();
            if ((UnityEngine.Object)val == (UnityEngine.Object)null)
            {
                val = go.AddComponent<T>();
                RMonoBehaviour RMonoBehaviour = val as RMonoBehaviour;
                if ((UnityEngine.Object)RMonoBehaviour != (UnityEngine.Object)null && !RMonoBehaviour.isPoolPreInit &&
                    !RMonoBehaviour.IsInitialized())
                {
                    Debug.LogErrorFormat("Could not find component " + typeof(T).ToString() + " on object " +
                                         go.ToString());
                }
            }
            else
            {
                InitializeComponent(val);
            }

            return val;
        }

        public static void PreInit(this GameObject go)
        {
            RMonoBehaviour.isPoolPreInit = true;
            RMonoBehaviour[] components = go.GetComponents<RMonoBehaviour>();
            foreach (RMonoBehaviour RMonoBehaviour in components)
            {
                RMonoBehaviour.InitializeComponent();
            }

            RMonoBehaviour.isPoolPreInit = false;
        }

        public static void Write(this BinaryWriter writer, Vector2 v)
        {
            writer.Write(v.x);
            writer.Write(v.y);
        }

        public static void Write(this BinaryWriter writer, Vector3 v)
        {
            writer.Write(v.x);
            writer.Write(v.y);
            writer.Write(v.z);
        }

        public static Vector2 ReadVector2(this BinaryReader reader)
        {
            Vector2 result = default(Vector2);
            result.x = reader.ReadSingle();
            result.y = reader.ReadSingle();
            return result;
        }

        public static Vector3 ReadVector3(this BinaryReader reader)
        {
            Vector3 result = default(Vector3);
            result.x = reader.ReadSingle();
            result.y = reader.ReadSingle();
            result.z = reader.ReadSingle();
            return result;
        }

        public static void Write(this BinaryWriter writer, Quaternion q)
        {
            writer.Write(q.x);
            writer.Write(q.y);
            writer.Write(q.z);
            writer.Write(q.w);
        }

        public static Quaternion ReadQuaternion(this IReader reader)
        {
            Quaternion result = default(Quaternion);
            result.x = reader.ReadSingle();
            result.y = reader.ReadSingle();
            result.z = reader.ReadSingle();
            result.w = reader.ReadSingle();
            return result;
        }
        
        public static Quaternion ReadQuaternion(this BinaryReader reader)
        {
            Quaternion result = default(Quaternion);
            result.x = reader.ReadSingle();
            result.y = reader.ReadSingle();
            result.z = reader.ReadSingle();
            result.w = reader.ReadSingle();
            return result;
        }

        public static GameObject RInstantiate(GameObject original, Vector3 position)
        {
            return RInstantiate(original, position, Quaternion.identity, null, null, true, 0);
        }

        public static GameObject RInstantiate(Component original, GameObject parent = null, string name = null)
        {
            return RInstantiate(original.gameObject, Vector3.zero, Quaternion.identity, parent, name, true, 0);
        }

        public static GameObject RInstantiate(GameObject original, GameObject parent = null, string name = null)
        {
            return RInstantiate(original, Vector3.zero, Quaternion.identity, parent, name, true, 0);
        }

        public static GameObject RInstantiate(GameObject original, Vector3 position, Quaternion rotation,
            GameObject parent = null, string name = null, bool initialize_id = true, int gameLayer = 0)
        {
            if (App.IsExiting)
            {
                return null;
            }

            GameObject gameObject = null;
            if ((UnityEngine.Object)original == (UnityEngine.Object)null)
            {
                DebugUtil.LogWarningArgs("Missing prefab");
            }

            if ((UnityEngine.Object)gameObject == (UnityEngine.Object)null)
            {
                if ((UnityEngine.Object)original.GetComponent<RectTransform>() != (UnityEngine.Object)null &&
                    (UnityEngine.Object)parent != (UnityEngine.Object)null)
                {
                    gameObject = UnityEngine.Object.Instantiate(original, position, rotation);
                    gameObject.transform.SetParent(parent.transform, true);
                }
                else
                {
                    Transform parent2 = null;
                    if ((UnityEngine.Object)parent != (UnityEngine.Object)null)
                    {
                        parent2 = parent.transform;
                    }

                    gameObject = UnityEngine.Object.Instantiate(original, position, rotation, parent2);
                }

                if (gameLayer != 0)
                {
                    gameObject.SetLayerRecursively(gameLayer);
                }
            }

            if (name != null)
            {
                gameObject.name = name;
            }
            else
            {
                gameObject.name = original.name;
            }

            RPrefabID component = gameObject.GetComponent<RPrefabID>();
            if ((UnityEngine.Object)component != (UnityEngine.Object)null)
            {
                if (initialize_id)
                {
                    component.InstanceID = RPrefabID.GetUniqueID();
                    RPrefabIDTracker.Get().Register(component);
                }

                RPrefabID component2 = original.GetComponent<RPrefabID>();
                component.CopyTags(component2);
                component.CopyInitFunctions(component2);
                component.RunInstantiateFn();
            }

            return gameObject;
        }
    }
}