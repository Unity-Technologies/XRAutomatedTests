using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Tests
{
    [PrebuildSetup(typeof(EnableVRPrebuildStep))]
    public class CardboardSetup
    {
        public static GameObject m_Camera;
        public static GameObject m_Light;
        public static GameObject m_Cube;

        public Tests.TestSetupHelpers m_TestSetupHelpers;

        [SetUp]
        public void Setup()
        {
            m_Cube = new GameObject("Cube");
            m_TestSetupHelpers = new TestSetupHelpers();

            m_TestSetupHelpers.TestStageSetup(TestStageConfig.BaseStageSetup);
        }

        [TearDown]
        public void TearDown()
        {
            m_TestSetupHelpers.TestStageSetup(TestStageConfig.CleanStage);
        }

        public class EnableVRPrebuildStep : IPrebuildSetup
        {
            public void Setup()
            {
#if UNITY_EDITOR
                PlayerSettings.virtualRealitySupported = true;
                SetupVRByPlatform(EditorUserBuildSettings.selectedBuildTargetGroup);
#endif
            }

#if UNITY_EDITOR
            private void SetupVRByPlatform(BuildTargetGroup platformTarget)
            {
                switch (platformTarget)
                {
                    case BuildTargetGroup.Android:
                        {
                            UnityEditorInternal.VR.VREditor.SetVREnabledDevicesOnTargetGroup(
                                platformTarget,
                                new[] { "cardboard" });
                            EditorUserBuildSettings.androidBuildType = AndroidBuildType.Development;
                            EditorUserBuildSettings.androidBuildSystem = AndroidBuildSystem.Internal;
                            PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel19;
                            break;
                        }
                    case BuildTargetGroup.iOS:
                        {
                            UnityEditorInternal.VR.VREditor.SetVREnabledDevicesOnTargetGroup(
                                platformTarget,
                                new[] { "cardboard" });
                            break;
                        }
                }
            }

#endif
        }

    }
}
