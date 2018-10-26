using UnityEngine;

public class DuplicateMeshes : MonoBehaviour
{
    public void Start()
    {
        var mesh = InitializeVertexColorMesh();
        var go1 = CloneMe(mesh);
        go1.transform.position = transform.position + new Vector3(1.2f, 0, 0);
        var go2 = CloneMe(mesh);
        go2.transform.position = transform.position + new Vector3(2.4f, 0, 0);
        go2.GetComponent<Renderer>().material.SetFloat("dummy", 2f); // cause material instantiate
    }

    public GameObject CloneMe(Mesh mesh)
    {
        var go = new GameObject("Foo", typeof(MeshFilter), typeof(MeshRenderer));
        ((MeshFilter)go.GetComponent(typeof(MeshFilter))).sharedMesh = mesh;
        go.GetComponent<Renderer>().sharedMaterial = GetComponent<Renderer>().sharedMaterial;
        return go;
    }

    public Mesh InitializeVertexColorMesh()
    {
        var filter = (MeshFilter)GetComponent(typeof(MeshFilter));
        var mesh = filter.mesh;
        var vertices = mesh.vertices;
        var colors = new Color[mesh.vertexCount];
        var i = 0;
        while (i < vertices.Length)
        {
            var p = vertices[i];
            colors[i] = new Color((p.x * 0.5f) + 0.5f, (p.y * 0.5f) + 0.5f, (p.z * 0.5f) + 0.5f, 1f);
            ++i;
        }
        mesh.colors = colors;
        return mesh;
    }
}
