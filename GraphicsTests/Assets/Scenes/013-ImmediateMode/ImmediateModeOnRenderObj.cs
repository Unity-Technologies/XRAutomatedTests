using UnityEngine;

public class ImmediateModeOnRenderObj : MonoBehaviour
{
    private Material mat;
    public Shader shader;

    public void Start()
    {
        mat = new Material(shader);
    }

    public void OnRenderObject()
    {
        if (!enabled)
            return;

        mat.SetPass(0);
        GL.Begin(GL.LINES);
        var i = 0;
        while (i < 20)
        {
            GL.Color(new Color(i * 0.05f, 1 - (i * 0.05f), 0, 0.8f));
            GL.Vertex3(0, 0.5f, 0);
            GL.Vertex3((i * 0.05f) - 0.5f, 1, 0);
            ++i;
        }
        GL.End();
    }
}
