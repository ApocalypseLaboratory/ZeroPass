using System;
using System.Collections.Generic;
using UnityEngine;

namespace ZeroPass
{
    public class RMonoBehaviour : MonoBehaviour, IStateMachineTarget
    {
        public static GameObject lastGameObject;

        public static RObject lastObj;

        public static bool isPoolPreInit;

        public static bool isLoadingScene;

        private RObject obj;

        private bool isInitialized;

        public bool isSpawned
        {
            get;
            private set;
        }
        public new Transform transform => base.transform;
        public bool isNull => this == null;

        public void Awake()
        {
            if (!App.IsExiting)
            {
                InitializeComponent();
            }
        }

        public void InitializeComponent()
        {
            if (!isInitialized)
            {
                if (!isPoolPreInit && Application.isPlaying && lastGameObject != base.gameObject)
                {
                    lastGameObject = base.gameObject;
                    lastObj = RObjectManager.Instance.GetOrCreateObject(base.gameObject);
                }
                obj = lastObj;
                isInitialized = true;
                MyCmp.OnAwake(this);
                if (!isPoolPreInit)
                {
                    try
                    {
                        OnPrefabInit();
                    }
                    catch (Exception innerException)
                    {
                        string message = "Error in " + base.name + "." + GetType().Name + ".OnPrefabInit";
                        throw new Exception(message, innerException);
                    }
                }
            }
        }

        private void OnEnable()
        {
            if (!App.IsExiting)
            {
                OnCmpEnable();
            }
        }

        private void OnDisable()
        {
            if (!App.IsExiting && !isLoadingScene)
            {
                OnCmpDisable();
            }
        }

        public bool IsInitialized()
        {
            return isInitialized;
        }

        public void OnDestroy()
        {
            OnForcedCleanUp();
            if (!App.IsExiting)
            {
                if (isLoadingScene)
                {
                    OnLoadLevel();
                }
                else
                {
                    if (RObjectManager.IsInitialized())
                    {
                        RObjectManager.Instance.QueueDestroy(obj);
                    }
                    OnCleanUp();
                }
            }
        }

        public void Start()
        {
            if (!App.IsExiting)
            {
                Spawn();
            }
        }

        public void Spawn()
        {
            if (!isSpawned)
            {
                if (!isInitialized)
                {
                    Debug.LogError(base.name + "." + GetType().Name + " is not initialized.");
                }
                else
                {
                    isSpawned = true;
                    MyCmp.OnStart(this);
                    try
                    {
                        OnSpawn();
                    }
                    catch (Exception innerException)
                    {
                        string message = "Error in " + base.name + "." + GetType().Name + ".OnSpawn";
                        throw new Exception(message, innerException);
                    }
                }
            }
        }

        protected virtual void OnPrefabInit()
        {
        }

        protected virtual void OnSpawn()
        {
        }

        protected virtual void OnCmpEnable()
        {
        }

        protected virtual void OnCmpDisable()
        {
        }

        protected virtual void OnCleanUp()
        {
        }

        protected virtual void OnForcedCleanUp()
        {
        }

        protected virtual void OnLoadLevel()
        {
        }

        public virtual void CreateDef()
        {
        }

        public T FindOrAdd<T>() where T : RMonoBehaviour
        {
            return this.FindOrAddComponent<T>();
        }

        public void FindOrAdd<T>(ref T c) where T : RMonoBehaviour
        {
            c = FindOrAdd<T>();
        }

        public T Require<T>() where T : Component
        {
            return this.RequireComponent<T>();
        }

        public int Subscribe(int hash, Action<object> handler)
        {
            return obj.GetEventSystem().Subscribe(hash, handler);
        }

        public int Subscribe(GameObject target, int hash, Action<object> handler)
        {
            return obj.GetEventSystem().Subscribe(target, hash, handler);
        }

        public int Subscribe<ComponentType>(int hash, EventSystem.IntraObjectHandler<ComponentType> handler)
        {
            return obj.GetEventSystem().Subscribe(hash, handler);
        }

        public void Unsubscribe(int hash, Action<object> handler)
        {
            if (obj != null)
            {
                obj.GetEventSystem().Unsubscribe(hash, handler);
            }
        }

        public void Unsubscribe(int id)
        {
            obj.GetEventSystem().Unsubscribe(id);
        }

        public void Unsubscribe(GameObject target, int hash, Action<object> handler)
        {
            obj.GetEventSystem().Unsubscribe(target, hash, handler);
        }

        public void Unsubscribe<ComponentType>(int hash, EventSystem.IntraObjectHandler<ComponentType> handler, bool suppressWarnings = false)
        {
            if (obj != null)
            {
                obj.GetEventSystem().Unsubscribe(hash, handler, suppressWarnings);
            }
        }

        public void Trigger(int hash, object data = null)
        {
            if (obj != null && obj.hasEventSystem)
            {
                obj.GetEventSystem().Trigger(base.gameObject, hash, data);
            }
        }

        #region event with result

        public int Register<ComponentType, TResult>(int eventName,
            EventSystem.WRIntraObjectHandler<ComponentType, TResult> handler)
        {
            return obj.GetEventSystem().Register(eventName, handler);
        }
        
        public int Register<TResult>(int eventName, EventSystem.GenericEventWithResultHandler<TResult> handler)
        {
            return obj.GetEventSystem().Register(eventName, handler);
        }

        public int Register<TResult>(GameObject target, int eventName, EventSystem.GenericEventWithResultHandler<TResult> handler)
        {
            return obj.GetEventSystem().Register(target, eventName, handler);
        }

        public void Unregister(int eventName, int subscribeHandle, bool suppressWarnings = false)
        {
            if (obj != null)
            {
                obj.GetEventSystem().Unregister(eventName, subscribeHandle, suppressWarnings);
            }
        }

        public void Unregister<TResult>(int hash, EventSystem.GenericEventWithResultHandler<TResult> handler)
        {
            if (obj != null)
            {
                obj.GetEventSystem().Unregister(hash, handler);
            }
        }

        public void Unregister<TResult>(GameObject target, int eventName, EventSystem.GenericEventWithResultHandler<TResult> handler)
        {
            if (obj != null)
            {
                obj.GetEventSystem().Unregister(target, eventName, handler);
            }
        }
        
        public void Unregister<ComponentType, TResult>(int hash, EventSystem.WRIntraObjectHandler<ComponentType, TResult> handler, bool suppressWarnings = false)
        {
            if (obj != null)
            {
                obj.GetEventSystem().Unregister(hash, handler, suppressWarnings);
            }
        }

        public void EventWithResult<ComponentType, TResult>(int hash, ref TResult result, params object[] args)
        {
            if (obj != null && obj.hasEventSystem)
            {
                obj.GetEventSystem().EventWithResult<ComponentType, TResult>(base.gameObject, hash, ref result, args);
            }
        }

        public void EventWithResult<TResult>(int hash, ref TResult result, params object[] args)
        {
            if (obj != null && obj.hasEventSystem)
            {
                obj.GetEventSystem().EventWithResult(hash, ref result, args);
            }
        }
        #endregion
    }
}