using System;
#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
#endif
using UnityEngine.TestTools;

public class EnableARPrebuildStep : IPrebuildSetup
{
    public void Setup()
    {
#if UNITY_EDITOR
        PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, "com.unity.ARCoreSmokeTest");
        PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel24;
        EditorUserBuildSettings.androidBuildType = AndroidBuildType.Development;
        EditorUserBuildSettings.androidBuildSystem = AndroidBuildSystem.Gradle;
#endif
    }
}
