using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.XR;
using UnityEngine.XR.WSA;


internal class RemotingApiCheck : HoloLensTestBaseRemoting
{
    Vector3 m_FocalPlanePosition;

    [UnityTest]
    public IEnumerator WorldManagerStateCheck()
    {
        yield return new WaitForSeconds(k_RemotingWait);
        m_StreamerState = PerceptionRemoting.GetConnectionState();
        Assert.AreEqual(HolographicStreamerConnectionState.Connected, m_StreamerState, "Holographic Streamer Connection Failure");

        Debug.Log("Current World Manager State : " + WorldManager.state);
        Assert.GreaterOrEqual(PositionalLocatorState.OrientationOnly, WorldManager.state, " World state is not available");

    }

    [UnityTest]
    public IEnumerator MobilePlatformCheck()
    {
        yield return new WaitForSeconds(k_RemotingWait);
        m_StreamerState = PerceptionRemoting.GetConnectionState();
        Assert.AreEqual(HolographicStreamerConnectionState.Connected, m_StreamerState, "Holographic Streamer Connection Failure");


        Assert.AreEqual(false, Application.isMobilePlatform, "Is not a mobile platform while in remoting!");
    }

    [UnityTest]
    public IEnumerator XrSettingsCheck()
    {
        yield return new WaitForSeconds(k_RemotingWait);
        m_StreamerState = PerceptionRemoting.GetConnectionState();
        Assert.AreEqual(HolographicStreamerConnectionState.Connected, m_StreamerState, "Holographic Streamer Connection Failure");


        Assert.IsTrue(XRSettings.isDeviceActive, "XR Device is not active");
    }

    [UnityTest]
    public IEnumerator DeviceCheck()
    {
        yield return new WaitForSeconds(k_RemotingWait);
        m_StreamerState = PerceptionRemoting.GetConnectionState();
        Assert.AreEqual(HolographicStreamerConnectionState.Connected, m_StreamerState, "Holographic Streamer Connection Failure");


        Assert.AreEqual("WindowsMR", XRSettings.loadedDeviceName, "Wrong XR Device reported");
    }

    [UnityTest]
    public IEnumerator XrModel()
    {
        yield return new WaitForSeconds(k_RemotingWait);
        m_StreamerState = PerceptionRemoting.GetConnectionState();
        Assert.AreEqual(HolographicStreamerConnectionState.Connected, m_StreamerState, "Holographic Streamer Connection Failure");


        string model = XRDevice.model;
        Assert.IsNotEmpty(model, "Model is empty");
    }

    [UnityTest]
    public IEnumerator NativePtr()
    {
        yield return new WaitForSeconds(k_RemotingWait);
        m_StreamerState = PerceptionRemoting.GetConnectionState();
        Assert.AreEqual(HolographicStreamerConnectionState.Connected, m_StreamerState, "Holographic Streamer Connection Failure");


        string ptr = XRDevice.GetNativePtr().ToString();
        Assert.IsNotEmpty(ptr, "Native Ptr is empty");
    }

    [UnityTest]
    public IEnumerator RefreshRate()
    {
        yield return new WaitForSeconds(k_RemotingWait);
        m_StreamerState = PerceptionRemoting.GetConnectionState();
        Assert.AreEqual(HolographicStreamerConnectionState.Connected, m_StreamerState, "Holographic Streamer Connection Failure");


        float refreshRate = XRDevice.refreshRate;
        Assert.AreNotEqual(refreshRate, 0, "Refresh is 0, something went wrong");
    }
}
