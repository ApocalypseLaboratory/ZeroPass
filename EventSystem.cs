using System;
using System.Collections.Generic;
using UnityEngine;

namespace ZeroPass
{
    public class EventSystem
    {
        private struct Entry
        {
            public Action<object> handler;

            public int hash;

            public int id;

            public Entry(int hash, Action<object> handler, int id)
            {
                this.handler = handler;
                this.hash = hash;
                this.id = id;
            }
        }

        private struct SubscribedEntry
        {
            public Action<object> handler;

            public int hash;

            public GameObject go;

            public SubscribedEntry(GameObject go, int hash, Action<object> handler)
            {
                this.go = go;
                this.hash = hash;
                this.handler = handler;
            }
        }

        private struct IntraObjectRoute
        {
            public int eventHash;

            public int handlerIndex;

            public IntraObjectRoute(int eventHash, int handlerIndex)
            {
                this.eventHash = eventHash;
                this.handlerIndex = handlerIndex;
            }

            public bool IsValid()
            {
                return eventHash != 0;
            }
        }

        public abstract class IntraObjectHandlerBase
        {
            public abstract void Trigger(GameObject gameObject, object eventData);
        }

        public class IntraObjectHandler<ComponentType> : IntraObjectHandlerBase
        {
            private Action<ComponentType, object> handler;

            public IntraObjectHandler(Action<ComponentType, object> handler)
            {
                // Debug.Assert(handler.Method.IsStatic);
                this.handler = handler;
            }

            public static implicit operator IntraObjectHandler<ComponentType>(Action<ComponentType, object> handler)
            {
                return new IntraObjectHandler<ComponentType>(handler);
            }

            public override void Trigger(GameObject gameObject, object eventData)
            {
                ListPool<ComponentType, IntraObjectHandler<ComponentType>>.PooledList pooledList = ListPool<ComponentType, IntraObjectHandler<ComponentType>>.Allocate();
                gameObject.GetComponents(pooledList);
                foreach (ComponentType item in pooledList)
                {
                    handler(item, eventData);
                }
                pooledList.Recycle();
            }

            public override string ToString()
            {
                return ((handler.Target == null) ? "STATIC" : handler.Target.GetType().ToString()) + "." + handler.Method.ToString();
            }
        }

        private int nextId;

        private int currentlyTriggering;

        private bool dirty;

        private ArrayRef<SubscribedEntry> subscribedEvents;

        private ArrayRef<Entry> entries;

        private ArrayRef<IntraObjectRoute> intraObjectRoutes;

        private static Dictionary<int, List<IntraObjectHandlerBase>> intraObjectDispatcher = new();

        public void Trigger(GameObject go, int hash, object data = null)
        {
            if (!App.IsExiting)
            {
                currentlyTriggering++;
                for (int i = 0; i != intraObjectRoutes.size; i++)
                {
                    IntraObjectRoute intraObjectRoute = intraObjectRoutes[i];
                    if (intraObjectRoute.eventHash == hash)
                    {
                        List<IntraObjectHandlerBase> list = intraObjectDispatcher[hash];
                        IntraObjectRoute intraObjectRoute2 = intraObjectRoutes[i];
                        list[intraObjectRoute2.handlerIndex].Trigger(go, data);
                    }
                }
                for (int k = 0; k < entries.size; k++)
                {
                    Entry entry = entries[k];
                    if (entry.hash == hash)
                    {
                        if (entry.handler != null)
                        {
                            entry.handler(data);
                        }
                    }
                }
                currentlyTriggering--;
                if (dirty && currentlyTriggering == 0)
                {
                    dirty = false;
                    entries.RemoveAllSwap((Entry x) => x.handler == null);
                    intraObjectRoutes.RemoveAllSwap(route => !route.IsValid());
                }
            }
        }

