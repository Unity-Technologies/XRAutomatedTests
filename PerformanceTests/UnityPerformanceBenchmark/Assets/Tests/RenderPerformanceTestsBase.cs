using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

[PrebuildSetup(typeof(RenderPerformancePrebuildStep))]
public abstract class RenderPerformanceTestsBase
{
    private int previousTargetFrameRate;

    // Time, in seconds, to allow settling after scene load, object creation, etc, before we start sampling metrics
#if UNITY_ANDROID || UNITY_IOS
    protected readonly float CoolOffDuration = 30f;
#else
    protected readonly float CoolOffDuration = 2f;
#endif

    protected readonly float SettleTime = 5f;

    
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
        previousTargetFrameRate = Application.targetFrameRate;
        Application.targetFrameRate = 1;

        yield return SceneManager.LoadSceneAsync(CoolDownSceneName, LoadSceneMode.Additive);
        SetActiveScene(CoolDownSceneName);
        yield return new WaitForSecondsRealtime(CoolOffDuration);

        // After cool down, restore framerate
        Application.targetFrameRate = previousTargetFrameRate;

        yield return SceneManager.UnloadSceneAsync(CoolDownSceneName);
    }
}