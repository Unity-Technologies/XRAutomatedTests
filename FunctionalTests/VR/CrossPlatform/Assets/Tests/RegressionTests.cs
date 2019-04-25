using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

internal class RegressionTests : TestBaseSetup
{
    [UnityTest]
    [Timeout(10000)] // 10 seconds
    [Description("Regression test against fogbugz issue https://fogbugz.unity3d.com/f/cases/1145324/ - particle systems crash mobile XR.")]
    public IEnumerator ParticleSmokeTest()
    {
        m_Camera = new GameObject();
        m_Camera.AddComponent<Camera>();
        m_Camera.tag = "MainCamera";

        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.transform.position = new Vector3(0, 0, 10);
        ParticleSystem particles = go.AddComponent<ParticleSystem>();
        go.GetComponent<ParticleSystemRenderer>().material = Resources.Load<Material>("Materials/Particle");
        Assert.IsNotNull(go.GetComponent<ParticleSystemRenderer>().material);
        particles.Play();

        // 100 frames ~= 1 or 2 seconds
        for (int i = 0; i < 100; i++)
            yield return null;
    }
}