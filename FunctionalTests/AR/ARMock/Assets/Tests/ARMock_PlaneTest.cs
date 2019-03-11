using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.XR.Mock;
using UnityEngine.Experimental.XR;
using UnityEngine.XR.ARFoundation;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Tests
{
    public class ARMock_PlaneTest : IPrebuildSetup
    {
        private TrackableId planeId;
        private ARSessionOrigin origin;
        private ARPlaneManager planeManager;

        public void Setup()
        {
#if UNITY_EDITOR
            // Android - ARCore Prebuild
            PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, "com.unity.ARMockTests");
            PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel24;
            PlayerSettings.SetMobileMTRendering(BuildTargetGroup.Android, false);
            EditorUserBuildSettings.androidBuildType = AndroidBuildType.Development;
            EditorUserBuildSettings.androidBuildSystem = AndroidBuildSystem.Gradle;


            // iOS - ARKit Prebuild
            PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.iOS, "com.unity.ARMockTests");
            PlayerSettings.iOS.cameraUsageDescription = "Capture video feed for AR Background Rendering";
            PlayerSettings.iOS.targetOSVersionString = "11.0";
            EditorUserBuildSettings.iOSBuildConfigType = iOSBuildType.Debug;
#endif
        }

        [SetUp]
        public void SetupTest()
        {
            GameObject arSessionOrigin = new GameObject("AR Session Origin");
            origin = arSessionOrigin.AddComponent<ARSessionOrigin>();
            planeManager = arSessionOrigin.AddComponent<ARPlaneManager>();
            GameObject arSession = new GameObject("AR Session");
            arSessionOrigin.AddComponent<ARSession>();

            Vector3 planePosition = new Vector3(1, 0, 0);
            Quaternion planeRotation = new Quaternion(0, 0, 0, 0);
            Vector2 planeSize = new Vector2(1, 1);
            Vector3 planeCenter = new Vector3(.25f, .25f);

            Pose planePose = new Pose(planePosition, planeRotation);

            planeId = PlaneApi.Add(planePose, planeCenter, planeSize);
        }

        // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
        // `yield return null;` to skip a frame.
        [UnityTest]
        public IEnumerator ARMock_PlaneTestWithEnumeratorPasses()
        {
            yield return null;

            ARPlane plane = planeManager.TryGetPlane(planeId);

            Assert.IsNotNull(plane);

            Assert.That(plane.trackingState == TrackingState.Tracking, "Plane Tracking State: {0}", plane.trackingState);
        }
    }
}
