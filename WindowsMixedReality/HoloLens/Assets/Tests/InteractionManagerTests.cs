using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using UnityEditorInternal.VR;
using UnityEngine.XR.WSA;
using UnityEngine.XR.WSA.Input;

//[Ignore("Metro/wsa is disabled on Katana")]
internal class InteractionManagerTests : HoloLensTestBase
{
    GestureRecognizer m_RightGestureRecognizer;

    Vector3 m_SourceLossMitigationDirection;
    Vector3 m_HandPosition;
    Vector3 m_HandVelocity;
    Vector3 m_HandForward;
    Quaternion m_HandRotation;
    Vector3 m_HandUp;
    Vector3 m_HandAngularVelocity;

    private InteractionSourceKind m_SourceKind;
    private InteractionSourcePressType m_PressType;

    uint m_SourceId = 0;

    double m_SourceLossRisk = 0;

    bool m_HasHandLocation = false;
    bool m_HasHandVelocity = false;

    bool m_SourceLost = false;
    bool m_SourceDetected = false;

    bool m_SourcePressed = false;
    bool m_SourceReleased = false;
    bool m_SourceUpdated = false;

    bool m_SourceManipulationStarted = false;
    bool m_SourceManipulationCanceled = false;
    bool m_SourceManipulationUpdated = false;
    bool m_SourceManipulationCompleted = false;

    bool m_SourceHoldStarted = false;
    bool m_SourceHoldCanceled = false;
    bool m_SourceHoldCompleted = false;

    bool m_HasHandForward;
    bool m_HasHandRotation;
    bool m_HasHandUp;
    bool m_HasHandAnglularVelocity;
    bool m_HasHandPositionAccuracy;

    const float k_GestureTapWait = 0.3f;

    private InteractionSourcePositionAccuracy m_PositionAccuracy;
    
    [SetUp]
    public void Setup()
    {
        m_HasHandLocation = m_HasHandVelocity = m_HasHandPositionAccuracy = m_HasHandForward = m_HasHandRotation = false;
        m_HasHandUp = m_HasHandAnglularVelocity = m_HasHandPositionAccuracy = false;
        m_SourceDetected = m_SourceLost = m_SourcePressed = m_SourceReleased = m_SourceUpdated = false;
        m_SourceManipulationStarted = m_SourceManipulationCanceled = m_SourceManipulationUpdated = m_SourceManipulationCompleted = false;
        m_SourceHoldStarted = m_SourceHoldCanceled = m_SourceHoldCompleted = false;

        rightHand.activated = true;
        rightHand.position = body.position;

        m_RightGestureRecognizer = new GestureRecognizer();
        m_RightGestureRecognizer.SetRecognizableGestures(GestureSettings.Tap | GestureSettings.ManipulationTranslate | GestureSettings.Hold);
        m_RightGestureRecognizer.HoldStarted += M_GestureRecognizer_HoldStartedEvent;
        m_RightGestureRecognizer.HoldCompleted += M_GestureRecognizer_HoldCompletedEvent;
        m_RightGestureRecognizer.HoldCanceled += M_GestureRecognizer_HoldCanceledEvent;
        m_RightGestureRecognizer.Tapped += M_GestureRecognizer_TappedEvent;
        m_RightGestureRecognizer.ManipulationStarted += M_GestureRecognizer_ManipulationStartedEvent;
        m_RightGestureRecognizer.ManipulationUpdated += M_GestureRecognizer_ManipulationUpdatedEvent;
        m_RightGestureRecognizer.ManipulationCanceled += M_GestureRecognizer_ManipulationCanceledEvent;
        m_RightGestureRecognizer.ManipulationCompleted += M_GestureRecognizer_ManipulationCompletedEvent;
        m_RightGestureRecognizer.StartCapturingGestures();

        InteractionManager.InteractionSourceDetected += InteractionManager_SourceDetected;
        InteractionManager.InteractionSourceLost += InteractionManager_SourceLost;
        InteractionManager.InteractionSourcePressed += InteractionManager_SourcePressed;
        InteractionManager.InteractionSourceReleased += InteractionManager_SourceReleased;
        InteractionManager.InteractionSourceUpdated += InteractionManager_SourceUpdated;
    }

