#if UNITY_EDITOR

using UnityEngine;
using UnityEngine.Rendering;
using UnityEditor;

[System.Serializable]
public struct PlatformConfigTestFilter
{
    public ColorSpace ColorSpace;
    public BuildTarget BuildPlatform;
    public GraphicsDeviceType GraphicsDevice;
    public string Reason;
}

#endif