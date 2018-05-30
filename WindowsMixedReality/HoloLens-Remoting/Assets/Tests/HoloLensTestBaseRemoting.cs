using System;
using NUnit.Framework;
using UnityEditor;
using UnityEngine.TestTools;
using UnityEngine;
using UnityEngine.XR.WSA;
using UnityEditorInternal.VR;
using Object = UnityEngine.Object;

[Ignore("Remoting is in a unstable state for controlling the device")]
[PrebuildSetup(typeof(HoloLensPrebuildRemotingSetup))]
internal class HoloLensTestBaseRemoting : MonoBehaviour
{
    public GameObject m_Camera;
    public GameObject m_Light;
    public GameObject m_Cube;

    public TestSetupRemotingHelpers m_TestSetupHelpers;
    public HolographicStreamerConnectionState m_StreamerState;

    public const float k_RemotingWait = 1f;

    [SetUp]
    public void HoloLensTestBaseRemotingSetup()
    {
        HolographicAutomation.Reset();

        m_Cube = new GameObject("Cube");
        m_TestSetupHelpers = new TestSetupRemotingHelpers();

        m_TestSetupHelpers.TestStageSetup(TestStageConfig.BaseStageSetup);

        PerceptionRemoting.SetEnableAudio(true);
        PerceptionRemoting.SetEnableVideo(true);
        PerceptionRemoting.SetVideoEncodingParameters(99999);
        PerceptionRemoting.Connect("10.1.18.9");
    }

    [TearDown]
    public void HoloLensTestBaseRemotingTearDown()
    {
        if (PerceptionRemoting.GetConnectionState() != HolographicStreamerConnectionState.Disconnected)
        {
            PerceptionRemoting.Disconnect();
        }

        m_TestSetupHelpers.TestStageSetup(TestStageConfig.CleanStage);
    }

    public class HoloLensPrebuildRemotingSetup : IPrebuildSetup
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
            VREditor.SetVREnabledDevicesOnTargetGroup(BuildTargetGroup.WSA, new[] { "WindowsMR" });

            // Configure Holographic Emulation
            var emulationWindow = EditorWindow.GetWindow<HolographicEmulationWindow>();
            emulationWindow.Show();
            emulationWindow.emulationMode = EmulationMode.RemoteDevice;
        }
    }
}