    [TearDown]
    public void TearDown()
    {
        m_RightGestureRecognizer.HoldStarted -= M_GestureRecognizer_HoldStartedEvent;
        m_RightGestureRecognizer.HoldCompleted -= M_GestureRecognizer_HoldCompletedEvent;
        m_RightGestureRecognizer.HoldCanceled -= M_GestureRecognizer_HoldCanceledEvent;
        m_RightGestureRecognizer.Tapped -= M_GestureRecognizer_TappedEvent;
        m_RightGestureRecognizer.ManipulationStarted -= M_GestureRecognizer_ManipulationStartedEvent;
        m_RightGestureRecognizer.ManipulationUpdated -= M_GestureRecognizer_ManipulationUpdatedEvent;
        m_RightGestureRecognizer.ManipulationCanceled -= M_GestureRecognizer_ManipulationCanceledEvent;
        m_RightGestureRecognizer.ManipulationCompleted -= M_GestureRecognizer_ManipulationCompletedEvent;
        m_RightGestureRecognizer.StopCapturingGestures();
        m_RightGestureRecognizer.Dispose();

        InteractionManager.InteractionSourceDetected -= InteractionManager_SourceDetected;
        InteractionManager.InteractionSourceLost -= InteractionManager_SourceLost;
        InteractionManager.InteractionSourcePressed -= InteractionManager_SourcePressed;
        InteractionManager.InteractionSourceReleased -= InteractionManager_SourceReleased;
        InteractionManager.InteractionSourceUpdated -= InteractionManager_SourceUpdated;
    }

    [UnityTest]
    public IEnumerator GetSourcePosition()
    {
        yield return null;
        rightHand.EnsureVisible();
        yield return null;

        rightHand.PerformGesture(SimulatedGesture.FingerPressed);
        yield return new WaitForSeconds(k_GestureTapWait);
        Assert.IsTrue(m_SourcePressed, "Finger Press wasn't detected");

        yield return new WaitForSeconds(1f);
        Assert.IsTrue(m_SourceHoldStarted, "Hold start wasn't detected");

        rightHand.PerformGesture(SimulatedGesture.FingerReleased);
        yield return new WaitForSeconds(k_GestureTapWait);
        Assert.AreEqual(m_SourceReleased, m_SourceHoldCompleted, "Hold completed wasn't detected");

        Assert.IsTrue(m_HasHandLocation, "Didn't get hand position");
    }

    [Ignore("Interaction manager is not reporting the data - Known Bug ")]
    [UnityTest]
    public IEnumerator GetSourceVelocity()
    {
        var handX = rightHand.position.x;
        var handY = rightHand.position.y;
        var handZ = rightHand.position.z;

        rightHand.EnsureVisible();
        yield return new WaitForSeconds(k_GestureTapWait);

        rightHand.PerformGesture(SimulatedGesture.FingerPressed);
        yield return new WaitForSeconds(k_GestureTapWait);
        Assert.IsTrue(m_SourcePressed, "Finger Press wasn't detected");

        rightHand.position = new Vector3(handX + 0.2f, handY + 0.52f, handZ + 0.2f);
        yield return new WaitForSeconds(k_GestureTapWait);
        Assert.IsTrue(m_SourceManipulationStarted, "Manipulation Update wasn't detected");

        rightHand.PerformGesture(SimulatedGesture.FingerReleased);
        yield return new WaitForSeconds(k_GestureTapWait);
        Assert.IsTrue(m_SourceReleased, "Finger Release wasn't detected");

        Assert.IsTrue(m_HasHandVelocity, "Didn't get hand Velocity");
    }

