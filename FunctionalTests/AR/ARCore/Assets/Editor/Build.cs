using UnityEditor;
using System.Threading;
using System.Diagnostics;


public class Build
{
    public static void CommandLineSetup()
    {
        PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, "com.unity.ARCoreSmokeTest");
        PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel24;
        EditorUserBuildSettings.androidBuildType = AndroidBuildType.Development;
        EditorUserBuildSettings.androidBuildSystem = AndroidBuildSystem.Gradle;

        ExecuteCommand();
    }

    static void Command()
    {
        var processInfo = new ProcessStartInfo("cmd.exe", @"/k adb install .\assets\dummy.apk");
        processInfo.CreateNoWindow = true;
        processInfo.UseShellExecute = false;

        var process = Process.Start(processInfo);

        process.WaitForExit();
        process.Close();
    }

    public static void ExecuteCommand()
    {
        var thread = new Thread(delegate () { Command(); });
        thread.Start();
    }
}
