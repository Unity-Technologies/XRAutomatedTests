using UnityEngine;
using System.Collections;
using Unity.PerformanceTesting;
using UnityEngine.SceneManagement;
using NUnit.Framework;
using UnityEngine.TestTools;
using QualitySettings = UnityEngine.QualitySettings;
#if ENABLE_VR
using UnityEngine.XR;
#endif


public class RenderPerformanceTests
{

    private readonly SampleGroupDefinition[] samplerNames = {
        new SampleGroupDefinition("Camera.Render"),
        new SampleGroupDefinition("Render.Mesh"),
        new SampleGroupDefinition("XR.WaitForGPU")
    };

    private readonly float settleTime = 4f;
    private readonly string basicSceneName = "RenderPerformance";
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
    private readonly string bakedLightingTestSceneName = "BakedLighting";


    [PrebuildSetup(typeof(RenderPerformancePrebuildStep))]
    [Category("XR")]
    [PerformanceUnityTest]
    public IEnumerator EmptyScene()
    {
        yield return SceneManager.LoadSceneAsync(basicSceneName, LoadSceneMode.Additive);

        SetActiveScene(basicSceneName);
        
        var renderPerformanceTest = SetupPerfTest<RenderPerformanceMonoBehaviorTest>();

        // allow time to settle before taking measurements
        yield return new WaitForSecondsRealtime(settleTime);

        using (Measure.ProfilerMarkers(samplerNames))
        {
            using (Measure.Scope())
            {
                renderPerformanceTest.component.CaptureMetrics = true;
                yield return renderPerformanceTest;
            }
        }

        yield return SceneManager.UnloadSceneAsync(basicSceneName);
    }

    [PrebuildSetup(typeof(RenderPerformancePrebuildStep))]
    [Category("XR")]
    [PerformanceUnityTest]
    public IEnumerator BakedLighting()
    {
        yield return SceneManager.LoadSceneAsync(bakedLightingTestSceneName, LoadSceneMode.Additive);

        SetActiveScene(bakedLightingTestSceneName);

        var renderPerformanceTest = SetupPerfTest<RenderPerformanceMonoBehaviorTest>();

        // allow time to settle before taking measurements
        yield return new WaitForSecondsRealtime(settleTime);

        using (Measure.ProfilerMarkers(samplerNames))
        {
            using (Measure.Scope())
            {
                renderPerformanceTest.component.CaptureMetrics = true;
                yield return renderPerformanceTest;
            }
        }

        yield return SceneManager.UnloadSceneAsync(bakedLightingTestSceneName);
    }

    [PrebuildSetup(typeof(RenderPerformancePrebuildStep))]
    [Category("XR")]
    [PerformanceUnityTest]
    public IEnumerator Terrain()
    {
        yield return SceneManager.LoadSceneAsync(terrainTestSceneName, LoadSceneMode.Additive);

        SetActiveScene(terrainTestSceneName);

        var renderPerformanceTest = SetupPerfTest<RenderPerformanceMonoBehaviorTest>();

        // allow time to settle before taking measurements
        yield return new WaitForSecondsRealtime(settleTime);

        using (Measure.ProfilerMarkers(samplerNames))
        {
            using (Measure.Scope())
            {
                renderPerformanceTest.component.CaptureMetrics = true;
                yield return renderPerformanceTest;
            }
        }

        yield return SceneManager.UnloadSceneAsync(terrainTestSceneName);
    }

    [PrebuildSetup(typeof(RenderPerformancePrebuildStep))]
    [Category("XR")]
    [PerformanceUnityTest]
    public IEnumerator RealtimeLighting_Directional()
    {
        yield return SceneManager.LoadSceneAsync(realTimeLightingDirectionalTestSceneName, LoadSceneMode.Additive);

        SetActiveScene(realTimeLightingDirectionalTestSceneName);

        var renderPerformanceTest = SetupPerfTest<RenderPerformanceMonoBehaviorTest>();

        // allow time to settle before taking measurements
        yield return new WaitForSecondsRealtime(settleTime);

        using (Measure.ProfilerMarkers(samplerNames))
        {
            using (Measure.Scope())
            {
                renderPerformanceTest.component.CaptureMetrics = true;
                yield return renderPerformanceTest;
            }
        }

        yield return SceneManager.UnloadSceneAsync(realTimeLightingDirectionalTestSceneName);
    }

