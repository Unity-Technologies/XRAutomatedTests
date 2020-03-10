using UnityEngine;

public class UseDepthTexture : MonoBehaviour
{
    void Start()
    {
        GetComponent<Camera>().depthTextureMode = DepthTextureMode.Depth;
    }
}
