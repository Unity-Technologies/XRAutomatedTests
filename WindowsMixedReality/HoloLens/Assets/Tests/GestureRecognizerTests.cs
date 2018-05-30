using UnityEngine;
using UnityEngine.TestTools;
using UnityEditorInternal.VR;
using NUnit.Framework;
using System.Collections;
using UnityEngine.XR.WSA;
using UnityEngine.XR.WSA.Input;

//[Ignore("Metro/wsa is disabled on Katana")]
internal class GestureRecognizerTests : HoloLensTestBase
{
    GestureRecognizer m_RightGestureRecognizer;
    GestureRecognizer m_LeftGestureRecognizer;

    bool m_SourceDetected = false;
    bool m_SourceLost = false;
    bool m_SourcePressed = false;
    bool m_SourceReleased = false;
    bool m_SourceUpdated = false;

    bool m_SourceManipulationStarted = false;
    bool m_SourceManipulationCanceled = false;
    bool m_SourceManipulationUpdated = false;
    bool m_SourceManipulationCompleted = false;

    bool m_SourceNavigationStarted = false;
    bool m_SourceNavigationCanceled = false;
    bool m_SourceNavigationUpdated = false;
    bool m_SourceNavigationCompleted = false;

    bool m_SourceHoldStarted = false;
    bool m_SourceHoldCanceled = false;
    bool m_SourceHoldCompleted = false;

    float gestureTapWait = 0.2f;

    int m_TapCount = 0;

    private Vector3 HandPosition;

    [SetUp]
    public void Setup()
    {
        m_SourceDetected = m_SourceLost = m_SourcePressed = m_SourceReleased = m_SourceUpdated = false;
        m_SourceManipulationStarted = m_SourceManipulationCanceled = m_SourceManipulationUpdated = m_SourceManipulationCompleted = false;
        m_SourceHoldStarted = m_SourceHoldCanceled = m_SourceHoldCompleted = false;

        rightHand.activated = true;
        rightHand.position = body.position;

        leftHand.activated = false;
        leftHand.position = body.position;

        // Right hand gesture recognizer
        m_RightGestureRecognizer = new GestureRecognizer();
        m_RightGestureRecognizer.SetRecognizableGestures(GestureSettings.Tap | GestureSettings.ManipulationTranslate | GestureSettings.Hold | GestureSettings.DoubleTap);
        m_RightGestureRecognizer.HoldStarted += M_RightGestureRecognizer_HoldStartedEvent;
        m_RightGestureRecognizer.HoldCompleted += M_RightGestureRecognizer_HoldCompletedEvent;
        m_RightGestureRecognizer.HoldCanceled += M_RightGestureRecognizer_HoldCanceledEvent;
        m_RightGestureRecognizer.Tapped += M_RightGestureRecognizer_TappedEvent;
        m_RightGestureRecognizer.ManipulationStarted += M_RightGestureRecognizer_ManipulationStartedEvent;
        m_RightGestureRecognizer.ManipulationUpdated += M_RightGestureRecognizer_ManipulationUpdatedEvent;
        m_RightGestureRecognizer.ManipulationCanceled += M_RightGestureRecognizer_ManipulationCanceledEvent;
        m_RightGestureRecognizer.ManipulationCompleted += M_RightGestureRecognizer_ManipulationCompletedEvent;
        m_RightGestureRecognizer.StartCapturingGestures();

        // Left hand gesture recognizer
        m_LeftGestureRecognizer = new GestureRecognizer();
        m_LeftGestureRecognizer.SetRecognizableGestures(GestureSettings.NavigationX | GestureSettings.NavigationY | GestureSettings.NavigationZ);
        m_LeftGestureRecognizer.NavigationStarted += NavigationGesture_NavigationStartedEvent;
        m_LeftGestureRecognizer.NavigationUpdated += NavigationGesture_NavigationUpdatedEvent;
        m_LeftGestureRecognizer.NavigationCanceled += NavigationGesture_NavigationCanceledEvent;
        m_LeftGestureRecognizer.NavigationCompleted += NavigationGesture_NavigationCompletedEvent;

        InteractionManager.InteractionSourceDetected += InteractionManager_SourceDetected;
        InteractionManager.InteractionSourceLost += InteractionManager_SourceLost;
        InteractionManager.InteractionSourcePressed += InteractionManager_SourcePressed;
        InteractionManager.InteractionSourceReleased += InteractionManager_SourceReleased;
        InteractionManager.InteractionSourceUpdated += InteractionManager_SourceUpdated;
    }