        public void OnCleanUp()
        {
            for (int num = subscribedEvents.size - 1; num >= 0; num--)
            {
                SubscribedEntry subscribedEntry = subscribedEvents[num];
                if (subscribedEntry.go != null)
                {
                    Unsubscribe(subscribedEntry.go, subscribedEntry.hash, subscribedEntry.handler);
                }
            }
            for (int i = 0; i < entries.size; i++)
            {
                Entry value = entries[i];
                value.handler = null;
                entries[i] = value;
            }
            entries.Clear();
            subscribedEvents.Clear();
            intraObjectRoutes.Clear();
            
            
            for (int num = wrSubscribedEnties.size - 1; num >= 0; num--)
            {
                IWRSubscribedEntry wrSubscribedEntryInter = wrSubscribedEnties[num];
                if (wrSubscribedEntryInter.GO != null)
                {
                    Unregister(wrSubscribedEntryInter.GO, wrSubscribedEntryInter.Hash, wrSubscribedEntryInter.ReferenceId);
                }
            }
            for (int i = 0; i < entries.size; i++)
            {
                Entry value = entries[i];
                value.handler = null;
                entries[i] = value;
            }
            
            wrEntries.Clear();
            wrSubscribedEnties.Clear();
            wrIntraObjectRoutes.Clear();
        }

        public void UnregisterEvent(GameObject target, int eventName, Action<object> handler)
        {
            int num = 0;
            while (true)
            {
                if (num >= subscribedEvents.size)
                {
                    return;
                }
                SubscribedEntry subscribedEntry = subscribedEvents[num];
                if (subscribedEntry.hash == eventName && subscribedEntry.handler == handler && subscribedEntry.go == target)
                {
                    break;
                }
                num++;
            }
            subscribedEvents.RemoveAt(num);
        }

        public void RegisterEvent(GameObject target, int eventName, Action<object> handler)
        {
            subscribedEvents.Add(new SubscribedEntry(target, eventName, handler));
        }

        public int Subscribe(int hash, Action<object> handler)
        {
            entries.Add(new Entry(hash, handler, ++nextId));
            return nextId;
        }

        public void Unsubscribe(int hash, Action<object> handler)
        {
            int num = 0;
            while (true)
            {
                if (num >= entries.size)
                {
                    return;
                }
                Entry entry = entries[num];
                if (entry.hash == hash && entry.handler == handler)
                {
                    break;
                }
                num++;
            }
            if (currentlyTriggering == 0)
            {
                entries.RemoveAt(num);
            }
            else
            {
                dirty = true;
                Entry value = entries[num];
                value.handler = null;
                entries[num] = value;
            }
        }

        public void Unsubscribe(int id)
        {
            int num = 0;
            while (true)
            {
                if (num >= entries.size)
                {
                    return;
                }
                Entry entry = entries[num];
                if (entry.id == id)
                {
                    break;
                }
                num++;
            }
            if (currentlyTriggering == 0)
            {
                entries.RemoveAt(num);
            }
            else
            {
                dirty = true;
                Entry value = entries[num];
                value.handler = null;
                entries[num] = value;
            }
        }

        public int Subscribe(GameObject target, int eventName, Action<object> handler)
        {
            RegisterEvent(target, eventName, handler);
            RObject orCreateObject = RObjectManager.Instance.GetOrCreateObject(target);
            return orCreateObject.GetEventSystem().Subscribe(eventName, handler);
        }

        public int Subscribe<ComponentType>(int eventName, IntraObjectHandler<ComponentType> handler)
        {
            if (!intraObjectDispatcher.TryGetValue(eventName, out List<IntraObjectHandlerBase> value))
            {
                value = new List<IntraObjectHandlerBase>();
                intraObjectDispatcher.Add(eventName, value);
            }
            int num = value.IndexOf((IntraObjectHandlerBase)handler);
            if (num == -1)
            {
                value.Add((IntraObjectHandlerBase)handler);
                num = value.Count - 1;
            }
            intraObjectRoutes.Add(new IntraObjectRoute(eventName, num));
            return num;
        }

        public void Unsubscribe(GameObject target, int eventName, Action<object> handler)
        {
            UnregisterEvent(target, eventName, handler);
            if (target!= null)
            {
                RObject orCreateObject = RObjectManager.Instance.GetOrCreateObject(target);
                orCreateObject.GetEventSystem().Unsubscribe(eventName, handler);
            }
        }

