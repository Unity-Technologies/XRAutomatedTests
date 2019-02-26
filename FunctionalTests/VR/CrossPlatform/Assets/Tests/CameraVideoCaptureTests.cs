using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using System;
using System.Linq;

#if UNITY_METRO

#endif

[Ignore("Test is not failing for a unknown reason")]
internal class CameraVideoCaptureTests : TestBaseSetup
{
#if UNITY_METRO
    UnityEngine.Windows.WebCam.VideoCapture m_VideoCapture = null;
    UnityEngine.Windows.WebCam.VideoCapture.VideoCaptureResult m_Result;


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
            UnityEngine.Windows.WebCam.VideoCapture.SupportedResolutions.OrderByDescending((res) => res.width * res.height).First();
        Debug.Log(cameraResolution);

        // Set the FPS by picking the first resolution from the supported list
        float cameraFramerate =
            UnityEngine.Windows.WebCam.VideoCapture.GetSupportedFrameRatesForResolution(cameraResolution).OrderByDescending((fps) => fps).First();
        Debug.Log("Camera fps: " + cameraFramerate);

        UnityEngine.Windows.WebCam.VideoCapture.CreateAsync(false, delegate (UnityEngine.Windows.WebCam.VideoCapture videoCapture)
        {
            if (videoCapture != null)
            {
                m_VideoCapture = videoCapture;
                Debug.Log("Created VideoCapture Instance!");

                UnityEngine.Windows.WebCam.CameraParameters cameraParameters = new UnityEngine.Windows.WebCam.CameraParameters(UnityEngine.Windows.WebCam.WebCamMode.VideoMode);
                Debug.Log("Initial WebCam Mode = " + UnityEngine.Windows.WebCam.WebCam.Mode);
                cameraParameters.hologramOpacity = 1f;
                cameraParameters.frameRate = cameraFramerate;
                cameraParameters.cameraResolutionWidth = cameraResolution.width;
                cameraParameters.cameraResolutionHeight = cameraResolution.height;
                cameraParameters.pixelFormat = UnityEngine.Windows.WebCam.CapturePixelFormat.BGRA32;

                m_VideoCapture.StartVideoModeAsync(cameraParameters,
                    UnityEngine.Windows.WebCam.VideoCapture.AudioState.ApplicationAndMicAudio,
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
    void OnStartedVideoCaptureMode(UnityEngine.Windows.WebCam.VideoCapture.VideoCaptureResult result)
    {
        Debug.Log("Started Video Capture Mode!");
        Debug.Log(UnityEngine.Windows.WebCam.WebCam.Mode);
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

    void OnStartedRecordingVideo(UnityEngine.Windows.WebCam.VideoCapture.VideoCaptureResult result)
    {
        Debug.Log("Started Recording Video!");
    }

    void OnStoppedRecordingVideo(UnityEngine.Windows.WebCam.VideoCapture.VideoCaptureResult result)
    {
        Debug.Log("Stopped Recording Video!");
        m_VideoCapture.StopVideoModeAsync(OnStoppedVideoCaptureMode);
    }

    void OnStoppedVideoCaptureMode(UnityEngine.Windows.WebCam.VideoCapture.VideoCaptureResult result)
    {
        Debug.Log("Stopped Video Capture Mode!");
        Debug.Log(UnityEngine.Windows.WebCam.WebCam.Mode);

        m_VideoCapture.Dispose();
        m_VideoCapture = null;
    }
#endif
}