    [PrebuildSetup(typeof(RenderPerformancePrebuildStep))]
    [Category("XR")]
    [PerformanceUnityTest]
    public IEnumerator RealtimeLighting_Point()
    {
        yield return SceneManager.LoadSceneAsync(realTimeLightingPointLightTestSceneName, LoadSceneMode.Additive);

        SetActiveScene(realTimeLightingPointLightTestSceneName);

        var renderPerformanceTest = SetupPerfTest<RenderPerformanceMonoBehaviorTest>();

        // allow time to settle before taking measurements
        yield return new WaitForSecondsRealtime(settleTime);

        using (Measure.ProfilerMarkers(samplerNames))
        {
            using (Measure.Scope())
            {
                renderPerformanceTest.component.CaptureMetrics = true;
                yield return renderPerformanceTest;
            }
        }

        yield return SceneManager.UnloadSceneAsync(realTimeLightingPointLightTestSceneName);
    }

    [PrebuildSetup(typeof(RenderPerformancePrebuildStep))]
    [Category("XR")]
    [PerformanceUnityTest]
    public IEnumerator RealtimeLighting_Spot()
    {
        yield return SceneManager.LoadSceneAsync(realTimeLightingSpotlightTestSceneName, LoadSceneMode.Additive);

        SetActiveScene(realTimeLightingSpotlightTestSceneName);

        var renderPerformanceTest = SetupPerfTest<RenderPerformanceMonoBehaviorTest>();

        // allow time to settle before taking measurements
        yield return new WaitForSecondsRealtime(settleTime);

        using (Measure.ProfilerMarkers(samplerNames))
        {
            using (Measure.Scope())
            {
                renderPerformanceTest.component.CaptureMetrics = true;
                yield return renderPerformanceTest;
            }
        }

        yield return SceneManager.UnloadSceneAsync(realTimeLightingSpotlightTestSceneName);
    }

    [PrebuildSetup(typeof(RenderPerformancePrebuildStep))]
    [Category("XR")]
    [PerformanceUnityTest]
    public IEnumerator RenderViewPortScale_100_Percent()
    {
        yield return SceneManager.LoadSceneAsync(spiralSceneName, LoadSceneMode.Additive);

        SetActiveScene(spiralSceneName);

        var renderPerformanceTest = SetupPerfTest<RenderPerformanceMonoBehaviorTest>();
#if ENABLE_VR
        XRSettings.renderViewportScale = 1f;
#endif
        // allow time to settle before taking measurements
        yield return new WaitForSecondsRealtime(settleTime);

        using (Measure.ProfilerMarkers(samplerNames))
        {
            using (Measure.Scope())
            {
                renderPerformanceTest.component.CaptureMetrics = true;
                yield return renderPerformanceTest;
            }
        }

        yield return SceneManager.UnloadSceneAsync(spiralSceneName);
    }

    [PrebuildSetup(typeof(RenderPerformancePrebuildStep))]
    [Category("XR")]
    [PerformanceUnityTest]
    public IEnumerator SetTargetBufferMaterial()
    {
        yield return SceneManager.LoadSceneAsync(setTargetBufferMaterialTestSceneName, LoadSceneMode.Additive);

        SetActiveScene(setTargetBufferMaterialTestSceneName);

        var renderPerformanceTest = SetupPerfTest<RenderPerformanceMonoBehaviorTest>();

        // allow time to settle before taking measurements
        yield return new WaitForSecondsRealtime(settleTime);

        using (Measure.ProfilerMarkers(samplerNames))
        {
            using (Measure.Scope())
            {
                renderPerformanceTest.component.CaptureMetrics = true;
                yield return renderPerformanceTest;
            }
        }

        yield return SceneManager.UnloadSceneAsync(setTargetBufferMaterialTestSceneName);
    }

