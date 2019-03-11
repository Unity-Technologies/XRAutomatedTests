#if UNITY_2018_1_OR_NEWER
using UnityEngine;


public class StaticRenderPerformanceWithObjMonoBehaviourTest : StaticRenderPerformanceMonoBehaviourTest
{
    public override void RunStartCode()
    {
        base.RunStartCode();
        var thisCamera = GameObject.Find("Camera").GetComponent<Camera>();
        thisCamera.clearFlags = CameraClearFlags.Skybox;

        var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        var meshRenderer = cube.GetComponent<MeshRenderer>();
        if(meshRenderer != null)
        {
            meshRenderer.material = new Material(Shader.Find("Lightweight Render Pipeline/Lit"));
        }

        cube.transform.rotation = new Quaternion(16, 41, 17, 0);
        cube.transform.position = new Vector3(thisCamera.transform.position.x, thisCamera.transform.position.y, -10);
        Camera.main.transform.LookAt(cube.transform);
    }
}
#endif

