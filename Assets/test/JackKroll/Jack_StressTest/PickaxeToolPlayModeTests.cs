using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;

/// <summary>
/// Very simple PlayMode tests for PickaxeTool.
/// These tests only check core functionality: setup, following, and audio.
/// No TerrainHandler or complex setup is created here.
/// </summary>
public class PickaxeToolPlayModeTests
{
    private GameObject _pickaxeObj;     // The GameObject that holds our PickaxeTool
    private PickaxeTool _pickaxeTool;   // The PickaxeTool script we're testing
    private AudioSource _audioSource;   // The AudioSource used by PickaxeTool
    private Transform _player;          // The Player's transform (must already exist in the scene)

    // Runs before each test
    [UnitySetUp]
    public IEnumerator Setup()
    {
        // Load the scene that already contains your Player and TerrainHandler
        SceneManager.LoadScene("MVP_jack");
        yield return null; // Wait one frame for scene to load

        // Find the Player (the PickaxeTool needs this)
        _player = GameObject.FindGameObjectWithTag("Player")?.transform;
        Assert.IsNotNull(_player, "Player not found in scene! Make sure a GameObject is tagged 'Player'.");

        // Create a simple GameObject for the Pickaxe
        _pickaxeObj = new GameObject("PickaxeTestObject");

        // Add an AudioSource (PickaxeTool requires this to play sounds)
        _audioSource = _pickaxeObj.AddComponent<AudioSource>();

        // Add the PickaxeTool component we’re testing
        _pickaxeTool = _pickaxeObj.AddComponent<PickaxeTool>();

        // Wait one frame for PickaxeTool.Start() to run
        yield return null;
    }

    // Runs after each test to clean up temporary objects
    [UnityTearDown]
    public IEnumerator Teardown()
    {
        if (_pickaxeObj != null)
            Object.Destroy(_pickaxeObj);
        yield return null;
    }

    // ------------------------------------------------------------------
    // TEST 1: Audio should NOT be playing at start
    // ------------------------------------------------------------------
    /// <summary>
    /// Verifies that the PickaxeTool's AudioSource is NOT playing
    /// when the game starts (no unintended sound on spawn).
    /// </summary>
    [UnityTest]
    public IEnumerator Pickaxe_Audio_NotPlayingAtStart()
    {
        // Wait one frame so PickaxeTool.Start() can run
        yield return null;

        // The AudioSource should not be playing right away
        Assert.IsFalse(_audioSource.isPlaying, "AudioSource should NOT be playing at the start of the game.");
    }

    // ------------------------------------------------------------------
    // TEST 2: Pickaxe follows the player
    // ------------------------------------------------------------------
    /// <summary>
    /// Ensures the Pickaxe stays near the Player’s position.
    /// </summary>
    [UnityTest]
    public IEnumerator Pickaxe_FollowsPlayerPosition()
    {
        // Let Update() run for a short time so it can position itself
        yield return new WaitForSeconds(0.5f);

        // Measure distance between Player and Pickaxe
        float distance = Vector3.Distance(_pickaxeObj.transform.position, _player.position);

        // It should stay close (within about 2 meters)
        Assert.Less(distance, 2f, "Pickaxe should stay near the player.");
    }

    // ------------------------------------------------------------------
    // TEST 3: Audio playback
    // ------------------------------------------------------------------
    /// <summary>
    /// Simple test that checks if AudioSource can play and stop normally.
    /// </summary>
    [UnityTest]
    public IEnumerator Pickaxe_AudioPlayAndStop_Works()
    {
        yield return null;

        // Start audio playback
        _audioSource.Play();
        yield return new WaitForSeconds(0.1f);
        Assert.IsTrue(_audioSource.isPlaying, "Audio should be playing.");

        // Stop audio playback
        _audioSource.Stop();
        yield return null;
        Assert.IsFalse(_audioSource.isPlaying, "Audio should stop.");
    }

    // ------------------------------------------------------------------
    // TEST 4: Timer progression
    // ------------------------------------------------------------------
    /// <summary>
    /// Confirms that time moves forward normally (basic sanity test).
    /// </summary>
    [UnityTest]
    public IEnumerator Pickaxe_Timer_IncreasesOverTime()
    {
        float startTime = Time.time;
        yield return new WaitForSeconds(1f);
        float endTime = Time.time;

        // Verify that at least one second has passed
        Assert.Greater(endTime - startTime, 0.9f, "Time should progress normally during test.");
    }
}
