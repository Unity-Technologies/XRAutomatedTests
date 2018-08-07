using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrentSettings : ScriptableObject 
{
    public string[] enabledXrTargets;
    public string[] playerGraphicsApis;

    public string[] stereoRenderingPaths;

    public bool mtRendering = true;
    public bool graphicsJobs;
    
}
