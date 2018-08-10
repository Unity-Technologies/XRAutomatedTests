using System;
using System.IO;
using NDesk.Options;
using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;

public class EnablePlatformPrebuildStep : IPrebuildSetup
{
    private static string buildTarget;
    private static string [] enabledXrTargets;
    private static string playerGraphicsApi;

    private static string stereoRenderingPath;

    //private static bool mtRendering = true;
    //private static bool graphicsJobs;
    private static AndroidSdkVersions minimumAndroidSdkVersion = AndroidSdkVersions.AndroidApiLevel24;
    private static AndroidSdkVersions targetAndroidSdkVersion = AndroidSdkVersions.AndroidApiLevel24;

    public void Setup()
    {
        var args =
//            "-runTests -projectPath \\Oculus\\ -enabledxrtargets=Oculus -playergraphicsapi=OpenGL stereoRenderingPath=MultiPass -testResults tests\\results.xml -logfile log.txt -testPlatform playmode -buildTarget Android".Split(' ');
            System.Environment.GetCommandLineArgs();
            
        var optionSet = DefineOptionSet();
        
        var unprocessedArgs = optionSet.Parse(args);
        
        EditorUserBuildSettings.SwitchActiveBuildTarget(
            PlatformSettings.BuildTargetGroup,
            PlatformSettings.BuildTarget);

        PlayerSettings.virtualRealitySupported = true;
        StereoRenderingPath stereoSetting;
        StereoRenderingPath.TryParse(stereoRenderingPath, out stereoSetting);

        PlayerSettings.stereoRenderingPath = stereoSetting;


        UnityEditorInternal.VR.VREditor.SetVREnabledDevicesOnTargetGroup(
            PlatformSettings.BuildTargetGroup,
            PlatformSettings.enabledXrTargets);

        EditorUserBuildSettings.androidBuildType = AndroidBuildType.Development;
        PlayerSettings.Android.minSdkVersion = PlatformSettings.minimumAndroidSdkVersion;
        EditorUserBuildSettings.androidBuildSystem =
            AndroidBuildSystem
                .Gradle;

        CopyOculusSignatureFilesToProject();
        
        PlatformSettings.SerializeToAsset();
        
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
                xrTargets => PlatformSettings.enabledXrTargets = ParseMultipleArgs(xrTargets)
            },
            {
                "playergraphicsapi=", "Graphics API based on GraphicsDeviceType.",
                graphicsDeviceType => PlatformSettings.playerGraphicsApi = graphicsDeviceType
            },
            {
                "stereoRenderingPath=", "Stereo rendering path to enable. SinglePass is default",
                stereoRenderingPath =>
                    PlatformSettings.stereoRenderingPath = stereoRenderingPath
            } //,
            //{
            //    "mtRendering", "Use multi threaded rendering; true is default.",
            //    gfxMultithreaded =>
            //    {
            //        if (gfxMultithreaded.ToLower() == "true")
            //        {
            //            mtRendering = true;
            //            graphicsJobs = false;
            //        }
            //    }
            //},
            //{
            //    "graphicsJobs", "Use graphics jobs rendering; false is default.",
            //    gfxJobs =>
            //    {
            //        if (gfxJobs.ToLower() == "true")
            //        {
            //            mtRendering = false;
            //            graphicsJobs = true;
            //        }
            //    }
            //},
            //{
            //    "minimumandroidsdkversion=", "Minimum Android SDK Version to use.",
            //    minAndroidSdkVersion => minimumAndroidSdkVersion = TryParse<AndroidSdkVersions>(minAndroidSdkVersion)
            //},
            //{
            //    "targetandroidsdkversion=", "Target Android SDK Version to use.",
            //    trgtAndroidSdkVersion => targetAndroidSdkVersion = TryParse<AndroidSdkVersions>(trgtAndroidSdkVersion)
            //}
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