using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using UnityEngine.XR;
using System.Collections.Generic;

internal class XrNodes : TestBaseSetup
{
    private List<XRNodeState> m_NodeList;

    private bool m_TrackingNodes;
    private bool m_TrackingHeadNode;
    private bool m_TrackingRightEyeNode;
    private bool m_TrackingLeftEyeNode;

    public bool m_TrackingEyeNode { get; private set; }

    [SetUp]
    public override void SetUp()
    {
        base.SetUp();
        m_NodeList = new List<XRNodeState>();

        InputTracking.trackingAcquired += InputTracking_trackingAcquired;
        InputTracking.trackingLost += InputTracking_trackingLost;
        InputTracking.nodeAdded += InputTracking_nodeAdded;
        InputTracking.nodeRemoved += InputTracking_nodeRemoved;

        m_TrackingNodes = m_TrackingHeadNode = m_TrackingLeftEyeNode = m_TrackingRightEyeNode = false;
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
    public IEnumerator XrNodesTracking()
    {
        InputTracking.GetNodeStates(m_NodeList);
        yield return new WaitForSeconds(1f);

        foreach (XRNodeState nodeState in m_NodeList)
        {
            if (nodeState.tracked)
            {
                m_TrackingNodes = true;
            }
        }

        Assert.IsTrue(m_TrackingNodes, "Nodes are not tracking");
    }

    [UnityTest]
    public IEnumerator XrNodesHeadTracking()
    {
        InputTracking.GetNodeStates(m_NodeList);
        yield return new WaitForSeconds(1f);

        foreach (XRNodeState nodeState in m_NodeList)
        {
            if (nodeState.tracked)
            {
                if (nodeState.nodeType == XRNode.Head)
                {
                    m_TrackingHeadNode = true;
                }
            }
        }

        Assert.IsTrue(m_TrackingHeadNode, "Head Node is not tracking");
    }

    [UnityTest]
    public IEnumerator XrNodesEyeTracking()
    {
        InputTracking.GetNodeStates(m_NodeList);
        yield return new WaitForSeconds(1f);

        foreach (XRNodeState nodeState in m_NodeList)
        {
            if (nodeState.tracked)
            {
                if (nodeState.nodeType == XRNode.LeftEye)
                {
                    m_TrackingRightEyeNode = true;
                }

                if (nodeState.nodeType == XRNode.RightEye)
                {
                    m_TrackingLeftEyeNode = true;
                }

                if (m_TrackingLeftEyeNode == m_TrackingRightEyeNode)
                {
                    m_TrackingEyeNode = true;
                }
            }
        }

        Assert.IsTrue(m_TrackingEyeNode, "Eye Nodes are not tracking");
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