    [TearDown]
    public void TearDown()
    {
        m_RightGestureRecognizer.Tapped -= M_RightGestureRecognizer_TappedEvent;
        m_RightGestureRecognizer.HoldStarted -= M_RightGestureRecognizer_HoldStartedEvent;
        m_RightGestureRecognizer.HoldCompleted -= M_RightGestureRecognizer_HoldCompletedEvent;
        m_RightGestureRecognizer.HoldCanceled -= M_RightGestureRecognizer_HoldCanceledEvent;
        m_RightGestureRecognizer.ManipulationStarted -= M_RightGestureRecognizer_ManipulationStartedEvent;
        m_RightGestureRecognizer.ManipulationUpdated -= M_RightGestureRecognizer_ManipulationUpdatedEvent;
        m_RightGestureRecognizer.ManipulationCanceled -= M_RightGestureRecognizer_ManipulationCanceledEvent;
        m_RightGestureRecognizer.ManipulationCompleted -= M_RightGestureRecognizer_ManipulationCompletedEvent;
        m_RightGestureRecognizer.StopCapturingGestures();
        m_RightGestureRecognizer.Dispose();

        m_LeftGestureRecognizer.NavigationStarted -= NavigationGesture_NavigationStartedEvent;
        m_LeftGestureRecognizer.NavigationUpdated -= NavigationGesture_NavigationUpdatedEvent;
        m_LeftGestureRecognizer.NavigationCanceled -= NavigationGesture_NavigationCanceledEvent;
        m_LeftGestureRecognizer.NavigationCompleted -= NavigationGesture_NavigationCompletedEvent;
        m_LeftGestureRecognizer.StopCapturingGestures();
        m_LeftGestureRecognizer.Dispose();

        InteractionManager.InteractionSourceDetected -= InteractionManager_SourceDetected;
        InteractionManager.InteractionSourceLost -= InteractionManager_SourceLost;
        InteractionManager.InteractionSourcePressed -= InteractionManager_SourcePressed;
        InteractionManager.InteractionSourceReleased -= InteractionManager_SourceReleased;
        InteractionManager.InteractionSourceUpdated -= InteractionManager_SourceUpdated;

        m_TapCount = 0;
    }

    [UnityTest]
    public IEnumerator GestureTap()
    {
        yield return new WaitForSeconds(gestureTapWait);
        rightHand.EnsureVisible();
        yield return null;

        rightHand.PerformGesture(SimulatedGesture.FingerPressed);
        yield return new WaitForSeconds(gestureTapWait);
        Assert.IsTrue(m_SourcePressed, "Finger Press wasn't detected");
        Assert.IsFalse(m_SourceReleased, "Finger Release was detected during a finger press");

        rightHand.PerformGesture(SimulatedGesture.FingerReleased);
        yield return new WaitForSeconds(gestureTapWait);
        Assert.IsTrue(m_SourceReleased,  "Finger Release wasn't detected");
        Assert.IsFalse(m_SourcePressed, "Finger Press was detected during a finger release");
    }

    [UnityTest]
    public IEnumerator GestureManipulation()
    {
        var handX = rightHand.position.x;
        var handY = rightHand.position.y;
        var handZ = rightHand.position.z;

        yield return new WaitForSeconds(gestureTapWait);
        rightHand.EnsureVisible();
        yield return null;

        rightHand.PerformGesture(SimulatedGesture.FingerPressed);
        yield return new WaitForSeconds(gestureTapWait);
        Assert.IsTrue(m_SourcePressed, "Finger Press wasn't detected");

        rightHand.position = new Vector3(0.0f, 1.5f, 0.8f);
        yield return new WaitForSeconds(0.5f);
        Assert.IsTrue(m_SourceManipulationStarted, "Manipulation Start wasn't detected");

        rightHand.position = new Vector3(0.0f, 1.2f, 0.4f);
        Assert.IsTrue(m_SourceManipulationUpdated, "Manipulation Update wasn't detected");

        rightHand.PerformGesture(SimulatedGesture.FingerReleased);
        yield return new WaitForSeconds(gestureTapWait);
        Assert.IsTrue(m_SourceReleased, "Finger Release wasn't detected");
        Assert.IsTrue(m_SourceManipulationCompleted, "Manipulation never completed");
    }

