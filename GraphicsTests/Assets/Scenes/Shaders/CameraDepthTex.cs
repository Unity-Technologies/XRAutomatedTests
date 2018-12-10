using UnityEngine;

public class CameraDepthTex : MonoBehaviour
{
    public Shader shader;
    public DepthTextureMode mode;
    private Material material;

    public void Start()
    {
        if (!shader.isSupported)
        {
            enabled = false;
            return;
        }
        material = new Material(shader);
        material.hideFlags = HideFlags.HideAndDontSave;
        GetComponent<Camera>().depthTextureMode = mode;
    }

    public void OnDisable()
    {
        if (material)
        {
            DestroyImmediate(material);
        }
    }

    public void OnRenderImage(RenderTexture source, RenderTexture dest)
    {
        if (UnityEngine.XR.XRSettings.stereoRenderingMode == UnityEngine.XR.XRSettings.StereoRenderingMode.SinglePassInstanced)
        {
            material.SetFloat("_Slice", 0.0f);
            Graphics.Blit(source, dest, material, -1, 0);
            material.SetFloat("_Slice", 1.0f);
            Graphics.Blit(source, dest, material, -1, 1);
        }
        else
        {
            Graphics.Blit(source, dest, material);
        }
    }

    public CameraDepthTex()
    {
        mode = DepthTextureMode.Depth;
    }
}
