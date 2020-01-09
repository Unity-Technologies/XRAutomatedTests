using UnityEngine;

public class TextureFilterChange : MonoBehaviour
{
    public Shader shader;
    private Material mat;
    private Texture2D tex;

    public void Start()
    {
        mat = new Material(shader);
        tex = new Texture2D(2, 2);
        Color32[] cols = new Color32[4];
        cols[0] = new Color32(255, 64, 64, 0);
        cols[1] = new Color32(64, 255, 64, 0);
        cols[2] = new Color32(64, 64, 255, 0);
        cols[3] = new Color32(255, 255, 64, 0);
        tex.SetPixels32(cols);
        tex.Apply();
        tex.filterMode = FilterMode.Point;
        mat.mainTexture = tex;
    }

    public void OnPostRender()
    {
        GL.LoadPixelMatrix();
        // check if changing texture parameters (like filter mode)
        // while same texture is already bound works properly
        tex.filterMode = FilterMode.Point;
        tex.mipMapBias = 0f;
        mat.SetPass(0);
        GL.Begin(GL.QUADS);
        GL.TexCoord2(0, 0);
        GL.Vertex3(10, 10, 0.1f);
        GL.TexCoord2(0, 1);
        GL.Vertex3(10, 29, 0.1f);
        GL.TexCoord2(1, 1);
        GL.Vertex3(29, 29, 0.1f);
        GL.TexCoord2(1, 0);
        GL.Vertex3(29, 10, 0.1f);
        GL.End();
        tex.filterMode = FilterMode.Bilinear;
        mat.SetPass(0);
        GL.Begin(GL.QUADS);
        GL.TexCoord2(0, 0);
        GL.Vertex3(30, 10, 0.1f);
        GL.TexCoord2(0, 1);
        GL.Vertex3(30, 29, 0.1f);
        GL.TexCoord2(1, 1);
        GL.Vertex3(49, 29, 0.1f);
        GL.TexCoord2(1, 0);
        GL.Vertex3(49, 10, 0.1f);
        GL.End();
        tex.mipMapBias = 4f;
        mat.SetPass(0);
        GL.Begin(GL.QUADS);
        GL.TexCoord2(0, 0);
        GL.Vertex3(50, 10, 0.1f);
        GL.TexCoord2(0, 1);
        GL.Vertex3(50, 29, 0.1f);
        GL.TexCoord2(1, 1);
        GL.Vertex3(69, 29, 0.1f);
        GL.TexCoord2(1, 0);
        GL.Vertex3(69, 10, 0.1f);
        GL.End();
    }
}
