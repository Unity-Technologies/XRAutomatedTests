using UnityEngine;

public class ImmediateMode : MonoBehaviour
{
    private Material mat;
    public Shader shader;

    public virtual void Start()
    {
        mat = new Material(shader);
    }

    public virtual void OnPostRender()
    {
        float xx;
        float yy;
        float zz;

        if (!enabled)
            return;

        mat.SetPass(0);
        GL.Begin(GL.TRIANGLES);
        GL.Color(new Color(0, 1, 0, 0.5f));
        GL.Vertex3(-0.5f, -0.5f, -0.5f);
        GL.Vertex3(0.5f, -0.5f, -0.5f);
        GL.Vertex3(0.5f, 0.5f, -0.5f);
        GL.End();
        GL.Begin(GL.LINES);
        GL.Color(Color.black);
        GL.Vertex3(-0.5f, 0.5f, -0.5f);
        GL.Vertex3(-0.5f, 0.5f, 0.5f);
        GL.Color(Color.blue);
        GL.Vertex3(0.5f, 0.5f, -0.5f);
        GL.Vertex3(-0.5f, 0.5f, 0.5f);
        GL.End();
        // lots of quads; won't fit into max. batch size on non-GL; will see if it works
        GL.Begin(GL.TRIANGLES);
        var tris = 19;
        var sc = 1f / tris;
        GL.Color(new Color(1, 0, 0, 0.1f));
        var e = 0;
        while (e < tris)
        {
            zz = -0.5f + ((e * sc) * 0.03f);
            var q = 0;
            while (q < tris)
            {
                yy = -0.5f + (q * sc);
                var w = 0;
                while (w <= q)
                {
                    xx = -0.5f + (w * sc);
                    GL.Vertex3(xx, yy, zz);
                    GL.Vertex3(xx + sc, yy + sc, zz);
                    GL.Vertex3(xx, yy + sc, zz);
                    ++w;
                }
                ++q;
            }
            ++e;
        }
        GL.End();
        // lots of quads; won't fit into max. batch size on non-GL; will see if it works
        GL.Begin(GL.QUADS);
        var divs = 48;
        sc = 1.5f / divs;
        var ss = 1f / divs;
        var z = 0;
        while (z < divs)
        {
            zz = ((z * ss) * 2f) - 1f;
            var zc = 0.5f - (z * sc);
            var y = 0;
            while (y < divs)
            {
                yy = ((y * ss) * 2f) - 1f;
                var x = 0;
                while (x < divs)
                {
                    xx = ((x * ss) * 2f) - 1f;
                    var dist = ((xx * xx) + (yy * yy)) + (zz * zz);
                    GL.Color(new Color(1f - (x * sc), 1f - (y * sc), ((x & y) == 0) && (dist <= 1f) ? 1f : 0f, dist > 1f ? 0.03f : 0.4f));
                    GL.Vertex3(0.5f + ((x + 0.9f) * sc), -0.5f + (y * sc), zc);
                    GL.Vertex3(0.5f + (x * sc), -0.5f + (y * sc), zc);
                    GL.Vertex3(0.5f + (x * sc), -0.5f + ((y + 0.9f) * sc), zc);
                    GL.Vertex3(0.5f + ((x + 0.9f) * sc), -0.5f + ((y + 0.9f) * sc), zc);
                    ++x;
                }
                ++y;
            }
            ++z;
        }
        GL.End();
        // Test that large triangle strips are handled correctly
        GL.Begin(GL.TRIANGLE_STRIP);
        var inner = 0.33f;
        int circ;
        while (inner < 0.6f)
        {
            var outer = inner + 0.11f;
            circ = 0;
            while (circ <= 3600)
            {
                var angle = circ / 10;
                var r = (Mathf.Cos((3 * angle) * Mathf.Deg2Rad) * 0.5f) + 0.5f;
                var g = (Mathf.Cos((5 * (angle + 30)) * Mathf.Deg2Rad) * 0.5f) + 0.5f;
                var b = (Mathf.Cos((3 * (angle + 60)) * Mathf.Deg2Rad) * 0.5f) + 0.5f;
                var c = Mathf.Cos(angle * Mathf.Deg2Rad);
                var s = Mathf.Sin(angle * Mathf.Deg2Rad);
                GL.Color(new Color(r, g, b, outer));
                GL.Vertex3((c * outer) + 0.5f, (s * outer) - 0.25f, 0f);
                GL.Color(new Color(r, g, b, inner));
                GL.Vertex3((c * inner) + 0.5f, (s * inner) - 0.25f, 0f);
                circ = circ + 1;
            }
            inner = outer;
        }
        GL.End();
        // Test that we ignore excess End() calls
        GL.End();
        // Test that we don't lose the color when we don't set it per vertex
        GL.Begin(GL.TRIANGLE_STRIP);
        GL.Color(new Color(1f, 0f, 0, 0.75f));
        circ = 0;
        while (circ <= 3600)
        {
            var angle = circ / 10;
            var c = Mathf.Cos(angle * Mathf.Deg2Rad);
            var s = Mathf.Sin(angle * Mathf.Deg2Rad);
            GL.Vertex3((c * 0.33f) + 0.5f, (s * 0.33f) - 0.25f, 0f);
            GL.Vertex3((c * 0.3f) + 0.5f, (s * 0.3f) - 0.25f, 0f);
            circ = circ + 1;
        }
        GL.End();
    }
}
