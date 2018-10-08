using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using System;
using System.Linq;

#if ENABLE_HOLOLENS_MODULE
using UnityEngine.XR.WSA.WebCam;
#endif

[Ignore("Test is not failing for a unknown reason")]
internal class CameraVideoCaptureTests : TestBaseSetup
{
#if ENABLE_HOLOLENS_MODULE
    VideoCapture m_VideoCapture = null;
    VideoCapture.VideoCaptureResult m_Result;


    [UnityTest]
	public IEnumerator CaptureVideoTests()
    {
        WmrDeviceCheck();
        StartVideoCaptureTest();
        yield return new WaitForSeconds(5f);
        StopVideoCaptureTest();
        yield return new WaitForSeconds(5f);

        Assert.IsTrue(m_Result.success, "Failed to capture Video");
    }

    // Starts and Sets up the video capture 
    void StartVideoCaptureTest()
    {
        // Set the camera resolution by picking the first resolution from the supported list
        Resolution cameraResolution =
            VideoCapture.SupportedResolutions.OrderByDescending((res) => res.width * res.height).First();
        Debug.Log(cameraResolution);

        // Set the FPS by picking the first resolution from the supported list
        float cameraFramerate =
            VideoCapture.GetSupportedFrameRatesForResolution(cameraResolution).OrderByDescending((fps) => fps).First();
        Debug.Log("Camera fps: " + cameraFramerate);

        VideoCapture.CreateAsync(false, delegate (VideoCapture videoCapture)
        {
            if (videoCapture != null)
            {
                m_VideoCapture = videoCapture;
                Debug.Log("Created VideoCapture Instance!");

                CameraParameters cameraParameters = new CameraParameters(WebCamMode.VideoMode);
                Debug.Log("Initial WebCam Mode = " + WebCam.Mode);
                cameraParameters.hologramOpacity = 1f;
                cameraParameters.frameRate = cameraFramerate;
                cameraParameters.cameraResolutionWidth = cameraResolution.width;
                cameraParameters.cameraResolutionHeight = cameraResolution.height;
                cameraParameters.pixelFormat = CapturePixelFormat.BGRA32;

                m_VideoCapture.StartVideoModeAsync(cameraParameters,
                    VideoCapture.AudioState.ApplicationAndMicAudio,
                    OnStartedVideoCaptureMode);
            }
            else
            {
                Debug.LogError("Failed to create VideoCapture Instance!");
            }
        });
    }

    void StopVideoCaptureTest()
    {
        m_VideoCapture.StopRecordingAsync(OnStoppedRecordingVideo);
    }

    //
    // Start of the Delegate calls for Video Capture
    //
    void OnStartedVideoCaptureMode(VideoCapture.VideoCaptureResult result)
    {
        Debug.Log("Started Video Capture Mode!");
        Debug.Log(WebCam.Mode);
        string timeStamp = Time.time.ToString().Replace(".", "");
        string fileNameVideo = string.Format("TestVideo_{0}.mp4", timeStamp);
        string filePathVideo = System.IO.Path.Combine(Application.persistentDataPath, fileNameVideo);
        Debug.Log("Video Filepath : " + filePathVideo);
        m_VideoCapture.StartRecordingAsync(filePathVideo, OnStartedRecordingVideo);

        if (!result.success)
        {
            Debug.Log("Bad State: " + Environment.NewLine + result.hResult);
        }
    }

    void OnStartedRecordingVideo(VideoCapture.VideoCaptureResult result)
    {
        Debug.Log("Started Recording Video!");
    }

    void OnStoppedRecordingVideo(VideoCapture.VideoCaptureResult result)
    {
        Debug.Log("Stopped Recording Video!");
        m_VideoCapture.StopVideoModeAsync(OnStoppedVideoCaptureMode);
    }

    void OnStoppedVideoCaptureMode(VideoCapture.VideoCaptureResult result)
    {
        Debug.Log("Stopped Video Capture Mode!");
        Debug.Log(WebCam.Mode);

        m_VideoCapture.Dispose();
        m_VideoCapture = null;
    }
#endif
}
