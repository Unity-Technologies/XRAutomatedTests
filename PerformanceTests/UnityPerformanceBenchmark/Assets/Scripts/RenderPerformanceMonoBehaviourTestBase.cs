#if UNITY_2018_1_OR_NEWER // Unity Performance Testing Extension supported on 2018.1 or newer
using System;
using System.Linq;
using Unity.PerformanceTesting;
using UnityEngine;
using UnityEngine.TestTools;
#if UNITY_ANALYTICS
using UnityEngine.Analytics;
#endif
#if ENABLE_VR && UNITY_2017_1_OR_NEWER
using UnityEngine.XR;
#endif

public abstract class RenderPerformanceMonoBehaviourTestBase : MonoBehaviour, IMonoBehaviourTest
{
    // Number of frames we capture and calculate metrics from
    private readonly int numCaptureFrames = 200;
    protected static readonly string FpsName = "FPS";
    private static readonly string ObjectCountName = "ObjectCount";
    private static readonly string VerticesName = "Vertices";
    private static readonly string TrianglesName = "Triangles";
#if UNITY_ANALYTICS && UNITY_2018_2_OR_NEWER
    private static readonly string AppStartupTimeName = "AppStartupTime";
#endif
#if ENABLE_VR
    protected static readonly string GpuTimeLastFrameName = "GpuTimeLastFrame";
#endif
#if UNITY_ANALYTICS && UNITY_2018_2_OR_NEWER
    private static long appStartupTime;
#endif

    private long verts;
    private long tris;
    private long renderedGameObjects;
    private int startFrameCount;

    // SampleGroupDefinitions
    private readonly SampleGroupDefinition objCountSg = new SampleGroupDefinition(ObjectCountName, SampleUnit.None);
    private readonly SampleGroupDefinition trianglesSg = new SampleGroupDefinition(TrianglesName, SampleUnit.None);
    private readonly SampleGroupDefinition verticesSg = new SampleGroupDefinition(VerticesName, SampleUnit.None);
#if UNITY_ANALYTICS && UNITY_2018_2_OR_NEWER
    private readonly SampleGroupDefinition startupTimeSg = new SampleGroupDefinition(AppStartupTimeName);
#endif
    protected abstract SampleGroupDefinition FpsSg { get; }
#if ENABLE_VR
    protected abstract SampleGroupDefinition GpuTimeLastFrameSg { get; }
#endif

    public int FrameCount { get; private set; }

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

    public bool IsMetricsCaptured
    {
        get { return FrameCount >= numCaptureFrames; }
    }

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
        var thisCamera = Camera.main.gameObject.GetComponent<Camera>();
        if (thisCamera != null)
        {
            XRDevice.DisableAutoXRCameraTracking(thisCamera, true);
        }
#endif
        
    }

    public virtual void RunStartCode()
    {
#if UNITY_ANALYTICS && UNITY_2018_2_OR_NEWER
        appStartupTime = PerformanceReporting.graphicsInitializationFinishTime / 1000;
#endif
    }

    private void Start()
    {
#if ENABLE_VR
        if (XRDevice.isPresent && !XRSettings.isDeviceActive)
        {
            Debug.LogAssertion("Expect XRSettings.isDeviceActive to be true, but it is false. Terminating test.");
        }
#endif

        RunStartCode();
    }

    private void Update()
    {
        if (CaptureMetrics)
        {
            FrameCount++;
            SampleFps();
#if ENABLE_VR
            SampleGpuTimeLastFrame();
#endif
        }

        if (IsMetricsCaptured)
        {
            EndMetricCapture();
        }
    }

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

#if ENABLE_VR
    private void SampleGpuTimeLastFrame()
    {
        var gpuTimeLastFrame = GetGpuTimeLastFrame();
        Measure.Custom(GpuTimeLastFrameSg, gpuTimeLastFrame * 1000);
    }

    private float GetGpuTimeLastFrame()
    {
        float GpuTimeLastFrame;
        float renderTime = 0;
        if (XRStats.TryGetGPUTimeLastFrame(out GpuTimeLastFrame))
        {
            renderTime = GpuTimeLastFrame;
        }
        return renderTime;
    }
#endif

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