        public void Unsubscribe(int eventName, int subscribeHandle, bool suppressWarnings = false)
        {
            int num = intraObjectRoutes.FindIndex((IntraObjectRoute route) => route.eventHash == eventName && route.handlerIndex == subscribeHandle);
            if (num == -1)
            {
                if (!suppressWarnings)
                {
                    Debug.LogWarning("Failed to Unsubscribe event handler: " + intraObjectDispatcher[eventName][subscribeHandle].ToString() + "\nNot subscribed to event");
                }
            }
            else if (currentlyTriggering == 0)
            {
                intraObjectRoutes.RemoveAtSwap(num);
            }
            else
            {
                dirty = true;
                intraObjectRoutes[num] = default;
            }
        }

        public void Unsubscribe<ComponentType>(int eventName, IntraObjectHandler<ComponentType> handler, bool suppressWarnings)
        {
            if (!intraObjectDispatcher.TryGetValue(eventName, out List<IntraObjectHandlerBase> value))
            {
                if (!suppressWarnings)
                {
                    Debug.LogWarning("Failed to Unsubscribe event handler: " + handler + "\nNo subscriptions have been made to event");
                }
            }
            else
            {
                int num = value.IndexOf(handler);
                if (num == -1)
                {
                    if (!suppressWarnings)
                    {
                        Debug.LogWarning("Failed to Unsubscribe event handler: " + handler + "\nNot subscribed to event");
                    }
                }
                else
                {
                    Unsubscribe(eventName, num, suppressWarnings);
                }
            }
        }

        #region event with result

        private int nextWRId; 
        public delegate void GenericEventWithResultHandler<TResult>(ref TResult result, params object[] args);
        
        private interface IWREntry
        {
            public int Hash { get;}
            public int ID { get;}

            public void Unregiser();

            public bool IsValid();
        }

        private struct WREntry<TResult> : IWREntry
        {
            public GenericEventWithResultHandler<TResult> handler;

            public WREntry(int hash, GenericEventWithResultHandler<TResult> handler, int id)
            {
                this.handler = handler;
                Hash = hash;
                ID = id;
            }

            public int Hash { get; private set; }
            public int ID { get; set; }
            public void Unregiser()
            {
                handler = null;
            }

            public bool IsValid()
            {
                return handler == null;
            }
        }
        
        private interface IWRSubscribedEntry
        {
            public int Hash { get;}
            public GameObject GO { get;}
            
            public int ReferenceId { get; }
        }

        private struct WRSubscribedEntry<TResult> : IWRSubscribedEntry
        {
            public GenericEventWithResultHandler<TResult> handler;

            public WRSubscribedEntry(GameObject go, int hash, GenericEventWithResultHandler<TResult> handler, int referenceId)
            {
                GO = go;
                Hash = hash;
                this.handler = handler;
                ReferenceId = referenceId;
            }

            public int Hash { get; private set; }
            public GameObject GO { get; private set; }
            
            public int ReferenceId { get; private set; }
        }

        private ArrayRef<IWREntry> wrEntries;
        private ArrayRef<IWRSubscribedEntry> wrSubscribedEnties;
        
        public delegate void EventWithResultHandler<ComponentType, TResult>(ComponentType sender, int evt, ref TResult result, params object[] args);

        private static Dictionary<int, List<WRIntraObjectHandlerBase>> wrIntraObjectDispatcher = new();
        
        private ArrayRef<WRIntraObjectRoute> wrIntraObjectRoutes;

        public abstract class WRIntraObjectHandlerBase
        {
        }
        
        public class WRIntraObjectHandler<ComponentType, TResult> : WRIntraObjectHandlerBase
        {
            private EventWithResultHandler<ComponentType, TResult> handler;
            
            public WRIntraObjectHandler(EventWithResultHandler<ComponentType, TResult> handler)
            {
                this.handler = handler;
            }
            
            public static implicit operator WRIntraObjectHandler<ComponentType, TResult>(EventWithResultHandler<ComponentType, TResult> handler)
            {
                return new WRIntraObjectHandler<ComponentType, TResult>(handler);
            }
            public void EventWithResult(GameObject gameObject, int evt, ref TResult result, params object[] args)
            {
                ListPool<ComponentType, IntraObjectHandler<ComponentType>>.PooledList pooledList = ListPool<ComponentType, IntraObjectHandler<ComponentType>>.Allocate();
                gameObject.GetComponents(pooledList);
                foreach (ComponentType item in pooledList)
                {
                    handler(item, evt, ref result, args);
                }
                pooledList.Recycle();
            }
        }
        
