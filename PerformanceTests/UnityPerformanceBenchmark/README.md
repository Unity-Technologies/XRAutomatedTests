##### Table of Contents   
<a href="#intro">Intro</a><br>
<a href="#xrbuildoptions">XR Player Build Option Support</a><br>
<a href="#components">Unity Performance Benchmark Components</a>
<br>
<a href="#whatyouneed">What You''ll Need</a>
<br>
<a href="#howtorun">How to Run the Unity Performance Benchmark Tests</a>
<br>
&nbsp;&nbsp;&nbsp;&nbsp;<a href="#options">Options</a>
<br>
&nbsp;&nbsp;&nbsp;&nbsp;<a href="#usage">Usage</a>
<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<a href="#android">Android</a>
<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<a href="#ios">iOS</a>
<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<a href="#osx">OSX</a>
<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<a href="#windows">Windows</a>

---
<a name="intro"></a>
# Intro

The Unity Performance Benchmark project contains a set of tests that use the [Unity Performance Test Extension](https://docs.unity3d.com/Packages/com.unity.test-framework.performance@1.0/manual/index.html) and run using the Unity Test Framework, aka as the [Unity Test Runner](https://docs.unity3d.com/Manual/testing-editortestsrunner.html). These tests provide examples of how you can compile a set of scenes and tests, leveraging the [Unity Test Runner](https://docs.unity3d.com/Manual/testing-editortestsrunner.html) and [Unity Performance Test Extension](https://docs.unity3d.com/Packages/com.unity.test-framework.performance@1.0/manual/index.html), to establish performance metrics baselines in Unity for your development work, and then trend those metrics across changes. These performance tests should not be considered definitive, but rather a good starting point for you to customize and create new performance tests that are more tailored to your needs.

The Unity Performance Benchmark test project should be able to be run on any of the official supported platforms of [Unity Test Runner](https://docs.unity3d.com/Manual/testing-editortestsrunner.html), and have been run and tested against the platforms where noted below:

1. StandaloneWindows <sup>**\* tested**</sup>
2. StandaloneOSX <sup>**\* tested**</sup>
3. iOS <sup>**\* tested**</sup>
4. tvOS
5. Android <sup>**\* tested**</sup>
6. PS4
7. XboxOne

<a name="xrbuildoptions"></a>
# XR Player Build Configuration Option Support

In addition to the default command line options provided by the [Unity Test Runner](https://docs.unity3d.com/Manual/testing-editortestsrunner.html), the Unity Performance Benchmark project uses a static method implemented in a class in the Editor folder of the Unity project to parse the command line for additional player configuration options. The fully-qualified method name is then passed using the `-executeMethod` command line option when launching Unity. In this manner you're able to programmatically build the player before running the tests in more varied configurations than are currently supported with the default [Unity Test Runner](https://docs.unity3d.com/Manual/testing-editortestsrunner.html) command line options. Some of these _custom_ command line options allow you to build the Unity Player with XR support, but they are not required to run the tests.

Therefore, you have the option to run the Unity Performance Benchmark tests in either non-XR or XR player configurations. It should be noted that the test scenes were put together with the idea that they would be run against XR platforms where rendering tends to be more CPU/GPU intensive. Thus, the scenes in the project may not be large or complex enough to stress non-XR player configurations in the way you require. If this is the case for you, a good option would be to augment the existing scenes, or create new ones, that deliver the load needed for rendering in your particular situation.

<a name="components"></a>
# Unity Performance Benchmark Components
The image below illustrates the components used to run performance tests and collect metrics with the Unity Test Runner framework and [Performance Testing Extension](https://docs.unity3d.com/Packages/com.unity.test-framework.performance@1.0/manual/index.html), then establish baseline performance metrics against subsequent metric collection.

<a name="whatyouneed"></a>
# What You'll Need

Here are the tools you'll need to start running the Unity Performance Benchmark tests.

**1. Unity Installation**  - version 2018.1 and forward is supported by the Unity Performance Test Extension. This needs to be configured for build and deployment for the platform, SDKs, development environment, etc.

**2. Performance Benchmark Tests**  - written in the [Unity Test Runner](https://docs.unity3d.com/Manual/testing-editortestsrunner.html) framework using the [Unity Performance Testing Extension](https://docs.unity3d.com/Packages/com.unity.test-framework.performance@1.0/manual/index.html) package.

The [XRAutomatedTests](https://github.com/Unity-Technologies/XRAutomatedTests) project contains a sample performance benchmark Unity project in the `PerformanceTests/UnityPerformanceBenchmark` subdirectory. [Download the latest released](https://github.com/Unity-Technologies/XRAutomatedTests/releases) sample project from the branch/release that corresponds to the major/minor version of Unity you'll be testing against (currently 2018.1 and forward is supported).
 
 This sample project uses the Unity Test Runner framework and the [Unity Performance Testing Extension](https://docs.unity3d.com/Packages/com.unity.test-framework.performance@1.0/manual/index.html). Feel free to start with this project, or use it to inspire your own performance test for your specific context.

**3. Unity Performance Benchmark Reporter**  - The [Unity Performance Benchmark Reporter](https://github.com/Unity-Technologies/PerformanceBenchmarkReporter/wiki/Home) enables the comparison of performance metric baselines and subsequent performance metrics (as generated using the Unity Test Runner with the Unity Performance Testing Extension) in an html report utilizing graphical visualizations.

<a name="howtorun"></a>
# How to Run the Unity Performance Benchmark Tests

The command line option format to run the Unity Performance Benchmark Tests looks like this:

`Unity.exe -runTests -batchmode -projectPath <Path To Performance Test Project> -testResults <Path To A Writable Location>\<Your Result File Name>.xml -testPlatform <An Officially Supported Unity Test Runner Platform> -buildTarget <SupportedUnity --buildTarget value> [-option…] -executeMethod Assets.Editor.RenderPerformancePrebuildStep.Setup `

* See [supported Unity --buildTarget values here.](https://docs.unity3d.com/Manual/CommandLineArguments.html)
* See [supported Unity Test Runner -testPlatform values](https://docs.unity3d.com/Manual/PlaymodeTestFramework.html) under the "Running from the command line" section

All of the required command line options above are Unity Test Runner options. The optional command line options [-option…] are custom to the Unity Performance Benchmark Test project as defined in the static method implemented in a class in the Editor folder of the Unity project:

<a name="options"></a>
## Options

      --enabledxrtargets=VALUE
                             XR targets to enable in XR enabled players,
                               separated by ';'. Values: Oculus,OpenVR, cardboard, daydream
      --playergraphicsapi=VALUE
                             Optionally force player to use the specified
                               GraphicsDeviceType. Values: Direct3D11, OpenGLES2,
                               OpenGLES3, PlayStationVita, PlayStation4,
                               XboxOne, Metal, OpenGLCore, Direct3D12, N3DS,
                               Vulkan, Switch, XboxOneD3D12
      --stereoRenderingPath=VALUE
                             StereoRenderingPath to use for XR enabled
                               players. Values: MultiPass, SinglePass, Instancing.
                               Default is SinglePass.
      --scriptingbackend=VALUE
                             Optionally specify the scripting backend to use. Values: IL2CPP, Mono. IL2CPP is default
      --mtRendering          Enable or disable multithreaded rendering.
                               Enabled is default. Use option to enable, or use
                               option and append '-' to disable.
      --graphicsJobs         Enable graphics jobs rendering. Disabled is
                               default. Use option to enable, or use option and
                               append '-' to disable.
      --minimumandroidsdkversion=VALUE
                             Minimum Android SDK Version to use. Default is
                               AndroidApiLevel24. Use for deployment and
                               running tests on Android device.
      --targetandroidsdkversion=VALUE
                             Target Android SDK Version to use. Default is
                               AndroidApiLevel24. Use for deployment and
                               running tests on Android device.
      --appleDeveloperTeamID=VALUE
                             Apple Developer Team ID. Use for deployment and
                               running tests on iOS device.
      --iOSProvisioningProfileID=VALUE
                             iOS Provisioning Profile ID. Use for deployment
                               and running tests on iOS device.

**NOTE** for the `-enabledxrtargets=` option above, while it's valid to add multiple XR target SDK values here, with respect to running performance tests, it's more common to only add a single value here.

**Tip** to enable single threaded rendering, use options '--mtRendering- --graphicsJobs-' (although disabling graphicsJobs is probably not necessary as it's default is disabled, this combination ensures single threaded rendering in the case that your test project has graphicsJobs enabled)

**Tip** The Performance Benchmark tests _are designed to be run from the command line_ using the Unity executable where a set of command line arguments are passed to tell the Unity Test Runner where to write the test results .xml file, where to find the project, as well as the configuration you want to build the player in prior to running the tests.

You can, however, launch the Unity Editor with the same commands _minus the `-runTests` and `-batchmode` command line options_ and then run the test in the Editor or Player from the Unity Test Runner window. This method _will not create the .xml result files needed for use with the Unity Performance Benchmark Reporter_. Launching the Unity Editor in this way manner is a good way to test and debug your performance test code.

<a name="usage"></a>
## Usage

<a name="android"></a>
### Android

**Example: No-XR Android with OpenGLES3 Graphics API, SingleThreaded rendering**

`Unity.exe -runTests -batchmode -projectPath <pathToYourPerformanceTestProject> -testResults <pathToYourPerfTestResultsDirectory>/Android_OpenGLES3_SinglethreadedRendering.xml -testPlatform Android -buildTarget Android -playergraphicsapi=OpenGLES3 -mtRendering- -executeMethod Assets.Editor.RenderPerformancePrebuildStep.Setup `

**Example: XR Android with OpenGLES3, SingleThreaded rendering, GearVR SDK, SinglePass StereoRendering**

`Unity.exe -runTests -batchmode -projectPath <pathToYourPerformanceTestProject> -testResults <pathToYourPerfTestResultsDirectory>/Android_OpenGLES3_SinglethreadedRendering.xml -testPlatform Android -buildTarget Android -playergraphicsapi=OpenGLES3 -mtRendering-  -enabledxrtargets=Oculus -stereoRenderingPath=SinglePass -executeMethod Assets.Editor.RenderPerformancePrebuildStep.Setup`

<a name="ios"></a>
### iOS

**Example: No-XR iOS with OpenGLES3 Graphics API, SingleThreaded rendering**

`./Unity -runTests —batch mode -projectPath <pathToYourPerformanceTestProject> -testResults <pathToYourPerfTestResultsDirectory>/iOS_OpenGLES3_SinglethreadedRendering.xml -testPlatform iOS -buildTarget iOS -playergraphicsapi=OpenGLES3 -mtRendering- -appleDeveloperTeamID=<yourAppleDeveloperTeamID> -iOSProvisioningProfileID=<yourIosProvisionProfileID> -executeMethod Assets.Editor.RenderPerformancePrebuildStep.Setup `

**Example: XR iOS with OpenGLES3 Graphics API, SingleThreaded rendering, Google Cardboard SDK, SinglePass StereoRendering**

`./Unity -runTests —batch mode -projectPath <pathToYourPerformanceTestProject> -testResults <pathToYourPerfTestResultsDirectory>/iOS_OpenGLES3_SinglethreadedRendering.xml -testPlatform iOS -buildTarget iOS -playergraphicsapi=OpenGLES3 -mtRendering- -enabledxrtargets=cardboard -stereoRenderingPath=SinglePass -appleDeveloperTeamID=<yourAppleDeveloperTeamID> -iOSProvisioningProfileID=<yourIosProvisionProfileID> -executeMethod Assets.Editor.RenderPerformancePrebuildStep.Setup`

<a name="osx"></a>
### OSX

**Example: OSX with Metal Graphics API, SingleThreaded rendering**

`./Unity -runTests —batch mode -projectPath <pathToYourPerformanceTestProject> -testResults <pathToYourPerfTestResultsDirectory>/OSX_Metal_SinglethreadedRendering.xml -testPlatform StandaloneOSX -buildTarget OSXUniversal -playergraphicsapi=Metal -mtRendering- -appleDeveloperTeamID=<yourAppleDeveloperTeamID> -iOSProvisioningProfileID=<yourIosProvisionProfileID> -executeMethod Assets.Editor.RenderPerformancePrebuildStep.Setup`

<a name="windows"></a>
### Windows

**Example: No-XR Windows with D3D11 Graphics API, SingleThreaded rendering**

`Unity.exe -runTests -batchmode -projectPath <pathToYourPerformanceTestProject> -testResults <pathToYourPerfTestResultsDirectory>/Windows_D3D11_SinglethreadedRendering.xml -testPlatform StandaloneWindows64 -buildTarget StandaloneWindows64 -playergraphicsapi=Direct3D11 -mtRendering--executeMethod Assets.Editor.RenderPerformancePrebuildStep.Setup`

**Example: XR Windows with D3D11 Graphics API, MultThreaded rendering, Oculus SDK, SinglePass StereoRendering**

`Unity.exe -runTests -batchmode -projectPath <pathToYourPerformanceTestProject> -testResults <pathToYourPerfTestResultsDirectory>/Windows_D3D11_SinglethreadedRendering_Oculus_SinglePass.xml -testPlatform StandaloneWindows64 -buildTarget StandaloneWindows64 -playergraphicsapi=Direct3D11 -mtRendering  -enabledxrtargets=Oculus -stereoRenderingPath=SinglePass -executeMethod Assets.Editor.RenderPerformancePrebuildStep.Setup`
