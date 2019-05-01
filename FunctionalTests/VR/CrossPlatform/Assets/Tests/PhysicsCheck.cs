using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.TestTools;
using UnityEngine.XR;

public class PhysicsCheck : TestBaseSetup
{
    private bool m_RaycastFired = false;
    private float kDeviceSetupWait = 1f;

    private Texture2D m_MobileTexture;

    private List<XRNodeState> m_XrNodeList;

    private Vector3 m_XrCenterNodePos;
    private bool m_RaycastDirection;

    [SetUp]
    public override void SetUp()
    {
        base.SetUp();

        m_XrNodeList = new List<XRNodeState>();

        InputTracking.trackingAcquired += InputTracking_trackingAcquired;
        InputTracking.trackingLost += InputTracking_trackingLost;
        InputTracking.nodeAdded += InputTracking_nodeAdded;
        InputTracking.nodeRemoved += InputTracking_nodeRemoved;

        m_RaycastFired = m_RaycastDirection = false;
    }

    [TearDown]
    public override void TearDown()
    {
        InputTracking.trackingAcquired -= InputTracking_trackingAcquired;
        InputTracking.trackingLost -= InputTracking_trackingLost;
        InputTracking.nodeAdded -= InputTracking_nodeAdded;
        InputTracking.nodeRemoved -= InputTracking_nodeRemoved;
        m_RaycastFired = m_RaycastDirection = false;
        base.TearDown();
    }

    [UnityTest]
    public IEnumerator GazeCheck()
    {
        yield return new WaitForSeconds(kDeviceSetupWait);

        InputTracking.GetNodeStates(m_XrNodeList);
        yield return new WaitForSeconds(1f);
        
        if (m_XrNodeList.Count != 0)
        {
            foreach (XRNodeState nodeState in m_XrNodeList)
            {
                if (nodeState.nodeType == XRNode.CenterEye)
                {
                    nodeState.TryGetPosition(out m_XrCenterNodePos);
                }
            }

            SkipFrame(100);

            Ray ray = new Ray(m_XrCenterNodePos, m_TestSetupHelpers.m_Camera.GetComponent<Camera>().transform.forward);
            Physics.Raycast(ray, 10f);
            yield return null;

            if (ray.origin == m_XrCenterNodePos)
            {
                m_RaycastFired = true;
            }

            if (ray.direction != Vector3.zero)
            {
                m_RaycastDirection = true;
            }

            if (m_TestSetupHelpers.m_Cube != null)
            {
                GameObject.Destroy(m_TestSetupHelpers.m_Cube);
            }

            if (Application.platform != RuntimePlatform.IPhonePlayer)
            {
                Assert.IsTrue(m_RaycastFired, "Gaze ray failed to leave the cetner eye position");
                Assert.IsTrue(m_RaycastDirection, "Gaze direction failed to travel!");
            }
        }
        else
        {
            Assert.Fail("Nodes are not being Tracked");
        }
    }

    private void InputTracking_nodeRemoved(XRNodeState obj)
    {
        Debug.Log("Node Removed");
    }

    private void InputTracking_nodeAdded(XRNodeState obj)
    {
        Debug.Log("Node Added");
    }

    private void InputTracking_trackingLost(XRNodeState obj)
    {
        Debug.Log("Tracking Lost");
    }

    private void InputTracking_trackingAcquired(XRNodeState obj)
    {
        Debug.Log("Tracking Acquire");
    }

    IEnumerator SkipFrame(int frames)
    {
        for (int f = 0; f < frames; f++)
        {
            yield return null;
            Debug.Log("Skip Frame");
        }
    }
}