        private struct WRIntraObjectRoute
        {
            public int eventHash;

            public int handlerIndex;

            public WRIntraObjectRoute(int eventHash, int handlerIndex)
            {
                this.eventHash = eventHash;
                this.handlerIndex = handlerIndex;
            }

            public bool IsValid()
            {
                return eventHash != 0;
            }
        }

        public int Register<ComponentType, TResult>(int eventName, WRIntraObjectHandler<ComponentType, TResult> handler)
        {
            if (!wrIntraObjectDispatcher.TryGetValue(eventName, out List<WRIntraObjectHandlerBase> value))
            {
                value = new List<WRIntraObjectHandlerBase>();
                wrIntraObjectDispatcher.Add(eventName, value);
            }
            int num = value.IndexOf(handler);
            if (num == -1)
            {
                value.Add(handler);
                num = value.Count - 1;
            }
            wrIntraObjectRoutes.Add(new WRIntraObjectRoute(eventName, num));
            return num;
        }
        
        public void Unregister(int eventName, int subscribeHandle, bool suppressWarnings = false)
        {
            int num = wrIntraObjectRoutes.FindIndex(route => route.eventHash == eventName && route.handlerIndex == subscribeHandle);
            if (num == -1)
            {
                if (!suppressWarnings)
                {
                    Debug.LogWarning("Failed to Unsubscribe event handler: " + wrIntraObjectDispatcher[eventName][subscribeHandle].ToString() + "\nNot subscribed to event");
                }
            }
            else if (currentlyTriggering == 0)
            {
                wrIntraObjectRoutes.RemoveAtSwap(num);
            }
            else
            {
                dirty = true;
                wrIntraObjectRoutes[num] = default;
            }
        }

        public void Unregister<ComponentType, TResult>(int eventName, WRIntraObjectHandler<ComponentType, TResult> handler, bool suppressWarnings)
        {
            if (!wrIntraObjectDispatcher.TryGetValue(eventName, out List<WRIntraObjectHandlerBase> value))
            {
                if (!suppressWarnings)
                {
                    Debug.LogWarning("Failed to Unsubscribe event handler: " + handler.ToString() + "\nNo subscriptions have been made to event");
                }
            }
            else
            {
                int num = value.IndexOf(handler);
                if (num == -1)
                {
                    if (!suppressWarnings)
                    {
                        Debug.LogWarning("Failed to Unsubscribe event handler: " + handler.ToString() + "\nNot subscribed to event");
                    }
                }
                else
                {
                    Unregister(eventName, num, suppressWarnings);
                }
            }
        }

        public void EventWithResult<ComponentType, TResult>(GameObject gameObject, int hash, ref TResult result,
            params object[] args)
        {
            if (!App.IsExiting)
            {
                currentlyTriggering++;
                for (int i = 0; i != wrIntraObjectRoutes.size; i++)
                {
                    WRIntraObjectRoute wrIntraObjectRoute = wrIntraObjectRoutes[i];
                    if (wrIntraObjectRoute.eventHash == hash)
                    {
                        List<WRIntraObjectHandlerBase> list =wrIntraObjectDispatcher[hash];
                        WRIntraObjectRoute wrIntraObjectRoute2 = wrIntraObjectRoutes[i];
                        if (list[wrIntraObjectRoute2.handlerIndex] is WRIntraObjectHandler<ComponentType, TResult>
                            handler)
                        {
                            handler.EventWithResult(gameObject, hash, ref result, args);
                        }
                    }
                }
                currentlyTriggering--;
                if (dirty && currentlyTriggering == 0)
                {
                    dirty = false;
                    wrEntries.RemoveAllSwap(x => x.IsValid());
                    wrIntraObjectRoutes.RemoveAllSwap(route => !route.IsValid());
                }
            }
        }
        
