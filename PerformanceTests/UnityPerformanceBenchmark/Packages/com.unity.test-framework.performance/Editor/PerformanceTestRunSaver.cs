#if UNITY_2018_3_OR_NEWER
using System;
using System.IO;
using System.Text;
using Unity.PerformanceTesting.Runtime;
using UnityEditor;
using UnityEditor.Networking.PlayerConnection;
using UnityEngine;
using UnityEditor.TestTools.TestRunner.Api;
using UnityEngine.Networking.PlayerConnection;

namespace Unity.PerformanceTesting.Editor
{
    [InitializeOnLoad]
    public class TestRunnerInitializer
    {
        static TestRunnerInitializer()
        {
            var api = ScriptableObject.CreateInstance<TestRunnerApi>();
            var obj = ScriptableObject.CreateInstance<PerformanceTestRunSaver>();
            api.RegisterCallbacks(obj);
        }
    }

    [Serializable]
    public class PerformanceTestRunSaver : ScriptableObject, ICallbacks
    {
        private PerformanceTestRun m_Run;

        public void OnEnable()
        {
            EditorConnection.instance.Initialize();
            EditorConnection.instance.Register(Utils.k_sendTestDataToEditor, OnTestDataReceived);
            EditorConnection.instance.Register(Utils.k_sendPlayerDataToEditor, OnPlayerDataReceived);
        }

        public void OnDisable()
        {
            EditorConnection.instance.Unregister(Utils.k_sendTestDataToEditor, OnTestDataReceived);
            EditorConnection.instance.Unregister(Utils.k_sendPlayerDataToEditor, OnPlayerDataReceived);
        }

        private void OnTestDataReceived(MessageEventArgs args)
        {
            var json = Encoding.ASCII.GetString(args.data);
            var test = JsonUtility.FromJson<PerformanceTest>(json);
            AddTest(test);
        }

        private void OnPlayerDataReceived(MessageEventArgs args)
        {
            var text = Encoding.ASCII.GetString(args.data);
            var runData = JsonUtility.FromJson<PerformanceTestRun>(text);

            if (m_Run == null) InitializeTestRun();
            m_Run.PlayerSystemInfo = runData.PlayerSystemInfo;
            m_Run.QualitySettings = runData.QualitySettings;
            m_Run.ScreenSettings = runData.ScreenSettings;
            m_Run.TestSuite = "Playmode";
            m_Run.BuildSettings.Platform = runData.BuildSettings.Platform;
        }

        private void AddTest(PerformanceTest test)
        {
            if (m_Run == null) InitializeTestRun();
            m_Run.Results.Add(test);
        }

        private void InitializeTestRun()
        {
            m_Run = new PerformanceTestRun
            {
                StartTime = Utils.DateToInt(DateTime.Now),
                EditorVersion = GetEditorInfo(),
                PlayerSettings = GetPlayerSettings(),
                BuildSettings = GetPlayerBuildInfo()
            };
        }

        private void SetHardwareAndQualitySettings()
        {
            m_Run.PlayerSystemInfo = Utils.GetSystemInfo();
            m_Run.QualitySettings = Utils.GetQualitySettings();
            m_Run.ScreenSettings = Utils.GetScreenSettings();
            m_Run.TestSuite = Application.isPlaying ? "Playmode" : "Editmode";
            m_Run.BuildSettings.Platform = Application.platform.ToString();
        }

        private static EditorVersion GetEditorInfo()
        {
            return new EditorVersion
            {
                FullVersion = UnityEditorInternal.InternalEditorUtility.GetFullUnityVersion(),
                DateSeconds = int.Parse(UnityEditorInternal.InternalEditorUtility.GetUnityVersionDate().ToString()),
                Branch = GetEditorBranch(),
                RevisionValue = int.Parse(UnityEditorInternal.InternalEditorUtility.GetUnityRevision().ToString())
            };
        }

        private static string GetEditorBranch()
        {
            foreach (var method in typeof(UnityEditorInternal.InternalEditorUtility).GetMethods())
            {
                if (method.Name.Contains("GetUnityBuildBranch"))
                {
                    return (string) method.Invoke(null, null);
                }
            }

            return "null";
        }

        private static PlayerSettings GetPlayerSettings()
        {
            var settings = new PlayerSettings
            {
                VrSupported = UnityEditor.PlayerSettings.virtualRealitySupported,
                MtRendering = UnityEditor.PlayerSettings.MTRendering,
                GpuSkinning = UnityEditor.PlayerSettings.gpuSkinning,
                GraphicsJobs = UnityEditor.PlayerSettings.graphicsJobs,
                GraphicsApi =
                    UnityEditor.PlayerSettings.GetGraphicsAPIs(EditorUserBuildSettings.activeBuildTarget)[0].ToString(),
                ScriptingBackend = UnityEditor.PlayerSettings
                    .GetScriptingBackend(EditorUserBuildSettings.selectedBuildTargetGroup).ToString(),
                StereoRenderingPath = UnityEditor.PlayerSettings.stereoRenderingPath.ToString(),
                RenderThreadingMode = UnityEditor.PlayerSettings.graphicsJobs ? "GraphicsJobs" :
                    UnityEditor.PlayerSettings.MTRendering ? "MultiThreaded" : "SingleThreaded",
                AndroidMinimumSdkVersion = UnityEditor.PlayerSettings.Android.minSdkVersion.ToString(),
                Batchmode = Application.isBatchMode.ToString()
                //playerSettings.StaticBatching = TODO
                //playerSettings.DynamicBatching = TODO
            };

            return settings;
        }

        private static BuildSettings GetPlayerBuildInfo()
        {
            var buildSettings = new BuildSettings
            {
                BuildTarget = EditorUserBuildSettings.activeBuildTarget.ToString(),
                DevelopmentPlayer = EditorUserBuildSettings.development,
                AndroidBuildSystem = EditorUserBuildSettings.androidBuildSystem.ToString()
            };
            return buildSettings;
        }

        void ICallbacks.RunStarted(ITest testsToRun)
        {
            m_Run = null;
        }

        void ICallbacks.RunFinished(ITestResult result)
        {
            if (m_Run.Results.Count == 0) SetHardwareAndQualitySettings();

            m_Run.EndTime = Utils.DateToInt(DateTime.Now);

            if (Utils.VerifyTestRunMetadata(m_Run))
            {
                var path = Utils.GetArg("-performanceTestResults");
                if (path != null)
                {
                    //File.WriteAllText(path, JsonUtility.ToJson(m_Run));
                }
            }

            m_Run = null;
        }

        void ICallbacks.TestStarted(ITest test)
        {
            if (m_Run == null) InitializeTestRun();
            PerformanceTest.OnTestEnded = () =>
            {
                SetHardwareAndQualitySettings();
                AddTest(PerformanceTest.Active);
            };
        }

        void ICallbacks.TestFinished(ITestResult result)
        {
        }
    }
}
#endif