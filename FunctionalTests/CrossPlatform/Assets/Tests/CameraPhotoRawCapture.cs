using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using UnityEngine.XR.WSA.WebCam;
using System.Linq;
using System.Collections.Generic;

internal class CameraPhotoRawCapture : TestBaseSetup
{
    PhotoCapture m_PhotoCaptureObject = null;
    Texture2D m_TargetTexture = null;
    Renderer m_QuadRenderer = null;

    private PhotoCapture.PhotoCaptureResult m_Result;

    [TearDown]
    public void TearDown()
    {
        m_TargetTexture = null;
        m_PhotoCaptureObject = null;
        GameObject.Destroy(m_QuadRenderer);
    }

	// A UnityTest behaves like a coroutine in PlayMode
	// and allows you to yield null to skip a frame in EditMode
	[UnityTest]
	public IEnumerator CapturePictureDisplayRawImage()
    {
        WmrDeviceCheck();
        GameObject.Destroy(m_Cube);

        PhotoCaptureCreate();

        yield return new WaitForSeconds(3f);

        m_PhotoCaptureObject.StopPhotoModeAsync(OnStoppedPhotoMode);

        yield return new WaitForSeconds(3f);

        Assert.IsTrue(m_Result.success, "Failed to capture photo");
        
	}

    public void OnCapturedPhotoToMemory(PhotoCapture.PhotoCaptureResult result, PhotoCaptureFrame photoCaptureFrame)
    {
        List<byte> imageBufferList = new List<byte>();
        // Copy the raw IMFMediaBuffer data into our empty byte list.
        photoCaptureFrame.CopyRawImageDataIntoBuffer(imageBufferList);

        // Captured the image using the BGRA32 format.
        // So our stride will be 4 since we have a byte for each RGBA channel.
        // The raw image data will also be flipped so we access our pixel data
        // in the reverse order.
        int stride = 4;
        float denominator = 1.0f / 255.0f;
        List<Color> colorArray = new List<Color>();
        for (int i = imageBufferList.Count - 1; i >= 0; i -= stride)
        {
            float a = (int)(imageBufferList[i - 0]) * denominator;
            float r = (int)(imageBufferList[i - 1]) * denominator;
            float g = (int)(imageBufferList[i - 2]) * denominator;
            float b = (int)(imageBufferList[i - 3]) * denominator;

            colorArray.Add(new Color(r, g, b, a));
        }

        m_TargetTexture.SetPixels(colorArray.ToArray());
        m_TargetTexture.Apply();

        if (m_QuadRenderer == null)
        {
            GameObject p = GameObject.CreatePrimitive(PrimitiveType.Quad);
            m_QuadRenderer = p.GetComponent<Renderer>() as Renderer;
            p.transform.localPosition = new Vector3(0.0f, 0.0f, 6.0f);
        }

        m_QuadRenderer.material.SetTexture("_MainTex", m_TargetTexture);
    }

    public void OnStoppedPhotoMode(PhotoCapture.PhotoCaptureResult result)
    {
        Debug.Log("Captured images have been saved at the following path.");
        Debug.Log(Application.persistentDataPath);

        m_PhotoCaptureObject.Dispose();
    }

    void PhotoCaptureCreate()
    {
        Resolution cameraResolution = PhotoCapture.SupportedResolutions.OrderByDescending((res) => res.width * res.height).First();

        m_TargetTexture = new Texture2D(cameraResolution.width, cameraResolution.height, TextureFormat.RGBA32, false);

        PhotoCapture.CreateAsync(false, delegate (PhotoCapture captureObject)
        {
            Debug.Log("Created PhotoCapture Object");
            m_PhotoCaptureObject = captureObject;

            CameraParameters c = new CameraParameters();
            c.cameraResolutionWidth = m_TargetTexture.width;
            c.cameraResolutionHeight = m_TargetTexture.height;
            c.pixelFormat = CapturePixelFormat.BGRA32;

            captureObject.StartPhotoModeAsync(c, delegate (PhotoCapture.PhotoCaptureResult result)
            {
                Debug.Log("Started Photo Capture Mode");
                m_PhotoCaptureObject.TakePhotoAsync(this.OnCapturedPhotoToMemory);
            });
        });
    }
}
