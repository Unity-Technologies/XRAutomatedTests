# XR Automated Tests - Functional Tests

Here we provide XR funtional automated test projects:
- AR
-- Including test projects for ARCore, ARFoundation, ARMock
- VR
-- Including test projects for CrossPlatform and Windows Mixed Reality

These tests are essentially Unity projects that is using UnityTestFramework and launches with UnityTestRunner. Users can either open project in Unity to explore and run tests or launch tests from Unity command line.

Unity command line are full defined here:
[Unity Command Line Arguments](https://docs.unity3d.com/Manual/CommandLineArguments.html)

Running these tests from Unity command line provides flexibilities for automations. However Unity command line only generate test results in XML mode given argument -testResults and plain text log file given argument -logfile. To make it easy to read and understand test results, a reporter can be used to parse test results and log file and generate an HTML report. 

The test reporter and instructions can be find here:
[Unity Test Runner Results Reporter](https://github.cds.internal.unity3d.com/unity/UnityTestRunnerResultsReporter)

Executables can be find here:
[Executables](https://github.cds.internal.unity3d.com/unity/xr.testresultreporters)

For usage and details please check each individual test projects.
