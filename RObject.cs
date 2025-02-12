using UnityEngine;

namespace ZeroPass
{
    public class RObject
    {
        private EventSystem eventSystem;

        public int id
        {
            get;
            private set;
        }

        public bool hasEventSystem => eventSystem != null;

        public RObject(GameObject go)
        {
            id = go.GetInstanceID();
        }

        ~RObject()
        {
            OnCleanUp();
        }

        public void OnCleanUp()
        {
            if (eventSystem != null)
            {
                eventSystem.OnCleanUp();
                eventSystem = null;
            }
        }

        public EventSystem GetEventSystem()
        {
            if (eventSystem == null)
            {
                eventSystem = new EventSystem();
            }
            return eventSystem;
        }
    }
}