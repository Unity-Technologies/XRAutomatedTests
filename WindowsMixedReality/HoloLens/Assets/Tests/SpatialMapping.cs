using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using UnityEditorInternal.VR;
using UnityEditor;
using UnityEngine.XR.WSA;
using UnityEngine.XR.WSA.Input;

[Ignore("ignore for now")]
[Platform("Win")]
internal class SpatialMapping : HoloLensTestBase
{
    GameObject m_SpatialComponents = null;
    SpatialMappingCollider m_Collider;
    SpatialMappingRenderer m_Renderer;
    SpatialMapping m_Mapping;
    GestureRecognizer m_GestureRecognizer = null;
    Vector3 m_AxisBox;

    bool m_SpatialCollison = false;
    bool m_SpatialSurfaceUpdating = false;
    bool m_SourcePressed = false;
    bool m_SourceReleased = false;

    const float k_GestureTapWait = 0.3f;
    const float k_SpatialRenderWait = 4f;
    const float k_SecondsBetweenUpdates = 1f;
    const float k_SphereRadius = 3f;

    const int k_NumUpdatesBeforeRemoval = 3;

    string m_Room = EditorApplication.applicationContentsPath +
        "/UnityExtensions/Unity/VR/HolographicSimulation/Rooms/DefaultRoom.xef";

    [SetUp]
    public void Setup()
    {
        m_SpatialCollison = m_SpatialSurfaceUpdating = m_SourcePressed = m_SourceReleased = false;

        HolographicAutomation.LoadRoom(m_Room);

        rightHand.activated = true;
        rightHand.position = body.position;

        m_SpatialComponents = new GameObject("SpatialComponents");

        m_Renderer = m_SpatialComponents.AddComponent<SpatialMappingRenderer>();
        m_Collider = m_SpatialComponents.AddComponent<SpatialMappingCollider>();

        m_Renderer.lodType = SpatialMappingBase.LODType.High;
        m_Collider.lodType = SpatialMappingBase.LODType.High;

        m_Renderer.renderState = SpatialMappingRenderer.RenderState.Visualization;

        m_GestureRecognizer = new GestureRecognizer();
        m_GestureRecognizer.SetRecognizableGestures(GestureSettings.Tap);
        m_GestureRecognizer.Tapped += M_GestureRecognizer_TappedEvent;
        m_GestureRecognizer.StartCapturingGestures();

        InteractionManager.InteractionSourcePressed += InteractionManager_SourcePressed;
        InteractionManager.InteractionSourceReleased += InteractionManager_SourceReleased;
        InteractionManager.GetCurrentReading();

        m_AxisBox = new Vector3(1f, 1f, 1f);
    }

    [TearDown]
    public void TearDown()
    {
        m_GestureRecognizer.Tapped -= M_GestureRecognizer_TappedEvent;
        m_GestureRecognizer.StopCapturingGestures();
        m_GestureRecognizer.Dispose();

        InteractionManager.InteractionSourcePressed -= InteractionManager_SourcePressed;
        InteractionManager.InteractionSourceReleased -= InteractionManager_SourceReleased;

        Object.Destroy(m_SpatialComponents);
        Object.Destroy(m_Renderer);
        Object.Destroy(m_Collider);
    }

    [UnityTest]
    public IEnumerator BasicMappingCheck()
    {
        yield return new WaitForSeconds(k_SpatialRenderWait);
        Assert.IsTrue(m_Collider.isActiveAndEnabled, "Spatial Collider is not active");
        Assert.IsTrue(m_Renderer.isActiveAndEnabled, "Spatial Renderer is not active");
    }

    //[Ignore("Metro/wsa is disabled on Katana which might causing this test to fail")]
    [UnityTest]
    public IEnumerator SpatialColliderCheck()
    {
        GameObject.Destroy(m_Cube);

        yield return new WaitForSeconds(k_SpatialRenderWait);
        rightHand.EnsureVisible();
        yield return new WaitForSeconds(2f);

        rightHand.PerformGesture(SimulatedGesture.FingerPressed);
        yield return new WaitForSeconds(k_GestureTapWait);
        Assert.IsTrue(m_SourcePressed, "Finger Press wasn't detected");

        rightHand.PerformGesture(SimulatedGesture.FingerReleased);
        yield return new WaitForSeconds(k_GestureTapWait);
        Assert.IsTrue(m_SourceReleased, "Finger Release wasn't detected");

        yield return new WaitForSeconds(k_GestureTapWait);
        Assert.IsTrue(m_SpatialCollison, "Didn't hit anything");
    }

    [UnityTest]
    public IEnumerator SpatialRendererLodTypeScriptControl()
    {
        yield return new WaitForSeconds(k_SpatialRenderWait);
        m_Renderer.lodType = SpatialMappingBase.LODType.Medium;
        Assert.AreEqual(m_Renderer.lodType, SpatialMappingBase.LODType.Medium, "LOD type was not set for the renderer");
        m_Renderer.lodType = SpatialMappingBase.LODType.Low;
        Assert.AreEqual(m_Renderer.lodType, SpatialMappingBase.LODType.Low, "LOD type was not set for the renderer");
        m_Renderer.lodType = SpatialMappingBase.LODType.High;
        Assert.AreEqual(m_Renderer.lodType, SpatialMappingBase.LODType.High, "LOD type was not set for the renderer");
    }

