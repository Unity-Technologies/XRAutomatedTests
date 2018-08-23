using System;
using System.Collections.Generic;
using System.IO;
using NDesk.Options;
using Unity.PerformanceTesting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.TestTools;
using QualitySettings = UnityEngine.QualitySettings;
#if UNITY_EDITOR
using UnityEditor;
using PlayerSettings = UnityEditor.PlayerSettings;
#endif

public class RenderPerformancePrebuildStep : IPrebuildSetup
{
#if UNITY_EDITOR
    private List<string> enabledXrTargets = new List<string>();
    private GraphicsDeviceType playerGraphicsApi;
    private StereoRenderingPath stereoRenderingPath = StereoRenderingPath.SinglePass;
    private bool mtRendering = true;
    private bool graphicsJobs;
    private AndroidSdkVersions minimumAndroidSdkVersion = AndroidSdkVersions.AndroidApiLevel24;
    private AndroidSdkVersions targetAndroidSdkVersion = AndroidSdkVersions.AndroidApiLevel24;
    private string appleDeveloperTeamId;
    private string iOsProvisioningProfileId;

    private string TestRunPath
    {
        get { return Path.Combine(Application.streamingAssetsPath, "PerformanceTestRunInfo.json"); }
    }
#endif
    public void Setup()
    {
#if UNITY_EDITOR
        // Get and parse args for player settings
        var args = Environment.GetCommandLineArgs();
        var optionSet = DefineOptionSet();
        var unprocessedArgs = optionSet.Parse(args);

        // Performance tests always need to be run as development build in order to capture performance profiler data
        EditorUserBuildSettings.development = true;
        PlayerSettings.SplashScreen.showUnityLogo = false;

        // Setup all-inclusive player settings
        PlayerSettings.SetGraphicsAPIs(EditorUserBuildSettings.activeBuildTarget, new[] {playerGraphicsApi});
        PlayerSettings.MTRendering = mtRendering;
        PlayerSettings.graphicsJobs = graphicsJobs;

        // If Android, setup Android player settings
        if (EditorUserBuildSettings.selectedBuildTargetGroup == BuildTargetGroup.Android)
        {
            EditorUserBuildSettings.androidBuildType = AndroidBuildType.Development;
            EditorUserBuildSettings.androidBuildSystem = AndroidBuildSystem.Internal;
            PlayerSettings.Android.minSdkVersion = minimumAndroidSdkVersion;
            PlayerSettings.Android.targetSdkVersion = targetAndroidSdkVersion;
        }

        // If iOS, setup iOS player settings
        if (EditorUserBuildSettings.selectedBuildTargetGroup == BuildTargetGroup.iOS)
        {
            PlayerSettings.iOS.appleDeveloperTeamID = appleDeveloperTeamId;
            PlayerSettings.iOS.iOSManualProvisioningProfileID = iOsProvisioningProfileId;
            PlayerSettings.iOS.iOSManualProvisioningProfileType = ProvisioningProfileType.Development;
        }

        PlayerSettings.virtualRealitySupported = enabledXrTargets.Count > 0;
        if (PlayerSettings.virtualRealitySupported)
        {
            PlayerSettings.virtualRealitySupported = true;
            UnityEditorInternal.VR.VREditor.SetVREnabledDevicesOnTargetGroup(
                EditorUserBuildSettings.selectedBuildTargetGroup,
                enabledXrTargets.ToArray());
            PlayerSettings.stereoRenderingPath = stereoRenderingPath;
        }

        QualitySettings.antiAliasing = 2;

        var perfTestRun = ReadPerformanceTestRunJson();
        if (perfTestRun.PlayerSettings == null)
        {
            perfTestRun.PlayerSettings = new Unity.PerformanceTesting.PlayerSettings();
        }

        perfTestRun.PlayerSettings.EnabledXrTargets = enabledXrTargets;

    CreatePerformanceTestRunJson(perfTestRun);
#endif
    }

#if UNITY_EDITOR
    private PerformanceTestRun ReadPerformanceTestRunJson()
    {
        string json;
        if (Application.platform == RuntimePlatform.Android)
        {
            var reader = new WWW(TestRunPath);
            while (!reader.isDone)
            {
            }
            json = reader.text;
        }
        else
        {
            json = File.ReadAllText(TestRunPath);
        }

        try
        {
            return JsonUtility.FromJson<PerformanceTestRun>(json);
        }
        catch
        {
            return new PerformanceTestRun();
        }
    }

    private void CreatePerformanceTestRunJson(PerformanceTestRun perfTestRun)
    {
        var json = JsonUtility.ToJson(perfTestRun, true);
        File.WriteAllText(TestRunPath, json);
        AssetDatabase.Refresh();
    }

    private OptionSet DefineOptionSet()
    {
        return new OptionSet
        {
            {
                "enabledxrtargets=",
                "XR targets to enable in player settings separated by ';'. Values: \r\n\"Oculus\"\r\n\"OpenVR\"\r\n\"cardboard\"\r\n\"daydream\"",
                xrTargets => enabledXrTargets = ParseEnabledXrTargets(xrTargets)
            },
            {
                "playergraphicsapi=", "Graphics API based on GraphicsDeviceType.",
                (GraphicsDeviceType graphicsDeviceType) => playerGraphicsApi = graphicsDeviceType
            },
            {
                "stereoRenderingPath=", "Stereo rendering path to enable. SinglePass is default",
                stereoRenderingPathMode => stereoRenderingPath = TryParse<StereoRenderingPath>(stereoRenderingPathMode)
            },
            {
                "mtRendering=", "Use multi threaded rendering; true is default.",
                gfxMultithreaded =>
                {
                    if (gfxMultithreaded.ToLower() == "false")
                    {
                        mtRendering = false;
                        
                    }
                    graphicsJobs = false;
                }
            },
            {
                "graphicsJobs=", "Use graphics jobs rendering; false is default.",
                gfxJobs =>
                {
                    if (gfxJobs.ToLower() == "true")
                    {
                        mtRendering = false;
                        graphicsJobs = true;
                    }
                }
            },
            {
                "minimumandroidsdkversion=", "Minimum Android SDK Version to use.",
                minAndroidSdkVersion => minimumAndroidSdkVersion = TryParse<AndroidSdkVersions>(minAndroidSdkVersion)
            },
            {
                "targetandroidsdkversion=", "Target Android SDK Version to use.",
                trgtAndroidSdkVersion => targetAndroidSdkVersion = TryParse<AndroidSdkVersions>(trgtAndroidSdkVersion)
            },
            {
                "appleDeveloperTeamID=", "Apple Developer Team ID",
                appleTeamId => appleDeveloperTeamId = appleTeamId
            },
            {
                "iOSProvisioningProfileID=", "iOS Provisioning Profile ID",
                id => iOsProvisioningProfileId = id
            }

        };
    }

    private T TryParse<T>(string stringToParse)
    {
        T thisType;
        try
        {
            thisType = (T)Enum.Parse(typeof(T), stringToParse);
        }
        catch (Exception e)
        {
            throw new ArgumentException(string.Format("Couldn't cast {0} to {1}", stringToParse, typeof(T)), e);
        }
        return thisType;
    }

    private List<string> ParseEnabledXrTargets(string paths)
    {
        var targets = paths.Split(';');

        var vrTargets = new List<string>();

        foreach (var target in targets)
        {
            vrTargets.Add(target);
        }

        return vrTargets;
    }
#endif
}
