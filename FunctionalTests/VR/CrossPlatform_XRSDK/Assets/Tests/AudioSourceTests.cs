using System;
using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class AudioSourceTests : XrFunctionalTestBase
{
    private AudioSource audioSource;

    private readonly int audioPlaySkipFrameCountWait = 2;
    private readonly float audioTolerance = .01f;

    [SetUp]
    public override void SetUp()
    {
        base.SetUp();
        TestCubeSetup(TestCubesConfig.TestCube);
        Cube.AddComponent<AudioSource>();
        Cube.GetComponent<AudioSource>().clip = Resources.Load("Audio/FOA_speech_ambiX", typeof(AudioClip)) as AudioClip;

        audioSource = Cube.GetComponent<AudioSource>();
        Camera.AddComponent<AudioListener>();
    }

    [TearDown]
    public override void TearDown()
    {
        GameObject.Destroy(audioSource);
        base.TearDown();
    }

    [UnityTest]
    public IEnumerator VerifyAudioSource_Play()
    {
        // Act
        yield return SkipFrame(DefaultFrameSkipCount);

        audioSource.Play();
        yield return SkipFrame(audioPlaySkipFrameCountWait);

        // Assert
        Assert.AreEqual(audioSource.isPlaying, true, "Audio source is not playing");
    }

    [UnityTest]
    public IEnumerator VerifyAudioSource_Pause()
    {
        // Arrange
        yield return SkipFrame(DefaultFrameSkipCount);

        audioSource.Play();
        yield return SkipFrame(audioPlaySkipFrameCountWait);

        // Act
        audioSource.Pause();
        yield return SkipFrame(audioPlaySkipFrameCountWait);

        // Assert
        Assert.AreEqual(audioSource.isPlaying, false, "Audio source is not paused");
    }

    [UnityTest]
    public IEnumerator VerifyAudioSource_UnPause()
    {
        // Arrange
        yield return SkipFrame(DefaultFrameSkipCount);

        audioSource.Play();
        yield return SkipFrame(audioPlaySkipFrameCountWait);

        audioSource.Pause();
        yield return SkipFrame(audioPlaySkipFrameCountWait);

        // Act
        audioSource.UnPause();
        yield return SkipFrame(audioPlaySkipFrameCountWait);

        // Assert
        Assert.AreEqual(audioSource.isPlaying, true, "Audio source didn't un-paused");
    }

    [UnityTest]
    public IEnumerator VerifyAudioSource_Stop()
    {
        // Arrange
        yield return SkipFrame(DefaultFrameSkipCount);

        audioSource.Play();
        yield return SkipFrame(audioPlaySkipFrameCountWait);

        // Act
        audioSource.Stop();
        yield return SkipFrame(DefaultFrameSkipCount);

        // Assert
        Assert.AreEqual(audioSource.isPlaying, false, "Audio failed to stop");
    }

    [UnityTest]
    public IEnumerator VerifyAudioSource_Adjust_SpatialBlend()
    {
        // Arrange
        yield return SkipFrame(DefaultFrameSkipCount);
        audioSource.spatialize = true;
        Debug.Log("Enabling Spatialized Audio");

        audioSource.Play();
        Debug.Log("Starting Audio");

        var blendAmount = 0f;

        for (var i = 0f; i < 10f; ++i)
        {
            blendAmount = blendAmount + 0.1f;

            // Act
            audioSource.spatialBlend = blendAmount;
            Debug.Log("Changing blend amount : " + blendAmount);

            yield return SkipFrame();

            // Assert
            Assert.True(Math.Abs(audioSource.spatialBlend - blendAmount) <= audioTolerance, "Spatial Blend failed to be set");
        }
    }

    [UnityTest]
    public IEnumerator VerifyAudioSource_Adjust_Volume()
    {
        // Arrange
        yield return SkipFrame(DefaultFrameSkipCount);

        audioSource.Play();
        Debug.Log("Starting Audio");

        audioSource.volume = 0f;
        Assert.AreEqual(0f, audioSource.volume, "Volume was not set to 0;");

        yield return SkipFrame(DefaultFrameSkipCount);

        var volumeAmount = 0f;

        for (var i = 0f; i < 10f; ++i)
        {
            volumeAmount = volumeAmount + 0.1f;

            // Act
            audioSource.volume = volumeAmount;
            Debug.Log("Changing volume amount : " + volumeAmount);

            yield return SkipFrame();

            // Assert
            Assert.True(Math.Abs(volumeAmount - audioSource.volume) <= audioTolerance, "volume failed to change to the desired level");
        }
    }
}
