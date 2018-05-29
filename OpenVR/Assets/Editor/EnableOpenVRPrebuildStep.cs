using System;
using System.IO;
using UnityEditor;
using UnityEngine.TestTools;

public class EnableOpenVRPrebuildStep : IPrebuildSetup
{
    public void Setup()
    {
        var buildTargetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;

        PlayerSettings.virtualRealitySupported = true;

        if (buildTargetGroup == BuildTargetGroup.Standalone)
        {
            UnityEditorInternal.VR.VREditor.SetVREnabledDevicesOnTargetGroup(
                buildTargetGroup,
                new[] {"OpenVR"});
        }
        else
        {
            throw new NotImplementedException(EditorUserBuildSettings.selectedBuildTargetGroup + "Platform not implemented for prebuild steps.");
        }
    }
}
