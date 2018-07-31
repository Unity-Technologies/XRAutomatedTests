using System;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine.TestTools;

public class EnableOculusPrebuildStep : IPrebuildSetup
{
    public void Setup()
    {
#if UNITY_EDITOR
        var buildTargetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;

        PlayerSettings.virtualRealitySupported = true;

        if (buildTargetGroup == BuildTargetGroup.Standalone)
        {
            UnityEditorInternal.VR.VREditor.SetVREnabledDevicesOnTargetGroup(
                buildTargetGroup,
                new[] {"Oculus"});
        }
        else if (buildTargetGroup == BuildTargetGroup.Android)
        {
            EditorUserBuildSettings.androidBuildType = AndroidBuildType.Development;
            PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel19;
            EditorUserBuildSettings.androidBuildSystem = AndroidBuildSystem.Internal; // Currently Oculus SDK doesn't provide a targetSDKVersion, which causes a permission request on startup
            CopyOculusSignatureFilesToProject();
            UnityEditorInternal.VR.VREditor.SetVREnabledDevicesOnTargetGroup(
                buildTargetGroup,
                new[] { "Oculus" });
        }
        else
        {
            throw new NotImplementedException(EditorUserBuildSettings.selectedBuildTargetGroup + "Platform not implemented for prebuild steps.");
        }
#endif
    }

    private void CopyOculusSignatureFilesToProject()
    {
        var files = Directory.GetFiles(@"..\OculusSignatureFiles");

        foreach (var file in files)
        {
            if (!File.Exists(@"Assets\Plugins\Android\assets" + file.Substring(file.LastIndexOf('\\'))))
            {
                File.Copy(file, @"Assets\Plugins\Android\assets" + file.Substring(file.LastIndexOf('\\')));
            }
        }
    }
}