    [Ignore("Interaction manager is not reporting the data - Known Bug ")]
    [UnityTest]
    public IEnumerator GetSourceRotation()
    {
        var handX = rightHand.position.x;
        var handY = rightHand.position.y;
        var handZ = rightHand.position.z;

        yield return null;
        rightHand.EnsureVisible();
        yield return null;

        rightHand.PerformGesture(SimulatedGesture.FingerPressed);
        yield return new WaitForSeconds(k_GestureTapWait);
        Assert.IsTrue(m_SourcePressed, "Finger Press wasn't detected");

        rightHand.position = new Vector3(handX + 0.2f, handY + 0.52f, handZ + 0.2f);
        yield return new WaitForSeconds(k_GestureTapWait);
        Assert.IsTrue(m_SourceManipulationStarted, "Manipulation Update wasn't detected");

        rightHand.PerformGesture(SimulatedGesture.FingerReleased);
        yield return new WaitForSeconds(k_GestureTapWait);
        Assert.IsTrue(m_SourceReleased, "Finger Release wasn't detected");

        Assert.IsTrue(m_HasHandRotation, "Didn't get hand rotation");
    }

    [Ignore("Interaction manager is not reporting the data - Known Bug ")]
    [UnityTest]
    public IEnumerator GetSourceUp()
    {
        var handX = rightHand.position.x;
        var handY = rightHand.position.y;
        var handZ = rightHand.position.z;

        yield return null;
        rightHand.EnsureVisible();
        yield return null;

        rightHand.PerformGesture(SimulatedGesture.FingerPressed);
        yield return new WaitForSeconds(k_GestureTapWait);
        Assert.IsTrue(m_SourcePressed, "Finger Press wasn't detected");

        rightHand.position = new Vector3(handX + 0.2f, handY + 0.52f, handZ + 0.2f);
        yield return new WaitForSeconds(2f);
        Assert.IsTrue(m_SourceManipulationStarted, "Manipulation Update wasn't detected");

        rightHand.PerformGesture(SimulatedGesture.FingerReleased);
        yield return new WaitForSeconds(k_GestureTapWait);
        Assert.IsTrue(m_SourceReleased, "Finger Release wasn't detected");

        Assert.IsTrue(m_HasHandUp, "Didn't get hand up");
    }

    [Ignore("Interaction manager is not reporting the data - Known Bug ")]
    [UnityTest]
    public IEnumerator GetSourceForward()
    {
        var handX = rightHand.position.x;
        var handY = rightHand.position.y;
        var handZ = rightHand.position.z;

        yield return null;
        rightHand.EnsureVisible();
        yield return null;

        rightHand.PerformGesture(SimulatedGesture.FingerPressed);
        yield return new WaitForSeconds(k_GestureTapWait);
        Assert.IsTrue(m_SourcePressed, "Finger Press wasn't detected");

        rightHand.position = new Vector3(handX + 0.2f, handY + 0.52f, handZ + 0.2f);
        yield return new WaitForSeconds(2f);
        Assert.IsTrue(m_SourceManipulationStarted, "Manipulation Update wasn't detected");

        rightHand.PerformGesture(SimulatedGesture.FingerReleased);
        yield return new WaitForSeconds(k_GestureTapWait);
        Assert.IsTrue(m_SourceReleased, "Finger Release wasn't detected");

        Assert.IsTrue(m_HasHandForward, "Didn't get hand forward");
    }

    [Ignore("Interaction manager is not reporting the data - Known Bug ")]
    [UnityTest]
    public IEnumerator GetSourceAngularVelocity()
    {
        var handX = rightHand.position.x;
        var handY = rightHand.position.y;
        var handZ = rightHand.position.z;

        rightHand.EnsureVisible();
        yield return new WaitForSeconds(k_GestureTapWait);

        rightHand.PerformGesture(SimulatedGesture.FingerPressed);
        yield return new WaitForSeconds(k_GestureTapWait);
        Assert.IsTrue(m_SourcePressed, "Finger Press wasn't detected");

        rightHand.position = new Vector3(handX + 0.2f, handY + 0.52f, handZ + 0.2f);
        yield return new WaitForSeconds(k_GestureTapWait);
        Assert.IsTrue(m_SourceManipulationStarted, "Manipulation Update wasn't detected");

        rightHand.PerformGesture(SimulatedGesture.FingerReleased);
        yield return new WaitForSeconds(k_GestureTapWait);
        Assert.IsTrue(m_SourceReleased, "Finger Release wasn't detected");

        Assert.IsTrue(m_HasHandAnglularVelocity, "Didn't get Angular Velocity");
    }

