#if UNITY_2018_1_OR_NEWER
using UnityEngine;
using System.Collections;
using Unity.PerformanceTesting;
using UnityEngine.SceneManagement;
using NUnit.Framework;
using UnityEngine.TestTools;

#if ENABLE_VR
[Category("XR")]
#endif
[Category("Performance")]
public class StaticScene_RenderPerformanceTests : RenderPerformanceTestsBase
{
    private readonly string basicSceneName = "RenderPerformance";
    private readonly string bakedLightingTestSceneName = "BakedLighting";
    
    protected readonly SampleGroupDefinition[] SamplerNames = {
        new SampleGroupDefinition("Camera.Render"),
        new SampleGroupDefinition("Render.Mesh"),
#if ENABLE_VR
        new SampleGroupDefinition("XR.WaitForGPU", SampleUnit.Millisecond, AggregationType.Min)
#endif
    };

    [PerformanceUnityTest]
    public IEnumerator EmptyScene()
    {
        yield return SceneManager.LoadSceneAsync(basicSceneName, LoadSceneMode.Additive);

        SetActiveScene(basicSceneName);

        // Instantiate performance test object in scene
        var renderPerformanceTest = SetupPerfTest<StaticRenderPerformanceMonoBehaviourTest>();

        // allow time to settle before taking measurements
        yield return new WaitForSecondsRealtime(SettleTime);

        // use ProfilerMarkers API from Performance Test Extension
        using (Measure.ProfilerMarkers(SamplerNames))
        {
            // Set CaptureMetrics flag to TRUE; let's start capturing metrics
            renderPerformanceTest.component.CaptureMetrics = true;
            
            // Run the MonoBehaviour Test
            yield return renderPerformanceTest;
        }

        yield return SceneManager.UnloadSceneAsync(basicSceneName);
    }

    [PerformanceUnityTest]
    public IEnumerator BakedLighting()
    {
        yield return SceneManager.LoadSceneAsync(bakedLightingTestSceneName, LoadSceneMode.Additive);

        SetActiveScene(bakedLightingTestSceneName);

        // Instantiate performance test object in scene
        var renderPerformanceTest = SetupPerfTest<StaticRenderPerformanceMonoBehaviourTest>();

        // allow time to settle before taking measurements
        yield return new WaitForSecondsRealtime(SettleTime);

        // use ProfilerMarkers API from Performance Test Extension
        using (Measure.ProfilerMarkers(SamplerNames))
        {
            // Set CaptureMetrics flag to TRUE; let's start capturing metrics
            renderPerformanceTest.component.CaptureMetrics = true;
             
            // Run the MonoBehaviour Test
            yield return renderPerformanceTest;
        }

        yield return SceneManager.UnloadSceneAsync(bakedLightingTestSceneName);
    }

    [PerformanceUnityTest]
    public IEnumerator SinglePrimitiveCube()
    {
        yield return SceneManager.LoadSceneAsync(basicSceneName, LoadSceneMode.Additive);

        SetActiveScene(basicSceneName);

        // Instantiate performance test object in scene
        var renderPerformanceTest = SetupPerfTest<StaticRenderPerformanceWithObjMonoBehaviourTest>();

        // allow time to settle before taking measurements
        yield return new WaitForSecondsRealtime(SettleTime);

        // use ProfilerMarkers API from Performance Test Extension
        using (Measure.ProfilerMarkers(SamplerNames))
        {
            // Set CaptureMetrics flag to TRUE; let's start capturing metrics
            renderPerformanceTest.component.CaptureMetrics = true;
             
            // Run the MonoBehaviour Test
            yield return renderPerformanceTest;
        }

        yield return SceneManager.UnloadSceneAsync(basicSceneName);
    }
}
#endif