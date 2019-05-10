using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.XR;

public class CurrentSettings : ScriptableObject 
{
    public string EnabledXrTarget;
    public GraphicsDeviceType PlayerGraphicsApi;

    public XRSettings.StereoRenderingMode StereoRenderingMode;
    
    public bool MtRendering = true;
    public bool GraphicsJobs;

    public string SimulationMode;
}
