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
[Category("LWRP")]
[Category("Performance")]
public class DynamicScene_RenderPerfTests_LWRP : RenderPerformanceTestsBase
{
    private readonly string spiralSceneName = "PerformanceTest";
    private readonly string renderTextureMaterialTestSceneName = "RenderTexture_MaterialTest";
    private readonly string transparentMaterialTestSceneName = "Transparent_MaterialTest";
    private readonly string gpuInstancingMaterialTestSceneName = "GPUInstancing_MaterialTest";
    private readonly string standardMaterialTestSceneName = "Standard_MaterialTest";
    private readonly string realTimeLightingDirectionalTestSceneName = "RealtimeLighting_Directional";
    private readonly string realTimeLightingPointLightTestSceneName = "RealtimeLighting_PointLight";
    private readonly string realTimeLightingSpotlightTestSceneName = "RealtimeLighting_SpotLight";

    protected SampleGroupDefinition[] SamplerNames = {
        new SampleGroupDefinition("Camera.Render", SampleUnit.Millisecond, AggregationType.Min),
        new SampleGroupDefinition("Render.Mesh", SampleUnit.Millisecond, AggregationType.Min)
    };

    [SetUp]
    public void Setup()
    {
#if ENABLE_VR
        if (XRSettings.enabled)
        {
            Array.Resize(ref SamplerNames, SamplerNames.Length + 1);
            SamplerNames[SamplerNames.Length - 1] = new SampleGroupDefinition("XR.WaitForGPU", SampleUnit.Millisecond, AggregationType.Min);
        }
#endif
    }

    
    [Version("5")]
    [UnityTest, Performance]
    [Timeout(120000)]
    public IEnumerator RealtimeLighting_Directional_LWRP()
    {
        yield return CoolDown();

        yield return SceneManager.LoadSceneAsync(realTimeLightingDirectionalTestSceneName, LoadSceneMode.Additive);

        SetActiveScene(realTimeLightingDirectionalTestSceneName);

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

        yield return SceneManager.UnloadSceneAsync(realTimeLightingDirectionalTestSceneName);
    }

    [Version("5")]
    [UnityTest, Performance]
    [Timeout(120000)]
    public IEnumerator RealtimeLighting_Point_LWRP()
    {
        yield return CoolDown();

        yield return SceneManager.LoadSceneAsync(realTimeLightingPointLightTestSceneName, LoadSceneMode.Additive);

        SetActiveScene(realTimeLightingPointLightTestSceneName);

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

        yield return SceneManager.UnloadSceneAsync(realTimeLightingPointLightTestSceneName);
    }

    [Version("5")]
    [UnityTest, Performance]
    [Timeout(120000)]
    public IEnumerator RealtimeLighting_Spot_LWRP()
    {
        yield return CoolDown();

        yield return SceneManager.LoadSceneAsync(realTimeLightingSpotlightTestSceneName, LoadSceneMode.Additive);

        SetActiveScene(realTimeLightingSpotlightTestSceneName);

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

        yield return SceneManager.UnloadSceneAsync(realTimeLightingSpotlightTestSceneName);
    }

    [Version("5")]
    [UnityTest, Performance]
    [Timeout(120000)]
    public IEnumerator SpiralFlame_RenderPerformance_LWRP()
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

    

    [Version("5")]
    [UnityTest, Performance]
    [Timeout(120000)]
    public IEnumerator RenderTextureMaterial_LWRP()
    {
        yield return CoolDown();

        yield return SceneManager.LoadSceneAsync(renderTextureMaterialTestSceneName, LoadSceneMode.Additive);

        SetActiveScene(renderTextureMaterialTestSceneName);

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

        yield return SceneManager.UnloadSceneAsync(renderTextureMaterialTestSceneName);
    }

    [Version("5")]
    [UnityTest, Performance]
    [Timeout(120000)]
    public IEnumerator TransparentMaterial_LWRP()
    {
        yield return CoolDown();

        yield return SceneManager.LoadSceneAsync(transparentMaterialTestSceneName, LoadSceneMode.Additive);

        SetActiveScene(transparentMaterialTestSceneName);

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

        yield return SceneManager.UnloadSceneAsync(transparentMaterialTestSceneName);
    }

    [Version("5")]
    [UnityTest, Performance]
    [Timeout(120000)]
    public IEnumerator GpuInstacingMaterial_LWRP()
    {
        yield return CoolDown();

        yield return SceneManager.LoadSceneAsync(gpuInstancingMaterialTestSceneName, LoadSceneMode.Additive);

        SetActiveScene(gpuInstancingMaterialTestSceneName);

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

        yield return SceneManager.UnloadSceneAsync(gpuInstancingMaterialTestSceneName);
    }

    [Version("5")]
    [UnityTest, Performance]
    [Timeout(120000)]
    public IEnumerator StandardMaterial_LWRP()
    {
        yield return CoolDown();

        yield return SceneManager.LoadSceneAsync(standardMaterialTestSceneName, LoadSceneMode.Additive);

        SetActiveScene(standardMaterialTestSceneName);

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

        yield return SceneManager.UnloadSceneAsync(standardMaterialTestSceneName);
    }
}
#endif