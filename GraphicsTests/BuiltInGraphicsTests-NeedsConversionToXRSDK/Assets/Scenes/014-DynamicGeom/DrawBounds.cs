using UnityEngine;

public class DrawBounds : MonoBehaviour
{
    private Material mat;
    public Shader shader;

    public virtual void Start()
    {
        if (shader == null)
            return;

        mat = new Material(shader);
    }

    public virtual void OnRenderObject()
    {
        if (!enabled || (mat == null))
            return;

        var r = GetComponent<Renderer>();
        if (r == null)
            return;

        var b = r.bounds;
        var bmin = b.min;
        var bmax = b.max;
        mat.SetPass(0);
        GL.Begin(GL.LINES);
        GL.Color(new Color(1, 0, 0, 0.3f));
        // X lines
        GL.Vertex3(bmin.x, bmin.y, bmin.z);
        GL.Vertex3(bmax.x, bmin.y, bmin.z);
        GL.Vertex3(bmin.x, bmax.y, bmin.z);
        GL.Vertex3(bmax.x, bmax.y, bmin.z);
        GL.Vertex3(bmin.x, bmin.y, bmax.z);
        GL.Vertex3(bmax.x, bmin.y, bmax.z);
        GL.Vertex3(bmin.x, bmax.y, bmax.z);
        GL.Vertex3(bmax.x, bmax.y, bmax.z);
        // Y lines
        GL.Vertex3(bmin.x, bmin.y, bmin.z);
        GL.Vertex3(bmin.x, bmax.y, bmin.z);
        GL.Vertex3(bmax.x, bmin.y, bmin.z);
        GL.Vertex3(bmax.x, bmax.y, bmin.z);
        GL.Vertex3(bmin.x, bmin.y, bmax.z);
        GL.Vertex3(bmin.x, bmax.y, bmax.z);
        GL.Vertex3(bmax.x, bmin.y, bmax.z);
        GL.Vertex3(bmax.x, bmax.y, bmax.z);
        // Z lines
        GL.Vertex3(bmin.x, bmin.y, bmin.z);
        GL.Vertex3(bmin.x, bmin.y, bmax.z);
        GL.Vertex3(bmax.x, bmin.y, bmin.z);
        GL.Vertex3(bmax.x, bmin.y, bmax.z);
        GL.Vertex3(bmin.x, bmax.y, bmin.z);
        GL.Vertex3(bmin.x, bmax.y, bmax.z);
        GL.Vertex3(bmax.x, bmax.y, bmin.z);
        GL.Vertex3(bmax.x, bmax.y, bmax.z);
        GL.End();
    }
}
