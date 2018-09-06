using NUnit.Framework;
using UnityEngine.TestTools;
using UnityEngine;
using UnityEngine.XR.WSA;

#if UNITY_EDITOR
using UnityEditorInternal.VR;
using UnityEditor;
#endif

[PrebuildSetup(typeof(WindowsMrPrebuildSetup))]
internal class WindowsMrTestBase
{
    public static GameObject m_Camera;
    public static GameObject m_Light;
    public static GameObject m_Cube;

    public TestSetupHelpers m_TestSetupHelpers;
  
    [SetUp]
    public void WindowsMrTestBaseSetup()
    {
        HolographicAutomation.Reset();

        m_TestSetupHelpers = new TestSetupHelpers();

        m_TestSetupHelpers.TestStageSetup(TestStageConfig.BaseStageSetup);
    }

    [TearDown]
    public void WindowsMrTestBaseTearDown()
    {
        m_TestSetupHelpers.TestStageSetup(TestStageConfig.CleanStage);
    }

    public class WindowsMrPrebuildSetup : IPrebuildSetup
    {
        public void Setup()
        {
#if UNITY_EDITOR
            // Configure WSA build
            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.WSA, BuildTarget.WSAPlayer);
            EditorUserBuildSettings.wsaUWPBuildType = WSAUWPBuildType.D3D;
            EditorUserBuildSettings.wsaSubtarget = WSASubtarget.HoloLens;
            EditorUserBuildSettings.allowDebugging = true;

            PlayerSettings.SetScriptingBackend(BuildTargetGroup.WSA, ScriptingImplementation.WinRTDotNET);

            // Enable HoloLens SDK
            VREditor.SetVREnabledOnTargetGroup(BuildTargetGroup.WSA, true);
            VREditor.SetVREnabledDevicesOnTargetGroup(BuildTargetGroup.WSA, new[] { "WindowsMR" });

            // Configure Holographic Emulation
            var emulationWindow = EditorWindow.GetWindow<HolographicEmulationWindow>();
            emulationWindow.Show();
            emulationWindow.emulationMode = EmulationMode.None;
#endif
        }
    }
}

