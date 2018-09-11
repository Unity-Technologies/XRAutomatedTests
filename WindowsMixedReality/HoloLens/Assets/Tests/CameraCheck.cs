using UnityEngine;
using UnityEngine.XR;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using System;
using System.IO;

//[Ignore("Metro/wsa is disabled on Katana")]
internal class CameraCheck : HoloLensTestBase
{
    private bool m_RaycastHit = false;
    private bool m_DidSaveScreenCapture = false;
    private string m_FileName;

    private float m_StartingScale;
    private float m_StartingZoomAmount;
    private float m_StartingRenderScale;

    void Start()
    {
        m_FileName = Application.persistentDataPath + "/ScreenCaptureHoloLens";

        m_StartingScale = XRSettings.eyeTextureResolutionScale;
        m_StartingZoomAmount = XRDevice.fovZoomFactor;
        m_StartingRenderScale = XRSettings.renderViewportScale;
    }

    [SetUp]
    public void Setup()
    {
        m_TestSetupHelpers.TestCubeSetup(TestCubesConfig.TestCube);
    }

    [TearDown]
    public void TearDown()
    {
        m_RaycastHit = false;

        XRSettings.eyeTextureResolutionScale = 1f;
        XRDevice.fovZoomFactor = m_StartingZoomAmount;
        XRSettings.renderViewportScale = 1f;

        UnityEditor.PlayerSettings.stereoRenderingPath = UnityEditor.StereoRenderingPath.Instancing;

        m_TestSetupHelpers.TestStageSetup(TestStageConfig.CleanStage);
    }

    [UnityTest]
    public IEnumerator GazeCheck()
    {
        RaycastHit info;
        var head = InputTracking.GetLocalPosition(XRNode.Head);

        m_Cube.transform.position = new Vector3(head.x, head.y, head.z + 3f);
        
        if (Physics.Raycast(head, m_Camera.transform.forward, out info, 10f))
        {
            yield return new WaitForSeconds(0.05f);
            if (info.collider.name == m_Cube.name)
            {
                m_RaycastHit = true;
            }
        }

        Assert.IsTrue(m_RaycastHit, "Gaze check failed to hit something!");
    }

    [Test]
    public void CameraCheckForMultiPass()
    {
        m_TestSetupHelpers.TestStageSetup(TestStageConfig.MultiPass);
        Assert.AreEqual(UnityEditor.StereoRenderingPath.MultiPass, UnityEditor.PlayerSettings.stereoRenderingPath, "Expected StereoRenderingPath to be Multi pass");
    }

    [Test]
    public void CameraCheckForInstancing()
    {
        m_TestSetupHelpers.TestStageSetup(TestStageConfig.Instancing);
        Assert.AreEqual(UnityEditor.StereoRenderingPath.Instancing, UnityEditor.PlayerSettings.stereoRenderingPath, "Expected StereoRenderingPath to be Instancing");
    }

    [Test]
    public void CheckRefreshRate()
    {
        var refreshRate = XRDevice.refreshRate;

        Assert.GreaterOrEqual(refreshRate, 60, "Refresh rate returned lower than expected");
    }

    [Test]
    public void RenderViewportScale()
    {
        XRSettings.renderViewportScale = 1f;
        Assert.AreEqual(1f, XRSettings.renderViewportScale, "Render viewport scale is not being respected");

        XRSettings.renderViewportScale = 0.7f;
        Assert.AreEqual(0.7f, XRSettings.renderViewportScale, "Render viewport scale is not being respected");

        XRSettings.renderViewportScale = 0.5f;
        Assert.AreEqual(0.5f, XRSettings.renderViewportScale, "Render viewport scale is not being respected");
    }

    [Test]
    public void EyeTextureResolutionScale()
    {
        float scale = 0f;
        float scaleCount = 0f;

        for (float i = 0f; i < 5; i++)
        {
            scale = scale + 0.1f;
            scaleCount = scaleCount + 0.1f;
            XRSettings.eyeTextureResolutionScale = scale;
            Debug.Log("EyeTextureResolutionScale = " + scale);
            Assert.AreEqual(scaleCount, XRSettings.eyeTextureResolutionScale, "Eye texture resolution scale is not being respected");
        }
    }

    [Test]
    public void DeviceZoom()
    {
        float zoomAmount = 0f;
        float zoomCount = 0f;

        for (int i = 0; i < 5; i++)
        {
            zoomAmount = zoomAmount + 1f;
            zoomCount = zoomCount + 1f;

            XRDevice.fovZoomFactor = zoomAmount;
            Assert.AreEqual(zoomCount, XRDevice.fovZoomFactor, "Zoom Factor was not set correclty");
        }
    }

    [UnityTest]
    public IEnumerator TakeScreenShot()
    {
        yield return null;

        try
        {
            if (XRDevice.model.Contains("HoloLens"))
            {
                m_FileName = Application.persistentDataPath + "/ScreenCaptureHoloLens.jpg";
            }
            else
            {
                m_FileName = Application.temporaryCachePath + "/ScreenCaptureMR.jpg";
            }

            ScreenCapture.CaptureScreenshot(m_FileName);

            m_DidSaveScreenCapture = true;
        }
        catch (Exception e)
        {
            Debug.Log("Failed to get capture! : " + e);
            m_DidSaveScreenCapture = false;
            Assert.Fail("Failed to get capture! : " + e);
        }

        if (m_DidSaveScreenCapture)
        {
            yield return new WaitForSeconds(5);

            Texture2D tex = new Texture2D(2, 2);
            var texData = File.ReadAllBytes(m_FileName);
            Debug.Log("Screen Shot Success!" + Environment.NewLine + "File Name = " + m_FileName);

            tex.LoadImage(texData);

            Assert.IsNotNull(tex, "Texture Data is empty");
        }
    }
}
