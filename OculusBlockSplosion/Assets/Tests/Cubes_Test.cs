using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

//Documentation on Unity Playmode tests can be found at: https://docs.unity3d.com/Manual/testing-editortestsrunner.html

namespace Tests
{
    //Define what platforms these tests scripts will work in. Attempting to run on other platforms will generate a console error
    [UnityPlatform (include = new[] { RuntimePlatform.Android, RuntimePlatform.WindowsPlayer, RuntimePlatform.WindowsEditor})]
    public class Cubes_Test : OculusPrebuildSetup
    {

        public void Setup()
        {

        }

        //This setup script is run after the one from OculusPrebuildSetup
        [SetUp]
        public void SetupTest()
        {
            SceneManager.LoadScene("Cubes");
        }

        // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
        // `yield return null;` to skip a frame. A 'yield return null' is required to
        // run the coroutine
        [UnityTest]
        public IEnumerator Cubes_TestLaunch()
        {
			yield return null;

			Debug.Log( "[LoadLevel] Cubes Time: " + Time.time );

            Assert.IsTrue(SceneManager.GetActiveScene().name == "Cubes");

            VerifyStandardOculusComponents();
        }
			
		[UnityTest]
		public IEnumerator Cubes_TestOVRPluginAndSDK()
		{
			yield return null;
			Assert.AreNotEqual (OVRPlugin.version.ToString().Substring(0,1), "0");
			Assert.AreNotEqual (OVRPlugin.nativeSDKVersion.ToString().Substring (0, 1), "0");
		}

        private void VerifyStandardOculusComponents()
        {
            GameObject cameraRig = GameObject.Find("OVRCameraRig");
            Assert.IsNotNull(cameraRig.GetComponent<OVRManager>());
            Assert.IsNotNull(cameraRig.GetComponent<OVRCameraRig>());

            // Will only pass if HMD is being worn
			Assert.IsTrue(OVRManager.isHmdPresent, "HMD is present");
            Assert.IsTrue(OVRManager.hasInputFocus, "App has input focus");
            Assert.IsTrue(OVRManager.hasVrFocus, "App has VR focus");
        }
    }
}
