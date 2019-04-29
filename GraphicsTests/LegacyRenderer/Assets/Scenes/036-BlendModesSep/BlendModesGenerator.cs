using UnityEngine;
using UnityEngine.Rendering;

public class BlendModesGenerator : MonoBehaviour
{
    public Shader shader;
    public Shader shaderAlpha;
    public Shader shaderShowAlpha;
    private BlendMode[] modes;
    public bool separateAlpha;
    public float dist;
    public Texture text1;
    private Material alphaMat;

    public void Start()
    {
        var r = 0;
        while (r < modes.Length)
        {
            var c = 0;
            while (c < modes.Length)
            {
                var go = GameObject.CreatePrimitive(PrimitiveType.Plane);
                go.name = ("Plane " + r) + c;
                go.transform.Rotate(90, 180, 0);

                {
                    var _79 = c * 11;
                    var _80 = go.transform.position;
                    _80.x = _79;
                    go.transform.position = _80;
                }

                {
                    var _81 = -r * 11;
                    var _82 = go.transform.position;
                    _82.y = _81;
                    go.transform.position = _82;
                }

                {
                    var _83 = dist;
                    var _84 = go.transform.position;
                    _84.z = _83;
                    go.transform.position = _84;
                }
                var mat = new Material(separateAlpha ? shaderAlpha : shader);
                if (!mat.shader.isSupported)
                {
                    Destroy(go);
                    goto Label_for_47;
                }
                mat.SetTexture("_MainTex", text1);
                mat.SetTextureScale("_MainTex", new Vector2(0.25f, 0.25f));
                mat.SetTextureOffset("_MainTex", new Vector2(-0.1f, 0.4f));
                mat.SetInt("_MySrcBlend", (int)modes[r]);
                mat.SetInt("_MyDstBlend", (int)modes[c]);
                mat.SetInt("_MySrcBlendA", (int)modes[c]);
                mat.SetInt("_MyDstBlendA", (int)modes[r]);
                go.GetComponent<Renderer>().material = mat;
            Label_for_47:
                ++c;
            }
            ++r;
        }
    }

    public void OnPostRender()
    {
        if (!alphaMat)
        {
            alphaMat = new Material(shaderShowAlpha);
        }
        GL.PushMatrix();
        GL.LoadIdentity();
        GL.LoadOrtho();
        alphaMat.SetPass(0);
        GL.Begin(GL.QUADS);
        GL.Color(Color.white);
        const float height = 0.95f;
        const float off = (1f - height) * 0.5f;
        var i = 0;
        while (i < modes.Length)
        {
            DrawQuad(new Rect(0f, off + ((height / modes.Length) * i), 1f, (height / modes.Length) * 0.5f));
            ++i;
        }
        GL.End();
        GL.PopMatrix();
    }

    private static void DrawQuad(Rect r)
    {
        GL.Vertex3(r.x, r.y + r.height, 0.1f);
        GL.Vertex3(r.x + r.width, r.y + r.height, 0.1f);
        GL.Vertex3(r.x + r.width, r.y, 0.1f);
        GL.Vertex3(r.x, r.y, 0.1f);
    }

    public BlendModesGenerator()
    {
        modes = new[] { BlendMode.One, BlendMode.Zero, BlendMode.SrcColor, BlendMode.SrcAlpha, BlendMode.DstColor, BlendMode.DstAlpha, BlendMode.OneMinusSrcColor, BlendMode.OneMinusSrcAlpha, BlendMode.OneMinusDstColor, BlendMode.OneMinusDstAlpha };
        dist = 20f;
    }
}
