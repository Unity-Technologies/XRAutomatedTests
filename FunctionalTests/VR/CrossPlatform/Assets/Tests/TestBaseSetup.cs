using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.XR;
#if UNITY_METRO
using UnityEngine.XR.WSA;
#endif
#if UNITY_EDITOR
using UnityEditor;
#endif

[UnityPlatform(include = new[]
{
    RuntimePlatform.WindowsEditor,
    RuntimePlatform.WindowsPlayer,
    RuntimePlatform.Android,
    RuntimePlatform.IPhonePlayer,
    RuntimePlatform.OSXEditor,
    RuntimePlatform.OSXPlayer,
    RuntimePlatform.Lumin,
    RuntimePlatform.WSAPlayerARM,
    RuntimePlatform.WSAPlayerX64,
    RuntimePlatform.WSAPlayerX86
})]

[PrebuildSetup("EnablePlatformPrebuildStep")]
internal class TestBaseSetup
{
    public enum HolographicRuntimeType
    {
        SimulatedHoloLens,
        MixedRealityHMD,
        Remoting
    };

    public TestSetupHelpers m_TestSetupHelpers;
    public CurrentSettings settings;

    public GameObject m_Camera;
    public GameObject m_Light;
    public GameObject m_Cube;

#if UNITY_EDITOR && UNITY_METRO
    //public SimulatedHead head { get { return HolographicAutomation.simulatedHead; } }
    //public SimulatedBody body { get { return HolographicAutomation.simulatedBody; } }
    //public SimulatedHand leftHand { get { return HolographicAutomation.simulatedLeftHand; } }
    //public SimulatedHand rightHand { get { return HolographicAutomation.simulatedRightHand; } }


    //public EditorWindow m_EmulationWindow;
#endif

    [OneTimeSetUp]
    public virtual void OneTimeSetUp()
    {
        settings = Resources.Load<CurrentSettings>("settings");

#if UNITY_EDITOR
        if(settings.enabledXrTarget == "WindowsMR")
        {
            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.WSA, BuildTarget.WSAPlayer);
        }
#endif

#if UNITY_EDITOR && UNITY_METRO
        //if (settings.simulationMode == "HoloLens" || settings.simulationMode == "WindowsMR")
        //{
        //    //Configure Holographic Emulation
        //    var emulationWindow = EditorWindow.GetWindow<HolographicEmulationWindow>();
        //    emulationWindow.Show();

        //    if (settings.simulationMode == "HoloLens")
        //    {
        //        emulationWindow.emulationMode = EmulationMode.Simulated;
        //        HolographicAutomation.SetPlaymodeInputType(PlaymodeInputType.LeftHand);
        //    }
        //    else if (settings.simulationMode == "WindowsMR")
        //    {
        //        emulationWindow.emulationMode = EmulationMode.None;
        //    }   
        //}
#endif
    }

    [SetUp]
    public virtual void SetUp()
    {
        m_TestSetupHelpers = new TestSetupHelpers();

        m_TestSetupHelpers.TestStageSetup(TestStageConfig.BaseStageSetup);
    }

    [TearDown]
    public virtual void TearDown()
    {
        m_TestSetupHelpers.TestStageSetup(TestStageConfig.CleanStage);
    }

    [UnitySetUp]
    public IEnumerator SetUpAndEnableXR()
    {
        if (XRSettings.loadedDeviceName != settings.enabledXrTarget)
        {
            XRSettings.LoadDeviceByName(settings.enabledXrTarget);
        }

        yield return null;

        XRSettings.enabled = true;
    }

    public bool WmrDeviceCheck()
    {
        if (settings.simulationMode == "HoloLens" || settings.simulationMode == "WindowsMR")
        {
            return true;
        }

        if (settings.enabledXrTarget == "WindowsMR")
        {
            return true;
        }
        
        Assert.Ignore("Current Setup is not for Windows MR device or Emulation");
        return false;
    }

    public bool OnlyRunHoloLensSimulatedDeviceCheck()
    {
//#if UNITY_EDITOR && UNITY_METRO
//        var emulationWindow = EditorWindow.GetWindow<HolographicEmulationWindow>();
        
//        if (emulationWindow.emulationMode == EmulationMode.None)
//        {
//            Assert.Ignore("Current Setup is not for Simulated Device");
//            return false;
//        }
//        else if(emulationWindow.emulationMode != EmulationMode.Simulated)
//        {
//            return true;
//        }
//#endif

        Assert.Ignore("Current Setup is not for Emulation");
        return false;

    }
}
