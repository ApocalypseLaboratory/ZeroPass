using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ZeroPass
{
    public class App : MonoBehaviour
    {
        public static App instance;

        public static bool IsExiting;

        public static System.Action OnPreLoadScene;

        public static System.Action OnPostLoadScene;

        public static bool isLoading;

        public static bool hasFocus;

        public static string loadingSceneName;

        private static string currentSceneName;

        private float lastSuspendTime;

        private const string PIPE_NAME = "R_EXIT_CODE_PIPE";

        private const string RESTART_FILENAME = "Restarter.exe";

        private static List<Type> types;

        private static float[] sleepIntervals;

        static App()
        {
            IsExiting = false;
            isLoading = false;
            hasFocus = true;
            loadingSceneName = null;
            currentSceneName = null;
            types = new List<Type>();
            sleepIntervals = new float[3]
            {
                8.333333f,
                16.666666f,
                33.3333321f
            };
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly assembly in assemblies)
            {
                try
                {
                    Type[] array = assembly.GetTypes();
                    foreach (Type item in array)
                    {
                        types.Add(item);
                    }
                }
                catch (Exception)
                {
                }
            }
        }

        public static string GetCurrentSceneName()
        {
            return currentSceneName;
        }

        private void OnApplicationQuit()
        {
            IsExiting = true;
        }

        public void Restart()
        {
            string fileName = Process.GetCurrentProcess().MainModule.FileName;
            string fullPath = Path.GetFullPath(fileName);
            string directoryName = Path.GetDirectoryName(fullPath);
            Debug.LogFormat("Restarting\n\texe ({0})\n\tfull ({1})\n\tdir ({2})", fileName, fullPath, directoryName);
            string filename = Path.Combine(directoryName, "Restarter.exe");
            ProcessStartInfo processStartInfo = new ProcessStartInfo(filename);
            processStartInfo.UseShellExecute = true;
            processStartInfo.CreateNoWindow = true;
            processStartInfo.Arguments = $"\"{fullPath}\"";
            Process.Start(processStartInfo);
            Quit();
        }

        public static void Quit()
        {
            Application.Quit();
        }

        private void Awake()
        {
            instance = this;
        }

        public static void LoadScene(string scene_name)
        {
            Debug.Assert(!isLoading, "Scene [" + loadingSceneName + "] is already being loaded!");
            RMonoBehaviour.isLoadingScene = true;
            isLoading = true;
            loadingSceneName = scene_name;
        }

        private void OnApplicationFocus(bool focus)
        {
            hasFocus = focus;
            lastSuspendTime = Time.realtimeSinceStartup;
        }

        public void LateUpdate()
        {
            if (isLoading)
            {
                RObjectManager.Instance.Cleanup();
                RMonoBehaviour.lastGameObject = null;
                RMonoBehaviour.lastObj = null;
                Resources.UnloadUnusedAssets();
                GC.Collect();
                if (OnPreLoadScene != null)
                {
                    OnPreLoadScene();
                }
                SceneManager.LoadScene(loadingSceneName);
                if (OnPostLoadScene != null)
                {
                    OnPostLoadScene();
                }
                isLoading = false;
                currentSceneName = loadingSceneName;
                loadingSceneName = null;
            }
        }

        private void OnDestroy()
        {
        }

        public static List<Type> GetCurrentDomainTypes()
        {
            return types;
        }
    }
}