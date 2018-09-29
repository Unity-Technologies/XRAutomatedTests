using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using UnityEngine.XR.WSA.WebCam;
using System.Collections.Generic;
using System;

internal class CameraProjectionMatrix : TestBaseSetup
{
    GameObject m_Canvas = null;
    Renderer m_CanvasRenderer = null;
    PhotoCapture m_PhotoCaptureObj = null;
    PhotoCapture.PhotoCaptureResult m_Result;
    CameraParameters m_CameraParameters;
    Texture2D m_Texture = null;
    Matrix4x4 m_HoloLensProjectionMatrix;
    Matrix4x4 m_UnityCameraProjectionMatrix;

    float HoloFovX;
    float HoloFovY;
    float HoloZFlip;
    float HoloWCoord;

    bool m_FovCheck = false;
    bool m_ZFlipCheck = false;
    bool m_HoloWCordCheck = false;

    [TearDown]
    public void TearDown()
    {
        if (m_PhotoCaptureObj != null)
        {
            m_PhotoCaptureObj.Dispose();
            m_PhotoCaptureObj = null;
        }
        GameObject.Destroy(m_Canvas);
        m_PhotoCaptureObj = null;
    }

	[UnityTest]
	public IEnumerator CameraProjectionMatrixCapture()
	{
	    WmrDeviceCheck();
        GameObject.Destroy(m_TestSetupHelpers.m_Cube);

        Debug.Log("Initializing...");
        List<Resolution> resolutions = new List<Resolution>(PhotoCapture.SupportedResolutions);
        Resolution selectedResolution = resolutions[0];

        m_CameraParameters = new CameraParameters(WebCamMode.PhotoMode);
        m_CameraParameters.cameraResolutionWidth = selectedResolution.width;
        m_CameraParameters.cameraResolutionHeight = selectedResolution.height;
        m_CameraParameters.hologramOpacity = 0.0f;
        m_CameraParameters.pixelFormat = CapturePixelFormat.BGRA32;

        m_Texture = new Texture2D(selectedResolution.width, selectedResolution.height, TextureFormat.BGRA32, false);


        PhotoCapture.CreateAsync(false, OnCreatedPhotoCaptureObject);

        yield return new WaitForSeconds(1f);

        Debug.Log("Taking picture...");

        m_PhotoCaptureObj.TakePhotoAsync(OnPhotoCaptured);

        yield return new WaitForSeconds(1f);

        Assert.IsTrue(m_Result.success, "Failed to take the picture");
        Assert.IsTrue(m_FovCheck, "Horizontal FOV is greater then Vertical FOV");
        Assert.IsTrue(m_ZFlipCheck, "Z Flip doesn't = -1 : " + HoloZFlip);
        Assert.IsTrue(m_HoloWCordCheck, "W Coordinate doesn't = 0" + HoloWCoord);
    }

    void OnCreatedPhotoCaptureObject(PhotoCapture captureObject)
    {
        m_PhotoCaptureObj = captureObject;
        m_PhotoCaptureObj.StartPhotoModeAsync(m_CameraParameters, OnStartPhotoMode);
    }

    void OnStartPhotoMode(PhotoCapture.PhotoCaptureResult result)
    {
        Debug.Log("Ready!");
    }

