using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;

public class SoundTest
{
    private GameObject testObject;
    private GameObject listenerObject;
    private SoundManager soundManager;

    [SetUp]
    public void Setup()
    {
        testObject = new GameObject("SoundManagerTestObject");
        soundManager = testObject.AddComponent<SoundManager>();
    }

    [UnitySetUp]
    public IEnumerator SceneSetup(){
        SceneManager.LoadScene("MVP");
        yield return null;

        soundManager = Object.FindAnyObjectByType<SoundManager>();
        Assert.IsNotNull(soundManager, "SoundManager not found in scene");

        listenerObject = new GameObject("AudioListenerObject");
        listenerObject.AddComponent<AudioListener>();
    }


    //Stress Test1: Simulates player jumping rapidly by playing audio clip 100 times
    //Get clip then call JumpSound 100 times

    [UnityTest]
    public IEnumerator Stress_JumpSoundMultipleRapidCalls() {
        
        AudioClip clip = Resources.Load<AudioClip>("Sounds/Jump");
        Assert.IsNotNull(clip, "Jump clip not found in Resources/Sounds");
        soundManager.SetJumpClip(clip);

        for (int i = 0; i < 100; i++){
            soundManager.JumpSound();
            Debug.Log($"Jump: {i}");
        }
        yield return null;

        //Check that only two clips were created and used
        var audioSources = testObject.GetComponents<AudioSource>();
        Assert.AreEqual(2, audioSources.Length, "Expected exactly one AudioSource");
        Assert.AreEqual(clip, audioSources[1].clip);
        Assert.IsTrue(audioSources[1].isPlaying);

    }

    //Boundary Test1: Call JumpSound with null clip

    [UnityTest]
    public IEnumerator Boundary_JumpSoundClipIsNull() {
        soundManager.SetJumpClip(null);

        soundManager.JumpSound();
        yield return null;

        var audioSources = testObject.GetComponents<AudioSource>();
        Assert.AreEqual(2, audioSources.Length, "JumpSource should still exist");
        Assert.IsFalse(audioSources[1].isPlaying, "JumpSound should not play when clip is null");
    }


    //Boundary Test2: Call BackgroundMusic at invalid volume
    [UnityTest]
    public IEnumerator Boundary_BackgroundVolumeNegative() {

        AudioClip clip = Resources.Load<AudioClip>("Sounds/Soliloquy");
        Assert.IsNotNull(clip, "Background clip not found in Resources/Sounds");
        soundManager.SetBackgroundClip(clip);

        soundManager.SetBackgroundVolume(-1f);

        soundManager.BackgroundMusic();
        yield return null;

        var audioSources = testObject.GetComponents<AudioSource>();
        Assert.IsTrue(audioSources.Length > 0, "Expected AudioSource to be added");
        Assert.AreEqual(0f, audioSources[0].volume, "Volume should be clamped to 0");
    }

    [TearDown]
    public void Teardown()
    {
        Object.Destroy(testObject);
        Object.Destroy(listenerObject);
    }

}
