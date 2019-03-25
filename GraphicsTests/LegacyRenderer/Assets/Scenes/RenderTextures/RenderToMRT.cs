using UnityEngine;

public class RenderToMRT : MonoBehaviour
{
    public Texture2D testTex;
    public Shader shader;
    public int size;
    public RenderTexture rt1;
    public RenderTexture rt2;
    public RenderTexture rt3;
    public RenderTexture rt4;
    private Material mat;

    public void Start()
    {
        rt1 = new RenderTexture(size, size, 24);
        rt2 = new RenderTexture(size, size, 0);
        rt3 = new RenderTexture(size, size, 0);
        rt4 = new RenderTexture(size, size, 0);
        mat = new Material(shader);
        mat.hideFlags = HideFlags.HideAndDontSave;
    }

    public void OnPostRender()
    {
        //if (SystemInfo.supportedRenderTargetCount < 4)
        //    return;

        //// render to all 4 RTs
        RenderBuffer[] mrt4 = { rt1.colorBuffer, rt2.colorBuffer, rt3.colorBuffer, rt4.colorBuffer };
        Graphics.SetRenderTarget(mrt4, rt1.depthBuffer);
        Graphics.Blit(null, mat, 0);
        Graphics.SetRenderTarget(null);
        GL.PushMatrix();
        mat.SetPass(0);
        GL.LoadPixelMatrix();
        // Graphics.DrawTexture(new Rect(size * 10, 32, size, size), rt1);
        //Graphics.DrawTexture(new Rect(size * 11, 32, size, size), rt2);
        //Graphics.DrawTexture(new Rect(size * 12, 32, size, size), rt3);
        //Graphics.DrawTexture(new Rect(size * 13, 32, size, size), rt4);
        Graphics.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), testTex, null, -1);
        GL.End();
        GL.PopMatrix();
        // render to 2 RTs
        //RenderBuffer[] mrt2 = { rt4.colorBuffer, rt3.colorBuffer };
        //Graphics.SetRenderTarget(mrt2, rt1.depthBuffer);
        //Graphics.Blit(null, mat, 0);
        //Graphics.SetRenderTarget(null);
        //GL.PushMatrix();
        //GL.LoadPixelMatrix();
        //Graphics.DrawTexture(new Rect(size * 0, size, size, size), rt1);
        //Graphics.DrawTexture(new Rect(size * 1, size, size, size), rt2);
        //Graphics.DrawTexture(new Rect(size * 2, size, size, size), rt3);
        //Graphics.DrawTexture(new Rect(size * 3, size, size, size), rt4);
        //GL.PopMatrix();
    }

    public RenderToMRT()
    {
        size = 32;
    }
}
