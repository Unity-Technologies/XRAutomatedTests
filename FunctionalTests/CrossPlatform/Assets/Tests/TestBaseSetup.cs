using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using NUnit.Framework.Api;
using NUnit.Framework.Interfaces;
using Tests;
using UnityEngine.XR.WSA;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.XR;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal.VR;
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
public class TestBaseSetup
{
    public TestSetupHelpers m_TestSetupHelpers;
    public CurrentSettings settings;

    public GameObject m_Camera;
    public GameObject m_Light;
    public GameObject m_Cube;

#if UNITY_EDITOR
    public EditorWindow m_EmulationWindow;
#endif

    [OneTimeSetUp]
    public void Setup()
    {
        settings = Resources.Load<CurrentSettings>("settings");

        m_TestSetupHelpers = new TestSetupHelpers();

#if UNITY_EDITOR
        if (settings.simulationMode == "HoloLens" || settings.simulationMode == "WindowsMR")
        {
            //Configure Holographic Emulation
            var emulationWindow = EditorWindow.GetWindow<HolographicEmulationWindow>();
            emulationWindow.Show();

            if (settings.simulationMode == "HoloLens")
            {
                emulationWindow.emulationMode = EmulationMode.Simulated;
            }
            else if (settings.simulationMode == "WindowsMR")
            {
                emulationWindow.emulationMode = EmulationMode.None;
            }   
        }
#endif
    }

    [OneTimeTearDown]
    public void TearDown()
    {
        m_TestSetupHelpers.TestStageSetup(TestStageConfig.CleanStage);
    }

    [SetUp]
    public void TestSetup()
    {
        m_TestSetupHelpers = new TestSetupHelpers();

        m_TestSetupHelpers.TestStageSetup(TestStageConfig.BaseStageSetup);
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
}
