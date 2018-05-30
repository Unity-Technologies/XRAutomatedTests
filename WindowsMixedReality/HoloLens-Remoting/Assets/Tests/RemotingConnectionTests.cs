using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.XR.WSA;

[Ignore("Remoting is in a unstable state for controlling the device")]
internal class RemotingConnectionTests : HoloLensTestBaseRemoting
{
    [UnityTest]
    public IEnumerator ConnectionTest()
    {
        yield return new WaitForSeconds(k_RemotingWait);
        m_StreamerState = PerceptionRemoting.GetConnectionState();
        
        Assert.AreEqual(HolographicStreamerConnectionState.Connected, m_StreamerState, "Holographic Streamer Connection Failure");
    }

    [UnityTest]
    public IEnumerator ConnectThenDisconnectTest()
    {
        yield return new WaitForSeconds(k_RemotingWait);
        m_StreamerState = PerceptionRemoting.GetConnectionState();

        Assert.AreEqual(HolographicStreamerConnectionState.Connected, m_StreamerState, "Holographic Streamer Connection Failure");

        PerceptionRemoting.Disconnect();

        yield return new WaitForSeconds(k_RemotingWait);
        m_StreamerState = PerceptionRemoting.GetConnectionState();

        Assert.AreEqual(HolographicStreamerConnectionState.Disconnected, m_StreamerState, "Holographic Streamer is still connected");
    }
}
