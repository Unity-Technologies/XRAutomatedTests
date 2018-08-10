using System;
using System.IO;
using NDesk.Options;
using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering;
using UnityEngine.TestTools;

public class EnablePlatformPrebuildStep : IPrebuildSetup
{
    public void Setup()
    {
        var args = System.Environment.GetCommandLineArgs();
            
        var optionSet = DefineOptionSet();
        
        var unprocessedArgs = optionSet.Parse(args);
        
        ConfigureSettings();

        CopyOculusSignatureFilesToProject();
        
        PlatformSettings.SerializeToAsset();
        
    }

    private static void ConfigureSettings()
    {
        EditorUserBuildSettings.SwitchActiveBuildTarget(
            PlatformSettings.BuildTargetGroup,
            PlatformSettings.BuildTarget);

        PlayerSettings.virtualRealitySupported = true;

        UnityEditorInternal.VR.VREditor.SetVREnabledDevicesOnTargetGroup(
            PlatformSettings.BuildTargetGroup,
            PlatformSettings.enabledXrTargets);

        PlayerSettings.Android.minSdkVersion = PlatformSettings.minimumAndroidSdkVersion;
        EditorUserBuildSettings.androidBuildType = AndroidBuildType.Development;
        EditorUserBuildSettings.androidBuildSystem = AndroidBuildSystem.Gradle;
    }

    private void CopyOculusSignatureFilesToProject()
    {
    var signatureFilePath =
        $"..{Path.DirectorySeparatorChar}..{Path.DirectorySeparatorChar}OculusSignatureFiles";
        var files = Directory.GetFiles(signatureFilePath);
        var assetsPluginPath =
            $"Assets{Path.DirectorySeparatorChar}Plugins{Path.DirectorySeparatorChar}Android{Path.DirectorySeparatorChar}assets";

        foreach (var file in files)
        {
            if (!File.Exists(assetsPluginPath+ file.Substring(file.LastIndexOf(Path.DirectorySeparatorChar))))
            {
                File.Copy(file, assetsPluginPath + file.Substring(file.LastIndexOf(Path.DirectorySeparatorChar)));
            }
        }
    }

    private static OptionSet DefineOptionSet()
    {
        return new OptionSet()
        {
            {
                "enabledxrtargets=",
                "XR targets to enable in player settings separated by ';'. Values: \r\n\"Oculus\"\r\n\"OpenVR\"\r\n\"cardboard\"\r\n\"daydream\"",
                xrTarget => PlatformSettings.enabledXrTargets = new string [] {xrTarget, "None"}
            },
            {
                "playergraphicsapi=", "Graphics API based on GraphicsDeviceType.",
                graphicsDeviceType => PlatformSettings.playerGraphicsApi = TryParse<GraphicsDeviceType>(graphicsDeviceType)
            },
            {
                "stereoRenderingPath=", "Stereo rendering path to enable. SinglePass is default",
                stereoRenderingPath => PlatformSettings.stereoRenderingPath = TryParse<StereoRenderingPath>(stereoRenderingPath)
            },
            {
                "mtRendering", "Use multi threaded rendering; true is default.",
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
                "graphicsJobs", "Use graphics jobs rendering; false is default.",
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
                minAndroidSdkVersion => PlatformSettings.minimumAndroidSdkVersion = TryParse<AndroidSdkVersions>(minAndroidSdkVersion)
            },
            {
                "targetandroidsdkversion=", "Target Android SDK Version to use.",
                trgtAndroidSdkVersion => PlatformSettings.targetAndroidSdkVersion = TryParse<AndroidSdkVersions>(trgtAndroidSdkVersion)
            }
        };
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