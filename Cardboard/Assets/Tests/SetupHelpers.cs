#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace Tests
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

    public class TestSetupHelpers : CardboardSetup
    {
        private int m_CubeCount = 0;

        public void CameraLightSetup()
        {
            m_Camera = new GameObject("Camera");
            m_Camera.AddComponent<Camera>();


            m_Light = new GameObject("Light");
            Light light = m_Light.AddComponent<Light>();
            light.type = LightType.Directional;
        }

        public void TestCubeCreation()
        {
            m_Cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            m_Cube.transform.position = 5f * Vector3.forward;
        }

        public void CreateMassFloorObjects()
        {
            float x = -3.0f;
            float y = -0.5f;
            float zRow1 = 2.0f;
            float zRow2 = 2.0f;
            float zRow3 = 2.0f;
            float zRow4 = 2.0f;

            for (int i = 0; i < 20; i++)
            {
                var obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
                m_CubeCount += 1;
                obj.name = "TestCube " + m_CubeCount;
                obj.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                obj.transform.localPosition = new Vector3(x, y, zRow1);

                zRow1 = zRow1 + 0.5f;
                x = -2f;
            }

            for (int i = 0; i < 20; i++)
            {
                var obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
                m_CubeCount += 1;
                obj.name = "TestCube " + m_CubeCount;
                obj.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                obj.transform.localPosition = new Vector3(x, y, zRow2);

                zRow2 = zRow2 + 0.5f;
                x = -1f;
            }

            for (int i = 0; i < 20; i++)
            {
                var obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
                m_CubeCount += 1;
                obj.name = "TestCube " + m_CubeCount;
                obj.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                obj.transform.localPosition = new Vector3(x, y, zRow3);

                zRow3 = zRow3 + 0.5f;
                x = 0f;
            }

            for (int i = 0; i < 20; i++)
            {
                var obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
                m_CubeCount += 1;
                obj.name = "TestCube " + m_CubeCount;
                obj.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                obj.transform.localPosition = new Vector3(x, y, zRow4);

                zRow4 = zRow4 + 0.5f;
                x = 1f;
            }
        }

        public void CreateMassObjects()
        {
            float xRow1 = -0.5f;
            float xRow2 = -0.5f;
            float xRow3 = -0.5f;
            float yRow1 = 0.2f;
            float yRow2 = -0.2f;
            float yRow3 = -0.01f;
            float zRow1 = 2.5f;
            float zRow2 = 2.3f;

            for (int i = 0; i < 17; i++)
            {
                var obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
                m_CubeCount += 1;
                obj.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                obj.transform.localPosition = new Vector3(xRow1, yRow1, zRow1);
                obj.name = "TestCube " + m_CubeCount;

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

        public void CleanUpTestCubes()
        {
            if (m_CubeCount > 0)
            {
                for (int i = 0; i < m_CubeCount + 1; i++)
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

        public void CleanUpCameraLights()
        {
            Object.Destroy(GameObject.Find("Camera"));
            Object.Destroy(GameObject.Find("Light"));
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

        public void TestStageSetup(TestStageConfig TestConfiguration)
        {
            switch (TestConfiguration)
            {
                case TestStageConfig.BaseStageSetup:
                    CameraLightSetup();
                    break;

                case TestStageConfig.CleanStage:
                    CleanUpCameraLights();
                    CleanUpTestCubes();
#if UNITY_EDITOR
                    InsureInstancingRendering();
#endif
                    break;

                case TestStageConfig.Instancing:
#if UNITY_EDITOR
                    InsureInstancingRendering();
#endif
                    break;

                case TestStageConfig.MultiPass:
#if UNITY_EDITOR
                    InsureMultiPassRendering();
#endif
                    break;
            }
        }


        public void TestCubeSetup(TestCubesConfig TestConfiguration)
        {
            switch (TestConfiguration)
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
}
