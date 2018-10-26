using UnityEngine;
using System.Collections;
using System.IO;

public class TestFPRT : MonoBehaviour
{
    public Material drawMaterial;
    public Material readMaterial;

    // we will draw into temp RT with selected format and then blit into result RGBA8 RT
    // when drawing we will forcibly output large values
    private RenderTexture   resultRT;

    // normal Textures will repeat the process with RT, but instead of rendering into we will manually fill the data
    private Texture2D[]     texRGBA             = new Texture2D[3];
    private Texture2D[]     texRG               = new Texture2D[3];
    private Texture2D[]     texR                = new Texture2D[3];

    private const float     gradientMultNormal  = 8.0f;
    private const float     gradientMultLarge   = 300000f;

    private const int       testTexExt          = 32;


    public Texture2D testTexture = null;


    void Start()
    {
        resultRT = new RenderTexture(testTexExt, testTexExt, 0);
        resultRT.Create();

        texRGBA[0]  = CreateTextureWithFormat(testTexExt, testTexExt, gradientMultNormal, TextureFormat.RGBAHalf);
        texRGBA[1]  = CreateTextureWithFormat(testTexExt, testTexExt, gradientMultLarge, TextureFormat.RGBAHalf);
        texRGBA[2]  = CreateTextureWithFormat(testTexExt, testTexExt, gradientMultLarge, TextureFormat.RGBAFloat);

        texRG[0]    = CreateTextureWithFormat(testTexExt, testTexExt, gradientMultNormal, TextureFormat.RGHalf);
        texRG[1]    = CreateTextureWithFormat(testTexExt, testTexExt, gradientMultLarge, TextureFormat.RGHalf);
        texRG[2]    = CreateTextureWithFormat(testTexExt, testTexExt, gradientMultLarge, TextureFormat.RGFloat);

        texR[0]     = CreateTextureWithFormat(testTexExt, testTexExt, gradientMultNormal, TextureFormat.RHalf);
        texR[1]     = CreateTextureWithFormat(testTexExt, testTexExt, gradientMultLarge, TextureFormat.RHalf);
        texR[2]     = CreateTextureWithFormat(testTexExt, testTexExt, gradientMultLarge, TextureFormat.RFloat);
    }

    private void DrawTexture(float xoffset, float yoffset, float gradientVal, Texture tex)
    {
        if (tex == null)
            return;

        readMaterial.SetFloat("_GradientMult", gradientVal);
        Graphics.Blit(tex, resultRT, readMaterial);

        RenderTexture.active = null;
        GUI.DrawTexture(new Rect(xoffset, yoffset, testTexExt, testTexExt), resultRT, ScaleMode.StretchToFill, false);
    }

    private void DrawRTWithFormat(float xoffset, float yoffset, float gradientVal, RenderTextureFormat fmt)
    {
        if (!SystemInfo.SupportsRenderTextureFormat(fmt))
            return;

        RenderTexture rt = RenderTexture.GetTemporary(testTexExt, testTexExt, 16, fmt, RenderTextureReadWrite.Linear);
        rt.filterMode = FilterMode.Point;

        drawMaterial.SetFloat("_GradientMult", gradientVal);
        Graphics.Blit(null, rt, drawMaterial);

        DrawTexture(xoffset, yoffset, gradientVal, rt);
        RenderTexture.ReleaseTemporary(rt);
    }

    //ushort


    private byte[]      GenerateTextureData(int w, int h, float gradientVal, TextureFormat fmt)
    {
        int     compCount   = 0;
        if (fmt == TextureFormat.RFloat || fmt == TextureFormat.RHalf)               compCount = 1;
        else if (fmt == TextureFormat.RGFloat || fmt == TextureFormat.RGHalf)        compCount = 2;
        else if (fmt == TextureFormat.RGBAFloat || fmt == TextureFormat.RGBAHalf)    compCount = 4;

        bool    isFloat     = fmt == TextureFormat.RFloat || fmt == TextureFormat.RGFloat || fmt == TextureFormat.RGBAFloat;

        using (MemoryStream stream = new MemoryStream())
            using (BinaryWriter writer = new BinaryWriter(stream))
            {
                // we do it in non-efficient way for clarity
                for (int y = 0; y < h; ++y)
                {
                    for (int x = 0; x < w; ++x)
                    {
                        float val = gradientVal * ((float)x / (float)w - 0.5f);
                        for (int c = 0; c < compCount; ++c)
                        {
                            if (isFloat) writer.Write(val);
                            else        writer.Write(Mathf.FloatToHalf(val));
                            val *= 0.7f;
                        }
                    }
                }

                return stream.ToArray();
            }
    }

