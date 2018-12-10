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
    public class BlockSplosion_Test : OculusPrebuildSetup
    {

        bool sceneChanged = false;

        public void Setup()
        {

        }

        //This setup script is run after the one from OculusPrebuildSetup
        [SetUp]
        public void SetupTest()
        {
            SceneManager.LoadScene("Startup_Sample");
        }

        // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
        // `yield return null;` to skip a frame. A 'yield return null' is required to
        // run the coroutine
        [UnityTest]
        public IEnumerator BlockSplosion_TestLoadingScreen()
        {
            yield return null;

            Assert.IsTrue(SceneManager.GetActiveScene().name == "Startup_Sample");

            VerifyStandardOculusComponents();

            GameObject waitCursor = GameObject.Find("WaitCursor");
            Assert.IsNotNull(waitCursor.GetComponent<OVRWaitCursor>());
        }

        [UnityTest]
        public IEnumerator BlockSplosion_TestSceneChange()
        {
            yield return null;

            SceneManager.activeSceneChanged += ChangedActiveScene;

            int framesWaited = 0;
            while ( !sceneChanged && framesWaited < 1000)
            {
                framesWaited++;
                yield return null;
            }

            Assert.IsTrue(sceneChanged);

            VerifyStandardOculusComponents();
        }

        private void ChangedActiveScene(Scene current, Scene next)
        {
            if (next.name == "Main")
            {
                sceneChanged = true;
            }
        }

        private void VerifyStandardOculusComponents()
        {
            GameObject cameraRig = GameObject.Find("OVRCameraRig");
            Assert.IsNotNull(cameraRig.GetComponent<OVRManager>());
            Assert.IsNotNull(cameraRig.GetComponent<OVRCameraRig>());

            // Will only pass if HMD is being worn
            Assert.IsTrue(OVRManager.hasInputFocus, "App has input focus");
            Assert.IsTrue(OVRManager.hasVrFocus, "App has VR focus");
        }
    }
}
