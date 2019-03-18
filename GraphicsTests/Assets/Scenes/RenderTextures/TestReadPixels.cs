using UnityEngine;
using UnityEngine.UI;

public class TestReadPixels : MonoBehaviour
{
    private Texture2D tex1;
    private Texture2D tex2;
    private Texture2D tex3;
    private RenderTexture rt;
    private Camera cam;
    public RawImage  @object;
    public RawImage object2;
    public RawImage object3;
    public Texture tex;
    public Shader shader;
    private Material mat;
    public bool hdr;

    public void Start()
    {
        var texFmt = hdr ? TextureFormat.RGBAFloat : TextureFormat.ARGB32;
        var rtFmt = hdr ? RenderTextureFormat.ARGBFloat : RenderTextureFormat.Default;
        tex1 = new Texture2D(256, 256, texFmt, true);
        tex2 = new Texture2D(256, 64, texFmt, true);
        tex3 = new Texture2D(64, 64, texFmt, true);
        rt = new RenderTexture(64, 64, 16, rtFmt);
        var go = new GameObject("Foo", typeof(Camera));
        cam = go.GetComponent<Camera>();
        cam.CopyFrom(GetComponent<Camera>());
        cam.clearFlags = CameraClearFlags.Color;
        cam.enabled = false;
        cam.targetTexture = rt;
        cam.allowHDR = hdr;
        mat = new Material(shader);
        mat.hideFlags = HideFlags.HideAndDontSave;
    }

    public void Update()
    {
        if (Time.frameCount > 2)
        {
            object3.enabled = false;
            cam.Render();
            object3.enabled = true;
            RenderTexture.active = rt;
            Graphics.Blit(tex, rt, mat);
            tex3.ReadPixels(new Rect(0, 0, rt.width / 2, rt.height), 0, 0, false);
            tex3.ReadPixels(new Rect(rt.width / 2, 0, rt.width / 2, rt.height), rt.width / 2, 0, false);
            tex3.Apply();
            object3.texture = tex3;
            RenderTexture.active = null;
        }
    }

    public void OnPostRender()
    {
        if (Time.frameCount > 2)
        {
            tex1.ReadPixels(new Rect(0, Screen.height - 256, 128, 256), 0, 0, true);
            tex1.ReadPixels(new Rect(128, Screen.height - 256, 128, 256), 128, 0, true);
            tex1.Apply();
            @object.texture = tex1;
            var i = 0;
            while (i < 4)
            {
                tex2.ReadPixels(new Rect(64, (Screen.height - 256) + (i * 64), 64, 64), i * 64, 0, false);
                ++i;
            }
            tex2.Apply();
            object2.texture = tex2;
        }
    }
}
