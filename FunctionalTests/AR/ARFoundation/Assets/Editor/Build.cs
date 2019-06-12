using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System.Threading;
using System.Diagnostics;

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

#if UNITY_EDITOR_WIN
        ExecuteCommand();
#endif
    }

    static void Command()
    {
        var processInfo = new ProcessStartInfo("cmd.exe", @"/k adb install .\assets\dummy.apk");
        processInfo.CreateNoWindow = true;
        processInfo.UseShellExecute = false;
        processInfo.RedirectStandardOutput = true;

        var process = new Process();
        process.StartInfo = processInfo;
        process.OutputDataReceived += (sender, args) => UnityEngine.Debug.Log(args.Data);

        process.Start();
        process.BeginOutputReadLine();

        process.WaitForExit();
        process.Close();
    }

    public static void ExecuteCommand()
    {
        var thread = new Thread(delegate () { Command(); });
        thread.Start();
    }
}
