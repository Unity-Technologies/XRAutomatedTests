#if UNITY_EDITOR

using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.XR;
using UnityEditor;

[System.Serializable]
public struct PlatformConfigTestFilter
{
    public ColorSpace ColorSpace;
    public BuildTarget BuildPlatform;
    public GraphicsDeviceType GraphicsDevice;
    public StereoRenderingPath[] stereoModes;
    public string Reason;
}

#endif