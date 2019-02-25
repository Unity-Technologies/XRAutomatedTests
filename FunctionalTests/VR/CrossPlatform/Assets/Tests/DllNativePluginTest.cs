using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TestTools;

internal class DllNativePluginTest : TestBaseSetup
{
    private bool m_SceneObjectsLoaded = false;
    private bool m_RenderingImage = false;

    private GameObject m_RenderPlain;
    private GameObject m_BaseSphere;

    private Light m_SpotLight;

    private RenderTexture m_CurrentRenderTexture;

    private int m_NonPerformantFrameCount;

    // we have observed a drop in performance between simulation and runtime
    // on the device - in the editor, we've seen it fluctuate from 54-60 FPS
    // when the device runs just fine (also giving a little bit of elbow room
    // for when simulation tanks the frame rate a bit more than what we've seen)
    const float k_FrameTimeMax = 1f / 52f;


    [SetUp]
    public void NativeDllTestSetUp()
    {
        if (Application.platform != RuntimePlatform.Android)
        {
            CreateDllLoad();
        }
    }

    [TearDown]
    public void NativeDllTestTearDown()
    {
        Object.Destroy(m_RenderPlain);
        Object.Destroy(m_BaseSphere);
        Object.Destroy(m_SpotLight);

        if (GameObject.Find("Spotlight(Clone)"))
        {
            Object.Destroy(GameObject.Find("Spotlight(Clone)"));
        }
    }

    public void Update()
    {
        if (Time.deltaTime > k_FrameTimeMax)
            ++m_NonPerformantFrameCount;
    }

    private void CreateDllLoad()
    {
        m_RenderPlain = Object.Instantiate(Resources.Load("Prefabs/_PlaneThatCallsIntoPlugin", typeof(GameObject)) as GameObject);
        m_SpotLight = Object.Instantiate(Resources.Load("Prefabs/Spotlight", typeof(Light)) as Light);
        m_BaseSphere = Object.Instantiate(Resources.Load("Prefabs/Sphere", typeof(GameObject)) as GameObject);

        m_SceneObjectsLoaded = true;
    }

    [Test]
    public void NativeDllSceneBuild()
    {
        if(Application.platform != RuntimePlatform.Android)
        {
            Assert.IsNotNull(m_SceneObjectsLoaded, "Scene Objects was not created");
        }
        else
        {
            Assert.Ignore("Android is not supported at the moment");
        }

    }

    [UnityTest]
    public IEnumerator NativeDllTest()
    {
        if (Application.platform != RuntimePlatform.Android)
        {
            yield return new WaitForSeconds(1);
            m_RenderingImage = IsPlaneRendering();
            Assert.IsTrue(m_RenderingImage, "Image rendering couldn't be found");
        }
        else
        {
            Assert.Ignore("Android is not supported at the moment");
        }
    }

    [UnityTest]
    public IEnumerator RenderingFPSCheck()
    {
        if (Application.platform != RuntimePlatform.Android)
        {
            yield return new WaitForSeconds(3f);
            Assert.AreEqual(0, m_NonPerformantFrameCount, "Failed to keep every frame inside the target frame time for the tested window");
        }
        else
        {
            Assert.Ignore("Android is not supported at the moment");
        }
    }

    public bool IsPlaneRendering()
    {
        bool filter = false;
        bool textsize = false;

        if(m_RenderPlain.GetComponent<Renderer>().material.mainTexture.filterMode == FilterMode.Point)
        {
            filter = true;
        }

        if(m_RenderPlain.GetComponent<Renderer>().material.mainTexture.height == 256 && m_RenderPlain.GetComponent<Renderer>().material.mainTexture.width == 256)
        {
            textsize = true;
        }

        if(filter && textsize)
        {
            return true;
        }

        return false;
    }
}
