// port of stb_easy_font.h into Unity/C# - public domain
// Aras Pranckevicius, 2015 November
// https://github.com/aras-p/UnityStbEasyFont


using UnityEngine;
using System.Collections.Generic;

[ExecuteInEditMode]
public class SimpleTextMesh : MonoBehaviour
{
    [Multiline]
    public string text = "ABC";
    public Color32 color = new Color32(255, 255, 255, 255);
    public float characterSize = 1.0f;
    public TextAnchor anchor = TextAnchor.UpperLeft;

    private string prevText = null;
    private Color32 prevColor = new Color32(0, 0, 0, 0);
    private Mesh mesh;
    private Material mat;

    void Start()
    {
        UpdateMesh();
    }

    void OnDisable()
    {
        DestroyImmediate(mesh);
        DestroyImmediate(mat);
    }

    void Update()
    {
        UpdateMesh();

        if (mesh != null)
        {
            UpdateMaterial();
            var mtx = transform.localToWorldMatrix;
            var scale = EasyFontUtilities.kScaleFactor * characterSize;
            var offset = EasyFontUtilities.CalcAnchorOffset(mesh, anchor);
            offset.y = -offset.y;
            var offsetMat = Matrix4x4.TRS(offset, Quaternion.identity, Vector3.one);
            var scaleMat = Matrix4x4.Scale(new Vector3(scale, -scale, scale));
            Graphics.DrawMesh(mesh, mtx * scaleMat * offsetMat, mat, gameObject.layer);
        }
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
    [UnityEditor.MenuItem("GameObject/3D Object/Simple 3D Text")]
    public static void CreateText()
    {
        var go = new GameObject("New 3D Text", typeof(SimpleTextMesh));
        go.GetComponent<SimpleTextMesh>().text = "Hello World";
        EasyFontUtilities.SelectAndMoveToView(go);
    }

    #endif
}
