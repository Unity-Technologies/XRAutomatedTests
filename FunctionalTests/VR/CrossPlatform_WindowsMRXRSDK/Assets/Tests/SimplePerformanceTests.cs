using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;

public class SimplePerformanceTests : XrFunctionalTestBase
{
    private int nonPerformantFrameCount;
    
    // we have observed a drop in performance between simulation and runtime
    // on the device - in the editor, we've seen it fluctuate from 54-60 FPS
    // when the device runs just fine (also giving a little bit of elbow room
    // for when simulation tanks the frame rate a bit more than what we've seen)
    const float KFrameTimeMax = 1f / 52f;

    public void Update()
    {
        if (Time.deltaTime > KFrameTimeMax)
            ++nonPerformantFrameCount;
    }

    [UnityTest]
    public IEnumerator SimpleFpsTest()
    {
        TestCubeSetup(TestCubesConfig.TestCube);
        yield return SkipFrame(DefaultFrameSkipCount);

        Assert.AreEqual(0, nonPerformantFrameCount, "Failed to keep every frame inside the target frame time for the tested window");
    }

    [UnityTest]
    public IEnumerator SimpleFpsTestWithFocalPoint()
    {
        TestCubeSetup(TestCubesConfig.TestCube);
        yield return SkipFrame(DefaultFrameSkipCount);

        Assert.AreEqual(0, nonPerformantFrameCount, "Failed to keep every frame inside the target frame time for the tested window");
    }

    [UnityTest]
    public IEnumerator MassObjectsFpsTest()
    {
        TestCubeSetup(TestCubesConfig.PerformanceMassObjects);

        yield return SkipFrame(DefaultFrameSkipCount);

        Assert.AreEqual(0, nonPerformantFrameCount, "Failed to keep every frame inside the target frame time for the tested window");
    }

    [UnityTest]
    public IEnumerator MassFloorObjectsFpsTest()
    {
        TestCubeSetup(TestCubesConfig.PerformanceMassFloorObjects);

        yield return SkipFrame(DefaultFrameSkipCount);

        Assert.AreEqual(0, nonPerformantFrameCount, "Failed to keep every frame inside the target frame time for the tested window");
    }
}
