using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.XR;

public class PhysicsCheck : TestBaseSetup
{
    private bool m_RaycastHit = false;
    private float kDeviceSetupWait = 1f;

    private Texture2D m_MobileTexture;

    private List<XRNodeState> m_XrNodeList;
    private XRNodeState m_XrNodeState;
    private Vector3 m_XrHeadNodePos;

    void Start()
    {
        m_XrNodeState = new XRNodeState();
        m_XrNodeList = new List<XRNodeState>();

        InputTracking.trackingAcquired += InputTracking_trackingAcquired;
        InputTracking.trackingLost += InputTracking_trackingLost;
        InputTracking.nodeAdded += InputTracking_nodeAdded;
        InputTracking.nodeRemoved += InputTracking_nodeRemoved;
    }

    void Update()
    {
        InputTracking.GetNodeStates(m_XrNodeList);
    }

    [SetUp]
    public override void SetUp()
    {
        base.SetUp();
        m_TestSetupHelpers.TestCubeSetup(TestCubesConfig.TestCube);
    }

    [TearDown]
    public override void TearDown()
    {
        m_RaycastHit = false;
        base.TearDown();
    }

    [UnityTest]
    public IEnumerator GazeCheck()
    {
        yield return new WaitForSeconds(kDeviceSetupWait);
        
        InputTracking.Recenter();

        RaycastHit info = new RaycastHit();
        
        if (m_XrNodeState.tracked)
        {
            foreach (XRNodeState node in m_XrNodeList)
            {
                if (node.nodeType == XRNode.Head)
                {
                    node.TryGetPosition(out m_XrHeadNodePos);
                }
            }
            
            yield return new WaitForSeconds(1f);

            if (m_TestSetupHelpers.m_Cube != null)
            {
                m_TestSetupHelpers.m_Cube.transform.position =
                    new Vector3(m_XrHeadNodePos.x, m_XrHeadNodePos.y, m_XrHeadNodePos.z + 2f);
            }
            else if (m_TestSetupHelpers.m_Cube == null)
            {
                m_TestSetupHelpers.TestCubeSetup(TestCubesConfig.TestCube);
                m_TestSetupHelpers.m_Cube.transform.position =
                    new Vector3(m_XrHeadNodePos.x, m_XrHeadNodePos.y, m_XrHeadNodePos.z + 2f);
            }

            yield return new WaitForSeconds(2f);

            if (Physics.Raycast(m_XrHeadNodePos, m_TestSetupHelpers.m_Camera.GetComponent<Camera>().transform.forward,
                out info, 10f))
            {
                yield return new WaitForSeconds(0.05f);
                if (info.collider.name == m_TestSetupHelpers.m_Cube.name)
                {
                    m_RaycastHit = true;
                }
            }

            if (m_TestSetupHelpers.m_Cube != null)
            {
                GameObject.Destroy(m_TestSetupHelpers.m_Cube);
            }

            if (Application.platform != RuntimePlatform.IPhonePlayer)
            {
                Assert.IsTrue(m_RaycastHit, "Gaze check failed to hit something!");
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

