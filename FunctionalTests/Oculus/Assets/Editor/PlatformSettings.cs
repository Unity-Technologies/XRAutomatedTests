using System.Linq;
using UnityEditor;
using UnityEngine;

public static class PlatformSettings
{
    public static BuildTargetGroup BuildTargetGroup => EditorUserBuildSettings.selectedBuildTargetGroup;
    public static BuildTarget BuildTarget => EditorUserBuildSettings.activeBuildTarget;
    
    public static string [] enabledXrTargets;
    public static string playerGraphicsApi;

    public static string stereoRenderingPath;

    public static bool mtRendering = true;
    public static bool graphicsJobs;
    public static AndroidSdkVersions minimumAndroidSdkVersion = AndroidSdkVersions.AndroidApiLevel24;
    public static AndroidSdkVersions targetAndroidSdkVersion = AndroidSdkVersions.AndroidApiLevel24;

    public static void SerializeToAsset()
    {
        var settingsAsset = ScriptableObject.CreateInstance<CurrentSettings>();

        settingsAsset.enabledXrTarget = enabledXrTargets.FirstOrDefault();
        settingsAsset.playerGraphicsApi = playerGraphicsApi;
        settingsAsset.stereoRenderingPath = stereoRenderingPath;
        
        AssetDatabase.CreateAsset(settingsAsset, "Assets/Resources/settings.asset");
        AssetDatabase.SaveAssets();
    }
}