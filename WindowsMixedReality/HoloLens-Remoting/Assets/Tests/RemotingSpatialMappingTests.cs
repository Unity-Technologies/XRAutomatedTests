using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.XR;
using UnityEngine.XR.WSA;

[Ignore("Remoting is in a unstable state for controlling the device")]
internal class RemotingSpatialMappingTests : HoloLensTestBaseRemoting
{
    const float k_SpatialRenderWait = 4f;
    const float k_SecondsBetweenUpdates = 1f;
    const float k_SphereRadius = 3f;

    const int k_NumUpdatesBeforeRemoval = 3;

    GameObject m_SpatialComponents = null;
    SpatialMappingCollider m_Collider;
    SpatialMappingRenderer m_Renderer;
    SpatialMappingBase m_Mapping;
    Vector3 m_AxisBox;

    [SetUp]
    public void Setup()
    {
        m_SpatialComponents = new GameObject("SpatialComponents");

        m_Renderer = m_SpatialComponents.AddComponent<SpatialMappingRenderer>();
        m_Collider = m_SpatialComponents.AddComponent<SpatialMappingCollider>();

        m_Renderer.lodType = SpatialMappingBase.LODType.High;
        m_Collider.lodType = SpatialMappingBase.LODType.High;

        m_Renderer.renderState = SpatialMappingRenderer.RenderState.Visualization;
    }

    [TearDown]
    public void TearDown()
    {
        Object.Destroy(m_SpatialComponents);
        Object.Destroy(m_Renderer);
        Object.Destroy(m_Collider);
    }

    [UnityTest]
    public IEnumerator BasicMappingTest()
    {
        yield return new WaitForSeconds(k_RemotingWait);
        m_StreamerState = PerceptionRemoting.GetConnectionState();
        Assert.AreEqual(HolographicStreamerConnectionState.Connected, m_StreamerState, "Holographic Streamer Connection Failure");

        yield return new WaitForSeconds(3f);

        Assert.IsTrue(m_Collider.isActiveAndEnabled, "Spatial Collider is not active");
        Assert.IsTrue(m_Renderer.isActiveAndEnabled, "Spatial Renderer is not active");
    }

    /// <summary>
    /// Commented out until the Device has a home were we can plan for surface collision
    /// </summary>

    //[UnityTest]
    //public IEnumerator SpatialColliderCheck()
    //{
    //    GameObject.Destroy(m_Cube);

    //    yield return new WaitForSeconds(k_SpatialRenderWait);

    //    Assert.IsTrue(m_SpatialCollison, "Didn't hit anything");
    //}

    [UnityTest]
    public IEnumerator SpatialRendererLodTypeScriptControl()
    {
        yield return new WaitForSeconds(k_RemotingWait);
        m_StreamerState = PerceptionRemoting.GetConnectionState();
        Assert.AreEqual(HolographicStreamerConnectionState.Connected, m_StreamerState, "Holographic Streamer Connection Failure");
        
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
        yield return new WaitForSeconds(k_RemotingWait);
        m_StreamerState = PerceptionRemoting.GetConnectionState();
        Assert.AreEqual(HolographicStreamerConnectionState.Connected, m_StreamerState, "Holographic Streamer Connection Failure");

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
        yield return new WaitForSeconds(k_RemotingWait);
        m_StreamerState = PerceptionRemoting.GetConnectionState();
        Assert.AreEqual(HolographicStreamerConnectionState.Connected, m_StreamerState, "Holographic Streamer Connection Failure");

        yield return new WaitForSeconds(k_SpatialRenderWait);
        m_Renderer.freezeUpdates = true;
        Assert.IsTrue(m_Renderer.freezeUpdates, "Didn't Freeze Renderer Updates");
        m_Collider.freezeUpdates = true;
        Assert.IsTrue(m_Collider.freezeUpdates, "Didn't Freeze Collider Updates");
    }

    [UnityTest]
    public IEnumerator SpatialResumeUpdatesScriptControl()
    {
        yield return new WaitForSeconds(k_RemotingWait);
        m_StreamerState = PerceptionRemoting.GetConnectionState();
        Assert.AreEqual(HolographicStreamerConnectionState.Connected, m_StreamerState, "Holographic Streamer Connection Failure");

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
        yield return new WaitForSeconds(k_RemotingWait);
        m_StreamerState = PerceptionRemoting.GetConnectionState();
        Assert.AreEqual(HolographicStreamerConnectionState.Connected, m_StreamerState, "Holographic Streamer Connection Failure");

        yield return new WaitForSeconds(k_SpatialRenderWait);
        m_Renderer.secondsBetweenUpdates = k_SecondsBetweenUpdates;
        Assert.AreEqual(m_Renderer.secondsBetweenUpdates, k_SecondsBetweenUpdates, "Seconds between updates failed");
        m_Collider.secondsBetweenUpdates = k_SecondsBetweenUpdates;
        Assert.AreEqual(m_Collider.secondsBetweenUpdates, k_SecondsBetweenUpdates, "Seconds between updates failed");
    }

    [UnityTest]
    public IEnumerator SpatialRemovalMeshTime()
    {
        yield return new WaitForSeconds(k_RemotingWait);
        m_StreamerState = PerceptionRemoting.GetConnectionState();
        Assert.AreEqual(HolographicStreamerConnectionState.Connected, m_StreamerState, "Holographic Streamer Connection Failure");

        yield return new WaitForSeconds(k_SpatialRenderWait);
        m_Renderer.numUpdatesBeforeRemoval = k_NumUpdatesBeforeRemoval;
        Assert.AreEqual(m_Renderer.numUpdatesBeforeRemoval, k_NumUpdatesBeforeRemoval, "Seconds between removals failed");
        m_Collider.numUpdatesBeforeRemoval = k_NumUpdatesBeforeRemoval;
        Assert.AreEqual(m_Collider.numUpdatesBeforeRemoval, k_NumUpdatesBeforeRemoval, "Seconds between removals failed");
    }

    [UnityTest]
    public IEnumerator VolumeBoxCheck()
    {
        yield return new WaitForSeconds(k_RemotingWait);
        m_StreamerState = PerceptionRemoting.GetConnectionState();
        Assert.AreEqual(HolographicStreamerConnectionState.Connected, m_StreamerState, "Holographic Streamer Connection Failure");

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
        yield return new WaitForSeconds(k_RemotingWait);
        m_StreamerState = PerceptionRemoting.GetConnectionState();
        Assert.AreEqual(HolographicStreamerConnectionState.Connected, m_StreamerState, "Holographic Streamer Connection Failure");

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

}
