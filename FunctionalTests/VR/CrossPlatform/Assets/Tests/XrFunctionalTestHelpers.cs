#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.SpatialTracking;

public enum TestStageConfig
{
    BaseStageSetup,
    CleanStage,
    MultiPass,
    Instancing
}

public enum TestCubesConfig
{
    None,
    TestCube,
    PerformanceMassFloorObjects,
    PerformanceMassObjects,
    TestMassCube
}

public class XrFunctionalTestHelpers
{
    private int cubeCount;
 
    public GameObject Camera
    {
        get { return test.Camera; }
        set { test.Camera = value; }
    }

    public GameObject Cube
    {
        get { return test.Cube; }
        set { test.Cube = value; }
    }

    public GameObject Light
    {
        get { return test.Light; }
        set { test.Light = value; }
    }

    private readonly XrFunctionalTestBase test;

    public XrFunctionalTestHelpers(XrFunctionalTestBase test)
    {
        this.test = test;
    }

    private void CameraLightSetup()
    {
        Camera = new GameObject("camera");
        Camera.AddComponent<Camera>();
        Camera.AddComponent<TrackedPoseDriver>();

        var trackedPoseDriver = Camera.GetComponent<TrackedPoseDriver>();
        trackedPoseDriver.SetPoseSource(TrackedPoseDriver.DeviceType.GenericXRDevice, TrackedPoseDriver.TrackedPose.Head);

        Light = new GameObject("light");
        var light = Light.AddComponent<Light>();
        light.type = LightType.Directional;
    }

    private void TestCubeCreation()
    {
        Cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        Cube.transform.position = 5f * Vector3.forward;
    }

    private void CreateMassFloorObjects()
    {
        var x = -3.0f;
        var y = -0.5f;
        var zRow1 = 2.0f;
        var zRow2 = 2.0f;
        var zRow3 = 2.0f;
        var zRow4 = 2.0f;

        for (var i = 0; i < 20; i++)
        {
            var obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cubeCount += 1;
            obj.name = "TestCube " + cubeCount;
            obj.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            obj.transform.localPosition = new Vector3(x, y, zRow1);

            zRow1 = zRow1 + 0.5f;
            x = -2f;
        }

        for (var i = 0; i < 20; i++)
        {
            var obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cubeCount += 1;
            obj.name = "TestCube " + cubeCount;
            obj.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            obj.transform.localPosition = new Vector3(x, y, zRow2);

            zRow2 = zRow2 + 0.5f;
            x = -1f;
        }

        for (var i = 0; i < 20; i++)
        {
            var obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cubeCount += 1;
            obj.name = "TestCube " + cubeCount;
            obj.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            obj.transform.localPosition = new Vector3(x, y, zRow3);

            zRow3 = zRow3 + 0.5f;
            x = 0f;
        }

        for (var i = 0; i < 20; i++)
        {
            var obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cubeCount += 1;
            obj.name = "TestCube " + cubeCount;
            obj.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            obj.transform.localPosition = new Vector3(x, y, zRow4);

            zRow4 = zRow4 + 0.5f;
            x = 1f;
        }
    }

    private void CreateMassObjects()
    {
        var xRow1 = -0.5f;
        var xRow2 = -0.5f;
        var xRow3 = -0.5f;
        var yRow1 = 0.2f;
        var yRow2 = -0.2f;
        var yRow3 = -0.01f;
        var zRow1 = 2.5f;
        var zRow2 = 2.3f;

        // TODO we should extract '17' here to be a constant and describe what it is
        for (var i = 0; i < 17; i++)
        {
            var obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cubeCount += 1;
            obj.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            obj.transform.localPosition = new Vector3(xRow1, yRow1, zRow1);
            obj.name = "TestCube " + cubeCount;

            if (i < 5)
            {
                xRow1 = xRow1 + 0.2f;
                obj.transform.localPosition = new Vector3(xRow1, yRow1, zRow1);
            }
            else if (i > 5 && i < 11)
            {
                xRow2 = xRow2 + 0.2f;
                obj.transform.localPosition = new Vector3(xRow2, yRow2, zRow1);
            }
            else if (i > 11)
            {
                xRow3 = xRow3 + 0.2f;
                obj.transform.localPosition = new Vector3(xRow3, yRow3, zRow2);
            }
        }
    }

    private void CleanUpTestCubes()
    {
        if (cubeCount > 0)
        {
            for (var i = 0; i < cubeCount + 1; i++)
            {
                var obj = GameObject.Find("TestCube " + i);
                Object.Destroy(obj);
            }
        }

        if (GameObject.Find("Cube"))
        {
            Object.Destroy(GameObject.Find("Cube"));
        }
    }

    private void CleanUpCameraLights()
    {
        Object.Destroy(GameObject.Find("camera"));
        Object.Destroy(GameObject.Find("light"));
    }

#if UNITY_EDITOR
    public void InsureMultiPassRendering()
    {
        PlayerSettings.stereoRenderingPath = StereoRenderingPath.MultiPass;
    }

    public void InsureInstancingRendering()
    {
        PlayerSettings.stereoRenderingPath = StereoRenderingPath.Instancing;
    }
#endif

    // TODO should singlepass be in here somewhere?
    public void TestStageSetup(TestStageConfig testConfiguration)
    {
        switch (testConfiguration)
        {
            case TestStageConfig.BaseStageSetup:
                CameraLightSetup();
                break;

            case TestStageConfig.CleanStage:
                CleanUpCameraLights();
                CleanUpTestCubes();
                break;
#if UNITY_EDITOR
            case TestStageConfig.Instancing:
                InsureInstancingRendering();
                break;

            case TestStageConfig.MultiPass:
                InsureMultiPassRendering();
                break;
#endif
        }
    }


    public void TestCubeSetup(TestCubesConfig testConfiguration)
    {
        switch (testConfiguration)
        {
            case TestCubesConfig.TestCube:
                TestCubeCreation();
                break;

            case TestCubesConfig.PerformanceMassFloorObjects:
                CreateMassFloorObjects();
                break;

            case TestCubesConfig.PerformanceMassObjects:
                CreateMassObjects();
                break;

            case TestCubesConfig.None:
                break;
        }
    }
}
