using NDesk.Options;
using System;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

public class Build
{
    public static string Name = "test";

    private static string buildTarget;
    private static readonly Regex CustomArgRegex = new Regex("-([^=]*)=", RegexOptions.Compiled);

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
            // Build player.
            BuildPipeline.BuildPlayer(EditorBuildSettings.scenes, path + $"{Name}" + $"{extension}", EditorUserBuildSettings.activeBuildTarget, BuildOptions.None);
    }
    
    
    [MenuItem("Build/Default Setup")]
    public static void CommandLineSetup()
    {
        var args = Environment.GetCommandLineArgs();

        EnsureOptionsLowerCased(args);

        var optionSet = DefineOptionSet();
        var unprocessedArgs = optionSet.Parse(args);

        if (args.Length == unprocessedArgs.Count)
        {
            switch (EditorUserBuildSettings.selectedBuildTargetGroup)
            {
                case BuildTargetGroup.Standalone:
                    PlatformSettings.EnabledXrTargets = new string[] { "MockHMD", "None" };
                    PlatformSettings.StereoRenderingPath = StereoRenderingPath.SinglePass;
                    PlatformSettings.PlayerGraphicsApi =
                        (EditorUserBuildSettings.activeBuildTarget == BuildTarget.StandaloneWindows)
                            ? GraphicsDeviceType.Direct3D11
                            : GraphicsDeviceType.OpenGLCore;
                    PlatformSettings.MtRendering = true;
                    PlatformSettings.GraphicsJobs = false;
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
                    PlatformSettings.StereoRenderingPath = StereoRenderingPath.SinglePass;

                    PlatformSettings.EnabledXrTargets = new string[] { "None" };
                    break;
                case BuildTargetGroup.Android:
                case BuildTargetGroup.iOS:
                    PlatformSettings.EnabledXrTargets = new string[] { "cardboard", "None" };
                    PlatformSettings.StereoRenderingPath = StereoRenderingPath.SinglePass;
                    PlatformSettings.PlayerGraphicsApi = GraphicsDeviceType.OpenGLES3;
                    break;
            }
        }

        ConfigureSettings();
        
        PlatformSettings.SerializeToAsset();

    }

    private static void EnsureOptionsLowerCased(string[] args)
    {
        for (int i = 0; i < args.Length; i++)
        {
            if (CustomArgRegex.IsMatch(args[i]))
            {
                args[i] = CustomArgRegex.Replace(args[i], CustomArgRegex.Matches(args[i])[0].ToString().ToLower());
            }
        }
    }
    
    private static void ConfigureSettings()
    {
        PlayerSettings.virtualRealitySupported = PlatformSettings.EnabledXrTargets.Length > 0;

        if (PlayerSettings.virtualRealitySupported)
        {
            UnityEditorInternal.VR.VREditor.SetVREnabledDevicesOnTargetGroup(
                EditorUserBuildSettings.selectedBuildTargetGroup,
                PlatformSettings.EnabledXrTargets);
            Debug.Log(string.Format("VR Enabled Devices on Target Group: {0}",string.Join(", ",UnityEditorInternal.VR.VREditor.GetVREnabledDevicesOnTargetGroup(EditorUserBuildSettings
                .selectedBuildTargetGroup))));

            PlayerSettings.stereoRenderingPath = PlatformSettings.StereoRenderingPath;
        }
       
        PlayerSettings.Android.minSdkVersion = PlatformSettings.MinimumAndroidSdkVersion;

        EditorUserBuildSettings.androidBuildType = AndroidBuildType.Development;
        EditorUserBuildSettings.androidBuildSystem = AndroidBuildSystem.Gradle;

        if (PlatformSettings.BuildTarget == BuildTarget.iOS || PlatformSettings.BuildTarget == BuildTarget.Android)
        {
            EditorUserBuildSettings.development = true;
        }

        PlayerSettings.SetGraphicsAPIs(PlatformSettings.BuildTarget, new[] { PlatformSettings.PlayerGraphicsApi });
    }

    private static OptionSet DefineOptionSet()
    {
        return new OptionSet()
        {
            {
                "simulationmode=",
                "Enable Simulation modes for Windows MR in Editor. Values: \r\n\"HoloLens\"\r\n\"WindowsMR\"\r\n\"Remoting\"",
                simMode => PlatformSettings.SimulationMode = simMode
            },
            {
                "enabledxrtarget=",
                "XR target to enable in player settings. Values: \r\n\"Oculus\"\r\n\"OpenVR\"\r\n\"cardboard\"\r\n\"daydream\"\r\n\"MockHMD\"",
                xrTarget => PlatformSettings.EnabledXrTargets = new string[] {xrTarget, "None"}
            },

            {
                "playergraphicsapi=", "Graphics API based on GraphicsDeviceType.",
                graphicsDeviceType => PlatformSettings.PlayerGraphicsApi =
                    TryParse<GraphicsDeviceType>(graphicsDeviceType)
            },
            {
                "stereorenderingpath=", "Stereo rendering path to enable. SinglePass is default",
                stereoRenderingPath => PlatformSettings.StereoRenderingPath =
                    TryParse<StereoRenderingPath>(stereoRenderingPath)
            },
            {
                "mtrendering", "Use multi threaded rendering; true is default.",
                gfxMultithreaded =>
                {
                    if (gfxMultithreaded.ToLower() == "true")
                    {
                        PlatformSettings.MtRendering = true;
                        PlatformSettings.GraphicsJobs = false;
                    }
                }
            },
            {
                "graphicsjobs", "Use graphics jobs rendering; false is default.",
                gfxJobs =>
                {
                    if (gfxJobs.ToLower() == "true")
                    {
                        PlatformSettings.MtRendering = false;
                        PlatformSettings.GraphicsJobs = true;
                    }
                }
            },
            {
                "minimumandroidsdkversion=", "Minimum Android SDK Version to use.",
                minAndroidSdkVersion => PlatformSettings.MinimumAndroidSdkVersion =
                    TryParse<AndroidSdkVersions>(minAndroidSdkVersion)
            },
            {
                "targetandroidsdkversion=", "Target Android SDK Version to use.",
                targetAndroidSdkVersion => PlatformSettings.TargetAndroidSdkVersion =
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
}
