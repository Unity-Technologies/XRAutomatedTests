using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.XR.WSA.Persistence;

public static class PlatformSettings
{
    public static BuildTargetGroup BuildTargetGroup => EditorUserBuildSettings.selectedBuildTargetGroup;
    public static BuildTarget BuildTarget => EditorUserBuildSettings.activeBuildTarget;
    
    public static string[] enabledXrTargets;
    public static string[] playerGraphicsApis;

    public static string[] stereoRenderingPaths;

    public static bool mtRendering = true;
    public static bool graphicsJobs;
    public static AndroidSdkVersions minimumAndroidSdkVersion = AndroidSdkVersions.AndroidApiLevel24;
    public static AndroidSdkVersions targetAndroidSdkVersion = AndroidSdkVersions.AndroidApiLevel24;
    
}