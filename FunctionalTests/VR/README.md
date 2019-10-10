# Cross Platform VR Automated Tests

The purpose of the CrossPlatform test projects is to run a common set of automated tests for the Unity APIs and features across the XR Platforms that Unity support.  This is not meant to be an exhaustive test across all of the XR Platforms, but should give a good idea of the status of each platform and the Unity APIs.

## Test Projects with Descriptions
| Project Name | Description | Supported Platform | Tested 3rd Party SDK |
|--------------|-------------|--------------------|----------------------|
| CrossPlatform|Built-in XR Functional Tests|StandaloneWindows, Android, iOS, MSA|Oculus, OpenVR, cardboard, daydream, MockHMD, WindowsMR|
| CrossPlatformSmokeTest|Subset of Built-in XR Functional Tests|StandaloneWindows, Android, iOS|Oculus, OpenVR, cardboard, daydream, MockHMD|
| CrossPlatform_XRSDK|Oculus XRSDK Functional Tests|StandaloneWindows, Android|Oculus XRSDK|
| CrossPlatformSmokeTest_XRSDK|Subset of Oculus XRSDK Functional Tests|StandaloneWindows, Android|Oculus XRSDK|
| CrossPlatform_MockHmdXRSDK|MockHMD XRSDK Functional Tests|StandaloneWindows, Android|MockHMD XRSDK|


## Platforms
- Windows Standalone
  - Oculus
  - OpenVR
  - MockHMD
- MacOS Standalone
  - OpenVR
  - MockHMD
- UWP
  - Windows Mixed Reality
- Android
  - Oculus
  - Daydream
  - Cardboard
- iOS
  - Carboard

## Tests

- Audio
  - Controls
  - Spatialized Audio
- Camera
  - Gaze
  - Configuration
  - Screenshot
  - Eye/Node Position
- Graphics Configuration
  - Coordinate System
  - Rendering
- Performance APIs
  - XR Settings APIs
  - XR Nodes

## How to run

### Run in Editor
1. Open the test project in Unity
2. Open the Test Runner Window - Window -> General -> Test Runner
3. Select the PlayMode Tab
4. Click Run All to run in the Editor PlayMode or Run all in Player to run in the currently selected Build Target

