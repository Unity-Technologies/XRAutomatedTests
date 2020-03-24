using System.Collections;
using com.unity.xr.test.runtimesettings;
using NUnit.Framework;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.SpatialTracking;

public abstract class XrFunctionalTestBase
{
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

    private int cubeCount;

    public GameObject Camera { get; set; }

    public GameObject Cube { get; set; }

    public GameObject Light {get;set;}

    protected CurrentSettings Settings;
    protected static int DefaultFrameSkipCount = 1;
    private readonly int numCubesToCreate = 17;

    [OneTimeSetUp]
    public virtual void OneTimeSetUp()
    {
        Settings = Resources.Load<CurrentSettings>("settings");
    }

    [SetUp]
    public virtual void SetUp()
    {
        TestStageSetup(TestStageConfig.BaseStageSetup);
    }

    [TearDown]
    public virtual void TearDown()
    {
        TestStageSetup(TestStageConfig.CleanStage);
    }

    protected void AssertNotUsingEmulation()
    {
        if (Settings != null && Settings.SimulationMode != string.Empty)
        {
            Assert.Ignore("This test cannot run in emulation mode. Skipping.");
        }
    }

    protected IEnumerator SkipFrame(int frames)
    {
        Debug.Log($"Skipping {frames} frames.");
        for (var f = 0; f < frames; f++)
        {
            yield return null;
        }
    }

    protected IEnumerator SkipFrame()
    {
        yield return SkipFrame(1);
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

        for (var i = 0; i < numCubesToCreate; i++)
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
