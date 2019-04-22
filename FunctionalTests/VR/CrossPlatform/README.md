# Cross Platform VR Automated Tests

The purpose of this project is to run a common set of automated tests for the Unity APIs and features across the XR Platforms that Unity support.  This is not meant to be an exhaustive test across all of the XR Platforms, but should give a good idea of the status of each platform and the Unity APIs.
***
## Platforms

- Windows Standalone
-- Oculus
-- OpenVR
-- MockHMD
- MacOS Standalone
-- OpenVR
-- MockHMD
- UWP
-- Windows Mixed Reality
- Android
-- Oculus
-- Daydream
-- Cardboard
- iOS
-- Carboard
***
## Tests

- Audio
-- Controls
-- Spatialized Audio
- Camera
-- Gaze
-- Configuration
-- Screenshot
-- Eye/Node Position
- Graphics Configuration
-- Coordinate System
-- Rendering
- Performance APIs
- XR Settings APIs
- XR Nodes
- XR Smoke Tests
***
## How to run

### Run from Command Line
The best way to run these tests are to run them from command line and configure them with the parameters defined below

These are full defined here:
[Unity Command Line Arguments](https://docs.unity3d.com/Manual/CommandLineArguments.html)

Running these tests from Unity command line provides flexibilities for automations. However Unity command line only generate test results in XML mode given argument -testResults and plain text log file given argument -logfile. To make it easy to read and understand test results, a reporter can be used to parse test results and log file and generate an HTML report. 

The test reporter and instructions can be find here:
[Unity Test Runner Results Reporter](https://github.cds.internal.unity3d.com/unity/UnityTestRunnerResultsReporter)

Executables can be find here:
[Executables](https://github.cds.internal.unity3d.com/unity/xr.testresultreporters)

#### Unity Command Line Args
-testPlatform = StandaloneWindows64, Android, iOS, MacOS
-buildTarget = StandaloneWindows64, Android, iOS, MacOS

#### Customer XR Args
These are case sensitive!

-enabledXRTarget - Oculus, OpenVR, cardboard, daydeam, MockHMD, WindowsMR
-playergraphicsapi - Direct3D11, OpenGLES2, OpenGLES3, Direct3D12, Vulkan, OpenGLCore
-simulationMode - HoloLens, WindowsMR, Remoting
-stereorenderingpath - MultiPass, SinglePass, SinglePassInstancing

### Example Run Tests from Unity Command Line
[Path to Unity]\Unity.exe -runTests -automated -projectPath [Path to Test Project] -testResults [Path to Test Results]\TestResults.xml -logfile [Path to Unity Log]\UnityLog.txt -testPlatform StandaloneWindows64 -buildTarget StandaloneWindows64 -enabledxrtarget=MockHMD -playergraphicsAPI=Direct3D11 -stereorenderingpath=MultiPass

### Run in Editor
1. Open the Test Runner Window - Window -> General -> Test Runner
2. Select the PlayMode Tab
3. Click Run All to run in the Editor PlayMode or Run all in Player to run in the currently selected Build Target