### Run from Command Line
To run the CrossPlatform tests from the command line, we'll use a combination of
1. Unity.exe command line options, 
2. Unity Test Framework command line options, and
2. Custom player and build settings options defined in the [com.unity.cli-config-manager package](https://github.cds.internal.unity3d.com/unity/com.unity.cli-config-manager/blob/trunk/README.md) 

#### Unity.exe command line options
Unity.exe command line options are command line options defined by the Unity.exe. 

The full list of these options can be found here: [Unity Command Line Arguments](https://docs.unity3d.com/Manual/CommandLineArguments.html), but a few important ones we'll be using while running our tests from the command line:

`-projectPath <full path to Unity Project>`
`-buildTarget <target platform e.g. Android, StandaloneWindows64, WindowsStoreApps>`
`-executeMethod <Fully qualified static editor method name>`
`-logfile <Path and filename where you want to write the Unity log>`

#### Unity Test Framework command line options
Unity Test Framework (UTF) command line options are command line options defined by UTF.

The full list of these options can be found here: [UTF Command Line Options] (https://docs.unity3d.com/Packages/com.unity.test-framework@1.1/manual/reference-command-line.html), but a few important ones we'll be using while running our tests from the command line:

`-runTests // instructs the UTF to run any tests it finds in your projects after the project opens and builds the project`
`-automated // provides addition ##utp log messages from the Unity Test Protocol logging system in the Unity log`
`-testPlatform <test target platform e.g. Android, StandaloneWindows64, playmode (to run tests in the editor in playmode>`
`-testResults <path and *.xml file name you want to write the test results to>`

#### com.unity.cli-config-manager command line options
The com.unity.cli-config-manager command line options are custom command line options available to further customize the player and build configuration for your test project.

The full list of these options can be found here: [com.unity.cli-config-manager package](https://github.cds.internal.unity3d.com/unity/com.unity.cli-config-manager/blob/trunk/README.md), but a few important ones we'll be using while running our tests from the command line:

`-enabledxrtarget=<XrTargetToEnable>`
`-scriptingbackend=<ScriptingBackend>`
`-playergraphicsapi=<GraphicsApi>`
`-stereorenderingpath=<StereoRenderingPath>`
`-simulationmode=<SimulationMode> // only relevant for WindowMR`

Running UTF tests from Unity command line provides flexibility for test automation. However, please be aware that when running tests from the command line using `-runTests` in combination with the `-testResults`, the test results will be generated in an XML format. If you'd like to generate an easy-to-read and understand test results HTML report from this .xml file, use the UnityTestRunnerResultsReporter to do so.

The test reporter repository and instructions can be find here:
[Unity Test Runner Results Reporter](https://github.cds.internal.unity3d.com/unity/UnityTestRunnerResultsReporter)

Prebuild executables for the Unity Test Runner Result Reporter can be found here:
[Executables](https://github.cds.internal.unity3d.com/unity/xr.testresultreporters)

<a name="usage"></a>
## Usage

<a name="android"></a>
### Android
**Example: From a Windows dev machine, run Oculus Quest/Go/GearVR using built-in VR with OpenGLES3, MultiPass rendering, Mono scripting backend**

`Unity.exe -runTests -automated -projectPath <pathToYourXrAutomatedTestsRepository>\FunctionalTests\VR\CrossPlatform -testPlatform Android -buildTarget Android -enabledxrtarget=Oculus -playergraphicsAPI=OpenGLES3 -stereorenderingpath=MultiPass -scriptingbackend=mono -executeMethod Assets.Editor.Build.CommandLineSetup -testResults <pathWhereYouWantYourResultsToBeWritten>\TestResults.xml -logfile <pathWhereYouWantYourUnityLogToBeWritten>\UnityLog.txt`

**Example: From an OSX dev machine, run Google Cardboard using built-in VR with OpenGLES3, MultiPass rendering, Mono scripting backend**

`Unity -runTests -automated -projectPath <pathToYourXrAutomatedTestsRepository>\FunctionalTests\VR\CrossPlatform -testPlatform Android -buildTarget Android -enabledxrtarget=cardboard -playergraphicsAPI=OpenGLES3 -stereorenderingpath=MultiPass -scriptingbackend=mono -executeMethod Assets.Editor.Build.CommandLineSetup -testResults <pathWhereYouWantYourResultsToBeWritten>\TestResults.xml -logfile <pathWhereYouWantYourUnityLogToBeWritten>\UnityLog.txt`

**Example: From a Windows dev machine, run Oculus Quest/Go/GearVR using Oculus XRSDK VR with OpenGLES3, Multiview rendering, IL2CPP scripting backend**

`Unity.exe -runTests -automated -projectPath <pathToYourXrAutomatedTestsRepository>\FunctionalTests\VR\CrossPlatform_XRSDK -testPlatform Android -buildTarget Android -enabledxrtarget=OculusXRSDK -playergraphicsAPI=OpenGLES3 -stereorenderingpath=Multiview -scriptingbackend=IL2CPP -executeMethod Assets.Editor.Build.CommandLineSetup -testResults <pathWhereYouWantYourResultsToBeWritten>\TestResults.xml -logfile <pathWhereYouWantYourUnityLogToBeWritten>\UnityLog.txt`

**Example: From a Windows dev machine, run Android using MockHMD XRSDK VR with OpenGLES3, Multiview rendering, IL2CPP scripting backend**

`Unity.exe -runtTests -batchmode -projectPath <pathToYourXrAutomatedTestsRepository>\FunctionalTests\VR\CrossPlatform_MockHmdXRSDK -testResults <pathWhereYouWantYourResultsToBeWritten>/TestResults.xml -testPlatform Android -buildTarget Android -playergraphicsapi=Direct3D11 -enabledxrtargets=MockHmdXRSDK -stereoRenderingPath=Multiview -scriptingbackend=IL2CPP -executeMethod Assets.Editor.Build.CommandLineSetup -testResults <pathWhereYouWantYourResultsToBeWritten>\TestResults.xml -logfile <pathWhereYouWantYourUnityLogToBeWritten>\UnityLog.txt`

### Windows
**Example: From a Windows dev machine, run Oculus Rift using built-in VR with D3D11, SinglePass rendering, IL2CPP scripting backend**

`Unity.exe -runtTests -batchmode -projectPath <pathToYourXrAutomatedTestsRepository>\FunctionalTests\VR\CrossPlatform -testResults <pathToYourPerfTestResultsDirectory>/Windows_D3D11_SinglethreadedRendering_Oculus_SinglePass.xml -testPlatform StandaloneWindows64 -buildTarget StandaloneWindows64 -playergraphicsapi=Direct3D11 -enabledxrtargets=Oculus -stereoRenderingPath=SinglePass -scriptingbackend=IL2CPP -executeMethod Assets.Editor.Build.CommandLineSetup -testResults <pathWhereYouWantYourResultsToBeWritten>\TestResults.xml -logfile <pathWhereYouWantYourUnityLogToBeWritten>\UnityLog.txt`

**Example: From a Windows dev machine, run Oculus Rift using XRSDK VR with D3D11, SinglePassInstanced rendering, IL2CPP scripting backend**

`Unity.exe -runtTests -batchmode -projectPath <pathToYourXrAutomatedTestsRepository>\FunctionalTests\VR\CrossPlatform_XRSDK -testResults <pathWhereYouWantYourResultsToBeWritten>/TestResults.xml -testPlatform StandaloneWindows64 -buildTarget StandaloneWindows64 -playergraphicsapi=Direct3D11 -enabledxrtargets=OculusXRSDK -stereoRenderingPath=SinglePassInstanced -scriptingbackend=IL2CPP -executeMethod Assets.Editor.Build.CommandLineSetup -testResults <pathWhereYouWantYourResultsToBeWritten>\TestResults.xml -logfile <pathWhereYouWantYourUnityLogToBeWritten>\UnityLog.txt`

**Example: From a Windows dev machine, run Windows Player using MockHMD XRSDK VR with D3D11, SinglePassInstanced rendering, IL2CPP scripting backend**

`Unity.exe -runtTests -batchmode -projectPath <pathToYourXrAutomatedTestsRepository>\FunctionalTests\VR\CrossPlatform_MockHmdXRSDK -testResults <pathWhereYouWantYourResultsToBeWritten>/TestResults.xml -testPlatform StandaloneWindows64 -buildTarget StandaloneWindows64 -playergraphicsapi=Direct3D11 -enabledxrtargets=MockHmdXRSDK -stereoRenderingPath=Instancing -scriptingbackend=IL2CPP -executeMethod Assets.Editor.Build.CommandLineSetup -testResults <pathWhereYouWantYourResultsToBeWritten>\TestResults.xml -logfile <pathWhereYouWantYourUnityLogToBeWritten>\UnityLog.txt`


