using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.XR;

public static class PlatformSettings
{
    public static BuildTargetGroup BuildTargetGroup => EditorUserBuildSettings.selectedBuildTargetGroup;
    public static BuildTarget BuildTarget => EditorUserBuildSettings.activeBuildTarget;
    
    public static string [] EnabledXrTargets;
    public static GraphicsDeviceType PlayerGraphicsApi;

    public static StereoRenderingPath StereoRenderingPath;

    public static bool MtRendering = true;
    public static bool GraphicsJobs;
    public static AndroidSdkVersions MinimumAndroidSdkVersion = AndroidSdkVersions.AndroidApiLevel24;
    public static AndroidSdkVersions TargetAndroidSdkVersion = AndroidSdkVersions.AndroidApiLevelAuto;

    public static string SimulationMode;

    public static void SerializeToAsset()
    {
        var settingsAsset = ScriptableObject.CreateInstance<CurrentSettings>();

        settingsAsset.EnabledXrTarget = EnabledXrTargets.FirstOrDefault();
        settingsAsset.PlayerGraphicsApi = PlayerGraphicsApi;
        settingsAsset.StereoRenderingMode = GetXrStereoRenderingPathMapping(StereoRenderingPath);
        settingsAsset.MtRendering = MtRendering;
        settingsAsset.GraphicsJobs = GraphicsJobs;
        settingsAsset.SimulationMode = SimulationMode;

        AssetDatabase.CreateAsset(settingsAsset, "Assets/Resources/settings.asset");
        AssetDatabase.SaveAssets();
    }

    private static XRSettings.StereoRenderingMode GetXrStereoRenderingPathMapping(StereoRenderingPath stereoRenderingPath)
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