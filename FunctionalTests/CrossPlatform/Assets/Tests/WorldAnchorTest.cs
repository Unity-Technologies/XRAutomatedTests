using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.XR;
using System;
using System.Text;

#if UNITY_METRO
using UnityEngine.XR.WSA.Input;
using UnityEngine.XR.WSA.Persistence;
using UnityEngine.XR.WSA;
#endif

//[Ignore("Metro/wsa is disabled on Katana")]
internal class WorldAnchorTest : TestBaseSetup
{
#if UNITY_METRO
    List<string> messages = new List<string>();
    List<GameObject> objects = new List<GameObject>();
    WorldAnchorStore store = null;

    float m_AnchorWait = 1f;
    private float kDeviceSetupWait = 2f;

    bool m_CreatedAnchor = false;
    bool m_KeywordRecognized = false;
    bool m_ErasedAnchor = false;
    bool m_LoadedAnchors = false;

    [SetUp]
    public void Setup()
    {
        m_CreatedAnchor = m_KeywordRecognized = m_ErasedAnchor = m_LoadedAnchors = false;

        WorldAnchorStore.GetAsync(storeLoaded);
        WorldManager.OnPositionalLocatorStateChanged += WorldManager_OnPositionalLocatorStateChanged;
    }

    [TearDown]
    public void TearDown()
    {
        ClearAnchors();
    }

    [UnityTest]
    public IEnumerator CreateAnchorOnObject()
    {
        WmrDeviceCheck();
        yield return new WaitForSeconds(kDeviceSetupWait);

        var thing = CreateCubeAt(new Vector3(0f, 0f, 5f));

        yield return new WaitForSeconds(1f);

        var worldAnchor = thing.AddComponent<WorldAnchor>();
        var guid = Guid.NewGuid();

        if (thing.GetComponent<WorldAnchor>())
        {
            m_CreatedAnchor = true;
        }

        yield return new WaitForSeconds(m_AnchorWait);

        Debug.Log("Position of Cube: " + worldAnchor.transform.position);
        Debug.Log("isLocated = " + worldAnchor.isLocated);
        
        Assert.IsTrue(m_CreatedAnchor, "Anchor was not created");
    }

    [UnityTest]
    public IEnumerator GetAnchorSpatialPtr()
    {
        WmrDeviceCheck();
        var thing = CreateCubeAt(new Vector3(0f, 0f, 2.5f));
        var worldAnchor = thing.AddComponent<WorldAnchor>();
        var guid = Guid.NewGuid();

        yield return new WaitForSeconds(m_AnchorWait);

        Debug.Log("Position of Cube: " + worldAnchor.transform.position);
        Debug.Log("isLocated = " + worldAnchor.isLocated);

        var SpatialPtr = worldAnchor.GetNativeSpatialAnchorPtr();

        Assert.IsNotNull(SpatialPtr, "Spatial Ptr is empty");

        worldAnchor.SetNativeSpatialAnchorPtr(SpatialPtr);

        Assert.AreEqual(worldAnchor.GetNativeSpatialAnchorPtr(), SpatialPtr, "Target Anchor doesn't match current Anchor");
    }

    [Ignore("Bug causing this test to crash")]
    [UnityTest]
    public IEnumerator SetAnchorSpatialPtr()
    {
        WmrDeviceCheck();
        var thing = CreateCubeAt(new Vector3(0f, 0f, 2.5f));
        var worldAnchor = thing.AddComponent<WorldAnchor>();
        var guid = Guid.NewGuid();

        yield return new WaitForSeconds(m_AnchorWait);

        Debug.Log("Position of Cube: " + worldAnchor.transform.position);
        Debug.Log("isLocated = " + worldAnchor.isLocated);

        worldAnchor.SetNativeSpatialAnchorPtr(new IntPtr(25));
        var SpatialPtr = worldAnchor.GetNativeSpatialAnchorPtr();

        Assert.AreEqual(25, SpatialPtr, "Spatial Ptr is empty");
    }


    [UnityTest]
    public IEnumerator CreateManySave()
    {
        WmrDeviceCheck();
        yield return null;
        yield return new WaitForSeconds(m_AnchorWait);
        CreateManyAnchors();
        Assert.IsTrue(m_CreatedAnchor, "Didn't create the anchors");
    }

    [UnityTest]
    public IEnumerator CreateManyThanCount()
    {
        WmrDeviceCheck();
        yield return null;
        CreateManyAnchors();
        Assert.IsTrue(m_CreatedAnchor, "Didn't create the anchors");

        Assert.AreEqual(store.anchorCount, 2, "Store count doesn't match");
    }

    [UnityTest]
    public IEnumerator CreateManyGetIDs()
    {
        WmrDeviceCheck();
        yield return null;
        CreateManyAnchors();
        Assert.IsTrue(m_CreatedAnchor, "Didn't create the anchors");

        string[] storeIDs = new string[]{}; 
        store.GetAllIds(storeIDs);

        foreach (var id in storeIDs)
        {
            Assert.IsNotEmpty(id, "GUID is empty");
        }
    }

