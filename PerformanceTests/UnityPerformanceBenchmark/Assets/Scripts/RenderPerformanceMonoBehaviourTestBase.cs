#if UNITY_2018_1_OR_NEWER // Unity Performance Testing Extension supported on 2018.1 or newer
using System;
using System.Collections;
using System.Linq;
using Unity.PerformanceTesting;
using UnityEngine;
using UnityEngine.TestTools;
#if UNITY_ANALYTICS
using UnityEngine.Analytics;
#endif
#if ENABLE_VR
using UnityEngine.XR;
#endif

public abstract class RenderPerformanceMonoBehaviourTestBase : MonoBehaviour, IMonoBehaviourTest
{
#if UNITY_ANDROID || UNITY_IOS
    private readonly int numCaptureSeconds = 30;
#else
    private readonly int numCaptureSeconds = 10;
#endif
    protected static readonly string FpsName = "FPS";
    private static readonly string ObjectCountName = "ObjectCount";
    private static readonly string VerticesName = "Vertices";
    private static readonly string TrianglesName = "Triangles";
    private static readonly string AppStartupTimeName = "AppStartupTime";
    private static long appStartupTime;

    protected static readonly string GpuTimeLastFrameName = "GpuTimeLastFrame";


    private long verts;
    private long tris;
    private long renderedGameObjects;
    private int startFrameCount;

    // SampleGroupDefinitions
    private readonly SampleGroupDefinition objCountSg = new SampleGroupDefinition(ObjectCountName, SampleUnit.None);
    private readonly SampleGroupDefinition trianglesSg = new SampleGroupDefinition(TrianglesName, SampleUnit.None);
    private readonly SampleGroupDefinition verticesSg = new SampleGroupDefinition(VerticesName, SampleUnit.None);
    private readonly SampleGroupDefinition startupTimeSg = new SampleGroupDefinition(AppStartupTimeName);

    protected abstract SampleGroupDefinition FpsSg { get; }
    protected abstract SampleGroupDefinition GpuTimeLastFrameSg { get; }
#if UNITY_ANDROID || UNITY_IOS
    protected abstract SampleGroupDefinition CurrentBatterySg { get; }
    protected abstract SampleGroupDefinition BatteryTempSg { get; }
    protected abstract SampleGroupDefinition CpuScoreSg { get; }
    protected abstract SampleGroupDefinition MemScoreSg { get; }
#endif

    private int frameCount;

    private float lastCpuScore;
    private float lastMemScore;
    private int reportFrame;

    public long Verts
    {
        get
        {
            if (verts == 0)
            {
                FindRenderedObjectMetrics();
            }

            return verts;
        }
    }

    public long Tris
    {
        get
        {
            if (tris == 0)
            {
                FindRenderedObjectMetrics();
            }

            return tris;
        }
    }

    public long RenderedGameObjects
    {
        get
        {
            if (renderedGameObjects == 0)
            {
                FindRenderedObjectMetrics();
            }

            return renderedGameObjects;
        }
    }

    public bool CaptureMetrics { get; set; }
    public float Fps { get; set; }
    public bool IsMetricsCaptured { get; set; }


    public bool IsTestFinished
    {
        get
        {
            bool isTestFinished = false;

            if (IsMetricsCaptured)
            {
                Measure.Custom(objCountSg, RenderedGameObjects);
                Measure.Custom(trianglesSg, Tris);
                Measure.Custom(verticesSg, Verts);
                isTestFinished = true;
            }

            return isTestFinished;
        }
    }

    public void Awake()
    {
#if ENABLE_VR && UNITY_2017_1_OR_NEWER
        if (XRSettings.enabled)
        {
            var thisCamera = Camera.main.gameObject.GetComponent<Camera>();
            if (thisCamera != null)
            {
                XRDevice.DisableAutoXRCameraTracking(thisCamera, true);
            }
        }
#endif
    }

