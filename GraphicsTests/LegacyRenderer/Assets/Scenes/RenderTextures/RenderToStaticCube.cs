using UnityEngine;

public class RenderToStaticCube : MonoBehaviour
{
    public Renderer debugSphere;
    private Camera cam;
    private Cubemap rtex;

    public void Start()
    {
        var go = new GameObject("CubemapCamera", typeof(Camera));
        go.transform.position = transform.position;
        go.transform.rotation = Quaternion.identity;
        cam = go.GetComponent<Camera>();
        cam.enabled = false;
        cam.cullingMask = 1;
        // Set camera to use deferred lighting. This is not supported and Unity
        // should fallback to Forward with no errors happening.
        cam.renderingPath = RenderingPath.DeferredLighting;
        rtex = new Cubemap(128, TextureFormat.RGB24, true);
        GetComponent<Renderer>().material.SetTexture("_Cube", rtex);
        cam.RenderToCubemap(rtex);
        if (debugSphere)
        {
            debugSphere.material.SetTexture("_MainTex", rtex);
        }
    }
}
