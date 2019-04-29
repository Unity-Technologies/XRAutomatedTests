// port of stb_easy_font.h into Unity/C# - public domain
// Aras Pranckevicius, 2015 November
// https://github.com/aras-p/UnityStbEasyFont

using UnityEngine;
using System.Collections.Generic;

[ExecuteInEditMode]
public class SimpleGUIText : MonoBehaviour
{
    [Multiline]
    public string text = "ABC";
    public TextAnchor anchor = TextAnchor.LowerLeft;
    public Vector2 pixelOffset = Vector2.zero;
    public Color32 color = new Color32(255, 255, 255, 255);
    public float characterSize = 1.0f;

    private string prevText = null;
    private Color32 prevColor = new Color32(0, 0, 0, 0);
    private Mesh mesh;
    private Material mat;

    void Start()
    {
        UpdateMesh();
    }

    void OnEnable()
    {
        Camera.onPostRender += RenderText;
    }

    void OnDisable()
    {
        Camera.onPostRender -= RenderText;
        DestroyImmediate(mesh);
        DestroyImmediate(mat);
    }

    void RenderText(Camera cam)
    {
        // don't render if not game view
        if (cam.cameraType != CameraType.Game)
            return;
        if (cam.tag != "MainCamera" && cam.tag != "UI")
            return;

        // our layer is culled
        if ((cam.cullingMask & (1 << gameObject.layer)) == 0)
            return;

        UpdateMesh();
        if (mesh == null)
            return;
        UpdateMaterial();

        GL.PushMatrix();
        GL.LoadPixelMatrix();
        mat.SetPass(0);

        var camRect = cam.pixelRect;
        var pos = transform.position;
        pos.x = camRect.x + pos.x * camRect.width + pixelOffset.x;
        pos.y = camRect.y + pos.y * camRect.height + pixelOffset.y;
        pos.z = 0;
        pos += EasyFontUtilities.CalcAnchorOffset(mesh, anchor);
        pos.x = Mathf.Round(pos.x);
        pos.y = Mathf.Round(pos.y);

        var scale = characterSize;
        var mtx = Matrix4x4.TRS(pos, Quaternion.identity, new Vector3(scale, -scale, scale));

        // Note: DrawMeshNow sets up current camera's view matrix;
        // so make sure that's identity and reset it afterwards
        cam.worldToCameraMatrix = Matrix4x4.identity;
        Graphics.DrawMeshNow(mesh, mtx);
        cam.ResetWorldToCameraMatrix();
        GL.PopMatrix();
    }

    void UpdateMaterial()
    {
        if (mat != null)
            return;
        mat = EasyFontUtilities.CreateFontMaterial();
    }

    void UpdateMesh()
    {
        if (text == prevText && color.Equals(prevColor) && mesh != null)
            return;
        prevText = text;
        prevColor = color;

        EasyFontUtilities.UpdateMesh(ref mesh, text, color);
    }

    #if UNITY_EDITOR
    [UnityEditor.MenuItem("GameObject/3D Object/Simple GUI Text")]
    public static void CreateText()
    {
        var go = new GameObject("New GUI Text", typeof(SimpleGUIText));
        go.GetComponent<SimpleGUIText>().text = "Hello World";
        UnityEditor.Selection.activeGameObject = go;
        go.transform.position = new Vector3(0.5f, 0.5f, 0.0f);
    }

    #endif
}
