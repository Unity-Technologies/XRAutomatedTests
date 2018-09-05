using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UAssert = UnityEngine.Assertions.Assert;

internal class AudioChecks : HoloLensTestBase
{
    private AudioSource m_AudioSource = null;

    private float kAudioPlayWait = 3f;
    private float kAudioTolerance = .01f;

    [SetUp]
    public void Setup()
    {
        m_Cube.AddComponent<AudioSource>();
        m_Cube.GetComponent<AudioSource>().clip = Resources.Load("Audio/FOA_speech_ambiX", typeof(AudioClip)) as AudioClip;

        m_AudioSource = m_Cube.GetComponent<AudioSource>();
    }

    [TearDown]
    public void TearDown()
    {
        GameObject.Destroy(m_AudioSource);
    }

    [UnityTest]
    public IEnumerator AudioPlayCheck()
    {
        yield return null;

        m_AudioSource.Play();
        yield return new WaitForSeconds(kAudioPlayWait);
        Assert.AreEqual(m_AudioSource.isPlaying, true, "Audio source is not playing");
    }

    [UnityTest]
    public IEnumerator AudioSourceControlCheck()
    {
        yield return null;

        m_AudioSource.Play();
        yield return new WaitForSeconds(kAudioPlayWait);
        Assert.AreEqual(m_AudioSource.isPlaying, true, "Audio source is not playing");

        m_AudioSource.Pause();
        yield return new WaitForSeconds(kAudioPlayWait);
        Assert.AreEqual(m_AudioSource.isPlaying, false, "Audio source is not paused");

        m_AudioSource.UnPause();
        yield return new WaitForSeconds(kAudioPlayWait);
        Assert.AreEqual(m_AudioSource.isPlaying, true, "Audio source didn't un-paused");

        m_AudioSource.Stop();
        yield return null;
        Assert.AreEqual(m_AudioSource.isPlaying, false, "Audio failed to stop");
    }

    [UnityTest]
    public IEnumerator AudioSpatlize()
    {
        yield return null;
        m_AudioSource.spatialize = true;
        Assert.IsTrue(m_AudioSource.spatialize, "Spatialize has failed to turn on");
        Debug.Log("Enabling Spatialized Audio");

        m_AudioSource.Play();
        Debug.Log("Starting Audio");
        Assert.AreEqual(m_AudioSource.isPlaying, true, "Audio source is not playing");

        var blendAmount = 0f;
        
        for (float i = 0f; i < 10f; ++i)
        {
            blendAmount = blendAmount + 0.1f;
            m_AudioSource.spatialBlend = blendAmount;
            Debug.Log("Changing bland amount : " + blendAmount);

            yield return new WaitForSeconds(1f);

            UAssert.AreApproximatelyEqual(blendAmount, m_AudioSource.spatialBlend, kAudioTolerance, "Spatial Blend as failed to be set");
        }
    }

    [UnityTest]
    public IEnumerator AudioVolumeControl()
    {
        yield return null;

        m_AudioSource.Play();
        Debug.Log("Starting Audio");
        Assert.AreEqual(m_AudioSource.isPlaying, true, "Audio source is not playing");

        m_AudioSource.volume = 0f;
        Assert.AreEqual(0f, m_AudioSource.volume, "Volume was not set to 0;");

        yield return null;

        var volumeAmount = 0f;

        for (float i = 0f; i < 10f; ++i)
        {
            volumeAmount = volumeAmount + 0.1f;
            m_AudioSource.volume = volumeAmount;
            Debug.Log("Changing volume amount : " + volumeAmount);

            yield return new WaitForSeconds(1f);

            UAssert.AreApproximatelyEqual(volumeAmount, m_AudioSource.volume, "volume has failed to change to the desired level");
        }
    }
}
