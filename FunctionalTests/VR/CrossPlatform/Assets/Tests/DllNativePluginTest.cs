using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TestTools;


[UnityPlatform(exclude = new [] {RuntimePlatform.Android})]
public class DllNativePluginTest : TestBaseSetup
{
    private bool m_SceneObjectsLoaded = false;
    private bool m_RenderingImage = false;

    private GameObject m_RenderPlane;
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
        CreateDllLoad();
    }

    [TearDown]
    public void NativeDllTestTearDown()
    {
        Object.Destroy(m_RenderPlane);
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
        m_RenderPlane = Object.Instantiate(Resources.Load<GameObject>("Prefabs/_PlaneThatCallsIntoPlugin"));
        m_SpotLight = Object.Instantiate(Resources.Load<Light>("Prefabs/Spotlight"));
        m_BaseSphere = Object.Instantiate(Resources.Load<GameObject>("Prefabs/Sphere"));

        m_SceneObjectsLoaded = true;
    }

    [Test]
    public void NativeDllSceneBuild()
    {
        Assert.IsTrue(m_SceneObjectsLoaded, "Scene Objects was not created");
    }

    [UnityTest]
    public IEnumerator NativeDllTest()
{
        yield return new WaitForSeconds(1);
        m_RenderingImage = IsPlaneRendering();
        Assert.IsTrue(m_RenderingImage, "Image rendering couldn't be found");
    }

    [UnityTest]
    public IEnumerator RenderingFPSCheck()
    {
        yield return new WaitForSeconds(3f);
        Assert.AreEqual(0, m_NonPerformantFrameCount, "Failed to keep every frame inside the target frame time for the tested window");
    }

    private bool IsPlaneRendering()
    {
        bool filter = false;
        bool textsize = false;

        if(m_RenderPlane.GetComponent<Renderer>().material.mainTexture.filterMode == FilterMode.Point)
        {
            filter = true;
        }

        if(m_RenderPlane.GetComponent<Renderer>().material.mainTexture.height == 256 && m_RenderPlane.GetComponent<Renderer>().material.mainTexture.width == 256)
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
