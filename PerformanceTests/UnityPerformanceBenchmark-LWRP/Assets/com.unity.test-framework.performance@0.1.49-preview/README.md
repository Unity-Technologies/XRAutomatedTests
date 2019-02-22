# Performance testing extension for Unity Test Runner

The Unity Performance Testing Extension is a Unity Editor package that, when installed, provides an API and test case decorators to make it easier to take measurements/samples of Unity profiler markers, and other custom metrics outside of the profiler, within the Unity Editor and built players. It also collects configuration metadata, such as build and player settings, which is useful when comparing data for against different hardware and configurations.


## Installing

To install the Performance Testing Extension package
1. Open the manifest.json file for your Unity project (located in the YourProject/Packages directory) in a text editor
2. Add com.unity.test-framework.performance to the dependencies as seen below
3. Add com.unity.test-framework.performance to the testables section. If there is not a testables section in your manifest.json file, go ahead and it add.
4. Save the manifest.json file
5. Verify the Performance Testing Extension is now installed opening the Unity Package Manager window
6. Ensure you have created an Assembly Definition file created in the same folder where your tests or scripts are that you’ll reference the Performance Testing Extension with. This Assembly Definition file needs to reference Unity.PerformanceTesting in order to use the Performance Testing Extension. Example of how to do this:
    * Create a new folder for storing tests in ("Tests", for example)
    * Create a new assembly definition file in the new folder using the context menu (right click/Create/Assembly definition) and name it "Tests" (or whatever you named the folder from step a. above)
    * In inspector for the assembly definition file check "Test Assemblies”, and then Apply.
    * Open the assembly definition file in a text editor and add Unity.PerformanceTesting. To the references section. Save the file when you’re done doing this.

> manifest.json
``` json
{
    "dependencies": {
        "com.unity.test-framework.performance": "0.1.49-preview",
        "com.unity.modules.jsonserialize": "1.0.0",
        "com.unity.modules.unitywebrequest": "1.0.0",
        "com.unity.modules.unityanalytics": "1.0.0",
        "com.unity.modules.vr": "1.0.0",
        "com.unity.modules.physics": "1.0.0",
        "com.unity.modules.xr": "1.0.0"
      },
      "testables": [
        "com.unity.test-framework.performance"
      ]
}
```

> assembly definition
``` json
{
    "name": "Tests.Editor",
    "references": [
        "Unity.PerformanceTesting"
    ],
    "optionalUnityReferences": [
        "TestAssemblies"
    ],
    "includePlatforms": [
        "Editor"
    ],
    "excludePlatforms": [],
    "allowUnsafeCode": false
}
```