    [Ignore("Interaction manager is not reporting the data - Known Bug ")]
    [UnityTest]
    public IEnumerator GetSourcePositionAccuracy()
    {
        var handX = rightHand.position.x;
        var handY = rightHand.position.y;
        var handZ = rightHand.position.z;

        yield return null;
        rightHand.EnsureVisible();
        yield return null;

        rightHand.PerformGesture(SimulatedGesture.FingerPressed);
        yield return new WaitForSeconds(k_GestureTapWait);
        Assert.IsTrue(m_SourcePressed, "Finger Press wasn't detected");

        rightHand.position = new Vector3(handX + 0.25f, handY + 0.52f, handZ + 0.23f);
        yield return new WaitForSeconds(k_GestureTapWait);
        Assert.IsTrue(m_SourceManipulationStarted, "Manipulation Update wasn't detected");

        rightHand.PerformGesture(SimulatedGesture.FingerReleased);
        yield return new WaitForSeconds(k_GestureTapWait);
        Assert.IsTrue(m_SourceReleased, "Finger Release wasn't detected");

        if (m_PositionAccuracy != InteractionSourcePositionAccuracy.None)
        {
            m_HasHandPositionAccuracy = true;
        }
        else if (m_PositionAccuracy == InteractionSourcePositionAccuracy.None)
        {
            m_HasHandPositionAccuracy = false;
        }

        Assert.IsTrue(m_HasHandPositionAccuracy, "Didn't get hand Position Accuracy");
    }

    [UnityTest]
    public IEnumerator GetSourceLossDirection()
    {
        var handX = rightHand.position.x;
        var handY = rightHand.position.y;
        var handZ = rightHand.position.z;

        rightHand.EnsureVisible();
        yield return null;

        rightHand.PerformGesture(SimulatedGesture.FingerPressed);
        yield return new WaitForSeconds(k_GestureTapWait);
        Assert.IsTrue(m_SourcePressed, "Finger Press wasn't detected");

        rightHand.position = new Vector3(handX + 0.2f, handY + 0.52f, handZ + 0.2f);
        yield return new WaitForSeconds(k_GestureTapWait);
        Assert.IsTrue(m_SourceManipulationStarted, "Manipulation Update wasn't detected");

        rightHand.PerformGesture(SimulatedGesture.FingerReleased);
        yield return new WaitForSeconds(k_GestureTapWait);
        Assert.IsTrue(m_SourceReleased, "Finger Release wasn't detected");

        Assert.AreNotEqual(Vector3.zero, m_SourceLossMitigationDirection, "Didn't get Source Loss Mitigation Direction in the Update");
    }

    [Ignore("Source Loss Risk is always 1 in simulation - Known Bug")]
    [UnityTest]
    public IEnumerator GetSourceLossRisk()
    {
        var handX = rightHand.position.x;
        var handY = rightHand.position.y;
        var handZ = rightHand.position.z;

        rightHand.EnsureVisible();
        yield return null;

        rightHand.PerformGesture(SimulatedGesture.FingerPressed);
        yield return new WaitForSeconds(k_GestureTapWait);
        Assert.IsTrue(m_SourcePressed, "Finger Press wasn't detected");

        rightHand.position = new Vector3(handX + 0.2f, handY + 0.52f, handZ + 0.2f);
        yield return new WaitForSeconds(k_GestureTapWait);
        Assert.IsTrue(m_SourceManipulationUpdated, "Manipulation Update wasn't detected");

        rightHand.PerformGesture(SimulatedGesture.FingerReleased);
        yield return new WaitForSeconds(k_GestureTapWait);
        Assert.IsTrue(m_SourceReleased, "Finger Release wasn't detected");

        Assert.AreNotEqual(0.000, m_SourceLossRisk, "Didn't get an updated SourceLossRisk");
        Assert.IsTrue(m_SourceLost, "Didn't get Loss Risk");
    }

