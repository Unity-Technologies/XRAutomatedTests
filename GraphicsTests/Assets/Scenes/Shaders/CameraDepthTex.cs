using UnityEngine;

public class CameraDepthTex : MonoBehaviour
{
    public Shader shader;
    public DepthTextureMode mode;
    private Material material;

    public void Start()
    {
        if (!SystemInfo.supportsImageEffects)
        {
            enabled = false;
            return;
        }
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
        Graphics.Blit(source, dest, material);
    }

    public CameraDepthTex()
    {
        mode = DepthTextureMode.Depth;
    }
}
