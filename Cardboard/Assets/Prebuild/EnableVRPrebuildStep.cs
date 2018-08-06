using System;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine.TestTools;

public class EnableVRPrebuildStep : IPrebuildSetup
{
    public void Setup()
    {
#if UNITY_EDITOR
        PlayerSettings.virtualRealitySupported = true;
        SetupVRByPlatform(EditorUserBuildSettings.selectedBuildTargetGroup);
#endif
    }

#if UNITY_EDITOR
    private void SetupVRByPlatform(BuildTargetGroup platformTarget)
    {
        switch (platformTarget)
        {
            case BuildTargetGroup.Android:
            {
                UnityEditorInternal.VR.VREditor.SetVREnabledDevicesOnTargetGroup(
                    platformTarget,
                    new[] { "cardboard" });
                EditorUserBuildSettings.androidBuildType = AndroidBuildType.Development;
                EditorUserBuildSettings.androidBuildSystem = AndroidBuildSystem.Internal;
                PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel19;
                break;
            }
            case BuildTargetGroup.iOS:
            {
                UnityEditorInternal.VR.VREditor.SetVREnabledDevicesOnTargetGroup(
                    platformTarget,
                    new[] { "cardboard" });
                break;
            }
        }
    }
#endif
}
