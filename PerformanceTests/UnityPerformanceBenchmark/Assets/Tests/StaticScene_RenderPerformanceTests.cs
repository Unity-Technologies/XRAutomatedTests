﻿#if UNITY_2018_1_OR_NEWER
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
public class StaticScene_RenderPerformanceTests : RenderPerformanceTestsBase
{
    private readonly string basicSceneName = "RenderPerformance";
    private readonly string bakedLightingTestSceneName = "BakedLighting";
    
    protected SampleGroupDefinition[] SamplerNames = {
        new SampleGroupDefinition("Camera.Render"),
        new SampleGroupDefinition("Render.Mesh"),
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
    public IEnumerator EmptyScene()
    {
        yield return CoolDown();

        yield return SceneManager.LoadSceneAsync(basicSceneName, LoadSceneMode.Additive);

        SetActiveScene(basicSceneName);

        // Instantiate performance test object in scene
        var renderPerformanceTest = SetupPerfTest<StaticRenderPerformanceMonoBehaviourTest>();

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

        yield return SceneManager.UnloadSceneAsync(basicSceneName);
    }

    [Ignore("Disable for default run.")]
    [Version("5")]
    [UnityTest, Performance]
    [Timeout(120000)]
    public IEnumerator BakedLighting()
    {
        yield return CoolDown();

        yield return SceneManager.LoadSceneAsync(bakedLightingTestSceneName, LoadSceneMode.Additive);

        SetActiveScene(bakedLightingTestSceneName);

        // Instantiate performance test object in scene
        var renderPerformanceTest = SetupPerfTest<StaticRenderPerformanceMonoBehaviourTest>();

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

        yield return SceneManager.UnloadSceneAsync(bakedLightingTestSceneName);
    }

    [Ignore("Disable for default run.")]
    [Version("5")]
    [UnityTest, Performance]
    [Timeout(120000)]
    public IEnumerator SinglePrimitiveCube()
    {
        yield return CoolDown();

        yield return SceneManager.LoadSceneAsync(basicSceneName, LoadSceneMode.Additive);

        SetActiveScene(basicSceneName);

        // Instantiate performance test object in scene
        var renderPerformanceTest = SetupPerfTest<StaticRenderPerformanceWithObjMonoBehaviourTest>();

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

        yield return SceneManager.UnloadSceneAsync(basicSceneName);
    }
}
#endif