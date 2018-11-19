using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using System;
using System.Linq;

#if UNITY_METRO
using UnityEngine.XR.WSA.WebCam;
#endif

internal class CameraPhotoCaptureTests : TestBaseSetup
{
#if UNITY_METRO
    PhotoCapture m_PhotoCaptureObject = null;
    private Texture2D m_TargetTexture = null;
    // Projection Matrix for the HoloLens Camera and the Unity Camera
    private Matrix4x4 m_HoloLensProjectionMatrix;
    private Matrix4x4 m_UnityCameraProjectionMatrix;
    CameraParameters m_PhotoParameters;
    // The plane created to view the image captured from the Web Camera
    private GameObject m_ImagePlane = null;
    PhotoCapture.PhotoCaptureResult m_Result;

    [SetUp]
    public void SetUp()
    {
        // Creating the plane for the Picture Texture to be seen by the user
        m_ImagePlane = GameObject.CreatePrimitive(PrimitiveType.Plane);
        // Placing the ImagePlane in front of the user and about 2.5m in front of the camera
        m_ImagePlane.transform.localPosition = new Vector3(0f, 0f, 5f);
        // Rotates the ImagePlane so the user can see it
        m_ImagePlane.transform.Rotate(new Vector3(90f, 90f, -90f));
        // Scale the ImagePlane for better visibility
        m_ImagePlane.transform.localScale = new Vector3(.1f, .1f, .1f);
    }

    [TearDown]
    public void TearDown()
    {
        GameObject.Destroy(m_TargetTexture = null);
        GameObject.Destroy(m_ImagePlane);
    }

	[UnityTest]
	public IEnumerator CapturePictureFromMemory()
	{
	    WmrDeviceCheck();
	    GameObject.Destroy(m_Cube);

        TakeThePicture();
	    m_HoloLensProjectionMatrix = default(Matrix4x4);

	    yield return new WaitForSeconds(5f);

        m_PhotoCaptureObject.StopPhotoModeAsync(OnStoppedPhotoMode);

        Debug.Log("Destroyed the Photo Capture Object");

        yield return new WaitForSeconds(3f);

        Assert.IsTrue(m_Result.success, "Didn't capture the photo");
    }

    //
    // Start of the delegate Calls
    //

    void OnCapturedPhotoToMemory(PhotoCapture.PhotoCaptureResult result, PhotoCaptureFrame photoCaptureFrame)
    {
        // Copy the raw image data into our target texture
        photoCaptureFrame.UploadImageDataToTexture(m_TargetTexture);
#if !UNITY_EDITOR
        if (!photoCaptureFrame.TryGetProjectionMatrix(Camera.main.nearClipPlane, Camera.main.farClipPlane, out m_HoloLensProjectionMatrix))
        {
            throw new Exception("*****Error : Failed to get projection matrix");
        }
#endif
        m_UnityCameraProjectionMatrix = m_TestSetupHelpers.m_Camera.GetComponent<Camera>().projectionMatrix;

        CompareMatrices();

        Renderer PlaneRenderer = m_ImagePlane.GetComponent<Renderer>() as Renderer;
        PlaneRenderer.material.SetTexture("_MainTex", m_TargetTexture);
    }

    void OnStoppedPhotoMode(PhotoCapture.PhotoCaptureResult result)
    {
        // Shutdown the photo capture object and check result.
        if (result.success)
        {
            Debug.Log("Success taking photo!");
        }
        else
        {
            Debug.Log("*** ERROR : OnStoppedPhotoMode - " + result.hResult);
        }

        m_PhotoCaptureObject.Dispose();
        m_PhotoCaptureObject = null;
    }

    //
    // End of the Delegate Calls
    //
    void CompareMatrices()
    {
        if (m_HoloLensProjectionMatrix.Equals(m_UnityCameraProjectionMatrix))
        {
            Debug.Log("Projection Matrices are the same, Only should happen in editor");
        }
        else if (!m_HoloLensProjectionMatrix.Equals(m_UnityCameraProjectionMatrix))
        {
            Debug.Log("Matrices are not equal, Should be the case in the device");
        }

        var HoloFovX = m_HoloLensProjectionMatrix[0, 0];
        var HoloFovY = m_HoloLensProjectionMatrix[1, 1];
        var HoloZFlip = m_HoloLensProjectionMatrix[3, 2];
        var HoloWCoord = m_HoloLensProjectionMatrix[3, 3];

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
        }

        if (HoloZFlip == -1)
        {
            Debug.Log("Z Flip = " + HoloZFlip);
        }
        else if (HoloZFlip != -1)
        {
            Debug.Log("Z Flip doesn't = -1 | Bad Things | " + HoloZFlip);
        }

        if (HoloWCoord == 0)
        {
            Debug.Log("W Coordinate = " + HoloWCoord);
        }
        else if (HoloWCoord != 0)
        {
            Debug.Log("W Coordinate doesn't = 0 | Bad Things | " + HoloWCoord);
        }
    }

    public void SetCameraParameters(float HologramOpacity, Resolution CameraResolution, CapturePixelFormat PixelFormat)
    {
        m_PhotoParameters = new CameraParameters();
        m_PhotoParameters.hologramOpacity = HologramOpacity;
        m_PhotoParameters.cameraResolutionWidth = CameraResolution.width;
        m_PhotoParameters.cameraResolutionHeight = CameraResolution.height;
        m_PhotoParameters.pixelFormat = PixelFormat;
        m_PhotoParameters.frameRate = 0f; // not actually used here
    }

    // Creates the Camera then takes a picture based on the camera parameters set
    public void TakeThePicture()
    {
        // Grab and set the resolution to the first in the list of supported resolutions
        Resolution cameraResolution = PhotoCapture.SupportedResolutions.OrderByDescending((res) => res.width * res.height).First();
        m_TargetTexture = new Texture2D(cameraResolution.width, cameraResolution.height);

        Debug.Log("Created the camera resolution: " + cameraResolution);

        PhotoCapture.CreateAsync(true, delegate (PhotoCapture captureObject)
        {
            this.m_PhotoCaptureObject = captureObject;

            SetCameraParameters(1f, cameraResolution, CapturePixelFormat.BGRA32);
            Debug.Log("camera resolution: " + cameraResolution +
                                                   Environment.NewLine + "Holo Opacity: " + 1f +
                                                   Environment.NewLine + "Format:  BGRA32");

            captureObject.StartPhotoModeAsync(m_PhotoParameters, delegate (PhotoCapture.PhotoCaptureResult result)
            {
                m_PhotoCaptureObject.TakePhotoAsync(OnCapturedPhotoToMemory);
                Debug.Log("Taking the Picture");
            });
        });
    }

    public IEnumerator StopCaptureObject()
    {
        m_PhotoCaptureObject.Dispose();
        yield return new WaitForSeconds(3f);
        m_PhotoCaptureObject = null;
    }
#endif
}
