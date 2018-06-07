using System.Collections;
using System.Collections.Generic;
using Unity.PerformanceTesting.Runtime;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Profiling;

namespace Unity.PerformanceTesting.Measurements
{
    internal class FrameTimeMeasurement : MonoBehaviour
    {
        public SampleGroupDefinition SampleGroupDefinition;

        void Update()
        {
            Measure.Custom(SampleGroupDefinition, Time.unscaledDeltaTime * 1000);
        }
    }
}