using Unity.PerformanceTesting;

#if UNITY_2018_1_OR_NEWER // Unity Performance Testing Extension supported on 2018.1 or newer


/// <summary>
/// For dynamic scenes, we might choose to aggregate some metrics using something other than the default median
/// so we allow for this here.
/// </summary>
public class DynamicRenderPerformanceMonoBehaviourTest : RenderPerformanceMonoBehaviourTestBase
{
    protected override SampleGroupDefinition FpsSg {
        get
        {
            return new SampleGroupDefinition(FpsName, SampleUnit.None, AggregationType.Median, threshold: 0.15, increaseIsBetter: true);
        }
    }

    protected override SampleGroupDefinition GpuTimeLastFrameSg
    {
        get { return new SampleGroupDefinition(GpuTimeLastFrameName); }
    }

#if UNITY_ANDROID || UNITY_IOS
    protected override SampleGroupDefinition CurrentBatterySg
    {
        get { return new SampleGroupDefinition("CurrentBattery", SampleUnit.None); }
    }

    protected override SampleGroupDefinition BatteryTempSg
    {
        get { return new SampleGroupDefinition("BatteryTemp", SampleUnit.None); }
    }

    protected override SampleGroupDefinition CpuScoreSg
    {
        get { return new SampleGroupDefinition("CpuScore"); }
    }

    protected override SampleGroupDefinition MemScoreSg
    {
        get { return new SampleGroupDefinition("MemScore", SampleUnit.Byte); }
    }
#endif
}
#endif