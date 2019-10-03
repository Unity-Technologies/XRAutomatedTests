using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;

public class EyeCameraTests : XrFunctionalTestBase
{
    private readonly float angleComparisonTolerance = 2f;

    [UnityTest]
    public IEnumerator VerifyEyesParallelWithHead()
    {
        yield return SkipFrame(DefaultFrameSkipCount);

        Assert.IsTrue(EyesParallelWithHead(), "Eyes are not parallel with the head");
    }

    bool AnglesApproximatelyEqual(float a, float b)
    {
        var check = Mathf.Abs(a - b) < angleComparisonTolerance;
        return (check);
    }

    public bool EyesParallelWithHead()
    {
        bool eyesParallelWithHead;

        var camera = Camera.GetComponent<Camera>();
        var left = camera.GetStereoViewMatrix(UnityEngine.Camera.StereoscopicEye.Left);
        var right = camera.GetStereoViewMatrix(UnityEngine.Camera.StereoscopicEye.Right);

        var leftEyePos = left.inverse.MultiplyPoint(Vector3.zero);
        var rightEyePos = right.inverse.MultiplyPoint(Vector3.zero);

        var eyesDelta = (rightEyePos - leftEyePos).normalized;
        var rightDir = Camera.transform.right;
        var angle = Vector3.Angle(eyesDelta, rightDir);

        if (AnglesApproximatelyEqual(angle, 0f))
        {
            Debug.Log("Eyes Parallel is OK : " + angle);
            eyesParallelWithHead = true;
        }
        else
        {
            Debug.Log("Eye Parallel is BAD = " + angle);
            eyesParallelWithHead = false;
        }

        return eyesParallelWithHead;
    }
}
