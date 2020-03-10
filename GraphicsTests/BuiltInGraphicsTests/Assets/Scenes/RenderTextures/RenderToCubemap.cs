using UnityEngine;
using UnityEngine.Rendering;

public class RenderToCubemap : MonoBehaviour
{
    public Renderer debugSphere;
    public bool mips;
    public int cullingMask;
    public bool msaa;
    private Camera cam;
    private RenderTexture rtex;

    public void Start()
    {
        var go = new GameObject("CubemapCamera", typeof(Camera));
        go.transform.position = transform.position;
        go.transform.rotation = Quaternion.identity;
        cam = go.GetComponent<Camera>();
        cam.enabled = false;
        cam.cullingMask = cullingMask;
        // Set camera to use deferred lighting. This is not supported and Unity
        // should fallback to Forward with no errors happening.
        cam.renderingPath = RenderingPath.DeferredLighting;

        rtex = new RenderTexture(128, 128, 16);
        rtex.dimension = TextureDimension.Cube;
        rtex.useMipMap = mips;
        rtex.name = "____CubemapRT";
        if (msaa)
        {
            rtex.antiAliasing = 8;
        }
        // clear to black
        var f = 0;
        while (f < 6)
        {
            Graphics.SetRenderTarget(rtex, 0, (CubemapFace)f);
            GL.Clear(false, true, Color.black);
            ++f;
        }
        //cam.targetTexture = rtex;
        //go.AddComponent("BlurEffect");
        GetComponent<Renderer>().material.SetTexture("_Cube", rtex);
        if (debugSphere)
        {
            debugSphere.material.SetTexture("_MainTex", rtex);
        }
    }

    public void Update()
    {
        // make this object itself not render into the cubemap
        var oldLayer = gameObject.layer;
        gameObject.layer = 2;
        cam.RenderToCubemap(rtex);
        gameObject.layer = oldLayer;
        RenderTexture.active = null;
    }

    public RenderToCubemap()
    {
        cullingMask = 1;
    }
}
