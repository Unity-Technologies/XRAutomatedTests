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
#if ENABLE_VR
using UnityEngine.XR;
#endif
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// The existence of this PlaymodeMetdataCollector class in the PerformanceBenchmark tests is a hack to
/// enable collection of test metadata when IL2CPP scriptingbackend is enabled. This same script is used in the
/// performance testing extension and works fine when mono scriptingbackend is used. However, not so much for IL2CPP
/// is used. This issue (most likely a JSON deserialization/stripping issue) is being investigated, but for now,
/// if you want to ensure test run metadata is included in the result when using IL2CPP, include this class/test in your project.
/// </summary>
[Category("Performance")]
public class PlaymodeMetadataCollector : IPrebuildSetup
{
    private PerformanceTestRun m_TestRun;

    private string m_TestRunPath
    {
        get { return Path.Combine(Application.streamingAssetsPath, "PerformanceTestRunInfo.json"); }
    }

    [UnityTest, Order(0), PrebuildSetup(typeof(RenderPerformancePrebuildStep))]
    public IEnumerator GetPlayerSettingsTest()
    {
        yield return ReadPerformanceTestRunJsonAsync();
        m_TestRun.PlayerSystemInfo = GetSystemInfo();
        m_TestRun.QualitySettings = GetQualitySettings();
        m_TestRun.ScreenSettings = GetScreenSettings();
        m_TestRun.TestSuite = "Playmode";
        m_TestRun.BuildSettings.Platform = Application.platform.ToString();

        // Begin hack within hack
        // It seem we're losing metadata potentially from deserialization. 
        // This hack seems to workaround the issue, resulting in more consistent 
        // metadata
        using (StreamWriter sw = new StreamWriter(Stream.Null))
        {
            // PlayerSystemInfo
            sw.Write(m_TestRun.PlayerSystemInfo.DeviceModel);
            sw.Write(m_TestRun.PlayerSystemInfo.DeviceName);
            sw.Write(m_TestRun.PlayerSystemInfo.GraphicsDeviceName);
            sw.Write(m_TestRun.PlayerSystemInfo.OperatingSystem);
            sw.Write(m_TestRun.PlayerSystemInfo.ProcessorCount);
            sw.Write(m_TestRun.PlayerSystemInfo.SystemMemorySize);
            sw.Write(m_TestRun.PlayerSystemInfo.XrDevice);
            sw.Write(m_TestRun.PlayerSystemInfo.XrModel);

            //PlayerSettings
            sw.Write(m_TestRun.PlayerSettings.AndroidMinimumSdkVersion);
            sw.Write(m_TestRun.PlayerSettings.AndroidTargetSdkVersion);
            sw.Write(m_TestRun.PlayerSettings.Batchmode);
            sw.Write(m_TestRun.PlayerSettings.EnabledXrTargets);
            sw.Write(m_TestRun.PlayerSettings.GpuSkinning);
            sw.Write(m_TestRun.PlayerSettings.GraphicsApi);
            sw.Write(m_TestRun.PlayerSettings.GraphicsJobs);
            sw.Write(m_TestRun.PlayerSettings.MtRendering);
            sw.Write(m_TestRun.PlayerSettings.RenderThreadingMode);
            sw.Write(m_TestRun.PlayerSettings.ScriptingBackend);
            sw.Write(m_TestRun.PlayerSettings.ScriptingRuntimeVersion);
            sw.Write(m_TestRun.PlayerSettings.StereoRenderingPath);
            sw.Write(m_TestRun.PlayerSettings.VrSupported);

            //QualitySettings
            sw.Write(m_TestRun.QualitySettings.AnisotropicFiltering);
            sw.Write(m_TestRun.QualitySettings.AntiAliasing);
            sw.Write(m_TestRun.QualitySettings.BlendWeights);
            sw.Write(m_TestRun.QualitySettings.ColorSpace);
            sw.Write(m_TestRun.QualitySettings.Vsync);

            //ScreenSettings
            sw.Write(m_TestRun.ScreenSettings.Fullscreen);
            sw.Write(m_TestRun.ScreenSettings.ScreenHeight);
            sw.Write(m_TestRun.ScreenSettings.ScreenRefreshRate);
            sw.Write(m_TestRun.ScreenSettings.ScreenWidth);

            //BuildSettings
            sw.Write(m_TestRun.BuildSettings.AndroidBuildSystem);
            sw.Write(m_TestRun.BuildSettings.BuildTarget);
            sw.Write(m_TestRun.BuildSettings.DevelopmentPlayer);
            sw.Write(m_TestRun.BuildSettings.Platform);

            //EditorSettings
            sw.Write(m_TestRun.EditorVersion.RevisionValue);
            sw.Write(m_TestRun.EditorVersion.FullVersion);
        }
        // End hack

        TestContext.Out.Write("##performancetestruninfo:" + JsonUtility.ToJson(m_TestRun));
    }

    private PerformanceTestRun ReadPerformanceTestRunJson()
    {
        try
        {
            string json;
            if (Application.platform == RuntimePlatform.Android)
            {
                UnityWebRequest reader = new UnityWebRequest("jar:file://" + m_TestRunPath);
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
            return new PerformanceTestRun { PlayerSettings = new Unity.PerformanceTesting.PlayerSettings() };
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
                m_TestRun = new PerformanceTestRun { PlayerSettings = new Unity.PerformanceTesting.PlayerSettings() };
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
            XrModel = XRDevice.model,
            GraphicsDeviceName = SystemInfo.graphicsDeviceName,
            SystemMemorySize = SystemInfo.systemMemorySize,
#if ENABLE_VR
            XrDevice = XRSettings.loadedDeviceName
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
            BlendWeights = UnityEngine.QualitySettings.blendWeights.ToString()
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
                return (string)method.Invoke(null, null);
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
            UnityEditor.PlayerSettings.GetGraphicsAPIs(EditorUserBuildSettings.activeBuildTarget)[0]
                .ToString();
        playerSettings.ScriptingBackend = UnityEditor.PlayerSettings
            .GetScriptingBackend(EditorUserBuildSettings.selectedBuildTargetGroup)
            .ToString();
        playerSettings.ScriptingRuntimeVersion = UnityEditor.PlayerSettings.scriptingRuntimeVersion.ToString();
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
            BuildTarget = EditorUserBuildSettings.activeBuildTarget.ToString(),
            DevelopmentPlayer = EditorUserBuildSettings.development,
            AndroidBuildSystem = EditorUserBuildSettings.androidBuildSystem.ToString()
        };
        return buildSettings;
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
#endif
    }
}
#endif