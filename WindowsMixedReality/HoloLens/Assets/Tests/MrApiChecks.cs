using System.Collections;
using System.Collections.Generic;
using NUnit.Framework.Constraints;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.XR.WSA;
using NUnit.Framework;
using UnityEditor;
using UnityEditorInternal.VR;
using System;
using UnityEngine.Experimental.XR;
using UnityEngine.XR;

internal class MrApiChecks : HoloLensTestBase
{
    [UnityTest]
    public IEnumerator ContentProtectionTest()
    {
        HolographicSettings.IsContentProtectionEnabled = true;
        yield return new WaitForSeconds(1f);
        Assert.IsTrue(HolographicSettings.IsContentProtectionEnabled, "Content Protection was not set to true to protect the user!");

        HolographicSettings.IsContentProtectionEnabled = false;
        yield return new WaitForSeconds(1f);
        Assert.IsFalse(HolographicSettings.IsContentProtectionEnabled, "Content Protection was not set to false");
    }

    [UnityTest]
    public IEnumerator ReprojectionModeTest()
    {
        foreach (HolographicSettings.HolographicReprojectionMode mode in Enum.GetValues(typeof(HolographicSettings.HolographicReprojectionMode)))
        {
            HolographicSettings.ReprojectionMode = mode;
            Debug.Log("Re-projection Mode = " + mode);
            yield return new WaitForSeconds(1f);
            Assert.AreEqual(HolographicSettings.ReprojectionMode, mode, "Re-projection mode failed to change to target!");
        }
    }

    [UnityTest]
    public IEnumerator DisplayOpaqueTest()
    {
        yield return null;
        Assert.IsFalse(HolographicSettings.IsDisplayOpaque, "Display came back as Opaque!");
    }

    [UnityTest]
    public IEnumerator FocusFrameTest()
    {
        GameObject.Destroy(m_Cube);

        GameObject focusPoint = new GameObject("Focus Point");
        focusPoint = GameObject.CreatePrimitive(PrimitiveType.Sphere);

        HolographicSettings.SetFocusPointForFrame(new Vector3(0, 0, 0), m_Camera.GetComponent<Camera>().transform.position.normalized);
        focusPoint.transform.localPosition = new Vector3(0, 0, 0);

        Assert.AreEqual(new Vector3(0, 0, 0), focusPoint.transform.localPosition);

        yield return new WaitForSeconds(1f);

        for (int i = 0; i < 10; i++)
        {
            HolographicSettings.SetFocusPointForFrame(new Vector3(0, 0, i), m_Camera.GetComponent<Camera>().transform.position.normalized);
            Debug.Log("Current Focus Frame Z Position:" + i);

            yield return new WaitForSeconds(0.5f);

            focusPoint.transform.localPosition = new Vector3(0, 0, i);

            Assert.AreEqual(new Vector3(0, 0, i), focusPoint.transform.localPosition);
        }

        for (int i = 10; i > 0; i--)
        {
            HolographicSettings.SetFocusPointForFrame(new Vector3(0, 0, i), m_Camera.GetComponent<Camera>().transform.position.normalized);
            Debug.Log("Current Focus Frame Z Position:" + i);

            yield return new WaitForSeconds(0.5f);

            focusPoint.transform.localPosition = new Vector3(0, 0, i);

            Assert.AreEqual(new Vector3(0, 0, i), focusPoint.transform.localPosition);
        }

        GameObject.Destroy(focusPoint);
    }
}
