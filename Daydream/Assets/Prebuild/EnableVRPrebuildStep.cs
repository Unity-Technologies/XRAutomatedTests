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
        UnityEditorInternal.VR.VREditor.SetVREnabledDevicesOnTargetGroup(
            BuildTargetGroup.Android,
            new[] { "daydream" });
        EditorUserBuildSettings.androidBuildType = AndroidBuildType.Development;
        EditorUserBuildSettings.androidBuildSystem = AndroidBuildSystem.Internal;
        PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel24;
#endif
    }
}
