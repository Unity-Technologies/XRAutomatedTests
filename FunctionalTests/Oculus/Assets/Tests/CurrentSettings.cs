using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.XR;

public class CurrentSettings : ScriptableObject 
{
    public string enabledXrTarget;
    public GraphicsDeviceType playerGraphicsApi;

    public XRSettings.StereoRenderingMode stereoRenderingPath;
    
    public bool mtRendering = true;
    public bool graphicsJobs;
    
}