    [PrebuildSetup(typeof(RenderPerformancePrebuildStep))]
    [Category("XR")]
    [PerformanceUnityTest]
    public IEnumerator RenderTextureMaterial()
    {
        yield return SceneManager.LoadSceneAsync(renderTextureMaterialTestSceneName, LoadSceneMode.Additive);

        SetActiveScene(renderTextureMaterialTestSceneName);

        var renderPerformanceTest = SetupPerfTest<RenderPerformanceMonoBehaviorTest>();

        // allow time to settle before taking measurements
        yield return new WaitForSecondsRealtime(settleTime);

        using (Measure.ProfilerMarkers(samplerNames))
        {
            using (Measure.Scope())
            {
                renderPerformanceTest.component.CaptureMetrics = true;
                yield return renderPerformanceTest;
            }
        }

        yield return SceneManager.UnloadSceneAsync(renderTextureMaterialTestSceneName);
    }

    [PrebuildSetup(typeof(RenderPerformancePrebuildStep))]
    [Category("XR")]
    [PerformanceUnityTest]
    public IEnumerator StencilMaterial()
    {
        yield return SceneManager.LoadSceneAsync(stencilMaterialTestSceneName, LoadSceneMode.Additive);

        SetActiveScene(stencilMaterialTestSceneName);

        var renderPerformanceTest = SetupPerfTest<RenderPerformanceMonoBehaviorTest>();

        // allow time to settle before taking measurements
        yield return new WaitForSecondsRealtime(settleTime);

        using (Measure.ProfilerMarkers(samplerNames))
        {
            using (Measure.Scope())
            {
                renderPerformanceTest.component.CaptureMetrics = true;
                yield return renderPerformanceTest;
            }
        }

        yield return SceneManager.UnloadSceneAsync(stencilMaterialTestSceneName);
    }

    [PrebuildSetup(typeof(RenderPerformancePrebuildStep))]
    [Category("XR")]
    [PerformanceUnityTest]
    public IEnumerator TransparentMaterial()
    {
        yield return SceneManager.LoadSceneAsync(transparentMaterialTestSceneName, LoadSceneMode.Additive);

        SetActiveScene(transparentMaterialTestSceneName);

        var renderPerformanceTest = SetupPerfTest<RenderPerformanceMonoBehaviorTest>();

        // allow time to settle before taking measurements
        yield return new WaitForSecondsRealtime(settleTime);

        using (Measure.ProfilerMarkers(samplerNames))
        {
            using (Measure.Scope())
            {
                renderPerformanceTest.component.CaptureMetrics = true;
                yield return renderPerformanceTest;
            }
        }

        yield return SceneManager.UnloadSceneAsync(transparentMaterialTestSceneName);
    }

    [PrebuildSetup(typeof(RenderPerformancePrebuildStep))]
    [Category("XR")]
    [PerformanceUnityTest]
    public IEnumerator GpuInstacingMaterial()
    {
        yield return SceneManager.LoadSceneAsync(gpuInstancingMaterialTestSceneName, LoadSceneMode.Additive);

        SetActiveScene(gpuInstancingMaterialTestSceneName);

        var renderPerformanceTest = SetupPerfTest<RenderPerformanceMonoBehaviorTest>();

        // allow time to settle before taking measurements
        yield return new WaitForSecondsRealtime(settleTime);

        using (Measure.ProfilerMarkers(samplerNames))
        {
            using (Measure.Scope())
            {
                renderPerformanceTest.component.CaptureMetrics = true;
                yield return renderPerformanceTest;
            }
        }

        yield return SceneManager.UnloadSceneAsync(gpuInstancingMaterialTestSceneName);
    }

