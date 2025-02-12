using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using TMPro;
using UnityEngine;

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
				Debug.LogErrorFormat(go, "{0} '{1}' requires a component of type {2}!", go.GetType().ToString(), go.name, name);
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
				Debug.LogErrorFormat(cmp.gameObject, "{0} '{1}' requires a component of type {2} as requested by {3}!", cmp.gameObject.GetType().ToString(), cmp.gameObject.name, typeof(T).ToString(), cmp.GetType().ToString());
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
				Debug.LogErrorFormat(gameObject, "{0} '{1}' requires a component of type {2}!", gameObject.GetType().ToString(), gameObject.name, typeof(T).ToString());
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
				if ((UnityEngine.Object)RMonoBehaviour != (UnityEngine.Object)null && !RMonoBehaviour.isPoolPreInit && !RMonoBehaviour.IsInitialized())
				{
					Debug.LogErrorFormat("Could not find component " + typeof(T).ToString() + " on object " + go.ToString());
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
	}
}