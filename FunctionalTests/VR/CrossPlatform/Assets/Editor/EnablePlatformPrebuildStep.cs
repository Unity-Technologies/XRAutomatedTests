using System;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering;
using UnityEngine.TestTools;
using NDesk.Options;

public class EnablePlatformPrebuildStep : IPrebuildSetup
{
    public void Setup()
    {
        var args = System.Environment.GetCommandLineArgs();

        if (args.Length <= 1)
        {
            switch (EditorUserBuildSettings.selectedBuildTargetGroup)
            {
                case BuildTargetGroup.Standalone:
                    PlatformSettings.enabledXrTargets = new string[] { "MockHMD", "None" };
                    PlatformSettings.stereoRenderingPath = StereoRenderingPath.SinglePass;
                    PlatformSettings.playerGraphicsApi =
                        (EditorUserBuildSettings.activeBuildTarget == BuildTarget.StandaloneWindows)
                            ? GraphicsDeviceType.Direct3D11
                            : GraphicsDeviceType.OpenGLCore;
                    PlatformSettings.mtRendering = true;
                    PlatformSettings.graphicsJobs = false;
                    break;
                case BuildTargetGroup.WSA:
                    // Configure WSA build
                    if (EditorUserBuildSettings.activeBuildTarget != BuildTarget.WSAPlayer && EditorUserBuildSettings.selectedBuildTargetGroup != BuildTargetGroup.WSA)
                    {
                        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.WSA, BuildTarget.WSAPlayer);
                    }
                    EditorUserBuildSettings.wsaUWPBuildType = WSAUWPBuildType.D3D;
                    EditorUserBuildSettings.wsaSubtarget = WSASubtarget.AnyDevice;
                    EditorUserBuildSettings.allowDebugging = true;

                    PlayerSettings.SetScriptingBackend(BuildTargetGroup.WSA, ScriptingImplementation.IL2CPP);
                    PlatformSettings.stereoRenderingPath = StereoRenderingPath.SinglePass;

                    PlatformSettings.enabledXrTargets = new string[] { "None" };
                    break;
                case BuildTargetGroup.Android:
                case BuildTargetGroup.iOS:
                    PlatformSettings.enabledXrTargets = new string[] { "cardboard", "None" };
                    PlatformSettings.stereoRenderingPath = StereoRenderingPath.SinglePass;
                    PlatformSettings.playerGraphicsApi = GraphicsDeviceType.OpenGLES3;
                    break;
            }
        }
        else
        {
            var optionSet = DefineOptionSet();
            var unprocessedArgs = optionSet.Parse(args);
        }

        ConfigureSettings();
        
        PlatformSettings.SerializeToAsset();

    }

    private void ConfigureSettings()
    {
        PlayerSettings.virtualRealitySupported = true;

        UnityEditorInternal.VR.VREditor.SetVREnabledDevicesOnTargetGroup(
            PlatformSettings.BuildTargetGroup,
            PlatformSettings.enabledXrTargets);

        if (PlatformSettings.enabledXrTargets.FirstOrDefault() != "None")
        {
            EditorUserBuildSettings.SwitchActiveBuildTarget(
                PlatformSettings.BuildTargetGroup,
                PlatformSettings.BuildTarget);
        }

        PlayerSettings.stereoRenderingPath = PlatformSettings.stereoRenderingPath;

        PlayerSettings.Android.minSdkVersion = PlatformSettings.minimumAndroidSdkVersion;
        EditorUserBuildSettings.androidBuildType = AndroidBuildType.Development;
        EditorUserBuildSettings.androidBuildSystem = AndroidBuildSystem.Gradle;
    }

    private static OptionSet DefineOptionSet()
    {
        return new OptionSet()
            {
                {
                    "enabledxrtarget=",
                    "XR target to enable in player settings. Values: \r\n\"Oculus\"\r\n\"OpenVR\"\r\n\"cardboard\"\r\n\"daydream\"\r\n\"MockHMD\"",
                    xrTarget => PlatformSettings.enabledXrTargets = new string[] {xrTarget, "None"}
                },

                {
                    "playergraphicsapi=", "Graphics API based on GraphicsDeviceType.",
                    graphicsDeviceType => PlatformSettings.playerGraphicsApi =
                        TryParse<GraphicsDeviceType>(graphicsDeviceType)
                },
                {
                    "stereorenderingpath=", "Stereo rendering path to enable. SinglePass is default",
                    stereoRenderingPath => PlatformSettings.stereoRenderingPath =
                        TryParse<StereoRenderingPath>(stereoRenderingPath)
                },
                {
                    "mtrendering", "Use multi threaded rendering; true is default.",
                    gfxMultithreaded =>
                    {
                        if (gfxMultithreaded.ToLower() == "true")
                        {
                            PlatformSettings.mtRendering = true;
                            PlatformSettings.graphicsJobs = false;
                        }
                    }
                },
                {
                    "graphicsjobs", "Use graphics jobs rendering; false is default.",
                    gfxJobs =>
                    {
                        if (gfxJobs.ToLower() == "true")
                        {
                            PlatformSettings.mtRendering = false;
                            PlatformSettings.graphicsJobs = true;
                        }
                    }
                },
                {
                    "minimumandroidsdkversion=", "Minimum Android SDK Version to use.",
                    minAndroidSdkVersion => PlatformSettings.minimumAndroidSdkVersion =
                        TryParse<AndroidSdkVersions>(minAndroidSdkVersion)
                },
                {
                    "targetandroidsdkversion=", "Target Android SDK Version to use.",
                    targetAndroidSdkVersion => PlatformSettings.targetAndroidSdkVersion =
                        TryParse<AndroidSdkVersions>(targetAndroidSdkVersion)
                }
            };
    }

    private static T TryParse<T>(string stringToParse)
    {
        T thisType;
        try
        {
            thisType = (T)Enum.Parse(typeof(T), stringToParse);
        }
        catch (Exception e)
        {
            throw new ArgumentException(($"Couldn't cast {stringToParse} to {typeof(T)}"), e);
        }

        return thisType;
    }

    private static string[] ParseMultipleArgs(string args)
    {
        return args.Split(';');
    }

    private BuildTargetGroup GetBuildTargetGroup(BuildTarget buildTarget)
    {
        switch (buildTarget)
        {
            case BuildTarget.StandaloneWindows64:
            case BuildTarget.StandaloneWindows:
            case BuildTarget.StandaloneOSX:
            case BuildTarget.StandaloneLinuxUniversal:
                {
                    return BuildTargetGroup.Standalone;
                }
            case BuildTarget.Android:
                {
                    return BuildTargetGroup.Android;
                }
            case BuildTarget.iOS:
                {
                    return BuildTargetGroup.iOS;
                }
            case BuildTarget.WSAPlayer:
                {
                    return BuildTargetGroup.WSA;
                }
            default:
                {
                    Debug.LogError("Unsupported build target.");
                    return BuildTargetGroup.Standalone;
                }
        }
    }
}

