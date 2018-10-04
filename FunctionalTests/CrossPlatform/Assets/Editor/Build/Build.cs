using NDesk.Options;
using System;
using System.Collections;
using System.Collections.Generic;
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
    //private static bool mtRendering = true;
    //private static bool graphicsJobs;
    private static AndroidSdkVersions minimumAndroidSdkVersion = AndroidSdkVersions.AndroidApiLevel24;
    private static AndroidSdkVersions targetAndroidSdkVersion = AndroidSdkVersions.AndroidApiLevel24;

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
    
    public static void CommandLineBuild()
    {
        var args = System.Environment.GetCommandLineArgs();
        var optionSet = DefineOptionSet();

        var unprocessedArgs = optionSet.Parse(args);

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
                    PlayerSettings.SetGraphicsAPIs(EditorUserBuildSettings.activeBuildTarget, new GraphicsDeviceType[] { TryParse<GraphicsDeviceType>(graphicDevice) });
                    Build.Name = $"/{EditorUserBuildSettings.activeBuildTarget}/{vrsdk}/{stereoRenderingPath}-{graphicDevice}";
                    Build.BuildProject();
                }
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
                xrTargets => enabledXrTargets = ParseMultipleArgs(xrTargets)
            },
            {
                "playergraphicsapi=", "Graphics API based on GraphicsDeviceType.",
                graphicsDeviceTypes => playerGraphicsApis = ParseMultipleArgs(graphicsDeviceTypes)
            },
            {
                "stereoRenderingPath=", "Stereo rendering path to enable. SinglePass is default",
                stereoRenderingPathMode => stereoRenderingPaths = ParseMultipleArgs(stereoRenderingPathMode)
            }//,
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
            thisType = (T)Enum.Parse(typeof(T), stringToParse);
        }
        catch (Exception e)
        {
            throw new ArgumentException(string.Format("Couldn't cast {0} to {1}", stringToParse, typeof(T)), e);
        }
        return thisType;
    }

    private static string[] ParseMultipleArgs(string args)
    {
        return args.Split(';');
    }
}
