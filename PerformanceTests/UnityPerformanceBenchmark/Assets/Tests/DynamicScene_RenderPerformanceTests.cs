#if UNITY_2018_1_OR_NEWER
using System;
using UnityEngine;
using System.Collections;
using Unity.PerformanceTesting;
using UnityEngine.SceneManagement;
using NUnit.Framework;
#if ENABLE_VR
using UnityEngine.XR;
#endif

[Category("XR")]
[Category("Performance")]
public class DynamicScene_RenderPerfTests : RenderPerformanceTestsBase
{
    private readonly string spiralSceneName = "PerformanceTest";
    private readonly string setTargetBufferMaterialTestSceneName = "SetTargetBuffer_MaterialTest";
    private readonly string renderTextureMaterialTestSceneName = "RenderTexture_MaterialTest";
    private readonly string stencilMaterialTestSceneName = "Stencil_MaterialTest";
    private readonly string transparentMaterialTestSceneName = "Transparent_MaterialTest";
    private readonly string gpuInstancingMaterialTestSceneName = "GPUInstancing_MaterialTest";
    private readonly string standardMaterialTestSceneName = "Standard_MaterialTest";
    private readonly string realTimeLightingDirectionalTestSceneName = "RealtimeLighting_Directional";
    private readonly string realTimeLightingPointLightTestSceneName = "RealtimeLighting_PointLight";
    private readonly string realTimeLightingSpotlightTestSceneName = "RealtimeLighting_SpotLight";
    private readonly string terrainTestSceneName = "TerrainTest";

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

    [Ignore("Disable for default run.")]
    [Version("5")]
    [PerformanceUnityTest]
    [Timeout(120000)]
    public IEnumerator Terrain()
    {
        yield return CoolDown();

        yield return SceneManager.LoadSceneAsync(terrainTestSceneName, LoadSceneMode.Additive);

        SetActiveScene(terrainTestSceneName);

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

        yield return SceneManager.UnloadSceneAsync(terrainTestSceneName);
    }

    [Ignore("Disable for default run.")]
    [Version("5")]
    [PerformanceUnityTest]
    [Timeout(120000)]
    public IEnumerator RealtimeLighting_Directional()
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

    [Ignore("Disable for default run.")]
    [Version("5")]
    [PerformanceUnityTest]
    [Timeout(120000)]
    public IEnumerator RealtimeLighting_Point()
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

    [Ignore("Disable for default run.")]
    [Version("5")]
    [PerformanceUnityTest]
    [Timeout(120000)]
    public IEnumerator RealtimeLighting_Spot()
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
    [PerformanceUnityTest]
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
    
    [Ignore("Disable for default run.")]
    [Version("5")]
    [PerformanceUnityTest]
    [Timeout(120000)]
    public IEnumerator SetTargetBufferMaterial()
    {
        yield return CoolDown();

        yield return SceneManager.LoadSceneAsync(setTargetBufferMaterialTestSceneName, LoadSceneMode.Additive);

        SetActiveScene(setTargetBufferMaterialTestSceneName);

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

        yield return SceneManager.UnloadSceneAsync(setTargetBufferMaterialTestSceneName);
    }

    [Ignore("Disable for default run.")]
    [Version("5")]
    [PerformanceUnityTest]
    [Timeout(120000)]
    public IEnumerator RenderTextureMaterial()
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
    
    [Ignore("Disable for default run.")]
    [Version("5")]
    [PerformanceUnityTest]
    [Timeout(120000)]
    public IEnumerator StencilMaterial()
    {
        yield return CoolDown();

        yield return SceneManager.LoadSceneAsync(stencilMaterialTestSceneName, LoadSceneMode.Additive);

        SetActiveScene(stencilMaterialTestSceneName);

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

        yield return SceneManager.UnloadSceneAsync(stencilMaterialTestSceneName);
    }
    
    [Ignore("Disable for default run.")]
    [Version("5")]
    [PerformanceUnityTest]
    [Timeout(120000)]
    public IEnumerator TransparentMaterial()
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

    [Ignore("Disable for default run.")]
    [Version("5")]
    [PerformanceUnityTest]
    [Timeout(120000)]
    public IEnumerator GpuInstacingMaterial()
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
    
    [Ignore("Disable for default run.")]
    [Version("5")]
    [PerformanceUnityTest]
    [Timeout(120000)]
    public IEnumerator StandardMaterial()
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