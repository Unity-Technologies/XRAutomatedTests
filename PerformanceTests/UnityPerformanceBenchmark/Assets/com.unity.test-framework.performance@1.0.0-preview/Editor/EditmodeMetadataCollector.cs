#if UNITY_2018_1_OR_NEWER
using System;
using System.IO;
using NUnit.Framework;
using Unity.PerformanceTesting;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;

[Category("Performance")]
public class EditmodeMetadataCollector : IPrebuildSetup
{
    private PerformanceTestRun m_TestRun;

    private string m_TestRunPath
    {
        get { return Path.Combine(Application.streamingAssetsPath, "PerformanceTestRunInfo.json"); }
    }
    
    [Test]
    public void GetPlayerSettingsTest()
    {
        m_TestRun = ReadPerformanceTestRunJson();
        m_TestRun.PlayerSystemInfo = GetSystemInfo();
        m_TestRun.PlayerSettings = GetPlayerSettings();
        m_TestRun.TestSuite = "Editmode";

        TestContext.Out.Write("##performancetestruninfo:" + JsonUtility.ToJson(m_TestRun));
    }

    public void Setup()
    {
        m_TestRun = new PerformanceTestRun
        {
            EditorVersion = GetEditorInfo(),
            BuildSettings = GetPlayerBuildInfo(),
            StartTime = DateTime.Now.ToUniversalTime().Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc))
                .TotalMilliseconds
        };

        CreateStreamingAssetsFolder();
        CreatePerformanceTestRunJson();
    }

    private void CreateStreamingAssetsFolder()
    {
        if (!Directory.Exists(Application.streamingAssetsPath))
        {
            AssetDatabase.CreateFolder("Assets", "StreamingAssets");
        }
    }

    private void CreatePerformanceTestRunJson()
    {
        string json = JsonUtility.ToJson(m_TestRun, true);
        File.WriteAllText(m_TestRunPath, json);
        AssetDatabase.Refresh();
    }

    private PerformanceTestRun ReadPerformanceTestRunJson()
    {
        string json = File.ReadAllText(m_TestRunPath);
        return JsonUtility.FromJson<PerformanceTestRun>(json);
    }

    private static PlayerSystemInfo GetSystemInfo()
    {
        return new PlayerSystemInfo
        {
            OperatingSystem = SystemInfo.operatingSystem,
            DeviceModel = SystemInfo.deviceModel,
            DeviceName = SystemInfo.deviceName,
            ProcessorType = SystemInfo.processorType,
            ProcessorCount = SystemInfo.processorCount,
            GraphicsDeviceName = SystemInfo.graphicsDeviceName,
            SystemMemorySize = SystemInfo.systemMemorySize,
#if ENABLE_VR
            XrModel = UnityEngine.XR.XRDevice.model,
            XrDevice = UnityEngine.XR.XRSettings.loadedDeviceName
#endif
        };
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

    private static Unity.PerformanceTesting.BuildSettings GetPlayerBuildInfo()
    {
        var buildSettings = new Unity.PerformanceTesting.BuildSettings
        {
            Platform = Application.platform.ToString(),
            BuildTarget = UnityEditor.EditorUserBuildSettings.activeBuildTarget.ToString(),
            DevelopmentPlayer = UnityEditor.EditorUserBuildSettings.development,
            AndroidBuildSystem = UnityEditor.EditorUserBuildSettings.androidBuildSystem.ToString()
        };
        return buildSettings;
    }


    private static Unity.PerformanceTesting.PlayerSettings GetPlayerSettings()
    {
        return new Unity.PerformanceTesting.PlayerSettings()
        {
            VrSupported = UnityEditor.PlayerSettings.virtualRealitySupported,
            MtRendering = UnityEditor.PlayerSettings.MTRendering,
            GpuSkinning = UnityEditor.PlayerSettings.gpuSkinning,
            GraphicsJobs = UnityEditor.PlayerSettings.graphicsJobs,
            GraphicsApi =
                UnityEditor.PlayerSettings.GetGraphicsAPIs(UnityEditor.EditorUserBuildSettings.activeBuildTarget)[0]
                    .ToString(),
            ScriptingBackend = UnityEditor.PlayerSettings
                .GetScriptingBackend(UnityEditor.EditorUserBuildSettings.selectedBuildTargetGroup)
                .ToString(),
            StereoRenderingPath = UnityEditor.PlayerSettings.stereoRenderingPath.ToString(),
            RenderThreadingMode = UnityEditor.PlayerSettings.graphicsJobs ? "GraphicsJobs" :
                UnityEditor.PlayerSettings.MTRendering ? "MultiThreaded" : "SingleThreaded",
            AndroidMinimumSdkVersion = UnityEditor.PlayerSettings.Android.minSdkVersion.ToString(),
            AndroidTargetSdkVersion = UnityEditor.PlayerSettings.Android.targetSdkVersion.ToString(),
            Batchmode = UnityEditorInternal.InternalEditorUtility.inBatchMode.ToString(),
            ScriptingRuntimeVersion = UnityEditor.PlayerSettings.scriptingRuntimeVersion.ToString()
            // @TODO playerSettings.EnabledXrTargets need to set this from Prebuild Setup method
            //EnabledXrTargets = TODO
        };
        // TODO currently no api on 2018.1 
        //playerSettings.StaticBatching = TODO
        //playerSettings.DynamicBatching = TODO
        //PlayerSettings.GetBatchingForPlatform(EditorUserBuildSettings.activeBuildTarget, out pbi.staticBatching, out pbi.dynamicBatching);
    }
}
#endif