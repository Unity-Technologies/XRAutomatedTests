using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrentSettings : ScriptableObject 
{
    public string enabledXrTarget;
    public string playerGraphicsApi;

    public string stereoRenderingPath;

    public bool mtRendering = true;
    public bool graphicsJobs;
    
}
