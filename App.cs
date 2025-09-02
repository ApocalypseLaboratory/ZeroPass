using System;
using System.Collections.Generic;
using System.Reflection;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;

namespace ZeroPass
{
    [RequireComponent(typeof(AppSM))]
    public class App : MonoBehaviour
    {
        public static App Instance;

        public static bool IsExiting;

        public static Action OnPreLoadScene;

        public static Action OnPostLoadScene;

        public static bool isLoading;

        public static bool hasFocus;

        public static string loadingSceneName;

        private static string currentSceneName;

        private float lastSuspendTime;

        private const string PIPE_NAME = "R_EXIT_CODE_PIPE";

        private const string RESTART_FILENAME = "Restarter.exe";

        private static List<Type> types;

        private static float[] sleepIntervals;

        public TMP_Text tmpText;

        static void InitApp()
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

        public void InitDll()
        {
            AddressableManager.Instance.LoadAssetAsync<TextAsset>("HotUpdate", obj =>
            {
                if (obj.Status == AsyncOperationStatus.Succeeded)
                {
                    // var hotUpdateAss = Assembly.Load(obj.Result.bytes);
                    // var type = hotUpdateAss.GetType("Hello");
                    // TuanJie 1.6.3 do not support HybirdCLR 
                    // type.GetMethod("Run").Invoke(null, null);
                }
                else
                {
                    Debug.LogError("Load HotUpdate Failed");
                }
            }).Forget();
        }

        public static string GetCurrentSceneName()
        {
            return currentSceneName;
        }

        private void OnApplicationQuit()
        {
            IsExiting = true;
        }

        public static void Quit()
        {
            Application.Quit();
        }

        private void Awake()
        {
            InitApp();
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            Singleton<StateMachineUpdater>.CreateInstance();
            Singleton<StateMachineManager>.CreateInstance();
            Singleton<AddressableManager>.CreateInstance();
            
#if UNITY_MINIGAME && !UNITY_EDITOR
            LoadFont();
            var appSM = GetComponent<AppSM>();
            appSM.smi.StartSM();
#else
            Debug.Log("App start");
            var appSM = GetComponent<AppSM>();
            appSM.smi.StartSM();
#endif
        }

        private void LoadFont()
        {
            // The ttf file needs to be placed in the StreamingAssets folder
            var fallbackFont = Application.streamingAssetsPath + "/LXGWFasmartGothic.ttf";
            Debug.Log($"fallbackFont : {fallbackFont}");
            
            WeChatWASM.WX.InitSDK(
                (ret) =>
                {
                    // fallbackFont serves as an alternative CDN URL for older versions of wechat or when system font files cannot be obtained
                    // "Note" needs to be replaced with the real font URL of the game!!
                    WeChatWASM.WX.GetWXFont(
                        fallbackFont,
                        (font) =>
                        {
                            tmpText.font = TMP_FontAsset.CreateFontAsset(font);
                        }
                    );
                }
            );
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
                // RObjectManager.Instance.Cleanup();
                // RMonoBehaviour.lastGameObject = null;
                // RMonoBehaviour.lastObj = null;
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
            if (IsExiting)
            {
                Singleton<StateMachineUpdater>.DestroyInstance();
                Singleton<StateMachineManager>.DestroyInstance();
                Singleton<AddressableManager>.DestroyInstance();
                RObjectManager.DestroyInstance();
            }
        }

        public static List<Type> GetCurrentDomainTypes()
        {
            return types;
        }
    }
}