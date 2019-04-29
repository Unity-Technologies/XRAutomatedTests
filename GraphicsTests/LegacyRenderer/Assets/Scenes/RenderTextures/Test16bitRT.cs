using UnityEngine;
using System.Collections;

public class Test16bitRT : MonoBehaviour
{
    private RenderTexture rt8888 = null;
    private RenderTexture rt4444 = null;
    private RenderTexture rt1555 = null;
    private RenderTexture rt565  = null;

    public bool useExplicitRtFormat     = false;
    public bool useExplicitRtCreation   = false;

    public Shader  testShader;
    public Texture testTexture;

    private Material InitTestMaterial(RenderTextureFormat testFormat, out RenderTexture rt, string unsupportedCaption)
    {
        Material ret = new Material(testShader);

        if (SystemInfo.SupportsRenderTextureFormat(testFormat))
        {
            if (useExplicitRtCreation)
            {
                rt = new RenderTexture(testTexture.width, testTexture.height, 0, testFormat);
                rt.Create();
            }
            else
            {
                rt = RenderTexture.GetTemporary(testTexture.width, testTexture.height, 0, testFormat);
            }
            Graphics.Blit(testTexture, rt);
            RenderTexture.active = null;

            ret.mainTexture = rt;
            ret.mainTextureScale = new Vector2(0.3f, 1.0f);

            (GameObject.Find(unsupportedCaption) as GameObject).SetActive(false);
        }
        else
        {
            rt = null;
        }


        return ret;
    }

    void Start()
    {
        Material testMat8888 = InitTestMaterial(RenderTextureFormat.ARGB32, out rt8888, "Caption8888Unsupported");
        Material testMat4444 = InitTestMaterial(useExplicitRtFormat ? RenderTextureFormat.ARGB4444 : RenderTextureFormat.Default, out rt4444, "Caption4444Unsupported");
        Material testMat1555 = InitTestMaterial(useExplicitRtFormat ? RenderTextureFormat.ARGB1555 : RenderTextureFormat.Default, out rt1555, "Caption1555Unsupported");
        Material testMat565  = InitTestMaterial(useExplicitRtFormat ? RenderTextureFormat.RGB565 : RenderTextureFormat.Default, out rt565, "Caption565Unsupported");

        GameObject.Find("Plane8888").GetComponent<MeshRenderer>().material  = testMat8888;
        GameObject.Find("Plane4444").GetComponent<MeshRenderer>().material  = testMat4444;
        GameObject.Find("Plane1555").GetComponent<MeshRenderer>().material  = testMat1555;
        GameObject.Find("Plane565").GetComponent<MeshRenderer>().material   = testMat565;
    }

    void OnDisable()
    {
        if (useExplicitRtCreation)
        {
            if (rt565 != null)
                rt565.Release();
            if (rt1555 != null)
                rt1555.Release();
            if (rt4444 != null)
                rt4444.Release();
            if (rt8888 != null)
                rt8888.Release();
        }
        else
        {
            RenderTexture.ReleaseTemporary(rt565);
            RenderTexture.ReleaseTemporary(rt1555);
            RenderTexture.ReleaseTemporary(rt4444);
            RenderTexture.ReleaseTemporary(rt8888);
        }
    }
}
