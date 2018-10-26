using UnityEngine;

public class RenderTextureNoZFx : MonoBehaviour
{
    public Material mat;

    public void OnRenderImage(RenderTexture src, RenderTexture dst)
    {
        var rt = RenderTexture.GetTemporary(src.width, src.height, 0);
        Graphics.SetRenderTarget(rt);
        GL.Clear(true, true, Color.blue, 0.0f);
        Graphics.Blit(src, rt, mat);
        Graphics.Blit(rt, dst);
        RenderTexture.ReleaseTemporary(rt);
    }
}
