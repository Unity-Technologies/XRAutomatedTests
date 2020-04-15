# Running the Oculus Performance Tests

## Prerequisites

Ensure that you have your local git client set up with an ssh key and that you have no passphrase associated with your ssh key file. Unity Editor is unable to resolve your passphrase (read: will not prompt you for the passphrase) and requires no passphrase be set. To set your ssh key, follow this guide:
- [Connecting to GitHub with SSH](https://help.github.com/en/enterprise/2.18/user/github/authenticating-to-github/connecting-to-github-with-ssh)

## Steps to run the Oculus Performance Tests

1) Pull from https://github.cds.internal.unity3d.com/unity/xr.sdk.oculus.
2) At the repo root run bee.exe.
3) Then open any version of unity and open the project located at <repo>/TestProjects/PerformanceTests
4) Then open the Test Runner in Window  > General > TestRunner
5) Set your build target to Windows or Android
6) Finally, select Playmode tests in the test runner window, then "Run all in player".

NOTE: If you'd like to run only one test, remove all other tests from the Build Settings. The Test Runner will automatically update the test cases based on the Build Settings. Then redeploy to the device by clicking "Run all in player"

### Guidelines for adding tests:
- In general, do not attach a tracked pose driver to the camera. The input tracking will throw off results.
- Always add your new test to the Build Settings if you'd like it to run!
- Never remove the cool_down scene from the Build Settings

# Test Types

## Object Count Stress Tests

### Summary
The object count stress tests search for a maximum amount of objects that can be spawned without dropping below a given frame rate. 

### Adding a new Object Count Stress Test 
1) Create a scene and drop it in the folder Assets/Scenes/ObjectCountStressTests
2) In this new scene, create an GameObject and name it "Prototype". this is the object that will be spawned continuosly to put stress on the system. The name of the scene should describe the properties of the "Prototype". e.g. "NonInstancedSphere" 
3) Make changes to the "Prototype" object
4) Add your test scene to the Build Settings
5) Observe the Test Runner populate your new test case
6) Deploy the test to device!

### How it works
The stress test works in the following way:
- First it searches for a GameObject in the scene named "Prototype". The "Prototype" object is the object that will be spawned continuosly to put stress on the system.
- Every 72 frames (NOTE: this test is currently hard configured for Quest) or "step", The test will spawn 32 instances of the "Prototype" object
- Simultaneously, the test will average the FPS of the previous 72 frames.
- Once the FPS drops below a certain threshold, the test will start to remove 32 instances of the object until FPS has stabilized above the threshold
- The test will spawn 16 instances of the "Prototype" object until the FPS is unstable
- The test removes 16 objects until FPS is stable
- The test spawns 8 instances until the FPS is unstable
- And so on until the test is only removing 1 instance of the object at a time, at which point we measure the amount of spawned objects. This is the test result.

## Oculus Stats Tests

### Summary
The Oculus stats tests are generic tests that gather a variety of statistics over 1000 frames on a given scene. 

The tests gather the following info over 1000 frames:

Oculus Specific:
- CPU Utilization Average
- CPU Utilization Worst
- GPU Utilization Average
- GPU App Time
- GPU Compositor Time  

Generic Unity Stats:
- Total Test Time
- FrameTime
- Camera.Render 
- Render.Mesh 

The oculus specific stats ultimately are gathered from the OVRPlugin itself. 

NOTE: The oculus specific stats are enabled with the following API call and are gathered from the 'OculusStats.PerfMetrics' class.
```csharp
OculusStats.PerfMetrics.EnablePerfMetrics(true);
```

### Adding a new Oculus Stats Test case
1) Create a scene and drop it in the folder Assets/Scenes/StatsTests
2) Make changes to the scene based on the type of scenario you'd like to measure
3) Add your test scene to the build settings
4) Observe the Test Runner populate your new test case
5) Deploy the test to device!

# Adding A New Type of Performance Test

The code sample below is an example of how to add a new test. It has 2 major components that integrate the test with the framework.

### OculusPerformanceTestBase

_protected IEnumerator SetupTestRun(string scene)_

_parameter_ scene : The scene to load before running the performance analysis

SetupTestRun will first load the _cool_down_ scene which lowers the application's target FPS to 1 for 30 seconds. This allows the device to cool off before running the performance analysis. SetupTestRun also sets the Oculus GPU and CPU levels to 2 to provide consistent performance results.

_protected IEnumerator TearDownTestRun(string scene)_

_parameter_ scene : The scene to unload after the performance analysis is complete.

TearDownTestRun will unload the test scene.

### PerformanceTestSourceAttribute

The PerformanceTestSource Attribute
 
_public PerformanceTestSource(string testSceneRootDirectory)_

_parameter_ testSceneRootDirectory : The directory to search for test case scenes

This attribute is used to generate test cases for a given performance test. It searches through the list of scenes added to the build settings and generates test cases based on scenes in the directory specified by the user.

In the following example, the ExamplePerfTest would source it's test cases from Assets/Scenes/ExampleTests.

```csharp
using System.Collections;
using NUnit.Framework;
using Unity.PerformanceTesting;
using UnityEngine;
using UnityEngine.TestTools;

public class ExamplePerfTest : OculusPerformanceTestBase
{
    [Version("1")]
    [UnityTest, Performance]
    [PerformanceTestSource("ExampleTests")
    [Timeout(120000    public IEnumerator ExamplePerfTest(string scene)
    {
        yield return SetupTestRun(scene);
        yield return new MonoBehaviourTest<ExampleTestMonoBehaviour>();
        yield return TearDownTestRun(scene);
    }

    public class ExampleTestMonoBehaviour : MonoBehaviour, IMonoBehaviourTest
    {
        private readonly SampleGroupDefinition deltaTime = new SampleGroupDefinition("deltaTime", SampleUnit.Millisecond);
        public int numSampleFrames = 1000;        
        public bool IsTestFinished { get; set; }

        IEnumerator Start()
        {
            for (int i = 0; i < numSampleFrames; i++)
            {
                yield return 0;
            }
            IsTestFinished = true;
        }

        void Update()
        {
            if (!IsTestFinished)
            {
                Measure.Custom(deltaTime, Time.deltaTime);
            }
        }
    }
}
```
