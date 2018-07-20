using UnityEngine;
using UnityEditor;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.AnimatedValues;
using UnityEngine.Rendering;

public class BuildWindow : EditorWindow
{
    private static List<XRTargetPlatform> AvailablePlatforms = new List<XRTargetPlatform>();

    private int selectedTargetPlatformIndex = 0;

    private Dictionary<StereoRenderingPath, bool> enabledStereoRenderingPaths = new Dictionary<StereoRenderingPath, bool>();
    private Dictionary<GraphicsDeviceType, bool> enabledGraphicsDevices = new Dictionary<GraphicsDeviceType, bool>();
    private Dictionary<string, bool> enabledVRSDKs = new Dictionary<string, bool>();

    private bool isBuilding = false;

    public struct XRTargetPlatform
    {
        public BuildTargetGroup targetGroup;
        public BuildTarget target;
        public string name;
        public string[] XRPlatforms;
    }

    [MenuItem("Build/Build Window")]
    public static void ShowWindow()
    {
        GetWindow<BuildWindow>("XR Build");

        InitializeXRPlatformList();
    }

    private static void InitializeXRPlatformList()
    {
        AvailablePlatforms.Add(new XRTargetPlatform
        {
            targetGroup = BuildTargetGroup.Standalone,
            target = EditorUserBuildSettings.selectedStandaloneTarget,
            name = Enum.GetName(typeof(BuildTargetGroup), BuildTargetGroup.Standalone),
            XRPlatforms = new string[] { "Oculus", "OpenVR" }
        });
        AvailablePlatforms.Add(new XRTargetPlatform
        {
            targetGroup = BuildTargetGroup.Android,
            target = BuildTarget.Android,
            name = Enum.GetName(typeof(BuildTargetGroup), BuildTargetGroup.Android),
            XRPlatforms = new string[] { "Oculus", "Daydream", "Cardboard" }
        });
        AvailablePlatforms.Add(new XRTargetPlatform
        {
            targetGroup = BuildTargetGroup.WSA,
            target = BuildTarget.WSAPlayer,
            name = "UWP",
            XRPlatforms = new string[] { "Windows Mixed Reality" }
        });
        AvailablePlatforms.Add(new XRTargetPlatform
        {
            targetGroup = BuildTargetGroup.iOS,
            target = BuildTarget.iOS,
            name = "iOS",
            XRPlatforms = new string[] { "Cardboard" }
        });
    }

