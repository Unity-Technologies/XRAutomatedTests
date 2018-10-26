using UnityEngine;
using UnityEngine.Rendering;

public class RenderToRTMips : MonoBehaviour
{
    public Renderer plane;
    private RenderTexture rt;
    public Renderer objManual;
    private RenderTexture rtManual;
    public Renderer objCube;
    private RenderTexture rtCube;
    public Renderer objCubeMip;
    private RenderTexture rtCubeMip;

    public void Start()
    {
        rt = new RenderTexture(256, 256, 16);
        rt.useMipMap = true;
        rt.mipMapBias = 3;
        //rt.filterMode = FilterMode.Trilinear;
        rt.wrapMode = TextureWrapMode.Clamp;
        GetComponent<Camera>().targetTexture = rt;
        plane.material.mainTexture = rt;
        rtManual = new RenderTexture(128, 128, 0);
        rtManual.useMipMap = true;
        rtManual.autoGenerateMips = false;
        objManual.material.mainTexture = rtManual;
        if (SystemInfo.supportsRenderToCubemap)
        {
            rtCube = new RenderTexture(64, 64, 16);
            rtCube.dimension = TextureDimension.Cube;
            objCube.material.SetTexture("_MainTex", rtCube);
            rtCubeMip = new RenderTexture(64, 64, 0);
            rtCubeMip.dimension = TextureDimension.Cube;
            rtCubeMip.useMipMap = true;
            rtCubeMip.autoGenerateMips = false;
            objCubeMip.material.SetTexture("_MainTex", rtCubeMip);
        }
    }

    public void Update()
    {
        ClearCubeRT(rtCube);
        ClearCubeRT(rtCubeMip);
        var mipCount = (int)Mathf.Log(rtManual.width, 2f);
        var i = 0;
        while (i < mipCount)
        {
            Graphics.SetRenderTarget(rtManual, i);
            GL.Clear(true, true, new Color(i * 0.3f, 0.5f - (i * 0.1f), 1f - (i * 0.3f), 0));
            ++i;
        }
    }

    private static void ClearCubeRT(RenderTexture cube)
    {
        if (!SystemInfo.supportsRenderToCubemap)
            return;

        // clear faces of a cubemap RT to different colors
        var mipCount = (int)Mathf.Log(cube.width, 2f);
        if (!cube.useMipMap)
        {
            mipCount = 1;
        }
        var i = 0;
        while (i < mipCount)
        {
            var ff = 1f - (i * 0.2f);
            var gg = 0.5f - (i * 0.1f);
            Graphics.SetRenderTarget(cube, i, CubemapFace.PositiveX);
            GL.Clear(true, true, new Color(ff, 0, 0, 0));
            Graphics.SetRenderTarget(cube, i, CubemapFace.NegativeX);
            GL.Clear(true, true, new Color(gg, 0, 0, 0));
            Graphics.SetRenderTarget(cube, i, CubemapFace.PositiveY);
            GL.Clear(true, true, new Color(0, ff, 0, 0));
            Graphics.SetRenderTarget(cube, i, CubemapFace.NegativeY);
            GL.Clear(true, true, new Color(0, gg, 0, 0));
            Graphics.SetRenderTarget(cube, i, CubemapFace.PositiveZ);
            GL.Clear(true, true, new Color(0, 0, ff, 0));
            Graphics.SetRenderTarget(cube, i, CubemapFace.NegativeZ);
            GL.Clear(true, true, new Color(0, 0, gg, 0));
            ++i;
        }
    }
}
