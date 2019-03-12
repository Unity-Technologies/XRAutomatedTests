using UnityEditor;

public class Build
{
    public static void CommandLineSetup()
    {
        // Android - ARCore Prebuild
        PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, "com.unity.XRSDKARSmokeTest");
        PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel24;
        PlayerSettings.SetMobileMTRendering(BuildTargetGroup.Android, false);
        EditorUserBuildSettings.androidBuildType = AndroidBuildType.Development;
        EditorUserBuildSettings.androidBuildSystem = AndroidBuildSystem.Gradle;


        // iOS - ARKit Prebuild
        PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.iOS, "com.unity.XRSDKARSmokeTest");
        PlayerSettings.iOS.cameraUsageDescription = "Capture video feed for AR Background Rendering";
        PlayerSettings.iOS.targetOSVersionString = "11.0";
        EditorUserBuildSettings.iOSBuildConfigType = iOSBuildType.Debug;
    }
}