    void OnGUI()
    {
        if (!AvailablePlatforms.Any())
        {
            InitializeXRPlatformList();
        }

        if (!isBuilding)
        {
            GUILayout.Label("Build Settings", EditorStyles.boldLabel);

            EditorGUI.BeginChangeCheck();
            string[] names = AvailablePlatforms.Select(x => x.name).ToArray();
            selectedTargetPlatformIndex = EditorGUILayout.Popup("Target Platform", selectedTargetPlatformIndex, names);

            if (EditorGUI.EndChangeCheck())
            {
                bool platformInstalled = true;
                switch (AvailablePlatforms.ElementAt(selectedTargetPlatformIndex).targetGroup)
                {
                    case BuildTargetGroup.iOS:
                        if (platformInstalled = IsPlatformSupportInstalled(BuildTarget.iOS) && platformInstalled)
                        {
                            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.iOS, BuildTarget.iOS);
                        }
                        break;
                    case BuildTargetGroup.Android:
                        if (platformInstalled = IsPlatformSupportInstalled(BuildTarget.Android) && platformInstalled)
                        {
                            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);
                        }
                        break;
                    case BuildTargetGroup.WSA:
                        if (platformInstalled = IsPlatformSupportInstalled(BuildTarget.WSAPlayer) && platformInstalled)
                        {
                            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.WSA, BuildTarget.WSAPlayer);
                        }
                        break;
                    case BuildTargetGroup.Standalone:
                        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Standalone, EditorUserBuildSettings.selectedStandaloneTarget);
                        break;
                }


                if (!platformInstalled)
                {
                    Debug.LogWarning($"Platform Support for {AvailablePlatforms.ElementAt(selectedTargetPlatformIndex).name} not installed.");
                    selectedTargetPlatformIndex = 0;
                }

                EditorUserBuildSettings.selectedBuildTargetGroup = AvailablePlatforms.ElementAt(selectedTargetPlatformIndex).targetGroup;

                enabledStereoRenderingPaths.Clear();
                enabledGraphicsDevices.Clear();
                enabledVRSDKs.Clear();
            }

            foreach (var vrsdk in AvailablePlatforms[selectedTargetPlatformIndex].XRPlatforms)
            {
                if (!enabledVRSDKs.ContainsKey(vrsdk))
                {
                    enabledVRSDKs.Add(vrsdk, false);
                }
            }

            foreach (StereoRenderingPath stereoRenderingPath in Enum.GetValues(typeof(StereoRenderingPath)))
            {
                if (!enabledStereoRenderingPaths.ContainsKey(stereoRenderingPath))
                {
                    enabledStereoRenderingPaths.Add(stereoRenderingPath, false);
                }
            }

            foreach (GraphicsDeviceType device in PlayerSettings.GetGraphicsAPIs(EditorUserBuildSettings.activeBuildTarget))
            {
                if (!enabledGraphicsDevices.ContainsKey(device))
                {
                    enabledGraphicsDevices.Add(device, false);
                }
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField($"Platform Settings: {AvailablePlatforms.ElementAt(selectedTargetPlatformIndex).target}", EditorStyles.boldLabel);

            ShowOptionsForActiveBuildTarget();

            EditorGUILayout.Space();

            if (GUILayout.Button("Build with Configs"))
            {
                BuildSelectedConfigs();
            }
        }
    }

    private void BuildSelectedConfigs()
    {
        isBuilding = true;
        foreach (var vrsdk in this.enabledVRSDKs)
        {
            if (vrsdk.Value)
            {
                foreach (var stereoRenderingPath in this.enabledStereoRenderingPaths)
                {
                    if (stereoRenderingPath.Value)
                    {
                        foreach (var graphicDevice in this.enabledGraphicsDevices)
                        {
                            if (graphicDevice.Value)
                            {
                                UnityEditorInternal.VR.VREditor.SetVREnabledDevicesOnTargetGroup(
                                                                                        AvailablePlatforms.ElementAt(selectedTargetPlatformIndex).targetGroup,
                                                                                        new[] { vrsdk.Key });
                                PlayerSettings.stereoRenderingPath = stereoRenderingPath.Key;
                                PlayerSettings.SetGraphicsAPIs(EditorUserBuildSettings.activeBuildTarget, new GraphicsDeviceType[] { graphicDevice.Key });
                                Build.Name = $"/{EditorUserBuildSettings.activeBuildTarget}/{vrsdk.Key}/{stereoRenderingPath.Key}-{graphicDevice.Key}";
                                Build.BuildProject();
                            }
                        }
                    }
                }
            }
        }
        isBuilding = false;
    }

    private bool IsPlatformSupportInstalled(BuildTarget target)
    {
        var moduleManager = System.Type.GetType("UnityEditor.Modules.ModuleManager,UnityEditor.dll");
        var isPlatformSupportLoaded = moduleManager.GetMethod("IsPlatformSupportLoaded", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
        var getTargetStringFromBuildTarget = moduleManager.GetMethod("GetTargetStringFromBuildTarget", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);

        return (bool)isPlatformSupportLoaded.Invoke(null, new object[] { (string)getTargetStringFromBuildTarget.Invoke(null, new object[] { target }) });
    }

    private void ShowOptionsForActiveBuildTarget()
    {
        SetGraphicsAPIsForActiveBuildTarget();

        EditorGUI.indentLevel++;
        EditorGUILayout.LabelField("VR SDKs", EditorStyles.boldLabel);
        EditorGUI.indentLevel++;
        for (var i = 0; i < enabledVRSDKs.Keys.Count; ++i) // (var stereoRenderingPath in enabledStereoRenderingPaths)
        {
            enabledVRSDKs[enabledVRSDKs.Keys.ElementAt(i)] = EditorGUILayout.Toggle(enabledVRSDKs.Keys.ElementAt(i), enabledVRSDKs[enabledVRSDKs.Keys.ElementAt(i)]);
        }
        EditorGUI.indentLevel--;

        EditorGUI.indentLevel--;
        EditorGUILayout.Space();

        EditorGUI.indentLevel++;
        EditorGUILayout.LabelField("Stereo Rendering Method", EditorStyles.boldLabel);
        EditorGUI.indentLevel++;
        for (var i = 0; i < enabledStereoRenderingPaths.Keys.Count; ++i) // (var stereoRenderingPath in enabledStereoRenderingPaths)
        {
            enabledStereoRenderingPaths[enabledStereoRenderingPaths.Keys.ElementAt(i)] = EditorGUILayout.Toggle(Enum.GetName(typeof(StereoRenderingPath), enabledStereoRenderingPaths.Keys.ElementAt(i)), enabledStereoRenderingPaths[enabledStereoRenderingPaths.Keys.ElementAt(i)]);
        }

        EditorGUI.indentLevel--;
        EditorGUILayout.Space();


        EditorGUILayout.LabelField("Graphics API", EditorStyles.boldLabel);
        EditorGUI.indentLevel++;
        for (var i = 0; i < enabledGraphicsDevices.Count; ++i) //foreach (var enabledDevice in enabledGraphicsDevices)
        {
            enabledGraphicsDevices[enabledGraphicsDevices.ElementAt(i).Key] = EditorGUILayout.Toggle(Enum.GetName(typeof(GraphicsDeviceType), enabledGraphicsDevices.ElementAt(i).Key), enabledGraphicsDevices[enabledGraphicsDevices.ElementAt(i).Key]);
        }
        EditorGUI.indentLevel--;

        EditorGUI.indentLevel--;
    }

    private void SetGraphicsAPIsForActiveBuildTarget()
    {

        PlayerSettings.SetUseDefaultGraphicsAPIs(EditorUserBuildSettings.activeBuildTarget, false);
        switch (EditorUserBuildSettings.selectedBuildTargetGroup)
        {
            case BuildTargetGroup.Standalone:
                if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.StandaloneWindows64 || EditorUserBuildSettings.activeBuildTarget == BuildTarget.StandaloneWindows)
                {
                    PlayerSettings.SetGraphicsAPIs(EditorUserBuildSettings.activeBuildTarget, new GraphicsDeviceType[] { GraphicsDeviceType.Direct3D11, GraphicsDeviceType.Direct3D12, GraphicsDeviceType.OpenGLCore, GraphicsDeviceType.Vulkan });
                }
                else if(EditorUserBuildSettings.activeBuildTarget == BuildTarget.StandaloneOSX)
                {
                    PlayerSettings.SetGraphicsAPIs(EditorUserBuildSettings.activeBuildTarget, new GraphicsDeviceType[] { GraphicsDeviceType.OpenGLCore, GraphicsDeviceType.Metal });
                }
                break;
            case BuildTargetGroup.iOS:
                if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.iOS)
                {
                    PlayerSettings.SetGraphicsAPIs(EditorUserBuildSettings.activeBuildTarget, new GraphicsDeviceType[] { GraphicsDeviceType.Metal, GraphicsDeviceType.OpenGLES3, GraphicsDeviceType.OpenGLES2 });
                }
                break;
            case BuildTargetGroup.Android:
                if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android)
                {
                    PlayerSettings.SetGraphicsAPIs(EditorUserBuildSettings.activeBuildTarget, new GraphicsDeviceType[] { GraphicsDeviceType.OpenGLES3, GraphicsDeviceType.OpenGLES2, GraphicsDeviceType.Vulkan });
                }
                break;
            case BuildTargetGroup.WSA:
                if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.WSAPlayer)
                {
                    PlayerSettings.SetGraphicsAPIs(EditorUserBuildSettings.activeBuildTarget, new GraphicsDeviceType[] { GraphicsDeviceType.Direct3D11, GraphicsDeviceType.Direct3D12 });
                }
                break;
            case BuildTargetGroup.PS4:
                break;
            default:
                break;
        }
    }
}