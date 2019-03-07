#if UNITY_2018_1_OR_NEWER
using System;
using System.Collections;
using System.IO;
using System.Threading;
using NUnit.Framework;
using Unity.PerformanceTesting;
using Unity.PerformanceTesting.Runtime;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.TestTools;

[Category("Performance")]
public class PlaymodeMetadataCollector : IPrebuildSetup
{
    private PerformanceTestRun m_TestRun;

    private string m_TestRunPath
    {
        get { return Path.Combine(Application.streamingAssetsPath, "PerformanceTestRunInfo.json"); }
    }

    [UnityTest, Order(0), PrebuildSetup(typeof(PlaymodeMetadataCollector))]
    public IEnumerator GetPlayerSettingsTest()
    {
        yield return ReadPerformanceTestRunJsonAsync();
        m_TestRun.PlayerSystemInfo = GetSystemInfo();
        m_TestRun.QualitySettings = GetQualitySettings();
        m_TestRun.ScreenSettings = GetScreenSettings();
        m_TestRun.TestSuite = "Playmode";
        m_TestRun.BuildSettings.Platform = Application.platform.ToString();

        TestContext.Out.Write("##performancetestruninfo:" + JsonUtility.ToJson(m_TestRun));
    }

    private PerformanceTestRun ReadPerformanceTestRunJson()
    {
        try
        {
            string json;
            if (Application.platform == RuntimePlatform.Android)
            {
                UnityWebRequest reader = new UnityWebRequest("jar:file://" +m_TestRunPath);
                while (!reader.isDone)
                {
                    Thread.Sleep(1);
                }

                json = reader.downloadHandler.text;
            }
            else
            {
                json = File.ReadAllText(m_TestRunPath);
            }

            return JsonUtility.FromJson<PerformanceTestRun>(json);
        }
        catch
        {
            return new PerformanceTestRun {PlayerSettings = new Unity.PerformanceTesting.PlayerSettings()};
        }
    }


    private IEnumerator ReadPerformanceTestRunJsonAsync()
    {
        string json;
        if (Application.platform == RuntimePlatform.Android)
        {
            var path = m_TestRunPath;
            UnityWebRequest reader = UnityWebRequest.Get(path);
            yield return reader.SendWebRequest();

            while (!reader.isDone)
            {
                yield return null;
            }

            json = reader.downloadHandler.text;
        }
        else
        {
            if (!File.Exists(m_TestRunPath))
            {
                m_TestRun = new PerformanceTestRun {PlayerSettings = new Unity.PerformanceTesting.PlayerSettings()};
                yield break;
            }
            json = File.ReadAllText(m_TestRunPath);
        }

        m_TestRun = JsonUtility.FromJson<PerformanceTestRun>(json);
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

    private static Unity.PerformanceTesting.QualitySettings GetQualitySettings()
    {
        return new Unity.PerformanceTesting.QualitySettings()
        {
            Vsync = UnityEngine.QualitySettings.vSyncCount,
            AntiAliasing = UnityEngine.QualitySettings.antiAliasing,
            ColorSpace = UnityEngine.QualitySettings.activeColorSpace.ToString(),
            AnisotropicFiltering = UnityEngine.QualitySettings.anisotropicFiltering.ToString(),
#if UNITY_2019_1_OR_NEWER
            BlendWeights = UnityEngine.QualitySettings.skinWeights.ToString()
#else
            BlendWeights = UnityEngine.QualitySettings.blendWeights.ToString()
#endif
        };
    }

    private static ScreenSettings GetScreenSettings()
    {
        return new ScreenSettings
        {
            ScreenRefreshRate = Screen.currentResolution.refreshRate,
            ScreenWidth = Screen.currentResolution.width,
            ScreenHeight = Screen.currentResolution.height,
            Fullscreen = Screen.fullScreen
        };
    }

    public void Setup()
    {
#if UNITY_EDITOR
        m_TestRun = ReadPerformanceTestRunJson();
        m_TestRun.EditorVersion = GetEditorInfo();
        m_TestRun.PlayerSettings = GetPlayerSettings(m_TestRun.PlayerSettings);
        m_TestRun.BuildSettings = GetPlayerBuildInfo();
        m_TestRun.StartTime = Utils.DateToInt(DateTime.Now);

        CreateStreamingAssetsFolder();
        CreatePerformanceTestRunJson();
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

    private static Unity.PerformanceTesting.PlayerSettings GetPlayerSettings(
        Unity.PerformanceTesting.PlayerSettings playerSettings)
    {
        playerSettings.VrSupported = UnityEditor.PlayerSettings.virtualRealitySupported;
        playerSettings.MtRendering = UnityEditor.PlayerSettings.MTRendering;
        playerSettings.GpuSkinning = UnityEditor.PlayerSettings.gpuSkinning;
        playerSettings.GraphicsJobs = UnityEditor.PlayerSettings.graphicsJobs;
        playerSettings.GraphicsApi =
            UnityEditor.PlayerSettings.GetGraphicsAPIs(UnityEditor.EditorUserBuildSettings.activeBuildTarget)[0]
                .ToString();
        playerSettings.ScriptingBackend = UnityEditor.PlayerSettings
            .GetScriptingBackend(UnityEditor.EditorUserBuildSettings.selectedBuildTargetGroup)
            .ToString();
        playerSettings.StereoRenderingPath = UnityEditor.PlayerSettings.stereoRenderingPath.ToString();
        playerSettings.RenderThreadingMode = UnityEditor.PlayerSettings.graphicsJobs ? "GraphicsJobs" :
            UnityEditor.PlayerSettings.MTRendering ? "MultiThreaded" : "SingleThreaded";
        playerSettings.AndroidMinimumSdkVersion = UnityEditor.PlayerSettings.Android.minSdkVersion.ToString();
        playerSettings.AndroidTargetSdkVersion = UnityEditor.PlayerSettings.Android.targetSdkVersion.ToString();
        playerSettings.Batchmode = UnityEditorInternal.InternalEditorUtility.inBatchMode.ToString();
        return playerSettings;
        // Currently no API on 2018.1 
        //playerSettings.StaticBatching = TODO
        //playerSettings.DynamicBatching = TODO
        //PlayerSettings.GetBatchingForPlatform(EditorUserBuildSettings.activeBuildTarget, out pbi.staticBatching, out pbi.dynamicBatching);
    }

    private static BuildSettings GetPlayerBuildInfo()
    {
        var buildSettings = new BuildSettings
        {
            BuildTarget = UnityEditor.EditorUserBuildSettings.activeBuildTarget.ToString(),
            DevelopmentPlayer = UnityEditor.EditorUserBuildSettings.development,
            AndroidBuildSystem = UnityEditor.EditorUserBuildSettings.androidBuildSystem.ToString()
        };
        return buildSettings;
    }

    private void CreateStreamingAssetsFolder()
    {
        if (!Directory.Exists(Application.streamingAssetsPath))
        {
            UnityEditor.AssetDatabase.CreateFolder("Assets", "StreamingAssets");
        }
    }

    private void CreatePerformanceTestRunJson()
    {
        string json = JsonUtility.ToJson(m_TestRun, true);
        File.WriteAllText(m_TestRunPath, json);
        UnityEditor.AssetDatabase.Refresh();
#endif
    }
}
#endif