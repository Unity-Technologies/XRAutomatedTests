using UnityEngine;
using System.Collections;

public class TestCameraTargetBuffers : MonoBehaviour
{
    public Camera targetCameraCube;
    public Camera targetCameraSphere;
    public Camera targetCameraMRT;

    public GameObject cube;
    public GameObject sphere;

    private RenderTexture rtCube;
    private RenderTexture rtSphere;

    private RenderTexture[] rtMRT;
    private RenderBuffer[]  rbMRT;

    private void DrawCamera(Camera cam, GameObject obj)
    {
        cam.enabled = true;
        obj.SetActive(true);
        cam.Render();
        obj.SetActive(false);
        cam.enabled = false;
    }

    void Start()
    {
        rtSphere = new RenderTexture(256, 256, 16);
        rtSphere.Create();

        rtCube   = new RenderTexture(256, 256, 16);
        rtCube.Create();

        if (SystemInfo.supportedRenderTargetCount >= 4)
        {
            rtMRT = new RenderTexture[4];
            for (int i = 0; i < 4; ++i)
            {
                rtMRT[i] = new RenderTexture(32, 32, i == 0 ? 24 : 0, RenderTextureFormat.ARGB32);
                rtMRT[i].Create();
            }

            rbMRT = new RenderBuffer[4];
            for (int i = 0; i < 4; ++i)
                rbMRT[i] = rtMRT[i].colorBuffer;
        }

        targetCameraSphere.enabled = false;
        targetCameraCube.enabled = false;


        if (SystemInfo.supportedRenderTargetCount >= 4)
        {
            targetCameraMRT.enabled = true;
            targetCameraMRT.SetTargetBuffers(rbMRT, rtMRT[0].depthBuffer);
        }
    }

    void OnPostRender()
    {
        RenderTexture.active = rtCube;
        GL.Clear(true, true, new Color(0, 0, 0, 1));
        RenderTexture.active = null;

        targetCameraSphere.targetTexture = rtSphere;
        DrawCamera(targetCameraSphere, sphere);

        targetCameraCube.SetTargetBuffers(rtCube.colorBuffer, rtSphere.depthBuffer);
        DrawCamera(targetCameraCube, cube);


        GL.PushMatrix();
        GL.LoadPixelMatrix();

        Graphics.DrawTexture(new Rect(0, Screen.height - 256, 256, 256), rtCube);

        if (SystemInfo.supportedRenderTargetCount >= 4)
        {
            for (int i = 0; i < 4; ++i)
                Graphics.DrawTexture(new Rect(268 + i * 32, 100, 32, 32), rtMRT[i]);
        }

        GL.PopMatrix();
    }
}
