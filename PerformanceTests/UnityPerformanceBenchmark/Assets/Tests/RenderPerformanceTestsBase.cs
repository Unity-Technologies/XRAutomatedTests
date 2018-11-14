using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

[PrebuildSetup(typeof(RenderPerformancePrebuildStep))]
public abstract class RenderPerformanceTestsBase
{
    // Time, in seconds, to allow settling after scene load, object creation, etc, before we start sampling metrics
    protected readonly float SettleTime = 10f;
    protected readonly float CoolOffDuration = 30f;
    
    protected readonly string CoolDownSceneName = "cool_down";

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

    protected IEnumerator CoolDown()
    {
        // Device is getting hot after test. Spend some time at very low framerate
        Application.targetFrameRate = 1;
        yield return SceneManager.LoadSceneAsync(CoolDownSceneName, LoadSceneMode.Additive);
        SetActiveScene(CoolDownSceneName);
        yield return new WaitForSecondsRealtime(CoolOffDuration);
        yield return SceneManager.UnloadSceneAsync(CoolDownSceneName);
    }
}