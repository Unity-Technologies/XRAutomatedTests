#if UNITY_2018_1_OR_NEWER
using System;
using UnityEngine;
using System.Collections;
using Unity.PerformanceTesting;
using UnityEngine.SceneManagement;
using NUnit.Framework;
using UnityEngine.TestTools;
#if ENABLE_VR
using UnityEngine.XR;
#endif

[Category("XR")]
[Category("Performance")]
public class DynamicScene_RenderPerfTests : RenderPerformanceTestsBase
{
    private readonly string spiralSceneName = "PerformanceTest";

    protected SampleGroupDefinition[] SamplerNames = {
        new SampleGroupDefinition("Camera.Render"),
        new SampleGroupDefinition("Render.Mesh")
    };

    [SetUp]
    public void Setup()
    {
#if ENABLE_VR
        if (XRSettings.enabled)
        {
            Array.Resize(ref SamplerNames, SamplerNames.Length + 1);
            SamplerNames[SamplerNames.Length - 1] = new SampleGroupDefinition("XR.WaitForGPU");
        }
#endif
    }

    [Version("5")]
    [UnityTest, Performance]
    [Timeout(120000)]
    public IEnumerator SpiralFlame_RenderPerformance()
    {
        yield return CoolDown();

        yield return SceneManager.LoadSceneAsync(spiralSceneName, LoadSceneMode.Additive);

        SetActiveScene(spiralSceneName);

        // Instantiate performance test object in scene
        var renderPerformanceTest = SetupPerfTest<DynamicRenderPerformanceMonoBehaviourTest>();

        // allow time to settle before taking measurements
        yield return new WaitForSecondsRealtime(SettleTime);

        using (Measure.Scope())
        {
            using (Measure.Frames().Scope())
            {
                // use ProfilerMarkers API from Performance Test Extension
                using (Measure.ProfilerMarkers(SamplerNames))
                {
                    // Set CaptureMetrics flag to TRUE; let's start capturing metrics
                    renderPerformanceTest.component.CaptureMetrics = true;

                    // Run the MonoBehaviour Test
                    yield return renderPerformanceTest;
                }
            }
        }

        yield return SceneManager.UnloadSceneAsync(spiralSceneName);
    }
}
#endif