using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System.Threading;
using System.Diagnostics;


public static class PostBuildCameraPermission
{
    [PostProcessBuildAttribute(5)]
    public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject)
    {


        UnityEngine.Debug.Log("OnPostprocesBuild callback executed");
        if (target == BuildTarget.Android)
        {
            UnityEngine.Debug.Log("Android BuildTarget detected");

#if UNITY_EDITOR_WIN
            ExecuteCommand();
#endif
        }
    }

    public static void ExecuteCommand()
    {
        var thread = new Thread(delegate () { Command(); });
        thread.Start();
    }

    static void Command()
    {
        var processInfo = new ProcessStartInfo("cmd.exe", @"/k adb shell pm grant com.UnityTestRunner.UnityTestRunner android.permission.CAMERA");
        processInfo.CreateNoWindow = true;
        processInfo.UseShellExecute = false;

        var process = Process.Start(processInfo);

        process.WaitForExit();
        process.Close();
    }
}