    [UnityTest]
    public IEnumerator GestureNavigation()
    {
        m_RightGestureRecognizer.StopCapturingGestures();
        yield return new WaitForSeconds(gestureTapWait);

        rightHand.activated = false;
        yield return null;
        leftHand.activated = true;
        yield return null;
        leftHand.position = body.position;
        yield return null;

        m_LeftGestureRecognizer.StartCapturingGestures();

        leftHand.EnsureVisible();
        yield return null;

        var handX = leftHand.position.x;
        var handY = leftHand.position.y;
        var handZ = leftHand.position.z;

        leftHand.PerformGesture(SimulatedGesture.FingerPressed);
        yield return new WaitForSeconds(gestureTapWait);
        Assert.IsTrue(m_SourcePressed, "Finger Press wasn't detected");

        leftHand.position = new Vector3(handX + 0.5f, handY + 0.5f, handZ + 0.5f);
        yield return new WaitForSeconds(gestureTapWait);
        Assert.IsTrue(m_SourceNavigationStarted, "Navigation Start wasn't detected");
        Assert.IsTrue(m_SourceNavigationUpdated, "Navigation Update wasn't detected");

        leftHand.PerformGesture(SimulatedGesture.FingerReleased);
        yield return new WaitForSeconds(gestureTapWait);
        Assert.IsTrue(m_SourceReleased, "Finger Release wasn't detected");
        Assert.IsTrue(m_SourceNavigationCompleted, "Navigation Completed wasn't detected");

        m_LeftGestureRecognizer.StopCapturingGestures();

        yield return new WaitForSeconds(gestureTapWait);
    }

    [UnityTest]
    public IEnumerator GestureManipulationCancel()
    {
        rightHand.EnsureVisible();
        yield return null;
        yield return new WaitForSeconds(0.5f);

        rightHand.PerformGesture(SimulatedGesture.FingerPressed);
        yield return new WaitForSeconds(gestureTapWait);
        Assert.IsTrue(m_SourcePressed, "Finger Press wasn't detected");

        rightHand.position = new Vector3(0.0f, 1.5f, 0.8f);
        yield return new WaitForSeconds(0.8f);
        Assert.IsTrue(m_SourceManipulationStarted, "Manipulation Start wasn't detected");

        rightHand.position = new Vector3(9f, 9f, 9f);
        rightHand.PerformGesture(SimulatedGesture.FingerReleased);
        yield return new WaitForSeconds(gestureTapWait);

        Assert.IsTrue(m_SourceManipulationCanceled, "Manipulation never canceled");
    }

    [UnityTest]
    public IEnumerator GestureNavigationCancel()
    {
        m_RightGestureRecognizer.StopCapturingGestures();
        yield return new WaitForSeconds(gestureTapWait);

        rightHand.activated = false;
        yield return null;
        leftHand.activated = true;
        yield return null;
        leftHand.position = body.position;
        yield return null;

        m_LeftGestureRecognizer.StartCapturingGestures();

        leftHand.EnsureVisible();
        yield return null;

        var handX = leftHand.position.x;
        var handY = leftHand.position.y;
        var handZ = leftHand.position.z;

        leftHand.PerformGesture(SimulatedGesture.FingerPressed);
        yield return new WaitForSeconds(gestureTapWait);
        Assert.IsTrue(m_SourcePressed, "Finger Press wasn't detected");

        leftHand.position = new Vector3(handX + 0.5f, handY + 0.5f, handZ + 0.5f);
        yield return new WaitForSeconds(gestureTapWait);
        Assert.IsTrue(m_SourceNavigationStarted, "Navigation Start wasn't detected");
        Assert.IsTrue(m_SourceNavigationUpdated, "Navigation Update wasn't detected");

        leftHand.position = new Vector3(9f, 9f, 9f);
        leftHand.PerformGesture(SimulatedGesture.FingerReleased);
        yield return new WaitForSeconds(gestureTapWait);
        Assert.IsTrue(m_SourceNavigationCanceled, "Navigation Cancel wasn't detected");

        m_LeftGestureRecognizer.StopCapturingGestures();

        yield return new WaitForSeconds(gestureTapWait);
    }

    [UnityTest]
    public IEnumerator GestureHold()
    {
        yield return null;
        rightHand.EnsureVisible();
        yield return null;

        yield return new WaitForSeconds(gestureTapWait);

        rightHand.PerformGesture(SimulatedGesture.FingerPressed);
        yield return new WaitForSeconds(gestureTapWait);
        Assert.IsTrue(m_SourcePressed, "Finger Press wasn't detected");

        yield return new WaitForSeconds(1f);
        Assert.IsTrue(m_SourceHoldStarted, "Hold start wasn't detected");

        rightHand.PerformGesture(SimulatedGesture.FingerReleased);
        yield return new WaitForSeconds(gestureTapWait);
        Assert.IsTrue(m_SourceReleased, "Finger wasn't released");
        Assert.IsTrue(m_SourceHoldCompleted, "Hold completed wasn't detected");

        yield return new WaitForSeconds(gestureTapWait);
    }

