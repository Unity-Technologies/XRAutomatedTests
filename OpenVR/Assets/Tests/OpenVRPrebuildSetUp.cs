using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using UnityEditor;
using System;
using UnityEngine.Experimental.XR;
using UnityEngine.XR;

namespace Tests
{
    [PrebuildSetup(typeof(EnableOpenVRPrebuildStep))]
    public class OpenVRTestBase
    {
        public static GameObject m_Camera;
        public static GameObject m_Light;
        public static GameObject m_Cube;

        public Tests.TestSetupHelpers m_TestSetupHelpers;

        [SetUp]
        public void Setup()
        {
            m_TestSetupHelpers = new TestSetupHelpers();

            m_TestSetupHelpers.TestStageSetup(TestStageConfig.BaseStageSetup);
        }

        [TearDown]
        public void TearDown()
        {
            m_TestSetupHelpers.TestStageSetup(TestStageConfig.CleanStage);
        }

        public class EnableOpenVRPrebuildStep : IPrebuildSetup
        {
            public void Setup()
            {
#if UNITY_EDITOR
                var buildTargetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;

                PlayerSettings.virtualRealitySupported = true;
                PlayerSettings.stereoRenderingPath = StereoRenderingPath.Instancing;

                if (buildTargetGroup == BuildTargetGroup.Standalone)
                {
                    UnityEditorInternal.VR.VREditor.SetVREnabledDevicesOnTargetGroup(
                        buildTargetGroup,
                        new[] { "OpenVR" });
                }
                else
                {
                    throw new NotImplementedException(EditorUserBuildSettings.selectedBuildTargetGroup + "Platform not implemented for prebuild steps.");
                }
#endif
            }
        }
    }
}
