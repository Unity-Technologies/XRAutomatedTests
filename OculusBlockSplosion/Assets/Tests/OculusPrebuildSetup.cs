using System;
using System.IO;
using NUnit.Framework;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine.TestTools;
using UnityEngine;
using Tests;

//Documentation on Unity Playmode tests can be found at: https://docs.unity3d.com/Manual/testing-editortestsrunner.html

// Creating a setup class that inherits from IPrebuildSetup will automatically run its Setup() script if your test
// class inherits from the setup class you created. This is useful for quickly running projects in the Editor on
// different devices and platforms, or in the case of GearVR, copying in OSig files for all your Android test devices.
namespace Tests
{
    [PrebuildSetup(typeof(EnableOculusPrebuildStep))]
    public class OculusPrebuildSetup
    {
        public class EnableOculusPrebuildStep : IPrebuildSetup
        {
            public void Setup()
            {
 //This code will only be run if the Playmode tests are run in the Editor. It will be ignored if you use the
 // "Run all in player (<platform>)" button
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
                    EditorUserBuildSettings.androidBuildSystem =
                        AndroidBuildSystem
                            .Gradle; // Currently Oculus SDK doesn't provide a targetSDKVersion, which causes a permission request on startup
                    CopyOculusSignatureFilesToProject();
                    UnityEditorInternal.VR.VREditor.SetVREnabledDevicesOnTargetGroup(
                        buildTargetGroup,
                        new[] {"Oculus"});
                }
                else
                {
                    throw new NotImplementedException(EditorUserBuildSettings.selectedBuildTargetGroup +
                                                      "Platform not implemented for prebuild steps.");
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
    }
}