    [UnityTest]
    public IEnumerator CreateThenErase()
    {
        WmrDeviceCheck();
        CreateAnchor();

        Assert.IsTrue(m_CreatedAnchor, "Anchor wasn't created on Tap");
        yield return new WaitForSeconds(m_AnchorWait);

        ClearAnchors();
        Assert.IsTrue(m_ErasedAnchor, "Didn't erase the anchors");
    }

    [UnityTest]
    public IEnumerator LoadCreateSaveErase()
    {
        WmrDeviceCheck();
        CreateAnchor();
        yield return new WaitForSeconds(m_AnchorWait);
        CreateManyAnchors();
        Assert.IsTrue(m_CreatedAnchor, "Didn't create the anchors");

        LoadAnchors();
        Assert.IsTrue(m_LoadedAnchors, "Didn't Load the anchors");

        ClearAnchors();
        Assert.IsTrue(m_ErasedAnchor, "Didn't erase the anchors");
    }

    private void WorldManager_OnPositionalLocatorStateChanged(PositionalLocatorState oldState, PositionalLocatorState newState)
    {
        Debug.Log("Old State : " + oldState);
        Debug.Log("New State : " + newState);
    }

    private void m_RightGestureRecognizer_TappedEvent(TappedEventArgs obj)
    {
        CreateAnchor();
    }

    private void ClearGameObjects()
    {
        foreach (var go in objects)
        {
            UnityEngine.Object.Destroy(go);
        }
        objects.Clear();
    }

    private void ClearAnchors()
    {
        if (store != null)
        {
            store.Clear();
            Log("cleared store");
        }

        ClearGameObjects();
        m_ErasedAnchor = true;
    }

    private void CreateManyAnchors()
    {
        var cube1 = CreateCubeAt(new Vector3(2, 0, 0));
        var anchor1 = cube1.AddComponent<WorldAnchor>();
        var guid1 = Guid.NewGuid();

        var cube2 = CreateCubeAt(new Vector3(4, 0, 0));
        var anchor2 = cube2.AddComponent<WorldAnchor>();
        var guid2 = Guid.NewGuid();

        Log(string.Format("guids are {0} and {1}", guid1.ToString(), guid2.ToString()));
        if (store.Save(guid1.ToString(), anchor1) && store.Save(guid2.ToString(), anchor2))
        {
            Log("Saved two");
            m_CreatedAnchor = true;
        }
        else
        {
            Log("Failed to save two");
        }
    }

    private GameObject CreateCubeAt(Vector3 position)
    {
        var head = InputTracking.GetLocalPosition(XRNode.CenterEye);
        var thing = GameObject.CreatePrimitive(PrimitiveType.Cube);
        objects.Add(thing);
        thing.transform.position = new Vector3(head.x, head.y, 5f);
        thing.transform.localScale = position;
        return thing;
    }

    private void CreateAnchor()
    {
        var thing = CreateCubeAt(new Vector3(0f, 0f, 2.5f));
        var worldAnchor = thing.AddComponent<WorldAnchor>();
        var guid = Guid.NewGuid();

        if (store.Save(guid.ToString(), worldAnchor))
        {
            Log("saved " + guid.ToString());
        }
        else
        {
            Log("failed to save " + guid.ToString());
        }
        Debug.Log("Position of Cube: " + worldAnchor.transform.position);
        Debug.Log("isLocated = " + worldAnchor.isLocated);
        m_CreatedAnchor = true;
    }

    private void storeLoaded(WorldAnchorStore store)
    {
        this.store = store;
        Log("connected.");
        var numIds = store.GetAllIds().Length;
        Debug.Log("numID's stored: " + numIds.ToString());

        LoadAnchors();
    }

    private void LoadAnchors()
    {
        foreach (var id in store.GetAllIds())
        {
            var cube = CreateCubeAt(new Vector3(0.3f, 0.3f, .3f));
            store.Load(id, cube);
            Debug.Log("Position of Cube: " + cube.transform.position);
        }

        m_LoadedAnchors = true;
    }

    string StoreContents()
    {
        if (store != null)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var id in store.GetAllIds())
            {
                sb.Append(id + ", ");
            }
            return sb.ToString();
        }
        else
        {
            return "[Unknown]";
        }
    }

    void Log(string message)
    {
        var stampedMessage = string.Format("{0}: {1} - contains {2}", Time.frameCount, message, StoreContents());
        messages.Add(stampedMessage);
        while (messages.Count > 5)
        {
            messages.RemoveAt(0);
        }

        Debug.Log(LastMessages());
        Debug.Log(stampedMessage);
    }

    string LastMessages()
    {
        StringBuilder sb = new StringBuilder();

        foreach (var message in messages)
        {
            sb.Append(message + "\n");
        }
        return sb.ToString();
    }
#endif
}
