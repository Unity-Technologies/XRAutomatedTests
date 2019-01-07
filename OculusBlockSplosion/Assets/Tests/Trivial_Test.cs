using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;
using UnityEngine.XR;
#if UNITY_EDITOR
using UnityEditor;
#endif

//Documentation on Unity Playmode tests can be found at: https://docs.unity3d.com/Manual/testing-editortestsrunner.html

namespace Tests
{
    //Define what platforms these tests scripts will work in. Attempting to run on other platforms will generate a console error
    [UnityPlatform (include = new[] { RuntimePlatform.Android, RuntimePlatform.WindowsPlayer, RuntimePlatform.WindowsEditor})]
    public class Trivial_Test : OculusPrebuildSetup
    {

        public void Setup()
        {

        }

        //This setup script is run after the one from OculusPrebuildSetup
        [SetUp]
        public void SetupTest()
        {
            SceneManager.LoadScene("Trivial");
        }

        // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
        // `yield return null;` to skip a frame. A 'yield return null' is required to
        // run the coroutine
        [UnityTest]
        public IEnumerator Trivial_TestLaunch()
        {
			yield return null;

			Debug.Log( "[LoadLevel] Trivial Time: " + Time.time );

            Assert.IsTrue(SceneManager.GetActiveScene().name == "Trivial");

            VerifyStandardOculusComponents();
        }

        private void VerifyStandardOculusComponents()
        {
            GameObject cameraRig = GameObject.Find("OVRCameraRig");
            Assert.IsNull(cameraRig);

            // Will only pass if HMD is being worn
			Assert.IsTrue(XRDevice.isPresent, "HMD is present");
            Assert.IsTrue(XRSettings.isDeviceActive, "App has VR focus");
			Assert.AreEqual ("Present", XRDevice.userPresence.ToString());
        }
    }
}
