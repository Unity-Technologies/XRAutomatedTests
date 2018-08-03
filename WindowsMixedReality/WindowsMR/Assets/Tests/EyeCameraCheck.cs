using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using UnityEngine.XR;
using System;

internal class EyeCameraCheck : WindowsMrTestBase
{
    private bool m_AngleCheck = false;
    private bool m_EyesInFront = false;
    private bool m_EyeAngleCheck = false;
    private bool m_LeftEyeAngleCheck = false;
    private bool m_RightEyeAngleCheck = false;

    [UnityTest]
	public IEnumerator EyesParallelHead()
    {
        yield return null;

        EyeParallelWithHead();

        Assert.IsTrue(m_AngleCheck, "Eyes are not parallel with the head");
    }

    [UnityTest]
    public IEnumerator EyePositionCheckWithHead()
    {
        yield return null;

        EyePositionCheck();

        Assert.IsTrue(m_EyesInFront, "Eyes are not in front with the head");
        Assert.IsTrue(m_EyeAngleCheck, "Eye Angles don't match with the head");
    }

    static bool AngleCheck(float a, float b)
    {
        float m_Tolerance = 2f;
        var check = Mathf.Abs(a - b) < m_Tolerance;
        return (check);
    }

    static bool CompareEyeAngles(float a, float b)
    {
        float m_Tolerance = 0.5f;
        var check = Mathf.Abs(a - b) < Mathf.Abs(a-b) + m_Tolerance;
        return (check);
    }

    static bool EyeZPositionCheck(float a, float b)
    {
        var check = (a > b);
        return (check);
    }

    static bool CheckMathForEyes(float Convergence, float EyeAngle)
    {
        // Verification of the math 
        // tan should be half of tan 2
        // tan 2 should be half of tan 3
        bool mathPassed = false;
        bool check1Pass = false;
        bool check2Pass = false;

        var tan = Mathf.Tan(EyeAngle);
        Debug.Log(tan);

        var tan2 = Convergence * Mathf.Tan(EyeAngle);
        Debug.Log(tan2);

        var tan3 = (2 * Convergence) * Mathf.Tan(EyeAngle);
        Debug.Log(tan3);

        var check = tan2 / Convergence;
        if (check == tan)
        {
            Debug.Log("Check 1 passed - " + check);
            check1Pass = true;
        }

        check = tan3 / (2 * Convergence);
        if (check == tan)
        {
            Debug.Log("Check 2 passed - " + check);
            check2Pass = true;
        }

        if (check1Pass & check2Pass == true)
        {
            mathPassed = true;
        }
        else
        {
            mathPassed = false;
        }

        return (mathPassed);
    }

    public void EyeParallelWithHead()
    {
        Matrix4x4 left = m_Camera.GetComponent<Camera>().GetStereoViewMatrix(Camera.StereoscopicEye.Left);
        Matrix4x4 right = m_Camera.GetComponent<Camera>().GetStereoViewMatrix(Camera.StereoscopicEye.Right);

        Vector3 m_LeftEyePos = left.inverse.MultiplyPoint(Vector3.zero);
        Vector3 m_RightEyePos = right.inverse.MultiplyPoint(Vector3.zero);

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
        Vector3 RightEye = InputTracking.GetLocalPosition(XRNode.RightEye);

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

        Vector3 forwardDirLeft = m_Camera.transform.forward;
        float leftEyeAngle = Vector3.Angle(LeftEye, forwardDirLeft);

        Vector3 forwardDirRight = m_Camera.transform.forward;
        float rightEyeAngle = Vector3.Angle(RightEye, forwardDirRight);

        CheckMathForEyes(m_Camera.GetComponent<Camera>().stereoConvergence, leftEyeAngle);

        // Check to make sure the eye angles from the head are the same
        if (CompareEyeAngles(leftEyeAngle, rightEyeAngle))
        {
            Debug.Log("Left and Right eye angles are the same : " + leftEyeAngle + " | " + rightEyeAngle);

            m_EyeAngleCheck = true;
        }
        else if (!CompareEyeAngles(leftEyeAngle, rightEyeAngle))
        {
            Debug.Log("Left and Right eye angles are not the same : " + leftEyeAngle + " | " + rightEyeAngle);
            m_EyeAngleCheck = false;
        }

        //Check to make sure the angle from the camera to the left eye is reasonable 
        if (!AngleCheck(leftEyeAngle, 60f))
        {
            Debug.Log("Left eye angle to the head is correct : " + leftEyeAngle);
            m_LeftEyeAngleCheck = true;
        }
        else if (AngleCheck(leftEyeAngle, 60f))
        {
            Debug.Log("Left eye angle to the head is incorrect : " + leftEyeAngle);
            m_LeftEyeAngleCheck = false;
        }

        //Check to make sure the angle from the camera to the right eye is reasonable 
        if (!AngleCheck(rightEyeAngle, 60f))
        {
            Debug.Log("Right eye angle to the head is correct : " + rightEyeAngle);
            m_RightEyeAngleCheck = true;
        }
        else if (AngleCheck(rightEyeAngle, 60f))
        {
            Debug.Log("Right eye angle to the head is incorrect : " + rightEyeAngle);
            m_RightEyeAngleCheck = false;
        }
    }
}