    [UnityTest]
    public IEnumerator GetKind()
    {
        yield return null;
        rightHand.EnsureVisible();
        yield return null;

        rightHand.PerformGesture(SimulatedGesture.FingerPressed);
        yield return new WaitForSeconds(k_GestureTapWait);
        Assert.IsTrue(m_SourcePressed, "Finger Press wasn't detected");

        rightHand.PerformGesture(SimulatedGesture.FingerReleased);
        yield return new WaitForSeconds(k_GestureTapWait);
        Assert.IsTrue(m_SourceReleased, "Finger Release wasn't detected");

        Assert.AreEqual(InteractionSourceKind.Hand, m_SourceKind, "Didn't get InteractionSourceKind.Hand");
    }

    [UnityTest]
    public IEnumerator GetID()
    {
        rightHand.EnsureVisible();
        yield return null;

        rightHand.PerformGesture(SimulatedGesture.FingerPressed);
        yield return new WaitForSeconds(k_GestureTapWait);
        Assert.IsTrue(m_SourcePressed, "Finger Press wasn't detected");

        rightHand.PerformGesture(SimulatedGesture.FingerReleased);
        yield return new WaitForSeconds(k_GestureTapWait);
        Assert.IsTrue(m_SourceReleased, "Finger Release wasn't detected");

        Assert.AreNotEqual(0, m_SourceId, "Didn't get hand ID");
    }

    [UnityTest]
    public IEnumerator GetSourcePressType()
    {
        rightHand.EnsureVisible();
        yield return null;

        rightHand.PerformGesture(SimulatedGesture.FingerPressed);
        yield return new WaitForSeconds(k_GestureTapWait);
        Assert.IsTrue(m_SourcePressed, "Finger Press wasn't detected");

        rightHand.PerformGesture(SimulatedGesture.FingerReleased);
        yield return new WaitForSeconds(k_GestureTapWait);
        Assert.IsTrue(m_SourceReleased, "Finger Release wasn't detected");

        Assert.AreEqual(InteractionSourcePressType.Select, m_PressType, "Press Type didn't come back as the expected Air-Tap / Select!");
    }

    [UnityTest]
    public IEnumerator SourcePressAndReleaseEventCheck()
    {
        rightHand.EnsureVisible();
        yield return null;

        rightHand.PerformGesture(SimulatedGesture.FingerPressed);
        yield return new WaitForSeconds(k_GestureTapWait);
        Assert.IsTrue(m_SourcePressed, "Finger Press wasn't detected");

        rightHand.PerformGesture(SimulatedGesture.FingerReleased);
        yield return new WaitForSeconds(k_GestureTapWait);
        Assert.IsTrue(m_SourceReleased, "Finger Release wasn't detected");
    }

    [UnityTest]
    public IEnumerator SourceDetectedEventCheck()
    {
        rightHand.EnsureVisible();
        yield return new WaitForSeconds(k_GestureTapWait);

        rightHand.PerformGesture(SimulatedGesture.FingerPressed);
        yield return new WaitForSeconds(k_GestureTapWait);
        Assert.IsTrue(m_SourcePressed, "Finger Press wasn't detected");

        rightHand.PerformGesture(SimulatedGesture.FingerReleased);
        yield return new WaitForSeconds(k_GestureTapWait);
        Assert.IsTrue(m_SourceReleased, "Finger Release wasn't detected");

        Assert.IsTrue(m_SourceDetected, "Source not detected");
    }

    [UnityTest]
    public IEnumerator SourceLostEventCheck()
    {
        rightHand.EnsureVisible();
        yield return new WaitForSeconds(k_GestureTapWait);

        rightHand.PerformGesture(SimulatedGesture.FingerPressed);
        yield return new WaitForSeconds(k_GestureTapWait);
        Assert.IsTrue(m_SourcePressed, "Finger Press wasn't detected");

        rightHand.PerformGesture(SimulatedGesture.FingerReleased);
        yield return new WaitForSeconds(k_GestureTapWait);
        Assert.IsTrue(m_SourceReleased, "Finger Release wasn't detected");

        rightHand.activated = false;
        yield return new WaitForSeconds(k_GestureTapWait);

        Assert.IsTrue(m_SourceLost, "Source is still detected");
    }

