using UnityEngine;

public class TestMRTBlendModes : MonoBehaviour
{
    public Shader shader;
    public int size = 64;
    private RenderTexture[] rts;
    private Material mat;

    public void Start()
    {
        mat = new Material(shader);
        mat.hideFlags = HideFlags.HideAndDontSave;
        rts = new RenderTexture[mat.passCount * 4];
        for (int i = 0; i < rts.Length; ++i)
            rts[i] = new RenderTexture(size, size, 0);
    }

    public void OnPostRender()
    {
        if (SystemInfo.supportedRenderTargetCount < 4)
            return;

        // for each shader pass (i.e. "test case"):
        for (int pass = 0; pass < mat.passCount; ++pass)
        {
            // setup 4 MRTs, clear to gray
            var mrt4 = new RenderBuffer[] { rts[pass * 4 + 0].colorBuffer, rts[pass * 4 + 1].colorBuffer, rts[pass * 4 + 2].colorBuffer, rts[pass * 4 + 3].colorBuffer };
            Graphics.SetRenderTarget(mrt4, rts[0].depthBuffer);
            GL.Clear(true, true, new Color(0.5f, 0.5f, 0.5f, 1f));
            // draw into into them; shader uses different blending modes in different passes
            Graphics.Blit(null, mat, pass);
        }

        // display results on screen, in columns for each test case
        Graphics.SetRenderTarget(null);
        GL.PushMatrix();
        GL.LoadPixelMatrix();
        for (int pass = 0; pass < mat.passCount; ++pass)
        {
            for (int rt = 0; rt < 4; ++rt)
            {
                Graphics.DrawTexture(new Rect(pass * size, Screen.height - size * (rt + 1), size, size), rts[pass * 4 + rt]);
            }
        }
        GL.PopMatrix();
    }
}
