using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.XR;

#if UNITY_EDITOR
using UnityEditorInternal.VR;
using UnityEditor;
#endif

#if UNITY_METRO
using UnityEngine.XR.WSA.Input;
using UnityEngine.XR.WSA;
#endif

public class WindowsMR
{
    internal class GestureTesting : TestBaseSetup
    {
#if UNITY_METRO
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
        public void GestureSetup()
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
        public void GestureTearDown()
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
            OnlyRunHoloLensSimulatedDeviceCheck();

            yield return new WaitForSeconds(gestureTapWait);
            rightHand.EnsureVisible();
            yield return null;

            rightHand.PerformGesture(SimulatedGesture.FingerPressed);
            yield return new WaitForSeconds(gestureTapWait);
            Assert.IsTrue(m_SourcePressed, "Finger Press wasn't detected");
            Assert.IsFalse(m_SourceReleased, "Finger Release was detected during a finger press");

            rightHand.PerformGesture(SimulatedGesture.FingerReleased);
            yield return new WaitForSeconds(gestureTapWait);
            Assert.IsTrue(m_SourceReleased, "Finger Release wasn't detected");
            Assert.IsFalse(m_SourcePressed, "Finger Press was detected during a finger release");
        }

        [UnityTest]
        public IEnumerator GestureManipulation()
        {
            OnlyRunHoloLensSimulatedDeviceCheck();

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
            OnlyRunHoloLensSimulatedDeviceCheck();

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
            OnlyRunHoloLensSimulatedDeviceCheck();

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
            OnlyRunHoloLensSimulatedDeviceCheck();

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
            OnlyRunHoloLensSimulatedDeviceCheck();

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
            OnlyRunHoloLensSimulatedDeviceCheck();

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
            OnlyRunHoloLensSimulatedDeviceCheck();

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
#endif
    }

    internal class InteractionManagerTesting : TestBaseSetup
    {
#if UNITY_METRO
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
        public void InteractionSetup()
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
        public void InteractionTearDown()
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
            OnlyRunHoloLensSimulatedDeviceCheck();
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
            OnlyRunHoloLensSimulatedDeviceCheck();
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
            OnlyRunHoloLensSimulatedDeviceCheck();
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
            OnlyRunHoloLensSimulatedDeviceCheck();
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
            OnlyRunHoloLensSimulatedDeviceCheck();
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
            OnlyRunHoloLensSimulatedDeviceCheck();
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
            OnlyRunHoloLensSimulatedDeviceCheck();
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
            OnlyRunHoloLensSimulatedDeviceCheck();
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
            OnlyRunHoloLensSimulatedDeviceCheck();
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
            OnlyRunHoloLensSimulatedDeviceCheck();
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
            OnlyRunHoloLensSimulatedDeviceCheck();
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
            OnlyRunHoloLensSimulatedDeviceCheck();
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
            OnlyRunHoloLensSimulatedDeviceCheck();
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
            OnlyRunHoloLensSimulatedDeviceCheck();
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
            OnlyRunHoloLensSimulatedDeviceCheck();
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

        [Ignore("Bug causing interaction source state coming back empty in simulation (965088)")]
        [UnityTest]
        public IEnumerator CheckCurrentReading()
        {
            OnlyRunHoloLensSimulatedDeviceCheck();
            InteractionSourceState[] sourceStates = new InteractionSourceState[] { };

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
            InteractionSourceState[] sourceState = new InteractionSourceState[] { };
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
#endif
    }
}
