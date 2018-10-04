using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.XR;

public static class PlatformSettings
{
    public static BuildTargetGroup BuildTargetGroup => EditorUserBuildSettings.selectedBuildTargetGroup;
    public static BuildTarget BuildTarget => EditorUserBuildSettings.activeBuildTarget;
    
    public static string [] enabledXrTargets;
    public static GraphicsDeviceType playerGraphicsApi;

    public static StereoRenderingPath stereoRenderingPath;

    public static bool mtRendering = true;
    public static bool graphicsJobs;
    public static AndroidSdkVersions minimumAndroidSdkVersion = AndroidSdkVersions.AndroidApiLevel24;
    public static AndroidSdkVersions targetAndroidSdkVersion = AndroidSdkVersions.AndroidApiLevel24;

    public static void SerializeToAsset()
    {
        var settingsAsset = ScriptableObject.CreateInstance<CurrentSettings>();

        settingsAsset.enabledXrTarget = enabledXrTargets.FirstOrDefault();
        settingsAsset.playerGraphicsApi = playerGraphicsApi;
        settingsAsset.stereoRenderingMode = GetXRStereoRenderingPathMapping(stereoRenderingPath);
        settingsAsset.mtRendering = mtRendering;
        settingsAsset.graphicsJobs = graphicsJobs;
        
        AssetDatabase.CreateAsset(settingsAsset, "Assets/Resources/settings.asset");
        AssetDatabase.SaveAssets();
    }

    private static XRSettings.StereoRenderingMode GetXRStereoRenderingPathMapping(StereoRenderingPath stereoRenderingPath)
    {
        switch (stereoRenderingPath)
        {
            case StereoRenderingPath.SinglePass:
                return XRSettings.StereoRenderingMode.SinglePass;
            case StereoRenderingPath.MultiPass:
                return XRSettings.StereoRenderingMode.MultiPass;
            case StereoRenderingPath.Instancing:
                return XRSettings.StereoRenderingMode.SinglePassInstanced;
            default:
                return XRSettings.StereoRenderingMode.SinglePassMultiview;
        }
    }
}