# AR Automated Tests

Two automated test projects are currently provided for AR:
- ARCore
  - A smoke test verifying that ARCore initializes and enters a tracking state
  - Uses Googles ARCore package which can be found here: https://github.com/google-ar/arcore-unity-sdk/releases
- ARFoundation
  - A smoke test verifying that XR SDK implementaitons of ARCore and ARKit initialize and enter a tracking state
  - Uses the following Unity XR SDK Packages: ARFoundation, ARCore, ARKit. Documentation is available here: https://docs.unity3d.com/Packages/com.unity.xr.arfoundation@1.0/manual/index.html
  - This test can be run both on Android and iOS

The purpose of these tests are to verify that no Unity developer breaks basic AR functionality with a code check-in.

The tests can be executed using the follow methods:
- Editor
  - Open the project in your Unity editor
  - Switch to the Android or iOS platform in Build Settings
  - Go to Window -> General -> Test Runner
  - Switch to the Playmode Tab in the Test Runner window
  - Attach an ARCore/ARkit supported device to your system
  - Press the "Run All in Player" button to deploy the tests to the device
  - Results will be displayed in the Test Runner window
- Command Line
  - Sample Command Line command: Unity.exe -runTests -projectPath \<projectpath\> -testResults \<path\>\TestResults.xml -logfile \<path\>\UnityLog.txt -testPlatform Android -buildTarget Android -executeMethod Build.CommandLineSetup
  - Command Line Documentation: https://docs.unity3d.com/Manual/CommandLineArguments.html

Known Issues:
- Camera Permissions
  - If the tests are run manually, camera permissions will need to be granted on the device before results can be reported
  - If the tests are run via command line, an editor script will give the camera permissions

- Lack of AR Feature coverage
  - Unity is working on developing AR automation tests with more in depth coverage. New tests will be added to this repository as they are developed.
