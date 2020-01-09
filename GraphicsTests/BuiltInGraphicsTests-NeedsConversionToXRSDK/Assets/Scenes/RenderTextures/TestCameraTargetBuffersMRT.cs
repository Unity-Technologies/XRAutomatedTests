using UnityEngine;
using System.Collections;

public class TestCameraTargetBuffersMRT : MonoBehaviour
{
    public  Shader      shader;
    private Material    mat;

    void Awake()
    {
        mat = new Material(shader);
        mat.hideFlags = HideFlags.HideAndDontSave;
    }

    void OnPostRender()
    {
        Graphics.Blit(null, mat, 0);
    }
}
