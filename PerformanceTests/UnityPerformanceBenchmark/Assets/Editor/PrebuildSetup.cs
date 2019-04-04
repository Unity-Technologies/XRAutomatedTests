#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using NDesk.Options;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using QualitySettings = UnityEngine.QualitySettings;
using PlayerSettings = UnityEditor.PlayerSettings;


namespace Assets.Editor
{
    public class RenderPerformancePrebuildStep
    {
        private static List<string> enabledXrTargets = new List<string>();
        private static GraphicsDeviceType playerGraphicsApi;
        private static StereoRenderingPath stereoRenderingPath = StereoRenderingPath.SinglePass;
        private static ScriptingImplementation scriptingImplementation = ScriptingImplementation.IL2CPP;
        private static bool mtRendering = true;
        private static bool graphicsJobs = false;
        private static AndroidSdkVersions minimumAndroidSdkVersion = AndroidSdkVersions.AndroidApiLevel25;
        private static AndroidSdkVersions targetAndroidSdkVersion = AndroidSdkVersions.AndroidApiLevelAuto;
        private static string appleDeveloperTeamId;
        private static string iOsProvisioningProfileId;

        private static string TestRunPath
        {
            get { return Path.Combine(Application.streamingAssetsPath, "PerformanceTestRunInfo.json"); }
        }

        public static void Setup()
        {
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
            PlayerSettings.SetScriptingBackend(EditorUserBuildSettings.selectedBuildTargetGroup,
                scriptingImplementation);


            // If Android, setup Android player settings
            if (EditorUserBuildSettings.selectedBuildTargetGroup == BuildTargetGroup.Android)
            {
                EditorUserBuildSettings.androidBuildType = AndroidBuildType.Development;
                EditorUserBuildSettings.androidBuildSystem = AndroidBuildSystem.Gradle;
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
                    new string[]{});

                UnityEditorInternal.VR.VREditor.SetVREnabledDevicesOnTargetGroup(
                    EditorUserBuildSettings.selectedBuildTargetGroup,
                    enabledXrTargets.ToArray());
                PlayerSettings.stereoRenderingPath = stereoRenderingPath;
            }

            switch (EditorUserBuildSettings.selectedBuildTargetGroup)
            {
                case BuildTargetGroup.Standalone:
                    SetQualitySettingsToUltra();
                    break;
                case BuildTargetGroup.Android:
                case BuildTargetGroup.iOS:
                    SetQualitySettingsToMobile();
                    break;
                default:
                    SetQualitySettingsToMedium();
                    break;
            }
        }

        private static T TryParse<T>(string stringToParse)
        {
            T thisType;
            try
            {
                thisType = (T) Enum.Parse(typeof(T), stringToParse);
            }
            catch (Exception e)
            {
                throw new ArgumentException(string.Format("Couldn't cast {0} to {1}", stringToParse, typeof(T)), e);
            }

            return thisType;
        }

        private static List<string> ParseEnabledXrTargets(string paths)
        {
            var targets = paths.Split(';');

            var vrTargets = new List<string>();

            foreach (var target in targets)
            {
                vrTargets.Add(target);
            }

            return vrTargets;
        }

        private static void ParseScriptingBackend(string scriptingBackend)
        {
            var sb = scriptingBackend.ToLower();
            if (sb.Equals("mono"))
            {
                scriptingImplementation = ScriptingImplementation.Mono2x;
            }
            else if (sb.Equals("il2cpp"))
            {
                scriptingImplementation = ScriptingImplementation.IL2CPP;
            }
            else
            {
                throw new ArgumentException(string.Format(
                    "Unrecognized scripting backend {0}. Valid options are Mono or IL2CPP", scriptingBackend));
            }
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
#if UNITY_2019_1_OR_NEWER
            QualitySettings.skinWeights = SkinWeights.TwoBones;
#else
            QualitySettings.blendWeights = BlendWeights.TwoBones;
#endif
            QualitySettings.vSyncCount = 2;
            QualitySettings.lodBias = 0.7f;
            QualitySettings.maximumLODLevel = 0;
            QualitySettings.particleRaycastBudget = 64;
            QualitySettings.asyncUploadTimeSlice = 2;
            QualitySettings.asyncUploadBufferSize = 4;
        }

        private static void SetQualitySettingsToMobile()
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
            QualitySettings.skinWeights = SkinWeights.TwoBones;
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
            QualitySettings.skinWeights = SkinWeights.FourBones;
            QualitySettings.vSyncCount = 1;
            QualitySettings.lodBias = 2;
            QualitySettings.maximumLODLevel = 0;
            QualitySettings.particleRaycastBudget = 4096;
            QualitySettings.asyncUploadTimeSlice = 2;
            QualitySettings.asyncUploadBufferSize = 4;
        }

        private static OptionSet DefineOptionSet()
        {
            return new OptionSet()
                .Add("scriptingbackend=",
                    "Scripting backend to use. IL2CPP is default. Values: IL2CPP, Mono",
                    ParseScriptingBackend)
                .Add("enabledxrtargets=",
                    "XR targets to enable in XR enabled players, separated by ';'. Values: \r\n\"Oculus\"\r\n\"OpenVR\"\r\n\"cardboard\"\r\n\"daydream\"",
                    xrTargets => enabledXrTargets = ParseEnabledXrTargets(xrTargets))
                .Add("playergraphicsapi=",
                    "Optionally force player to use the specified GraphicsDeviceType. Direct3D11, OpenGLES2, OpenGLES3, PlayStationVita, PlayStation4, XboxOne, Metal, OpenGLCore, Direct3D12, N3DS, Vulkan, Switch, XboxOneD3D12",
                    (GraphicsDeviceType graphicsDeviceType) => playerGraphicsApi = graphicsDeviceType)
                .Add("stereoRenderingPath=",
                    "StereoRenderingPath to use for XR enabled players. MultiPass, SinglePass, Instancing. Default is SinglePass.",
                    stereoRenderingPathMode =>
                        stereoRenderingPath = TryParse<StereoRenderingPath>(stereoRenderingPathMode))
                .Add("mtRendering",
                    "Enable or disable multithreaded rendering. Enabled is default. Use option to enable, or use option and append '-' to disable.",
                    option => mtRendering = option != null)
                .Add("graphicsJobs",
                    "Enable graphics jobs rendering. Disabled is default. Use option to enable, or use option and append '-' to disable.",
                    option => graphicsJobs = option != null)
                .Add("minimumandroidsdkversion=",
                    "Minimum Android SDK Version to use. Default is AndroidApiLevel24. Use for deployment and running tests on Android device.",
                    minAndroidSdkVersion =>
                        minimumAndroidSdkVersion = TryParse<AndroidSdkVersions>(minAndroidSdkVersion))
                .Add("targetandroidsdkversion=",
                    "Target Android SDK Version to use. Default is AndroidApiLevel24. Use for deployment and running tests on Android device.",
                    trgtAndroidSdkVersion =>
                        targetAndroidSdkVersion = TryParse<AndroidSdkVersions>(trgtAndroidSdkVersion))
                .Add("appleDeveloperTeamID=",
                    "Apple Developer Team ID. Use for deployment and running tests on iOS device.",
                    appleTeamId => appleDeveloperTeamId = appleTeamId)
                .Add("iOSProvisioningProfileID=",
                    "iOS Provisioning Profile ID. Use for deployment and running tests on iOS device.",
                    id => iOsProvisioningProfileId = id);
        }
    }
}
#endif