    private Color[]     GenerateTextureDataColor(int w, int h, float gradientVal, TextureFormat fmt)
    {
        Color[] ret = new Color[w * h];
        for (int y = 0; y < h; ++y)
        {
            for (int x = 0; x < w; ++x)
            {
                float val = gradientVal * ((float)x / (float)w - 0.5f);
                ret[y * w + x] = new Color(val, val * 0.7f, val * 0.7f * 0.7f, val);
            }
        }
        return ret;
    }

    private Texture2D   CreateTextureWithFormat(int w, int h, float gradientVal, TextureFormat fmt)
    {
        if (!SystemInfo.SupportsTextureFormat(fmt))
            return null;

        Texture2D ret = new Texture2D(w, h, fmt, false);
        ret.wrapMode = TextureWrapMode.Clamp;
        ret.filterMode = FilterMode.Point;

        //ret.LoadRawTextureData(GenerateTextureData(w,h,gradientVal,fmt));
        ret.SetPixels(0, 0, w, h, GenerateTextureDataColor(w, h, gradientVal, fmt));
        ret.Apply();
        return ret;
    }

    void OnGUI()
    {
        DrawRTWithFormat(0 * testTexExt + 2,   0 * testTexExt + 2, gradientMultNormal, RenderTextureFormat.ARGB32);
        DrawRTWithFormat(1 * testTexExt + 4,   0 * testTexExt + 2, gradientMultNormal, RenderTextureFormat.ARGBHalf);
        DrawRTWithFormat(2 * testTexExt + 6,   0 * testTexExt + 2, gradientMultLarge,  RenderTextureFormat.ARGBHalf);
        DrawRTWithFormat(3 * testTexExt + 8,   0 * testTexExt + 2, gradientMultLarge,  RenderTextureFormat.ARGBFloat);
        DrawTexture(4 * testTexExt + 10,  0 * testTexExt + 2, gradientMultNormal, texRGBA[0]);
        DrawTexture(5 * testTexExt + 12,  0 * testTexExt + 2, gradientMultLarge,  texRGBA[1]);
        DrawTexture(6 * testTexExt + 14,  0 * testTexExt + 2, gradientMultLarge,  texRGBA[2]);

        DrawRTWithFormat(1 * testTexExt + 4,   1 * testTexExt + 4, gradientMultNormal, RenderTextureFormat.RGHalf);
        DrawRTWithFormat(2 * testTexExt + 6,   1 * testTexExt + 4, gradientMultLarge,  RenderTextureFormat.RGHalf);
        DrawRTWithFormat(3 * testTexExt + 8,   1 * testTexExt + 4, gradientMultLarge,  RenderTextureFormat.RGFloat);
        DrawTexture(4 * testTexExt + 10,  1 * testTexExt + 4, gradientMultNormal, texRG[0]);
        DrawTexture(5 * testTexExt + 12,  1 * testTexExt + 4, gradientMultLarge,  texRG[1]);
        DrawTexture(6 * testTexExt + 14,  1 * testTexExt + 4, gradientMultLarge,  texRG[2]);

        DrawRTWithFormat(0 * testTexExt + 2,   2 * testTexExt + 6, gradientMultNormal, RenderTextureFormat.R8);
        DrawRTWithFormat(1 * testTexExt + 4,   2 * testTexExt + 6, gradientMultNormal, RenderTextureFormat.RHalf);
        DrawRTWithFormat(2 * testTexExt + 6,   2 * testTexExt + 6, gradientMultLarge,  RenderTextureFormat.RHalf);
        DrawRTWithFormat(3 * testTexExt + 8,   2 * testTexExt + 6, gradientMultLarge,  RenderTextureFormat.RFloat);
        DrawTexture(4 * testTexExt + 10,  2 * testTexExt + 6, gradientMultNormal, texR[0]);
        DrawTexture(5 * testTexExt + 12,  2 * testTexExt + 6, gradientMultLarge,  texR[1]);
        DrawTexture(6 * testTexExt + 14,  2 * testTexExt + 6, gradientMultLarge,  texR[2]);

        DrawRTWithFormat(1 * testTexExt + 20,   4 * testTexExt + 0, gradientMultNormal, RenderTextureFormat.RGB111110Float);
        DrawRTWithFormat(2 * testTexExt + 22,   4 * testTexExt + 0, gradientMultLarge,  RenderTextureFormat.RGB111110Float);
    }

    void OnDisable()
    {
        resultRT.Release();
    }
}
