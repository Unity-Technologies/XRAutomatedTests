# Running the Performance Tests
## Examples

**OculusQuest_BuiltInXR_BuiltInRP_MultiPass_OpenGLES3_Linear_IL2CPP_ARM64**

```Unity.exe -runTests -automated -projectPath <Path>\BuiltIn-VR-Perf -testPlatform Android -buildTarget Android -enabledxrtargets=Oculus -playergraphicsapi=OpenGLES3 -colorspace=Linear -stereoRenderingPath=MultiPass -scriptingbackend=il2cpp -androidtargetarchitecture=ARM64 -executeMethod Assets.Editor.RenderPerformancePrebuildStep.Setup -testResults <Path>\TestResults.xml -logfile <Path>\UnityLog.txt ```

**OculusQuest_XRSDK_BuiltInRP_Multiview_OpenGLES3_Linear_IL2CPP_ARM64**

```Unity.exe -runTests -automated -projectPath <Path>\Oculus-XRSDK-Perf -testPlatform Android -buildTarget Android -enabledxrtargets=OculusXRSDK -playergraphicsapi=OpenGLES3 -colorspace=Linear -stereoRenderingPath=Multiview -scriptingbackend=il2cpp -androidtargetarchitecture=ARM64 -executeMethod Assets.Editor.RenderPerformancePrebuildStep.Setup -testResults <Path>\TestResults.xml -logfile <Path>\UnityLog.txt ```

**OculusQuest_XRSDK_URP_MultiPass_Vulkan_Linear_IL2CPP_ARM64**

```Unity.exe -runTests -automated -projectPath <Path>\Oculus-XRSDK-URP-Perf -testPlatform Android -buildTarget Android -enabledxrtargets=OculusXRSDK -playergraphicsapi=Vulkan -colorspace=Linear -stereoRenderingPath=MultiPass -scriptingbackend=il2cpp -androidtargetarchitecture=ARM64 -executeMethod Assets.Editor.RenderPerformancePrebuildStep.Setup -testResults <Path>\TestResults.xml -logfile <Path>\UnityLog.txt ```

**Android_BuiltInRP_OpenGLES3_Linear_Mono**

```Unity.exe -runTests -automated -projectPath <Path>\NoXR-Perf -testPlatform Android -buildTarget Android -playergraphicsapi=OpenGLES3 -colorspace=Linear -scriptingbackend=mono -executeMethod Assets.Editor.RenderPerformancePrebuildStep.Setup -testResults <Path>\TestResults.xml -logfile <Path>\UnityLog.txt```

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
