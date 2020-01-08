using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.XR;

public class PhysicsTests : XrFunctionalTestBase
{
    private bool raycastFired;

    private Texture2D mobileTexture;

    private List<XRNodeState> xrNodeList;

    private Vector3 xrCenterNodePos;
    private bool raycastDirection;

    [SetUp]
    public override void SetUp()
    {
        base.SetUp();

        xrNodeList = new List<XRNodeState>();

        InputTracking.trackingAcquired += InputTracking_trackingAcquired;
        InputTracking.trackingLost += InputTracking_trackingLost;
        InputTracking.nodeAdded += InputTracking_nodeAdded;
        InputTracking.nodeRemoved += InputTracking_nodeRemoved;

        raycastFired = raycastDirection = false;
    }

    [TearDown]
    public override void TearDown()
    {
        InputTracking.trackingAcquired -= InputTracking_trackingAcquired;
        InputTracking.trackingLost -= InputTracking_trackingLost;
        InputTracking.nodeAdded -= InputTracking_nodeAdded;
        InputTracking.nodeRemoved -= InputTracking_nodeRemoved;
        raycastFired = raycastDirection = false;
        base.TearDown();
    }

    [UnityTest]
    public IEnumerator GazeCheck()
    {
        yield return SkipFrame(DefaultFrameSkipCount);

        InputTracking.GetNodeStates(xrNodeList);
        yield return SkipFrame(DefaultFrameSkipCount);
        
        if (xrNodeList.Count != 0)
        {
            foreach (XRNodeState nodeState in xrNodeList)
            {
                if (nodeState.nodeType == XRNode.CenterEye)
                {
                    nodeState.TryGetPosition(out xrCenterNodePos);
                }
            }

            yield return SkipFrame(100);

            Ray ray = new Ray(xrCenterNodePos, Camera.GetComponent<Camera>().transform.forward);
            Physics.Raycast(ray, 10f);
            yield return SkipFrame(DefaultFrameSkipCount);

            if (ray.origin == xrCenterNodePos)
            {
                raycastFired = true;
            }

            if (ray.direction != Vector3.zero)
            {
                raycastDirection = true;
            }

            if (Cube != null)
            {
                GameObject.Destroy(Cube);
            }

            if (Application.platform != RuntimePlatform.IPhonePlayer)
            {
                Assert.IsTrue(raycastFired, "Gaze ray failed to leave the cetner eye position");
                Assert.IsTrue(raycastDirection, "Gaze direction failed to travel!");
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
}

