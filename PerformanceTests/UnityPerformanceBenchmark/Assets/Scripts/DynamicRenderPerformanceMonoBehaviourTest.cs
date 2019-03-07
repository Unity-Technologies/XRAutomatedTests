using Unity.PerformanceTesting;

#if UNITY_2018_1_OR_NEWER // Unity Performance Testing Extension supported on 2018.1 or newer


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

#if UNITY_ANDROID || UNITY_IOS
    protected override SampleGroupDefinition CurrentBatterySg
    {
        get { return new SampleGroupDefinition("CurrentBattery", SampleUnit.None, AggregationType.Min); }
    }

    protected override SampleGroupDefinition BatteryTempSg
    {
        get { return new SampleGroupDefinition("BatteryTemp", SampleUnit.None, AggregationType.Min); }
    }

    protected override SampleGroupDefinition CpuScoreSg
    {
        get { return new SampleGroupDefinition("CpuScore", SampleUnit.Millisecond, AggregationType.Min); }
    }

    protected override SampleGroupDefinition MemScoreSg
    {
        get { return new SampleGroupDefinition("MemScore", SampleUnit.Byte, AggregationType.Min); }
    }
#endif
}
#endif