    //[Ignore("Bug causing interaction source state coming back empty in simulation (965088)")]
    [UnityTest]
    public IEnumerator CheckCurrentReading()
    {
        InteractionSourceState[] sourceStates = new InteractionSourceState[] {};

        yield return new WaitForSeconds(k_GestureTapWait);
        InteractionManager.GetCurrentReading(sourceStates);

        Assert.IsNotEmpty(sourceStates, "Source State Array is Empty");
    }

    private void M_GestureRecognizer_TappedEvent(TappedEventArgs obj)
    {
        Debug.Log("Tap" + " Count: " + obj.tapCount);
        Debug.Log("Source: " + obj.source);
    }

    private void M_GestureRecognizer_ManipulationCompletedEvent(ManipulationCompletedEventArgs obj)
    {
        m_SourceManipulationCompleted = true;
    }

    private void M_GestureRecognizer_ManipulationCanceledEvent(ManipulationCanceledEventArgs obj)
    {
        m_SourceManipulationCanceled = true;
    }

    private void M_GestureRecognizer_ManipulationUpdatedEvent(ManipulationUpdatedEventArgs obj)
    {
        m_SourceManipulationUpdated = true;
        Debug.Log("Manipulation: " + obj.cumulativeDelta);
    }

    private void M_GestureRecognizer_ManipulationStartedEvent(ManipulationStartedEventArgs obj)
    {
        m_SourceManipulationStarted = true;
    }

    private void M_GestureRecognizer_HoldCanceledEvent(HoldCanceledEventArgs obj)
    {
        m_SourceHoldCanceled = true;
    }

    private void M_GestureRecognizer_HoldCompletedEvent(HoldCompletedEventArgs obj)
    {
        m_SourceHoldCompleted = true;
    }

    private void M_GestureRecognizer_HoldStartedEvent(HoldStartedEventArgs obj)
    {
        m_SourceHoldStarted = true;
    }

    private void InteractionManager_SourceDetected(InteractionSourceDetectedEventArgs obj)
    {
        m_SourceDetected = true;
    }

    private void InteractionManager_SourceUpdated(InteractionSourceUpdatedEventArgs obj)
    {
        InteractionSourceState[] sourceState = new InteractionSourceState[] {}; 
        m_SourceUpdated = true;

        m_HasHandLocation = obj.state.sourcePose.TryGetPosition(out m_HandPosition);
        m_HasHandVelocity = obj.state.sourcePose.TryGetVelocity(out m_HandVelocity);
        m_HasHandRotation = obj.state.sourcePose.TryGetRotation(out m_HandRotation);
        m_HasHandUp = obj.state.sourcePose.TryGetUp(out m_HandUp);
        m_HasHandAnglularVelocity = obj.state.sourcePose.TryGetAngularVelocity(out m_HandAngularVelocity);
        m_HasHandForward = obj.state.sourcePose.TryGetForward(out m_HandForward);

        m_PositionAccuracy = obj.state.sourcePose.positionAccuracy;

        m_SourceKind = obj.state.source.kind;

        m_SourceId = obj.state.source.id;

        m_SourceLossMitigationDirection = obj.state.properties.sourceLossMitigationDirection;

        // There is a bug for this test that sourceLossRisk will always be 1 for simulation
        m_SourceLossRisk = obj.state.properties.sourceLossRisk;

        Debug.Log("Location: " + m_HandPosition);
        Debug.Log("Velocity: " + m_HandVelocity);
        Debug.Log("SourceLossMitigationDirection: " + obj.state.properties.sourceLossMitigationDirection);
        Debug.Log("SourceLossRisk: " + obj.state.properties.sourceLossRisk);
        Debug.Log("Kind: " + obj.state.source.kind);
        Debug.Log("ID: " + obj.state.source.id);
    }

    private void InteractionManager_SourceReleased(InteractionSourceReleasedEventArgs obj)
    {
        m_SourceReleased = true;
    }

    private void InteractionManager_SourcePressed(InteractionSourcePressedEventArgs obj)
    {
        m_SourcePressed = true;

        m_PressType = obj.pressType;
    }

    private void InteractionManager_SourceLost(InteractionSourceLostEventArgs obj)
    {
        m_SourceLost = true;
    }
}