    [UnityTest]
    public IEnumerator SpatialColliderLodTypeScriptControl()
    {
        yield return new WaitForSeconds(k_SpatialRenderWait);
        m_Collider.lodType = SpatialMappingBase.LODType.Medium;
        Assert.AreEqual(m_Collider.lodType, SpatialMappingBase.LODType.Medium, "LOD type was not set for the renderer");
        m_Collider.lodType = SpatialMappingBase.LODType.Low;
        Assert.AreEqual(m_Collider.lodType, SpatialMappingBase.LODType.Low, "LOD type was not set for the renderer");
        m_Collider.lodType = SpatialMappingBase.LODType.High;
        Assert.AreEqual(m_Collider.lodType, SpatialMappingBase.LODType.High, "LOD type was not set for the renderer");
    }

    [UnityTest]
    public IEnumerator SpatialFreezeUpdatesScriptControl()
    {
        yield return new WaitForSeconds(k_SpatialRenderWait);
        m_Renderer.freezeUpdates = true;
        Assert.IsTrue(m_Renderer.freezeUpdates, "Didn't Freeze Renderer Updates");
        m_Collider.freezeUpdates = true;
        Assert.IsTrue(m_Collider.freezeUpdates, "Didn't Freeze Collider Updates");
    }

    [UnityTest]
    public IEnumerator SpatialResumeUpdatesScriptControl()
    {
        yield return new WaitForSeconds(k_SpatialRenderWait);
        m_Renderer.freezeUpdates = false;
        Assert.IsFalse(m_Renderer.freezeUpdates, "Didn't Resume Renderer Updates");
        yield return new WaitForSeconds(3f);
        m_Collider.freezeUpdates = false;
        Assert.IsFalse(m_Collider.freezeUpdates, "Didn't Resume Collider Updates");
    }

    [UnityTest]
    public IEnumerator SpatialUpdateMeshTime()
    {
        yield return new WaitForSeconds(k_SpatialRenderWait);
        m_Renderer.secondsBetweenUpdates = k_SecondsBetweenUpdates;
        Assert.AreEqual(m_Renderer.secondsBetweenUpdates, k_SecondsBetweenUpdates, "Seconds between updates failed");
        m_Collider.secondsBetweenUpdates = k_SecondsBetweenUpdates;
        Assert.AreEqual(m_Collider.secondsBetweenUpdates, k_SecondsBetweenUpdates, "Seconds between updates failed");
    }

    [UnityTest]
    public IEnumerator SpatialRemovalMeshTime()
    {
        yield return new WaitForSeconds(k_SpatialRenderWait);
        m_Renderer.numUpdatesBeforeRemoval = k_NumUpdatesBeforeRemoval;
        Assert.AreEqual(m_Renderer.numUpdatesBeforeRemoval, k_NumUpdatesBeforeRemoval, "Seconds between removals failed");
        m_Collider.numUpdatesBeforeRemoval = k_NumUpdatesBeforeRemoval;
        Assert.AreEqual(m_Collider.numUpdatesBeforeRemoval, k_NumUpdatesBeforeRemoval, "Seconds between removals failed");
    }

    [UnityTest]
    public IEnumerator VolumeBoxCheck()
    {
        yield return new WaitForSeconds(k_SpatialRenderWait);
        Assert.AreEqual(m_Renderer.volumeType, SpatialMappingBase.VolumeType.AxisAlignedBox, "Renderer Volume Type failed for Axis Box");
        m_Renderer.halfBoxExtents = m_AxisBox;
        Assert.AreEqual(m_Renderer.halfBoxExtents, m_AxisBox, "Axis Box Extents for Renderer were not set properly");

        Assert.AreEqual(m_Collider.volumeType, SpatialMappingBase.VolumeType.AxisAlignedBox, "Collider Volume Type failed for Axis Box");
        m_Collider.halfBoxExtents = m_AxisBox;
        Assert.AreEqual(m_Collider.halfBoxExtents, m_AxisBox, "Axis Box Extents for Collider were not set properly");
    }

    [UnityTest]
    public IEnumerator VolumeSphereCheck()
    {
        yield return new WaitForSeconds(k_SpatialRenderWait);
        m_Renderer.volumeType = SpatialMappingBase.VolumeType.Sphere;
        Assert.AreEqual(m_Renderer.volumeType, SpatialMappingBase.VolumeType.Sphere, "Renderer Volume Type failed for Sphere");
        m_Renderer.sphereRadius = k_SphereRadius;
        Assert.AreEqual(m_Renderer.sphereRadius, k_SphereRadius, "Sphere Radius was not set");

        m_Collider.volumeType = SpatialMappingBase.VolumeType.Sphere;
        Assert.AreEqual(m_Collider.volumeType, SpatialMappingBase.VolumeType.Sphere, "Collider Volume Type failed for Sphere");
        m_Collider.sphereRadius = k_SphereRadius;
        Assert.AreEqual(m_Collider.sphereRadius, k_SphereRadius, "Sphere Radius was not set");
    }

    private void M_GestureRecognizer_TappedEvent(TappedEventArgs obj)
    {
        Ray ray = new Ray(obj.headPose.position, obj.headPose.rotation * Vector3.forward);
        RaycastHit rayCastHitInfo;

        if (Physics.Raycast(ray, out rayCastHitInfo, 20f))
        {
            Debug.Log("Hit: " + rayCastHitInfo.collider.name);
            m_SpatialCollison = true;
        }
    }

    private void InteractionManager_SourceReleased(InteractionSourceReleasedEventArgs obj)
    {
        m_SourceReleased = true;
    }

    private void InteractionManager_SourcePressed(InteractionSourcePressedEventArgs obj)
    {
        m_SourcePressed = true;
    }
}
