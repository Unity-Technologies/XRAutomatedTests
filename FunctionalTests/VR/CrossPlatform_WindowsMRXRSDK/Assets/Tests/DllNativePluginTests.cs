#if !UNITY_ANDROID
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TestTools;


public class DllNativePluginTests : XrFunctionalTestBase
{
    private bool sceneObjectsLoaded;
    private bool renderingImage;

    private GameObject renderPlane;
    private GameObject baseSphere;

    private Light spotLight;

    private RenderTexture currentRenderTexture;

    private int nonPerformantFrameCount;

    // we have observed a drop in performance between simulation and runtime
    // on the device - in the editor, we've seen it fluctuate from 54-60 FPS
    // when the device runs just fine (also giving a little bit of elbow room
    // for when simulation tanks the frame rate a bit more than what we've seen)
    const float KFrameTimeMax = 1f / 52f;


    [SetUp]
    public override void SetUp()
    {
        base.SetUp();
        CreateObjectsFromPrefabs();
    }

    [TearDown]
    public override void TearDown()
    {
        Object.Destroy(renderPlane);
        Object.Destroy(baseSphere);
        Object.Destroy(spotLight);

        if (GameObject.Find("Spotlight(Clone)"))
        {
            Object.Destroy(GameObject.Find("Spotlight(Clone)"));
        }
        base.TearDown();
    }

    public void Update()
    {
        if (Time.deltaTime > KFrameTimeMax)
            ++nonPerformantFrameCount;
    }

    private void CreateObjectsFromPrefabs()
    {
        renderPlane = Object.Instantiate(Resources.Load<GameObject>("Prefabs/_PlaneThatCallsIntoPlugin"));
        spotLight = Object.Instantiate(Resources.Load<Light>("Prefabs/Spotlight"));
        baseSphere = Object.Instantiate(Resources.Load<GameObject>("Prefabs/Sphere"));

        // TODO I don't think this is the way we want to do this. Shouldn't we be trying to find these objects in the scene
        // before we determine if they are loaded?
        sceneObjectsLoaded = true;
    }

    [Test]
    public void VerifySceneObjectsLoaded()
    {
        Assert.IsTrue(sceneObjectsLoaded, "Scene Objects was not created");
    }

    //[Ignore("mainTexture is coming back as null from material. I think the UseRenderingPlugin.Start method where the texture is assigned is not being called. Need to investigate.")]
    [UnityTest]
    public IEnumerator VerifyIsPlaneRendering()
{
        yield return SkipFrame(1);
        Assert.IsTrue(IsPlaneRendering(), "Image rendering couldn't be found");
    }

    // TODO: what is this test checking?
    [UnityTest]
    public IEnumerator VerifyRenderingFps()
    {
        yield return SkipFrame(2);
        Assert.AreEqual(0, nonPerformantFrameCount, "Failed to keep every frame inside the target frame time for the tested window");
    }

    private bool IsPlaneRendering()
    {
        var filter = false;
        var textsize = false;

        var renderer = renderPlane.GetComponent<Renderer>();
        if (renderer.material.mainTexture.filterMode == FilterMode.Point)
        {
            filter = true;
        }

        // TODO pull these magic numbers (256) into readonly vars with description
        if(renderer.material.mainTexture.height == 256 && renderer.material.mainTexture.width == 256)
        {
            textsize = true;
        }

        return filter && textsize;
    }
}
#endif