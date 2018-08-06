using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using UnityEditor;
using System;

namespace Tests
{
    [PrebuildSetup(typeof(EnableVRPrebuildStep))]
    public class DayDreamTestBase
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
                UnityEditorInternal.VR.VREditor.SetVREnabledDevicesOnTargetGroup(
                    BuildTargetGroup.Android,
                    new[] {"daydream"});
                EditorUserBuildSettings.androidBuildType = AndroidBuildType.Development;
                EditorUserBuildSettings.androidBuildSystem = AndroidBuildSystem.Internal;
                PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel24;
#endif
            }
        }
    }
}
