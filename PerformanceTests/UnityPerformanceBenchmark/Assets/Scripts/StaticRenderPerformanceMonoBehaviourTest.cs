#if UNITY_2018_1_OR_NEWER // Unity Performance Testing Extension supported on 2018.1 or newer
using Unity.PerformanceTesting;


public class StaticRenderPerformanceMonoBehaviourTest : RenderPerformanceMonoBehaviourTestBase
{
    protected override SampleGroupDefinition FpsSg {
        get
        {
            return new SampleGroupDefinition(FpsName, SampleUnit.None, AggregationType.Median, threshold: 0.15, increaseIsBetter: true);
        }
    }

    protected override SampleGroupDefinition GpuTimeLastFrameSg
    {
        get { return new SampleGroupDefinition(GpuTimeLastFrameName, SampleUnit.Millisecond, AggregationType.Median); }
    }
}
#endif