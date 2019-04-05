using NDesk.Options;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

public class Build
{
    public static string Name = "test";

    private static string buildTarget;
    private static string[] enabledXrTargets;
    private static string[] playerGraphicsApis;
    private static string[] stereoRenderingPaths;

    [MenuItem("Build/Build Project")]
    public static void BuildProject()
    {
        string path = Application.dataPath + "/../Builds/";

        string extension = string.Empty;

        switch (EditorUserBuildSettings.activeBuildTarget)
        {
            case BuildTarget.StandaloneOSX:
                extension = ".app";
                break;
            case BuildTarget.StandaloneWindows64:
            case BuildTarget.StandaloneWindows:
                extension = ".exe";
                break;
            case BuildTarget.iOS:
                extension = ".ipa";
                break;
            case BuildTarget.Android:
                extension = ".apk";
                break;
            case BuildTarget.StandaloneLinux:
                break;
            case BuildTarget.WSAPlayer:
                break;
            case BuildTarget.StandaloneLinux64:
            case BuildTarget.StandaloneLinuxUniversal:
                break;
            case BuildTarget.PS4:
                break;
        }

        if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.WSAPlayer)
        {
            HoloToolkit.Unity.HoloToolkitCommands.BuildSLNAndAPPX(path + $"{Name}");
        }
        else
        {
            // Build player.
            BuildPipeline.BuildPlayer(EditorBuildSettings.scenes, path + $"{Name}" + $"{extension}", EditorUserBuildSettings.activeBuildTarget, BuildOptions.None);
        }
    }
    
    
    [MenuItem("Build/Default Setup")]
    public static void CommandLineSetup()
    {
        var args = Environment.GetCommandLineArgs();
        
        var optionSet = DefineOptionSet();
        var unprocessedArgs = optionSet.Parse(args);

        if (args.Length == unprocessedArgs.Count)
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

        ConfigureSettings();
        
        PlatformSettings.SerializeToAsset();

    }
    
    public static void CommandLineBuild()
    {
        CommandLineSetup();
        
        foreach (var vrsdk in enabledXrTargets)
        {
            foreach (var stereoRenderingPath in stereoRenderingPaths)
            {
                foreach (var graphicDevice in playerGraphicsApis)
                {

                    UnityEditorInternal.VR.VREditor.SetVREnabledDevicesOnTargetGroup(
                                                                            EditorUserBuildSettings.selectedBuildTargetGroup,
                                                                            new[] { vrsdk });
                    PlayerSettings.stereoRenderingPath = TryParse<StereoRenderingPath>(stereoRenderingPath);
                    PlayerSettings.SetGraphicsAPIs(EditorUserBuildSettings.activeBuildTarget, new [] { TryParse<GraphicsDeviceType>(graphicDevice) });
                    Name = $"/{EditorUserBuildSettings.activeBuildTarget}/{vrsdk}/{stereoRenderingPath}-{graphicDevice}";
                    BuildProject();
                }
            }
        }

    }
    
    private static void ConfigureSettings()
    {
        PlayerSettings.virtualRealitySupported = true;

        // Remove any existing VR targets before we add any; helps to ensure the correct packages are loaded for
        // each VR sdk when we set them below.
        UnityEditorInternal.VR.VREditor.SetVREnabledDevicesOnTargetGroup(
            EditorUserBuildSettings.selectedBuildTargetGroup,
            new string[]{});

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

        if (PlatformSettings.BuildTarget == BuildTarget.iOS || PlatformSettings.BuildTarget == BuildTarget.Android)
        {
            EditorUserBuildSettings.development = true;
        }
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
}
