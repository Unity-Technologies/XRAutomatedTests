using UnityEditor;

public class Build
{
    public static void CommandLineSetup()
    {
        PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, "com.unity.ARCoreSmokeTest");
        PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel24;
        EditorUserBuildSettings.androidBuildType = AndroidBuildType.Development;
        EditorUserBuildSettings.androidBuildSystem = AndroidBuildSystem.Gradle;
    }
}