        public void EventWithResult<TResult>(int hash, ref TResult result, params object[] args)
        {
            if (!App.IsExiting)
            {
                currentlyTriggering++;
                for (int j = 0; j < wrEntries.size; j++)
                {
                    IWREntry wrEntryInter = wrEntries[j];
                    if (wrEntryInter.Hash == hash)
                    {
                        if (wrEntryInter is WREntry<TResult> wrEntry)
                            wrEntry.handler?.Invoke(ref result, args);
                    }
                }
                currentlyTriggering--;
                if (dirty && currentlyTriggering == 0)
                {
                    dirty = false;
                    wrEntries.RemoveAllSwap(x => x.IsValid());
                    wrIntraObjectRoutes.RemoveAllSwap(route => !route.IsValid());
                }
            }
        }
        
        public int Register<TResult>(int hash, GenericEventWithResultHandler<TResult> handler)
        {
            wrEntries.Add(new WREntry<TResult>(hash, handler, ++nextWRId));
            return nextWRId;
        }
        
        public int Register<TResult>(GameObject target, int eventName, GenericEventWithResultHandler<TResult> handler)
        {
            RObject orCreateObject = RObjectManager.Instance.GetOrCreateObject(target);
            var referenceId = orCreateObject.GetEventSystem().Register(eventName, handler);
            wrSubscribedEnties.Add(new WRSubscribedEntry<TResult>(target, eventName, handler, referenceId));
            return referenceId;
        }
        
        public void Unregister(GameObject target, int eventName, int referenceId)
        {
            int num = 0;
            while (true)
            {
                if (num >= wrSubscribedEnties.size)
                {
                    return;
                }
                var subscribedEntryInter = wrSubscribedEnties[num];
                if (subscribedEntryInter.Hash == eventName && subscribedEntryInter.GO == target && subscribedEntryInter.ReferenceId == referenceId)
                {
                    break;
                }
                num++;
            }
            wrSubscribedEnties.RemoveAt(num);
            
            if (target!= null)
            {
                RObject orCreateObject = RObjectManager.Instance.GetOrCreateObject(target);
                orCreateObject.GetEventSystem().Unregister(eventName, referenceId);
            }
        }
        
        public void Unregister(int hash, int id)
        {
            int num = 0;
            while (true)
            {
                if (num >= wrEntries.size)
                {
                    return;
                }
                var wrEntryInter = wrEntries[num];
                if (wrEntryInter.Hash == hash && wrEntryInter.ID == id)
                {
                    break;
                }
                num++;
            }
            if (currentlyTriggering == 0)
            {
                wrEntries.RemoveAt(num);
            }
            else
            {
                dirty = true;
                var value = wrEntries[num];
                value.Unregiser();
                wrEntries[num] = value;
            }
        }
        
        public void Unregister<TResult>(GameObject target, int eventName, GenericEventWithResultHandler<TResult> handler)
        {
            int num = 0;
            while (true)
            {
                if (num >= wrSubscribedEnties.size)
                {
                    return;
                }
                var wrSubscribedEntryInter = wrSubscribedEnties[num];
                if (wrSubscribedEntryInter.Hash == eventName && wrSubscribedEntryInter.GO == target)
                {
                    if (wrSubscribedEntryInter is WRSubscribedEntry<TResult> wrSubscribedEntry && wrSubscribedEntry.handler == handler)
                    {
                        break;
                    }
                }
                num++;
            }
            wrSubscribedEnties.RemoveAt(num);
            
            if (target!= null)
            {
                RObject orCreateObject = RObjectManager.Instance.GetOrCreateObject(target);
                orCreateObject.GetEventSystem().Unregister(eventName, handler);
            }
        }
        
        public void Unregister<TResult>(int hash, GenericEventWithResultHandler<TResult> handler)
        {
            int num = 0;
            while (true)
            {
                if (num >= wrEntries.size)
                {
                    return;
                }
                IWREntry wrEntryInter = wrEntries[num];
                if (wrEntryInter.Hash == hash)
                {
                    if (wrEntryInter is WREntry<TResult> wrEntry && wrEntry.handler == handler)
                    {
                        break;
                    }
                }
                num++;
            }
            if (currentlyTriggering == 0)
            {
                wrEntries.RemoveAt(num);
            }
            else
            {
                dirty = true;
                var value = (WREntry<TResult>)wrEntries[num];
                value.handler = null;
                wrEntries[num] = value;
            }
        }
        
        #endregion
    }
}