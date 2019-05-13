using System;
using System.IO;
using NDesk.Options;
using UnityEngine;
using UnityEngine.Rendering;

namespace UnityEditor.TestTools.ConfigManager
{
    public class EnablePlatformPrebuildStep
    {
        private static bool stereoPathWasSet;

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
                        PlayerSettings.colorSpace = ColorSpace.Linear;
                        PlatformSettings.mtRendering = true;
                        PlatformSettings.graphicsJobs = false;
                        break;
                    case BuildTargetGroup.WSA:
                        // Configure WSA build
                        EditorUserBuildSettings.wsaUWPBuildType = WSAUWPBuildType.D3D;
                        EditorUserBuildSettings.wsaSubtarget = WSASubtarget.AnyDevice;
                        EditorUserBuildSettings.allowDebugging = true;

                        PlayerSettings.SetScriptingBackend(BuildTargetGroup.WSA, ScriptingImplementation.IL2CPP);
                        PlatformSettings.stereoRenderingPath = StereoRenderingPath.SinglePass;

                        PlatformSettings.enabledXrTargets = new string[] { "WindowsMR", "None" };

                        // Configure Holographic Emulation
                        //var emulationWindow = EditorWindow.GetWindow<HolographicEmulationWindow>();
                        //emulationWindow.Show();
                        //emulationWindow.emulationMode = EmulationMode.Simulated;
                        break;
                    case BuildTargetGroup.Android:
                    case BuildTargetGroup.iOS:
                        PlatformSettings.enabledXrTargets = new string[] { "cardboard", "None" };
                        PlatformSettings.stereoRenderingPath = StereoRenderingPath.SinglePass;
                        PlatformSettings.playerGraphicsApi = GraphicsDeviceType.OpenGLES3;
                        PlayerSettings.colorSpace = ColorSpace.Gamma;
                        break;
                }
            }
            else
            {
                var optionSet = DefineOptionSet();
                var unprocessedArgs = optionSet.Parse(args);

                if (PlatformSettings.enabledXrTargets == null)
                    PlatformSettings.enabledXrTargets =
                        PlayerSettings.GetVirtualRealitySDKs(EditorUserBuildSettings.selectedBuildTargetGroup);

                if (!stereoPathWasSet)
                    PlatformSettings.stereoRenderingPath = PlayerSettings.stereoRenderingPath;
            }

            ConfigureSettings();

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

            PlayerSettings.stereoRenderingPath = PlatformSettings.stereoRenderingPath;

            PlayerSettings.Android.minSdkVersion = PlatformSettings.minimumAndroidSdkVersion;
            EditorUserBuildSettings.androidBuildType = AndroidBuildType.Development;
            EditorUserBuildSettings.androidBuildSystem = AndroidBuildSystem.Gradle;

            PlayerSettings.SetGraphicsAPIs(PlatformSettings.BuildTarget, new [] { PlatformSettings.playerGraphicsApi });
        }

        private void CopyOculusSignatureFilesToProject()
        {
            var signatureFilePath =
                $"..{Path.DirectorySeparatorChar}OculusSignatureFiles";
            var files = Directory.GetFiles(signatureFilePath);
            var assetsPluginPath =
                $"Assets{Path.DirectorySeparatorChar}Plugins{Path.DirectorySeparatorChar}Android{Path.DirectorySeparatorChar}assets";

            foreach (var file in files)
            {
                if (!File.Exists(assetsPluginPath + file.Substring(file.LastIndexOf(Path.DirectorySeparatorChar))))
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
                    "enabledxrtarget=",
                    "XR target to enable in player settings. Values: \r\n\"Oculus\"\r\n\"OpenVR\"\r\n\"cardboard\"\r\n\"daydream\"\r\n\"MockHMD\"",
                    xrTarget => PlatformSettings.enabledXrTargets = new string[] {xrTarget, "None"}
                },
                {
                    "playergraphicsapi=", "Graphics API based on GraphicsDeviceType.",
                    graphicsDeviceType => { PlatformSettings.playerGraphicsApi = TryParse<GraphicsDeviceType>(graphicsDeviceType); }
                },
                {
                    "colorspace=", "Linear or Gamma color space.",
                    colorSpace => { PlayerSettings.colorSpace = TryParse<ColorSpace>(colorSpace); }
                },
                {
                    "stereorenderingpath=", "Stereo rendering path to enable. SinglePass is default",
                    stereoRenderingPath => {
                        PlatformSettings.stereoRenderingPath = TryParse<StereoRenderingPath>(stereoRenderingPath);
                        stereoPathWasSet = true; }
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
                    trgtAndroidSdkVersion => PlatformSettings.targetAndroidSdkVersion =
                        TryParse<AndroidSdkVersions>(trgtAndroidSdkVersion)
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
}