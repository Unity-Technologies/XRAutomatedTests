using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using UnityEngine.XR;
using System;
using UnityEngine.XR.WSA;

internal class EyeCameraRemotingCheck : HoloLensTestBaseRemoting  
{
    private Transform m_CameraTransform;

    private Vector3 m_LeftEyePos;
    private Vector3 m_RightEyePos;

    private bool m_AngleCheck = false;
    private bool m_EyesInFront = false;

    [SetUp]
    public void Setup()
    {
        m_TestSetupHelpers.TestStageSetup(TestStageConfig.CleanStage);

        m_Camera = new GameObject("Camera");
        m_Camera.AddComponent<Camera>();


        m_Light = new GameObject("Light");
        Light light = m_Light.AddComponent<Light>();
        light.type = LightType.Directional;
    }

    [UnityTest]
    public IEnumerator EyesParallelHead()
    {
        yield return new WaitForSeconds(k_RemotingWait);
        m_StreamerState = PerceptionRemoting.GetConnectionState();
        Assert.AreEqual(HolographicStreamerConnectionState.Connected, m_StreamerState, "Holographic Streamer Connection Failure");


        m_CameraTransform = m_Camera.transform;

        yield return null;

        EyeParallelWithHead();

        Assert.IsTrue(m_AngleCheck, "Eyes are not parallel with the head");
    }

    [UnityTest]
    public IEnumerator EyePositionCheckWithHead()
    {
        yield return new WaitForSeconds(k_RemotingWait);
        m_StreamerState = PerceptionRemoting.GetConnectionState();
        Assert.AreEqual(HolographicStreamerConnectionState.Connected, m_StreamerState, "Holographic Streamer Connection Failure");

        m_CameraTransform = m_Camera.transform;

        yield return null;

        EyePositionCheck();

        Assert.IsTrue(m_EyesInFront, "Eyes are not in front with the head");
    }

    static bool AngleCheck(float a, float b)
    {
        float m_Tolerance = 2f;
        var check = Mathf.Abs(a - b) < m_Tolerance;
        return (check);
    }

    static bool EyeZPositionCheck(float a, float b)
    {
        var check = (a > b);
        return (check);
    }

    public void EyeParallelWithHead()
    {
        Matrix4x4 left = m_Camera.GetComponent<Camera>().GetStereoViewMatrix(Camera.StereoscopicEye.Left);
        Matrix4x4 right = m_Camera.GetComponent<Camera>().GetStereoViewMatrix(Camera.StereoscopicEye.Right);

        m_LeftEyePos = left.inverse.MultiplyPoint(Vector3.zero);
        m_RightEyePos = right.inverse.MultiplyPoint(Vector3.zero);

        Vector3 eyesDelta = (m_RightEyePos - m_LeftEyePos).normalized;
        Vector3 rightDir = m_Camera.transform.right;
        float angle = Vector3.Angle(eyesDelta, rightDir);

        if (AngleCheck(angle, 0f))
        {
            Debug.Log("Eyes Parallel is OK : " + angle);
            m_AngleCheck = true;
        }
        else if (!AngleCheck(angle, 0f))
        {
            Debug.Log("Eye Parallel is BAD = " + angle);
            m_AngleCheck = false;
        }
    }

    public void EyePositionCheck()
    {
        Vector3 LeftEye = InputTracking.GetLocalPosition(XRNode.LeftEye);
        Vector3 RightEye = InputTracking.GetLocalPosition(XRNode.LeftEye);

        Vector3 LeftEyeInverse = m_Camera.transform.InverseTransformVector(LeftEye);
        Vector3 RightEyeInverse = m_Camera.transform.InverseTransformVector(RightEye);

        Debug.Log("Eye Left Inverse Position = " + LeftEyeInverse +
                  Environment.NewLine + "Eye Right Inverse Position = " + RightEyeInverse);

        if (EyeZPositionCheck(LeftEyeInverse.z, 0f))
        {
            Debug.Log("Eyes are in front of the head : " + LeftEyeInverse.z);
            m_EyesInFront = true;
        }
        else if (!EyeZPositionCheck(LeftEyeInverse.z, 0f))
        {
            Debug.Log("Eyes are in behind of the head : " + LeftEyeInverse.z);
            m_EyesInFront = false;
        }
    }
}
