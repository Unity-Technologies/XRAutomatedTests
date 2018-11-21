using UnityEngine.TestTools;
using System.Collections;
using UnityEditor;
using UnityEngine;

[UnityPlatform(include = new[] { RuntimePlatform.WindowsEditor, RuntimePlatform.OSXEditor })]
public class TestBridge : SmokeTest, IPrebuildSetup
{
    public void Setup()
    {
        EditorBuildSettingsScene[] scenes = new EditorBuildSettingsScene[1];
        scenes[0] = new EditorBuildSettingsScene("Assets/test01.unity", true);
        EditorBuildSettings.scenes = scenes;

        PlayerSettings.SetPlatformVuforiaEnabled(EditorUserBuildSettings.selectedBuildTargetGroup, true);
    }
}
