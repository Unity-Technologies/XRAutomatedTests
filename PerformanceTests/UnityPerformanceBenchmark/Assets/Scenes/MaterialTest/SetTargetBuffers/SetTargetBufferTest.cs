using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Tests:
//1) Rendering to color and depth buffer of the same render texture
//2) Rendering to color and depth buffer of different render textures
//3) Rendering to color and depth buffer without writing depth
//4) Rendering to color and depth buffer while writing depth
//4) Rendering to prefab render texture
//5) Rendering to constructed render texture

public class SetTargetBufferTest : MonoBehaviour {
    public Camera depth_camera;
    public Camera color_camera;
    public Camera clear_camera;

    public RenderTexture output_RT;
    private RenderTexture unused_RT;        //Used as a throw-away color buffer when rendering the Depth Camera
    
    // Use this for initialization
    void Start () {
        
        if(output_RT != null)
        {
            unused_RT = new RenderTexture(output_RT.width, output_RT.height, 0, RenderTextureFormat.ARGB32);

            //Clear output buffers
            clear_camera.SetTargetBuffers(output_RT.colorBuffer, output_RT.depthBuffer);
            clear_camera.depthTextureMode = DepthTextureMode.Depth;
            clear_camera.depth = Camera.main.depth - 3;

            //Draw to depth buffer, sending color buffer to the unused render texture which will be discarded.
            depth_camera.SetTargetBuffers(unused_RT.colorBuffer, output_RT.depthBuffer);
            depth_camera.depthTextureMode = DepthTextureMode.Depth;
            depth_camera.depth = Camera.main.depth - 2;

            //Draw to color and depth buffer without writing depth values
            color_camera.SetTargetBuffers(output_RT.colorBuffer, output_RT.depthBuffer);
            color_camera.depthTextureMode = DepthTextureMode.None;
            color_camera.depth = Camera.main.depth - 1;
        }
	}
	
}
