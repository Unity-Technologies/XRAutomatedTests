using System.Collections;
using NUnit.Framework;
using UnityEngine.TestTools;
using UnityEngine.XR;
using Assert = UnityEngine.Assertions.Assert;

internal class XrSmokeXrFunctionalTest : XrFunctionalTestBase
{
    [UnityTest]
    public IEnumerator VerifyXrSettings_EyeTextureHeight_GreaterThan0()
    {
        Assert.IsTrue(XRSettings.eyeTextureHeight > 0f);
        yield return null;
    }

    [UnityTest]
    public IEnumerator VerifyXrSettings_EyeTextureWidth_GreaterThan0()
    {
        Assert.IsTrue(XRSettings.eyeTextureWidth > 0f);
        yield return null;
    }

    [UnityTest]
    public IEnumerator VerifyXrSettings_EyeTextureResolutionScale_GreaterThan0()
    {
        Assert.IsTrue(XRSettings.eyeTextureResolutionScale > 0f);
        yield return null;
    }

    [UnityTest]
    public IEnumerator VerifyXrSettings_RenderViewportScale_GreaterThan0()
    {
        Assert.IsTrue(XRSettings.renderViewportScale > 0f);
        yield return null;
    }

    [UnityTest]
    public IEnumerator VerifyXrSettings_UseOcclusionMesh()
    {
        Assert.IsTrue(XRSettings.useOcclusionMesh);
        yield return null;
    }
#if !UNITY_EDITOR
    [UnityTest]
    public IEnumerator XrApVerifyXrSettings_StereoRenderingMode()
    {

        Assert.IsTrue(XRSettings.stereoRenderingMode.ToString().Contains(Settings.StereoRenderingMode.ToString()), $"{XRSettings.stereoRenderingMode} != {Settings.StereoRenderingMode}");

    yield return null;
    }
#endif

    [UnityTest]
    [Ignore("Inconsistent results for test. For example, this doesn't work on GearVR.")]
    public IEnumerator CanDisableAndEnableXr()
    {
        yield return new MonoBehaviourTest<SwapXrEnabled>();
    }
}