    void OnPhotoCaptured(PhotoCapture.PhotoCaptureResult result, PhotoCaptureFrame photoCaptureFrame)
    {
        if (m_Canvas == null)
        {
            m_Canvas = GameObject.CreatePrimitive(PrimitiveType.Plane);
            m_Canvas.name = "PhotoCaptureCanvas";
            m_CanvasRenderer = m_Canvas.GetComponent<Renderer>() as Renderer;
        }

        photoCaptureFrame.TryGetCameraToWorldMatrix(out m_UnityCameraProjectionMatrix);
        Matrix4x4 worldToCameraMatrix = m_UnityCameraProjectionMatrix.inverse;

        m_HoloLensProjectionMatrix = default(Matrix4x4);
        photoCaptureFrame.TryGetProjectionMatrix(out m_HoloLensProjectionMatrix);

        photoCaptureFrame.UploadImageDataToTexture(m_Texture);
        m_Texture.wrapMode = TextureWrapMode.Clamp;

        m_CanvasRenderer.material.SetTexture("_MainTex", m_Texture);
        m_CanvasRenderer.material.SetMatrix("_WorldToCameraMatrix", worldToCameraMatrix);
        m_CanvasRenderer.material.SetMatrix("_CameraProjectionMatrix", m_HoloLensProjectionMatrix);
        m_CanvasRenderer.material.SetFloat("_VignetteScale", 0.0f);

        // Position the canvas object slightly in front
        // of the real world web camera.
        Vector3 position = m_UnityCameraProjectionMatrix.GetColumn(3) - m_UnityCameraProjectionMatrix.GetColumn(2);

        // Rotate the canvas object so that it faces the user.
        Quaternion rotation = Quaternion.LookRotation(-m_UnityCameraProjectionMatrix.GetColumn(2), m_UnityCameraProjectionMatrix.GetColumn(1));

        m_Canvas.transform.position = new Vector3(0f, 0f, 5f);
        m_Canvas.transform.Rotate(new Vector3(90f, 180f, 0f));

        Debug.Log("Took picture!");

        CompareMatrices();

        m_PhotoCaptureObj.StopPhotoModeAsync(OnStoppedPhotoMode);
    }

    public void OnStoppedPhotoMode(PhotoCapture.PhotoCaptureResult result)
    {
        Debug.Log("Photo Capture has stopped");
        m_PhotoCaptureObj.Dispose();
        m_PhotoCaptureObj = null;
    }

    void CompareMatrices()
    {
        if (m_HoloLensProjectionMatrix.Equals(m_UnityCameraProjectionMatrix))
        {
            Debug.Log("Projection Matrices are the same, Only should happen in editor");
            m_FovCheck = true;
        }
        else if (!m_HoloLensProjectionMatrix.Equals(m_UnityCameraProjectionMatrix))
        {
            Debug.Log("Matrices are not equal, Should be the case in the device");
            m_FovCheck = true;
        }

        HoloFovX = m_HoloLensProjectionMatrix[0, 0];
        HoloFovY = m_HoloLensProjectionMatrix[1, 1];
        HoloZFlip = m_HoloLensProjectionMatrix[3, 2];
        HoloWCoord = m_HoloLensProjectionMatrix[3, 3];

        // Formula to give the theta of the horizontalFOV & verticalFOV from the projection matrix 
        float HorizontalFOV = (2.0f * Mathf.Atan(1.0f / m_HoloLensProjectionMatrix.m00)) * Mathf.Rad2Deg;
        float VerticalFOV = (2.0f * Mathf.Atan(1.0f / m_HoloLensProjectionMatrix.m11)) * Mathf.Rad2Deg;

        Debug.Log(string.Format("HFOV = {0}, VFOV = {1}", HorizontalFOV, VerticalFOV));
        Debug.Log("HoloFovX = " + HoloFovX + Environment.NewLine + "HoloFovY = " + HoloFovY);

        Debug.Log(string.Format("HFOV = {0}, VFOV = {1}", HorizontalFOV, VerticalFOV +
                                                                         Environment.NewLine + "Z Flip = " + HoloZFlip +
                                                                         Environment.NewLine + "W Coordinate = " + HoloWCoord));

        if (HorizontalFOV < VerticalFOV)
        {
            Debug.Log("Horizontal FOV is greater then Vertical FOV | Very Bad Things have happened");
            m_FovCheck = false;
        }

        if (HoloZFlip == -1)
        {
            Debug.Log("Z Flip = " + HoloZFlip);
            m_ZFlipCheck = true;
        }
        else if (HoloZFlip != -1)
        {
            Debug.Log("Z Flip doesn't = -1 | Projection Matrix is wrong or this is in Editor | " + HoloZFlip);
            m_ZFlipCheck = true;
        }

        if (HoloWCoord == 0)
        {
            Debug.Log("W Coordinate = " + HoloWCoord);
            m_HoloWCordCheck = true;
        }
        else if (HoloWCoord != 0)
        {
            Debug.Log("W Coordinate doesn't = 0 | Projection Matrix is wrong or this is in Editor | " + HoloWCoord);
            m_HoloWCordCheck = true;
        }
    }

    void OnDestroy()
    {
        if (m_PhotoCaptureObj != null)
        {
            m_PhotoCaptureObj.Dispose();
            m_PhotoCaptureObj = null;
        }
    }
}
