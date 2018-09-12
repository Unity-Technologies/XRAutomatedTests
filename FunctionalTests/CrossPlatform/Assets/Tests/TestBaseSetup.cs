using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Tests;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.XR;

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
    public static GameObject m_Camera;
    public static GameObject m_Light;
    public static GameObject m_Cube;

    public TestSetupHelpers m_TestSetupHelpers;

    public CurrentSettings settings;

    [OneTimeSetUp]
    public void Setup()
    {
        settings = Resources.Load<CurrentSettings>("settings");

        m_TestSetupHelpers = new TestSetupHelpers();

        m_TestSetupHelpers.TestStageSetup(TestStageConfig.BaseStageSetup);
    }

    [OneTimeTearDown]
    public void TearDown()
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

}
