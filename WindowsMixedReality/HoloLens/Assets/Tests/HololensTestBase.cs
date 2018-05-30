using System;
using NUnit.Framework;
using UnityEditor;
using UnityEngine.TestTools;
using UnityEngine;
using UnityEngine.XR.WSA;
using UnityEditorInternal.VR;
using Object = UnityEngine.Object;

[PrebuildSetup(typeof(HoloLensPrebuildSetup))]
internal class HoloLensTestBase
{
    public static GameObject m_Camera;
    public static GameObject m_Light;
    public static GameObject m_Cube;

    public TestSetupSimulationHelpers m_TestSetupHelpers;

    public SimulatedHead head { get { return HolographicAutomation.simulatedHead; } }
    public SimulatedBody body { get { return HolographicAutomation.simulatedBody; } }
    public SimulatedHand leftHand { get { return HolographicAutomation.simulatedLeftHand; } }
    public SimulatedHand rightHand { get { return HolographicAutomation.simulatedRightHand; } }

    [SetUp]
    public void HoloLensTestBaseSetup()
    {
        HolographicAutomation.Reset();

        m_Cube = new GameObject("Cube");
        m_TestSetupHelpers = new TestSetupSimulationHelpers();

        m_TestSetupHelpers.TestStageSetup(TestStageConfig.BaseStageSetup);
    }

    [TearDown]
    public void HoloLensTestBaseTearDown()
    {
        m_TestSetupHelpers.TestStageSetup(TestStageConfig.CleanStage);
    }

    public class HoloLensPrebuildSetup : IPrebuildSetup
    {
        public void Setup()
        {
            // Configure WSA build
            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.WSA, BuildTarget.WSAPlayer);
            EditorUserBuildSettings.wsaUWPBuildType = WSAUWPBuildType.D3D;
            EditorUserBuildSettings.wsaSubtarget = WSASubtarget.HoloLens;
            EditorUserBuildSettings.allowDebugging = true;

            PlayerSettings.SetScriptingBackend(BuildTargetGroup.WSA, ScriptingImplementation.WinRTDotNET);
            PlayerSettings.stereoRenderingPath = UnityEditor.StereoRenderingPath.Instancing;

            // Enable HoloLens SDK
            VREditor.SetVREnabledOnTargetGroup(BuildTargetGroup.WSA, true);
            VREditor.SetVREnabledDevicesOnTargetGroup(BuildTargetGroup.WSA, new[] {"WindowsMR"});

            // Configure Holographic Emulation
            var emulationWindow = EditorWindow.GetWindow<HolographicEmulationWindow>();
            emulationWindow.Show();
            emulationWindow.emulationMode = EmulationMode.Simulated;
        }
    }
}
