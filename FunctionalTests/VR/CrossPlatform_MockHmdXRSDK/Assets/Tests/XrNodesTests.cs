#if !MOCKHMD_SDK
using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using UnityEngine.XR;
using System.Collections.Generic;
using System.Linq;

internal class XrNodesTests : XrFunctionalTestBase
{
    private List<XRNodeState> nodeList;
    
    [SetUp]
    public override void SetUp()
    {
        base.SetUp();
        nodeList = new List<XRNodeState>();

        InputTracking.trackingAcquired += InputTracking_trackingAcquired;
        InputTracking.trackingLost += InputTracking_trackingLost;
        InputTracking.nodeAdded += InputTracking_nodeAdded;
        InputTracking.nodeRemoved += InputTracking_nodeRemoved;
    }

    [TearDown]
    public override void TearDown()
    {
        InputTracking.trackingAcquired -= InputTracking_trackingAcquired;
        InputTracking.trackingLost -= InputTracking_trackingLost;
        InputTracking.nodeAdded -= InputTracking_nodeAdded;
        InputTracking.nodeRemoved -= InputTracking_nodeRemoved;
        base.TearDown();
    }
    
    [UnityTest]
    public IEnumerator XrNodesHeadTracking()
    {
        InputTracking.GetNodeStates(nodeList);
        yield return SkipFrame(DefaultFrameSkipCount);

        var headNodes = nodeList.Where(n => n.nodeType == XRNode.Head);
        Assert.True(headNodes.Any(), "Failed to find XRNode.Head node type.");
        Assert.True(headNodes.Select(n=>n.tracked == false).Any(), "Found untracked head node.");
    }

    [UnityTest]
    public IEnumerator XrNodesEyeTracking()
    {
        InputTracking.GetNodeStates(nodeList);
        yield return SkipFrame(DefaultFrameSkipCount);

        // Verify left eye node
        var leftEyeNodes = nodeList.Where(n => n.nodeType == XRNode.LeftEye);
        Assert.True(leftEyeNodes.Any(), "Failed to find XRNode.LeftEye node type.");
        Assert.True(leftEyeNodes.Select(n => n.tracked == false).Any(), "Found untracked left eye node.");

        // Verify right eye node
        var rightEyeNodes = nodeList.Where(n => n.nodeType == XRNode.RightEye);
        Assert.True(rightEyeNodes.Any(), "Failed to find XRNode.RightEye node type.");
        Assert.True(rightEyeNodes.Select(n => n.tracked == false).Any(), "Found untracked right eye node.");
    }

    private void InputTracking_nodeAdded(XRNodeState obj)
    {
        Debug.Log("Node Added : " + obj.nodeType);
    }

    private void InputTracking_trackingAcquired(XRNodeState obj)
    {
        Debug.Log("Tracking Acquired: " + obj.nodeType);
    }

    private void InputTracking_trackingLost(XRNodeState obj)
    {
        Debug.Log("Tracking Lost : " + obj.nodeType);
    }

    private void InputTracking_nodeRemoved(XRNodeState obj)
    {
        Debug.Log("Node Removed : " + obj.nodeType);
    }
}
#endif