    [PrebuildSetup(typeof(RenderPerformancePrebuildStep))]
    [Category("XR")]
    [PerformanceUnityTest]
    public IEnumerator StandardMaterial()
    {
        yield return SceneManager.LoadSceneAsync(standardMaterialTestSceneName, LoadSceneMode.Additive);

        SetActiveScene(standardMaterialTestSceneName);

        var renderPerformanceTest = SetupPerfTest<RenderPerformanceMonoBehaviorTest>();

        // allow time to settle before taking measurements
        yield return new WaitForSecondsRealtime(settleTime);

        using (Measure.ProfilerMarkers(samplerNames))
        {
            using (Measure.Scope())
            {
                renderPerformanceTest.component.CaptureMetrics = true;
                yield return renderPerformanceTest;
            }
        }

        yield return SceneManager.UnloadSceneAsync(standardMaterialTestSceneName);
    }

    [PrebuildSetup(typeof(RenderPerformancePrebuildStep))]
    [Category("XR")]
    [PerformanceUnityTest]
    public IEnumerator SinglePrimitiveCube()
    {
        yield return SceneManager.LoadSceneAsync(basicSceneName, LoadSceneMode.Additive);

        SetActiveScene(basicSceneName);

        var renderPerformanceTest = SetupPerfTest<RenderPerformanceWithObjMonoBehaviorTest>();

        // allow time to settle before taking measurements
        yield return new WaitForSecondsRealtime(settleTime);

        using (Measure.ProfilerMarkers(samplerNames))
        {
            using (Measure.Scope())
            {
                renderPerformanceTest.component.CaptureMetrics = true;
                yield return renderPerformanceTest;
            }
        }

        yield return SceneManager.UnloadSceneAsync(basicSceneName);
    }

    [PrebuildSetup(typeof(RenderPerformancePrebuildStep))]
    [Category("XR")]
    [PerformanceUnityTest]
    public IEnumerator RenderViewPortScale_90_Percent()
    {
        yield return SceneManager.LoadSceneAsync(spiralSceneName, LoadSceneMode.Additive);

        SetActiveScene(spiralSceneName);

        var renderPerformanceTest = SetupPerfTest<RenderPerformanceMonoBehaviorTest>();
#if ENABLE_VR
        XRSettings.renderViewportScale = .9f;
#endif
        // allow time to settle before taking measurements
        yield return new WaitForSecondsRealtime(settleTime);

        using (Measure.ProfilerMarkers(samplerNames))
        {
            using (Measure.Scope())
            {
                renderPerformanceTest.component.CaptureMetrics = true;
                yield return renderPerformanceTest;
            }
        }

#if ENABLE_VR
        XRSettings.renderViewportScale = 1f;
#endif
        yield return SceneManager.UnloadSceneAsync(spiralSceneName);
    }

    [PrebuildSetup(typeof(RenderPerformancePrebuildStep))]
    [Category("XR")]
    [PerformanceUnityTest]
    public IEnumerator RenderViewPortScale_50_Percent()
    {
        yield return SceneManager.LoadSceneAsync(spiralSceneName, LoadSceneMode.Additive);

        SetActiveScene(spiralSceneName);

        var renderPerformanceTest = SetupPerfTest<RenderPerformanceMonoBehaviorTest>();
#if ENABLE_VR
        XRSettings.renderViewportScale = .5f;
#endif
        // allow time to settle before taking measurements
        yield return new WaitForSecondsRealtime(settleTime);

        using (Measure.ProfilerMarkers(samplerNames))
        {
            using (Measure.Scope())
            {
                renderPerformanceTest.component.CaptureMetrics = true;
                yield return renderPerformanceTest;
            }
        }

#if ENABLE_VR
        XRSettings.renderViewportScale = 1f;
#endif
        yield return SceneManager.UnloadSceneAsync(spiralSceneName);
    }

    private ExistingMonobehaviourTest<T> SetupPerfTest<T>() where T : RenderPerformanceMonoBehaviorTest
    {
        Camera.main.gameObject.AddComponent(typeof(T));
        var testComponent = Object.FindObjectOfType<T>();
        return new ExistingMonobehaviourTest<T>(testComponent);
    }

    private void SetActiveScene(string sceneName)
    {
        var scene = SceneManager.GetSceneByName(sceneName);
        SceneManager.SetActiveScene(scene);
    }
}
