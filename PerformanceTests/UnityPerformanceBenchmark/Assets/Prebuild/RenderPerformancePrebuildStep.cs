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
    private ScriptingImplementation scriptingImplementation = ScriptingImplementation.IL2CPP;
    private bool mtRendering = true;
    private bool graphicsJobs = false;
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

        PlayerSettings.SplashScreen.showUnityLogo = false;

        // Setup all-inclusive player settings
        PlayerSettings.SetGraphicsAPIs(EditorUserBuildSettings.activeBuildTarget, new[] {playerGraphicsApi});
        PlayerSettings.MTRendering = mtRendering;
        PlayerSettings.graphicsJobs = graphicsJobs;
        PlayerSettings.SetScriptingBackend(EditorUserBuildSettings.selectedBuildTargetGroup, scriptingImplementation);
        

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
            PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.iOS, "com.unity3d.performance.benchmark");
            PlayerSettings.iOS.appleDeveloperTeamID = appleDeveloperTeamId;
            PlayerSettings.iOS.appleEnableAutomaticSigning = false;
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

        if (EditorUserBuildSettings.selectedBuildTargetGroup == BuildTargetGroup.Standalone)
        {
            SetQualitySettingsToUltra();
        }
        else
        {
            SetQualitySettingsToMedium();
        }

        var perfTestRun = ReadPerformanceTestRunJson();
        if (perfTestRun.PlayerSettings == null)
        {
            perfTestRun.PlayerSettings = new Unity.PerformanceTesting.PlayerSettings();
        }

        perfTestRun.PlayerSettings.EnabledXrTargets = enabledXrTargets;

    CreatePerformanceTestRunJson(perfTestRun);
#endif
    }

    private static void SetQualitySettingsToMedium()
    {
        QualitySettings.pixelLightCount = 1;
        QualitySettings.anisotropicFiltering = AnisotropicFiltering.Enable;
        QualitySettings.antiAliasing = 0;
        QualitySettings.softParticles = false;
        QualitySettings.realtimeReflectionProbes = false;
        QualitySettings.billboardsFaceCameraPosition = false;
        QualitySettings.resolutionScalingFixedDPIFactor = 1;
        QualitySettings.shadowmaskMode = ShadowmaskMode.Shadowmask;
        QualitySettings.shadows = ShadowQuality.HardOnly;
        QualitySettings.shadowResolution = ShadowResolution.Low;
        QualitySettings.shadowProjection = ShadowProjection.StableFit;
        QualitySettings.shadowDistance = 20;
        QualitySettings.shadowNearPlaneOffset = 3;
        QualitySettings.shadowCascades = 0;
        QualitySettings.blendWeights = BlendWeights.TwoBones;
        QualitySettings.vSyncCount = 2;
        QualitySettings.lodBias = 0.7f;
        QualitySettings.maximumLODLevel = 0;
        QualitySettings.particleRaycastBudget = 64;
        QualitySettings.asyncUploadTimeSlice = 2;
        QualitySettings.asyncUploadBufferSize = 4;
    }

    private static void SetQualitySettingsToUltra()
    {
        QualitySettings.pixelLightCount = 4;
        QualitySettings.anisotropicFiltering = AnisotropicFiltering.ForceEnable;
        QualitySettings.antiAliasing = 2;
        QualitySettings.softParticles = true;
        QualitySettings.realtimeReflectionProbes = true;
        QualitySettings.billboardsFaceCameraPosition = true;
        QualitySettings.resolutionScalingFixedDPIFactor = 1;
        QualitySettings.shadowmaskMode = ShadowmaskMode.DistanceShadowmask;
        QualitySettings.shadows = ShadowQuality.All;
        QualitySettings.shadowResolution = ShadowResolution.High;
        QualitySettings.shadowProjection = ShadowProjection.StableFit;
        QualitySettings.shadowDistance = 150;
        QualitySettings.shadowNearPlaneOffset = 3;
        QualitySettings.shadowCascades = 4;
        QualitySettings.blendWeights = BlendWeights.FourBones;
        QualitySettings.vSyncCount = 1;
        QualitySettings.lodBias = 2;
        QualitySettings.maximumLODLevel = 0;
        QualitySettings.particleRaycastBudget = 4096;
        QualitySettings.asyncUploadTimeSlice = 2;
        QualitySettings.asyncUploadBufferSize = 4;
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
        return new OptionSet()
            .Add("scriptingbackend=",
                "Scripting backend to use. IL2CPP is default. Values: IL2CPP, Mono",
                ParseScriptingBackend)
            .Add("enabledxrtargets=",
                "XR targets to enable in XR enabled players, separated by ';'. Values: \r\n\"Oculus\"\r\n\"OpenVR\"\r\n\"cardboard\"\r\n\"daydream\"",
                xrTargets => enabledXrTargets = ParseEnabledXrTargets(xrTargets))
            .Add("playergraphicsapi=", "Optionally force player to use the specified GraphicsDeviceType. Direct3D11, OpenGLES2, OpenGLES3, PlayStationVita, PlayStation4, XboxOne, Metal, OpenGLCore, Direct3D12, N3DS, Vulkan, Switch, XboxOneD3D12",
                (GraphicsDeviceType graphicsDeviceType) => playerGraphicsApi = graphicsDeviceType)
            .Add("stereoRenderingPath=", "StereoRenderingPath to use for XR enabled players. MultiPass, SinglePass, Instancing. Default is SinglePass.",
                stereoRenderingPathMode => stereoRenderingPath = TryParse<StereoRenderingPath>(stereoRenderingPathMode))
            .Add("mtRendering", "Enable or disable multithreaded rendering. Enabled is default. Use option to enable, or use option and append '-' to disable.",
                option => mtRendering = option != null)
            .Add("graphicsJobs", "Enable graphics jobs rendering. Disabled is default. Use option to enable, or use option and append '-' to disable.",
                option => graphicsJobs = option != null)
            .Add("minimumandroidsdkversion=", "Minimum Android SDK Version to use. Default is AndroidApiLevel24. Use for deployment and running tests on Android device.",
                minAndroidSdkVersion => minimumAndroidSdkVersion = TryParse<AndroidSdkVersions>(minAndroidSdkVersion))
            .Add("targetandroidsdkversion=", "Target Android SDK Version to use. Default is AndroidApiLevel24. Use for deployment and running tests on Android device.",
                trgtAndroidSdkVersion => targetAndroidSdkVersion = TryParse<AndroidSdkVersions>(trgtAndroidSdkVersion))
            .Add("appleDeveloperTeamID=", "Apple Developer Team ID. Use for deployment and running tests on iOS device.",
                appleTeamId => appleDeveloperTeamId = appleTeamId)
            .Add("iOSProvisioningProfileID=", "iOS Provisioning Profile ID. Use for deployment and running tests on iOS device.",
                id => iOsProvisioningProfileId = id);
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

    private void ParseScriptingBackend(string scriptingBackend)
    {
        var sb = scriptingBackend.ToLower();
        if (sb.Equals("mono"))
        {
            scriptingImplementation = ScriptingImplementation.Mono2x;
        } else if (sb.Equals("il2cpp"))
        {
            scriptingImplementation = ScriptingImplementation.IL2CPP;
        }
        else
        {
            throw new ArgumentException(string.Format("Unrecognized scripting backend {0}. Valid options are Mono or IL2CPP", scriptingBackend));
        }


    }
#endif
}