    [UnityTest]
    public IEnumerator GestureHoldCancel()
    {
        yield return null;
        rightHand.EnsureVisible();
        yield return null;

        yield return new WaitForSeconds(gestureTapWait);

        rightHand.PerformGesture(SimulatedGesture.FingerPressed);
        yield return new WaitForSeconds(gestureTapWait);
        Assert.IsTrue(m_SourcePressed, "Finger Press wasn't detected");

        yield return new WaitForSeconds(1f);
        Assert.IsTrue(m_SourceHoldStarted, "Hold start wasn't detected");

        rightHand.position = new Vector3(9f, 9f, 9f);

        rightHand.PerformGesture(SimulatedGesture.FingerReleased);
        yield return new WaitForSeconds(gestureTapWait);

        Assert.IsTrue(m_SourceHoldCanceled, "Hold Cancel wasn't detected");

        yield return new WaitForSeconds(gestureTapWait);
    }

    [UnityTest]
    public IEnumerator GestureDoubleTap()
    {
        yield return new WaitForSeconds(gestureTapWait);
        rightHand.EnsureVisible();
        yield return new WaitForSeconds(0.5f);

        rightHand.PerformGesture(SimulatedGesture.FingerPressed);
        yield return new WaitForSeconds(gestureTapWait);
        Assert.IsTrue(m_SourcePressed, "Finger Press wasn't detected");

        rightHand.PerformGesture(SimulatedGesture.FingerReleased);
        yield return new WaitForSeconds(gestureTapWait);
        Assert.IsTrue(m_SourceReleased, "Finger Release wasn't detected");

        rightHand.PerformGesture(SimulatedGesture.FingerPressed);
        yield return new WaitForSeconds(gestureTapWait);
        Assert.IsTrue(m_SourcePressed, "Finger Press wasn't detected");

        rightHand.PerformGesture(SimulatedGesture.FingerReleased);
        yield return new WaitForSeconds(gestureTapWait);
        Assert.IsTrue(m_SourceReleased, "Finger Release wasn't detected");

        Assert.AreEqual(2, m_TapCount);
    }

    private void M_RightGestureRecognizer_TappedEvent(TappedEventArgs obj)
    {
        Debug.Log("Tap" + " Count: " + obj.tapCount);
        Debug.Log("Source: " + obj.source);

        m_TapCount = obj.tapCount;
    }

    private void M_RightGestureRecognizer_ManipulationCompletedEvent(ManipulationCompletedEventArgs obj)
    {
        m_SourceManipulationCompleted = true;
    }

    private void M_RightGestureRecognizer_ManipulationCanceledEvent(ManipulationCanceledEventArgs obj)
    {
        m_SourceManipulationCanceled = true;
    }

    private void M_RightGestureRecognizer_ManipulationUpdatedEvent(ManipulationUpdatedEventArgs obj)
    {
        m_SourceManipulationUpdated = true;
    }

    private void M_RightGestureRecognizer_ManipulationStartedEvent(ManipulationStartedEventArgs obj)
    {
        m_SourceManipulationStarted = true;
    }

    private void M_RightGestureRecognizer_HoldCanceledEvent(HoldCanceledEventArgs obj)
    {
        m_SourceHoldCanceled = true;
    }

    private void M_RightGestureRecognizer_HoldCompletedEvent(HoldCompletedEventArgs obj)
    {
        m_SourceHoldCompleted = true;
    }

    private void M_RightGestureRecognizer_HoldStartedEvent(HoldStartedEventArgs obj)
    {
        m_SourceHoldStarted = true;
    }

    private void InteractionManager_SourceDetected(InteractionSourceDetectedEventArgs obj)
    {
        m_SourceDetected = true;
    }

    private void InteractionManager_SourceUpdated(InteractionSourceUpdatedEventArgs obj)
    {
        m_SourceUpdated = true;
    }

    private void InteractionManager_SourceReleased(InteractionSourceReleasedEventArgs obj)
    {
        m_SourcePressed = false;
        m_SourceReleased = true;
    }

    private void InteractionManager_SourcePressed(InteractionSourcePressedEventArgs obj)
    {
        m_SourcePressed = true;
        m_SourceReleased = false;
    }

    private void InteractionManager_SourceLost(InteractionSourceLostEventArgs obj)
    {
        m_SourceLost = true;
    }

    //******************************Left Hand Navigation Events ************************
    private void NavigationGesture_NavigationCompletedEvent(NavigationCompletedEventArgs obj)
    {
        m_SourceNavigationCompleted = true;
    }

    private void NavigationGesture_NavigationCanceledEvent(NavigationCanceledEventArgs obj)
    {
        m_SourceNavigationCanceled = true;
    }

    private void NavigationGesture_NavigationUpdatedEvent(NavigationUpdatedEventArgs obj)
    {
        m_SourceNavigationUpdated = true;
        Debug.Log(obj.normalizedOffset);
    }

    private void NavigationGesture_NavigationStartedEvent(NavigationStartedEventArgs obj)
    {
        obj.sourcePose.TryGetPosition(out HandPosition);
        m_SourceNavigationStarted = true;
        Debug.Log(HandPosition);
    }
}
