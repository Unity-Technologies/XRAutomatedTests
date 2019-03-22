using UnityEngine;
using System.Collections;
using System.IO;


public class TestFloatReadPixels : MonoBehaviour
{
    public Material drawMaterial;
    public Material readMaterial;

    private RenderTexture rtRGBAFloat;
    private RenderTexture rtRGBAHalf;
    private Texture2D texRGBAFloat;
    private Texture2D texRGBAHalf;

    private const int kSize = 64;

    void Start()
    {
        var hasRTRGBAFloat = SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.ARGBFloat);
        var hasRTRGBAHalf = SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.ARGBHalf);
        var hasTexRGBAFloat = SystemInfo.SupportsTextureFormat(TextureFormat.RGBAFloat);
        var hasTexRGBAHalf = SystemInfo.SupportsTextureFormat(TextureFormat.RGBAHalf);

        if (hasRTRGBAFloat)
        {
            rtRGBAFloat = new RenderTexture(kSize, kSize, 0, RenderTextureFormat.ARGBFloat);
            rtRGBAFloat.filterMode = FilterMode.Point;
        }
        if (hasRTRGBAHalf)
        {
            rtRGBAHalf = new RenderTexture(kSize, kSize, 0, RenderTextureFormat.ARGBHalf);
            rtRGBAHalf.filterMode = FilterMode.Point;
        }

        texRGBAFloat = CreateTextureWithFormat(kSize, kSize * 2, TextureFormat.RGBAFloat);
        texRGBAHalf = CreateTextureWithFormat(kSize, kSize * 2, TextureFormat.RGBAHalf);
    }

    public void OnPostRender()
    {
        // float RT
        if (rtRGBAFloat)
        {
            Graphics.Blit(null, rtRGBAFloat, drawMaterial);

            // readback into float/half textures (top part)
            if (texRGBAFloat)
                texRGBAFloat.ReadPixels(new Rect(0, 0, kSize, kSize), 0, 0, false);
            if (texRGBAHalf)
                texRGBAHalf.ReadPixels(new Rect(0, 0, kSize, kSize), 0, 0, false);
        }
        // half RT
        if (rtRGBAHalf)
        {
            Graphics.Blit(null, rtRGBAHalf, drawMaterial);

            // readback into float/half textures (bottom part)
            if (texRGBAFloat)
                texRGBAFloat.ReadPixels(new Rect(0, 0, kSize, kSize), 0, kSize, false);
            if (texRGBAHalf)
                texRGBAHalf.ReadPixels(new Rect(0, 0, kSize, kSize), 0, kSize, false);
        }

        // display RTs and textures on screen
        RenderTexture.active = null;
        GL.LoadPixelMatrix();
        var sz = kSize + 2;
        var h = Screen.height;
        if (rtRGBAFloat)
            Graphics.DrawTexture(new Rect(0, h - sz, kSize, kSize), rtRGBAFloat, readMaterial);
        if (rtRGBAHalf)
            Graphics.DrawTexture(new Rect(sz, h - sz, kSize, kSize), rtRGBAHalf, readMaterial);
        if (texRGBAFloat)
        {
            texRGBAFloat.Apply();
            Graphics.DrawTexture(new Rect(0, h - sz * 3.5f, kSize, kSize * 2), texRGBAFloat, readMaterial);
        }
        if (texRGBAHalf)
        {
            texRGBAHalf.Apply();
            Graphics.DrawTexture(new Rect(sz, h - sz * 3.5f, kSize, kSize * 2), texRGBAHalf, readMaterial);
        }
    }

    private Texture2D CreateTextureWithFormat(int w, int h, TextureFormat fmt)
    {
        if (!SystemInfo.SupportsTextureFormat(fmt))
            return null;

        Texture2D ret = new Texture2D(w, h, fmt, false);
        ret.wrapMode = TextureWrapMode.Clamp;
        ret.filterMode = FilterMode.Point;
        ret.Apply();
        return ret;
    }

    void OnDestroy()
    {
        DestroyImmediate(rtRGBAFloat);
        DestroyImmediate(rtRGBAHalf);
        DestroyImmediate(texRGBAFloat);
        DestroyImmediate(texRGBAHalf);
    }
}
