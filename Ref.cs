using System.Diagnostics;
using System.Runtime.Serialization;
using UnityEngine;
using ZeroPass.Serialization;

namespace ZeroPass
{
    [SerializationConfig(MemberSerialization.OptIn)]
    [DebuggerDisplay("{id}")]
    public class Ref<ReferenceType> : ISaveLoadable where ReferenceType : RMonoBehaviour
    {
        [Serialize]
        private int id = -1;

        private ReferenceType obj;

        public Ref(ReferenceType obj)
        {
            Set(obj);
        }

        public Ref()
        {
        }

        private void UpdateID()
        {
            ReferenceType exists = Get();
            if ((bool)(Object)exists)
            {
                id = obj.GetComponent<RPrefabID>().InstanceID;
            }
            else
            {
                id = -1;
            }
        }

        [OnSerializing]
        public void OnSerializing()
        {
            UpdateID();
        }

        public int GetId()
        {
            UpdateID();
            return id;
        }

        public ComponentType Get<ComponentType>() where ComponentType : MonoBehaviour
        {
            ReferenceType x = Get();
            if (x == null)
            {
                return null;
            }
            return x.GetComponent<ComponentType>();
        }

        public ReferenceType Get()
        {
            if (obj == null && id != -1)
            {
                RPrefabID instance = RPrefabIDTracker.Get().GetInstance(id);
                if (instance != null)
                {
                    obj = instance.GetComponent<ReferenceType>();
                    if (obj == null)
                    {
                        id = -1;
                        Debug.LogWarning("Missing " + typeof(ReferenceType).Name + " reference: " + id);
                    }
                }
                else
                {
                    Debug.LogWarning("Missing RPrefabID reference: " + id);
                    id = -1;
                }
            }
            return obj;
        }

        public void Set(ReferenceType obj)
        {
            if (obj == null)
            {
                id = -1;
            }
            else
            {
                id = obj.GetComponent<RPrefabID>().InstanceID;
            }
            this.obj = obj;
        }
    }
}