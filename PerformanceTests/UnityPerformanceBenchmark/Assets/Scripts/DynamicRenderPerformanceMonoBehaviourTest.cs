#if UNITY_2018_1_OR_NEWER // Unity Performance Testing Extension supported on 2018.1 or newer
using Unity.PerformanceTesting;


/// <summary>
/// For dynamic scenes, we aggregate some metrics using the min value as other aggregation types can vary widely
/// </summary>
public class DynamicRenderPerformanceMonoBehaviourTest : RenderPerformanceMonoBehaviourTestBase
{
    protected override SampleGroupDefinition FpsSg {
        get
        {
            return new SampleGroupDefinition(FpsName, SampleUnit.None, AggregationType.Min, threshold: 0.15, increaseIsBetter: true);
        }
    }

    protected override SampleGroupDefinition GpuTimeLastFrameSg
    {
        get { return new SampleGroupDefinition(GpuTimeLastFrameName, SampleUnit.Millisecond, AggregationType.Min); }
    }
}
#endif