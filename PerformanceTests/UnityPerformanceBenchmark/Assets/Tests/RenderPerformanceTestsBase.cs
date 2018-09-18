using Unity.PerformanceTesting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

[PrebuildSetup(typeof(RenderPerformancePrebuildStep))]
public abstract class RenderPerformanceTestsBase
{
    // Time, in seconds, to allow settling after scene load, object creation, etc, before we start sampling metrics
    protected readonly float SettleTime = 4f;

    protected ExistingMonobehaviourTest<T> SetupPerfTest<T>() where T : RenderPerformanceMonoBehaviourTestBase
    {
        Camera.main.gameObject.AddComponent(typeof(T));
        var testComponent = Object.FindObjectOfType<T>();
        return new ExistingMonobehaviourTest<T>(testComponent);
    }

    protected void SetActiveScene(string sceneName)
    {
        var scene = SceneManager.GetSceneByName(sceneName);
        SceneManager.SetActiveScene(scene);
    }
}