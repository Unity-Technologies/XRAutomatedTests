using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using UnityEditorInternal.VR;
using UnityEngine;
using UnityEngine.XR.WSA.Input;
using UnityEngine.XR.WSA;
using UAssert = UnityEngine.Assertions.Assert;

//[Ignore("Metro/wsa is disabled on Katana")]
internal class EmulationControlTests : HoloLensTestBase
{
    const float k_RotationTolerance = .05f;

    bool m_PressedDetected;
    bool m_ReleasedDetected;
    bool m_SourceDetected;
    bool m_SourceLost;

    [SetUp]
    public void SetUp()
    {
        m_PressedDetected = m_ReleasedDetected = m_SourceDetected = m_SourceLost = false;

        InteractionManager.InteractionSourceDetected += InteractionManagerOnSourceDetected;
        InteractionManager.InteractionSourceLost += InteractionManagerOnSourceLost;
        InteractionManager.InteractionSourcePressed += InteractionManagerOnSourcePressed;
        InteractionManager.InteractionSourceReleased += InteractionManagerOnSourceReleased;
    }

    [TearDown]
    public void TearDown()
    {
        InteractionManager.InteractionSourceDetected -= InteractionManagerOnSourceDetected;
        InteractionManager.InteractionSourceLost -= InteractionManagerOnSourceLost;
        InteractionManager.InteractionSourcePressed -= InteractionManagerOnSourcePressed;
        InteractionManager.InteractionSourceReleased -= InteractionManagerOnSourceReleased;

        Object.Destroy(m_Camera);
        Object.Destroy(m_Light);
    }

    [UnityTest]
    public IEnumerator CameraMovesWhenBodyMoves()
    {
        body.position = new Vector3(1, 2, 3);
        UAssert.AreApproximatelyEqual(1, body.position.x);
        UAssert.AreApproximatelyEqual(2, body.position.y);
        UAssert.AreApproximatelyEqual(3, body.position.z);

        yield return null;

        var expectedPos = HolographicEmulationHelpers.CalcExpectedCameraPosition(head, body);
        var actualPos = m_Camera.GetComponent<Camera>().transform.position;
        UAssert.AreApproximatelyEqual(expectedPos.x, actualPos.x);
        UAssert.AreApproximatelyEqual(expectedPos.y, actualPos.y);
        UAssert.AreApproximatelyEqual(expectedPos.z, actualPos.z);
    }

    [UnityTest]
    public IEnumerator CameraRotatesWhenBodyRotates()
    {
        body.rotation = 79.3f;
        yield return new WaitForSeconds(2f);

        UAssert.AreApproximatelyEqual(79.3f, m_Camera.GetComponent<Camera>().transform.eulerAngles.y, k_RotationTolerance);
    }

    [Ignore("Test is failing for a unknown reason")]
    [UnityTest]
    public IEnumerator CameraRotatesWhenHeadRotates()
    {
        head.eulerAngles = new Vector3(12f, 34f, 56f);

        yield return new WaitForSeconds(2f);

        var camRotation = m_Camera.GetComponent<Camera>().transform.rotation.eulerAngles;

        UAssert.AreApproximatelyEqual(12f, camRotation.x, k_RotationTolerance);
        UAssert.AreApproximatelyEqual(34f, camRotation.y, k_RotationTolerance);
        UAssert.AreApproximatelyEqual(56f, camRotation.z, k_RotationTolerance);
    }

    [UnityTest]
    public IEnumerator SimulatedHandsCanPerformGestures()
    {
        // 0.02 seconds seems to be the minimum required time for gestures to appear in the API.
        var gestureWait = new WaitForSeconds(0.3f);

        var hand = HolographicAutomation.simulatedRightHand;
        yield return null;
        hand.activated = true;
        hand.EnsureVisible();
        yield return null;

        hand.PerformGesture(SimulatedGesture.FingerPressed);
        yield return gestureWait;
        Assert.IsTrue(m_PressedDetected, "FingerPressed was not detected");

        hand.PerformGesture(SimulatedGesture.FingerReleased);
        yield return gestureWait;
        Assert.IsTrue(m_ReleasedDetected, "FingerReleased was not detected");
    }

    [UnityTest]
    public IEnumerator SimulatedHandsAreDetectedAndLost()
    {
        var hand = HolographicAutomation.simulatedRightHand;
        yield return null;
        hand.activated = true;
        hand.position = body.position;
        hand.EnsureVisible();
        yield return null;
        yield return new WaitForSeconds(0.02f);
        Assert.IsTrue(m_SourceDetected, "Hand was not detected");

        // Hand seems to require being out of the detection frustum for two frames in order to be lost.
        hand.position = new Vector3(0, 5f, 0);
        yield return null;
        yield return null;
        hand.activated = false;
        yield return null;
        Assert.IsTrue(m_SourceLost, "Hand was not lost");
    }

    void InteractionManagerOnSourceReleased(InteractionSourceReleasedEventArgs obj)
    {
        m_ReleasedDetected = true;
    }

    void InteractionManagerOnSourcePressed(InteractionSourcePressedEventArgs obj)
    {
        m_PressedDetected = true;
    }

    void InteractionManagerOnSourceLost(InteractionSourceLostEventArgs obj)
    {
        m_SourceLost = true;
    }

    void InteractionManagerOnSourceDetected(InteractionSourceDetectedEventArgs obj)
    {
        m_SourceDetected = true;
    }
}
