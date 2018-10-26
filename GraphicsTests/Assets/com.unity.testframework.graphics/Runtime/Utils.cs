#if UNITY_EDITOR
using System;
using UnityEngine;
using System.Collections.Generic;

namespace UnityEditor.TestTools.Graphics
{
    internal static class Utils
    {
        public static RuntimePlatform BuildTargetToRuntimePlatform(BuildTarget target)
        {
            switch (target)
            {
                case BuildTarget.Android:
                    return RuntimePlatform.Android;
                case BuildTarget.iOS:
                    return RuntimePlatform.IPhonePlayer;
                case BuildTarget.StandaloneLinux:
                case BuildTarget.StandaloneLinux64:
                case BuildTarget.StandaloneLinuxUniversal:
                    return RuntimePlatform.LinuxPlayer;
                case BuildTarget.StandaloneOSX:
                    return RuntimePlatform.OSXPlayer;
                case BuildTarget.PS4:
                    return RuntimePlatform.PS4;
                case BuildTarget.PSP2:
                    return RuntimePlatform.PSP2;
                case BuildTarget.Switch:
                    return RuntimePlatform.Switch;
                case BuildTarget.WebGL:
                    return RuntimePlatform.WebGLPlayer;
                case BuildTarget.WSAPlayer:
                    throw new NotImplementedException(
                        "Don't know how to determine the target UWP architecture from the build settings");
                case BuildTarget.StandaloneWindows:
                case BuildTarget.StandaloneWindows64:
                    return RuntimePlatform.WindowsPlayer;
                case BuildTarget.XboxOne:
                    return RuntimePlatform.XboxOne;
                case BuildTarget.tvOS:
                    return RuntimePlatform.tvOS;
            }

            throw new ArgumentOutOfRangeException("target", target, "Unknown BuildTarget");
        }

        public static BuildTarget RuntimePlatformToBuildTarget(RuntimePlatform platform)
        {
            switch (platform)
            {
                case RuntimePlatform.Android:
                    return BuildTarget.Android;
                case RuntimePlatform.IPhonePlayer:
                    return BuildTarget.iOS;
                case RuntimePlatform.LinuxEditor:
                case RuntimePlatform.LinuxPlayer:
                    return BuildTarget.StandaloneLinuxUniversal;
                case RuntimePlatform.OSXEditor:
                case RuntimePlatform.OSXPlayer:
                    return BuildTarget.StandaloneOSX;
                case RuntimePlatform.PS4:
                    return BuildTarget.PS4;
                case RuntimePlatform.PSP2:
                    return BuildTarget.PSP2;
                case RuntimePlatform.Switch:
                    return BuildTarget.Switch;
#if !UNITY_2017_2_OR_NEWER
                case RuntimePlatform.TizenPlayer:
                    return BuildTarget.Tizen;
#endif
                case RuntimePlatform.tvOS:
                    return BuildTarget.tvOS;
                case RuntimePlatform.WebGLPlayer:
                    return BuildTarget.WebGL;
                case RuntimePlatform.WindowsEditor:
                case RuntimePlatform.WindowsPlayer:
                    return BuildTarget.StandaloneWindows;
                case RuntimePlatform.WSAPlayerARM:
                case RuntimePlatform.WSAPlayerX64:
                case RuntimePlatform.WSAPlayerX86:
                    return BuildTarget.WSAPlayer;
                case RuntimePlatform.XboxOne:
                    return BuildTarget.XboxOne;
            }

            throw new ArgumentOutOfRangeException("platform", platform, "Unknown RuntimePlatform");
        }

        public static void SetupReferenceImageImportSettings(IEnumerable<string> imageAssetPaths)
        {
            // Make sure that all the images have compression turned off and are readable
            AssetDatabase.StartAssetEditing();
            try
            {
                foreach (var path in imageAssetPaths)
                {
                    var importer = AssetImporter.GetAtPath(path) as TextureImporter;
                    if (!importer)
                        continue;

                    if (importer.textureCompression == TextureImporterCompression.Uncompressed && importer.isReadable)
                        continue;

                    importer.textureCompression = TextureImporterCompression.Uncompressed;
                    importer.isReadable = true;
                    importer.mipmapEnabled = false;
                    importer.npotScale = TextureImporterNPOTScale.None;
                    AssetDatabase.ImportAsset(path);
                }
            }
            finally
            {
                AssetDatabase.StopAssetEditing();
            }
        }
    }
}
#endif