    public virtual void RunStartCode()
    {
#if UNITY_ANALYTICS && UNITY_2018_2_OR_NEWER
        appStartupTime = PerformanceReporting.graphicsInitializationFinishTime / 1000;
#endif
    }

    IEnumerator Start()
    {
#if ENABLE_VR
        if (XRSettings.enabled && !XRSettings.isDeviceActive)
        {
            Debug.LogAssertion("Expect XRSettings.isDeviceActive to be true, but it is false. Terminating test.");
        }
#endif
        RunStartCode();

        yield return new WaitUntil(() => CaptureMetrics);

        yield return new WaitForSecondsRealtime(numCaptureSeconds);

        IsMetricsCaptured = true;
    }

    private void LateUpdate()
    {
        if (CaptureMetrics)
        {
#if UNITY_ANDROID || UNITY_IOS
            ReportMobileStats();
#endif
            
            SampleFps();
#if ENABLE_VR
            if (XRSettings.enabled)
            {
                SampleGpuTimeLastFrame();
            }
#endif
        }

        if (IsMetricsCaptured)
        {
            EndMetricCapture();
        }
    }

#if UNITY_ANDROID || UNITY_IOS
    void ReportMobileStats()
    {
        frameCount = (frameCount + 1) % 30;
       
        if (frameCount == 0)
        {
            // Benchmarks are expsensive so run them at 1/2 of the reporting freq
            if (reportFrame % 2 == 0)
            {
                lastCpuScore = Utils.GetMicroBenchmarkScore();
                lastMemScore = Utils.GetMemBenchmarkScore();
            }

            var currentBattery = Utils.GetBatteryCurrent();
            var batteryTemp = Utils.GetBatteryTemp();


            Measure.Custom(CurrentBatterySg, currentBattery);
            Measure.Custom(BatteryTempSg, batteryTemp);
            Measure.Custom(CpuScoreSg, lastCpuScore);
            Measure.Custom(MemScoreSg, lastMemScore);

            

            reportFrame++;
        }
    }
#endif

    public void EndMetricCapture()
    {
        CaptureMetrics = false;
#if UNITY_ANALYTICS && UNITY_2018_2_OR_NEWER
        Measure.Custom(startupTimeSg, appStartupTime);
#endif
    }

    private void SampleFps()
    {
        Fps = GetFps();
        Measure.Custom(FpsSg, Fps);
        startFrameCount = Time.renderedFrameCount;
    }

    private float GetFps()
    {
        return (Time.renderedFrameCount - startFrameCount) / Time.unscaledDeltaTime;
    }

    private void SampleGpuTimeLastFrame()
    {
        var gpuTimeLastFrame = GetGpuTimeLastFrame();
        Measure.Custom(GpuTimeLastFrameSg, gpuTimeLastFrame * 1000);
    }

    private float GetGpuTimeLastFrame()
    {
        float gpuTimeLastFrame;
        float renderTime = 0;
        if (XRStats.TryGetGPUTimeLastFrame(out gpuTimeLastFrame))
        {
            renderTime = gpuTimeLastFrame;
        }
        return renderTime;
    }

    public void FindRenderedObjectMetrics()
    {
        var meshVertexCount = 0;
        var trianglesLength = 0;

        var meshRenderers = FindObjectsOfType<MeshRenderer>().Where(mr => mr.isVisible);
        var meshRendererArray = meshRenderers.ToArray();
        if (meshRendererArray.Length > 0)
        {
            renderedGameObjects = meshRendererArray.Length;
            foreach (var meshRenderer in meshRendererArray)
            {
                var meshFilter = meshRenderer.GetComponent<MeshFilter>();
                meshVertexCount += meshFilter.mesh.vertexCount;
                try
                {
                    trianglesLength += meshFilter.mesh.triangles.Length;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }
        }

        verts = meshVertexCount;
        tris = trianglesLength;
    }
}
#endif