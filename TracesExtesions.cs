using UnityEngine;

namespace ZeroPass
{
    public static class TracesExtesions
    {
        public static void DeleteObject(this GameObject go)
        {
            RMonoBehaviour component = go.GetComponent<RMonoBehaviour>();
            if (component != null)
            {
                component.Trigger((int)UtilHashes.QueueDestroyObject, go);
            }
            Object.Destroy(go);
        }

        public static void DeleteObject(this Component cmp)
        {
            cmp.gameObject.DeleteObject();
        }
    }
}