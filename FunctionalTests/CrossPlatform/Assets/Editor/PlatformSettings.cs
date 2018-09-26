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

    public static string simulationMode;

    public static void SerializeToAsset()
    {
        var settingsAsset = ScriptableObject.CreateInstance<CurrentSettings>();

        settingsAsset.enabledXrTarget = enabledXrTargets.FirstOrDefault();
        settingsAsset.playerGraphicsApi = playerGraphicsApi;
        settingsAsset.stereoRenderingMode = TryParse<XRSettings.StereoRenderingMode>(stereoRenderingPath.ToString());
        settingsAsset.mtRendering = mtRendering;
        settingsAsset.graphicsJobs = graphicsJobs;
        if (simulationMode != String.Empty || simulationMode != null)
        {
            settingsAsset.simulationMode = simulationMode;
        }
        else
        {
            settingsAsset.simulationMode = "None";
        }

        AssetDatabase.CreateAsset(settingsAsset, "Assets/Resources/settings.asset");
        AssetDatabase.SaveAssets();
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
}