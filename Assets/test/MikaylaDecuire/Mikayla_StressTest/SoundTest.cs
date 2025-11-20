using System.Collections;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;

public class SoundManagerExtendedTests
{
    private GameObject testObject;
    private SoundManager soundManager;

    private void SetPrivateField(string fieldName, object value)
    {
        var field = typeof(SoundManager).GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
        field.SetValue(soundManager, value);
    }

    // Runs before each test
    [SetUp]
    public void Setup()
    {
        testObject = new GameObject("SoundManagerTestObject");
        soundManager = testObject.AddComponent<SoundManager>();
        if (Object.FindAnyObjectByType<AudioListener>() == null)
        {
            var camera = new GameObject("TestCamera");
            camera.AddComponent<AudioListener>();
        }
    }

    /* ---------------- Singleton Tests ---------------- */

    /* Verify SoundManager. Instance is assigned after Awake.
    Wait a frame for Awake to run
    Check that a SoundManager Instance was created
    Check that the test object is the Instance
    */
    [UnityTest]
    public IEnumerator Singleton_InstanceIsSetOnAwake()
    {
        yield return null;

        Assert.IsNotNull(SoundManager.Instance);
        Assert.AreEqual(soundManager, SoundManager.Instance);
    }

    // Add a second SoundManager and confirm it destroys itself.
    [UnityTest]
    public IEnumerator Singleton_DestroyDuplicateManager()
    {
        var duplicate = new GameObject("DuplicateSM").AddComponent<SoundManager>();
        
        yield return null;

        Assert.AreNotEqual(duplicate, SoundManager.Instance);
    }

    /* Load a new scene and check that the same Instance persists
    Save the real SM Instance
    Make a new GameObject and give it another SoundManager
    Wait 1 Frame for Awake to run when it will self delete
    */
    [UnityTest]
    public IEnumerator Singleton_PersistsAcrossScenes()
    {
        var instanceBefore = SoundManager.Instance;
        var duplicate = new GameObject("DuplicateSM").AddComponent<SoundManager>();
        yield return null;
        Assert.AreEqual(instanceBefore, SoundManager.Instance);
        Assert.AreNotEqual(duplicate, SoundManager.Instance);
    }

    // Ensure only one SoundManager exists in the scene hierarchy
    [UnityTest]
    public IEnumerator Singleton_InstanceIsUnique()
    {
        yield return null;
        var managers = Object.FindObjectsByType<SoundManager>(FindObjectsSortMode.None);
        Assert.AreEqual(1, managers.Length);
    }

    /* ---------------- Observer Tests ---------------- */


    /* Call SoundEvents.PlayerJump() and confirm jump sound plays
    Make fake clip
    Broadcast Sound Event
    Grab AudioSources (background music and sfx)
    Check if SFX is playing
    */
    [UnityTest]
    public IEnumerator Observer_PlayerJumpEventTriggersSound()
    {
        var clip = AudioClip.Create("Jump", 44100, 1, 44100, false);
        SetPrivateField("playerJump", clip);

        SoundEvents.PlayerJump();
        yield return null;

        var sources = testObject.GetComponents<AudioSource>();
        Assert.IsTrue(sources[1].isPlaying);
    }

    // Call SoundEvents.EnemyDamage()
    [UnityTest]
    public IEnumerator Observer_EnemyDamageEventTriggersSound()
    {
        var clip = AudioClip.Create("EnemyDamage", 44100, 1, 44100, false);
        SetPrivateField("enemyDamageClip", clip);

        SoundEvents.EnemyDamage();
        yield return null;

        var sources = testObject.GetComponents<AudioSource>();
        Assert.IsTrue(sources[1].isPlaying, "Enemy damage SFX should be playing");
    }


    // Call SoundEvents.ToolUse()
    [UnityTest]
    public IEnumerator Observer_ToolUseEventTriggersSound()
    {
        var clip = AudioClip.Create("ToolUse", 44100, 1, 44100, false);
        SetPrivateField("useTool", clip);

        SoundEvents.ToolUse();
        yield return null;

        var sources = testObject.GetComponents<AudioSource>();
        Assert.IsTrue(sources[1].isPlaying);
    }

    // Call SoundEvents.ToolPickup()
    [UnityTest]
    public IEnumerator Observer_ToolPickupEventTriggersSound()
    {
        var clip = AudioClip.Create("ToolPickup", 44100, 1, 44100, false);
        SetPrivateField("addTool", clip);

        SoundEvents.ToolPickup();
        yield return null;

        var sources = testObject.GetComponents<AudioSource>();
        Assert.IsTrue(sources[1].isPlaying);
    }

    // Ensure invoking events with no subscribers does not throw exceptions
    [UnityTest]
    public IEnumerator Observer_NoSubscribersDoesNotThrow()
    {
        soundManager.enabled = false;
        yield return null;

        Assert.DoesNotThrow(() => SoundEvents.PlayerJump());
        yield return null;
    }

    // Disable SoundManager and confirm events no longer trigger sounds
    [UnityTest]
    public IEnumerator Observer_UnsubscribeOnDisable()
    {
        soundManager.enabled = false;
        yield return null;

        SoundEvents.PlayerJump();
        yield return null;

        var sources = testObject.GetComponents<AudioSource>();
        Assert.IsFalse(sources[1].isPlaying);
    }

    // Re-enable SoundManager and confirm events trigger again
    [UnityTest]
    public IEnumerator Observer_ResubscribeOnEnable()
    {
        soundManager.enabled = true;
        yield return null;
        var clip = AudioClip.Create("Jump", 44100, 1, 44100, false);
        SetPrivateField("playerJump", clip);

        SoundEvents.PlayerJump();
        yield return null;

        var sources = testObject.GetComponents<AudioSource>();
        Assert.IsTrue(sources[1].isPlaying);
    }

    /* ---------------- Background Music Tests ---------------- */

    /* Ensure no playback occurs when background_clip is null

    Result: Throws MissingReferenceException
    Explanation: The singleton was destroyed but the code still tried to access its methods
    Fix: Add a null check for backgroundSource in code
    */
    [UnityTest]
    public IEnumerator BackgroundMusic_ClipIsNull_NoPlay()
    {
        SetPrivateField("background_clip", null);
        soundManager.BackgroundMusic();
        yield return null;

        var sources = testObject.GetComponents<AudioSource>();
        Assert.IsFalse(sources[0].isPlaying);
    }

    // Verify background music loops continuously
    [UnityTest]
    public IEnumerator BackgroundMusic_ClipPlaysLooped()
    {
        var clip = AudioClip.Create("BG", 44100, 1, 44100, false);
        SetPrivateField("background_clip", clip);

        soundManager.BackgroundMusic();
        yield return null;

        var sources = testObject.GetComponents<AudioSource>();
        Assert.IsTrue(sources[0].isPlaying);
        Assert.IsTrue(sources[0].loop);
    }

    // Set volume > 0.5 and confirm it clamps to 0.5
    [UnityTest]
    public IEnumerator BackgroundMusic_VolumeClampedAboveMax()
    {
        var clip = AudioClip.Create("BG", 44100, 1, 44100, false);
        SetPrivateField("background_clip", clip);
        SetPrivateField("background_volume", 1f);

        soundManager.BackgroundMusic();
        yield return null;

        var sources = testObject.GetComponents<AudioSource>();
        Assert.AreEqual(0.5f, sources[0].volume);
    }

    // Set volume < 0 and confirm it clamps to 0
    [UnityTest]
    public IEnumerator BackgroundMusic_VolumeClampedBelowMin()
    {
        var clip = AudioClip.Create("BG", 44100, 1, 44100, false);
        SetPrivateField("background_clip", clip);
        SetPrivateField("background_volume", -1f);

        soundManager.BackgroundMusic();
        yield return null;

        var sources = testObject.GetComponents<AudioSource>();
        Assert.AreEqual(0f, sources[0].volume);
    }

    // Stop background music manually, then call BackgroundMusic() again to restart
    // Make sure its safe to stop and restart
    [UnityTest]
    public IEnumerator BackgroundMusic_StopAndRestart()
    {
        var clip = AudioClip.Create("BG", 44100, 1, 44100, false);
        SetPrivateField("background_clip", clip);

        soundManager.BackgroundMusic();
        var sources = testObject.GetComponents<AudioSource>();
        sources[0].Stop();

        soundManager.BackgroundMusic();
        yield return null;

        Assert.IsTrue(sources[0].isPlaying);
    }

    /* ---------------- SFX Playback Tests ---------------- */

    // Assign jump clip and confirm playback
    [UnityTest]
    public IEnumerator SFX_JumpClipPlaysCorrectly()
    {
        var clip = AudioClip.Create("Jump", 44100, 1, 44100, false);
        SetPrivateField("playerJump", clip);

        SoundEvents.PlayerJump();
        yield return null;

        var sources = testObject.GetComponents<AudioSource>();
        Assert.IsTrue(sources[1].isPlaying);
    }

    // Enemy Damage
    [UnityTest]
    public IEnumerator SFX_EnemyDamageClipPlaysCorrectly(){
        var clip = AudioClip.Create("EnemyDamage", 44100, 1, 44100, false);
        SetPrivateField("enemyDamageClip", clip);
        
        SoundEvents.EnemyDamage();
        yield return null;

        var sources = testObject.GetComponents<AudioSource>();
        Assert.IsTrue(sources[1].isPlaying, "Enemy damage SFX should be playing");
    }


    // Tool-use
    [UnityTest]
    public IEnumerator SFX_ToolUseClipPlaysCorrectly()
    {
        var clip = AudioClip.Create("ToolUse", 44100, 1, 44100, false);
        SetPrivateField("useTool", clip);

        SoundEvents.ToolUse();
        yield return null;

        var sources = testObject.GetComponents<AudioSource>();
        Assert.IsTrue(sources[1].isPlaying);
    }

    // Tool-pickup
    [UnityTest]
    public IEnumerator SFX_ToolPickupClipPlaysCorrectly()
    {
        var clip = AudioClip.Create("ToolPickup", 44100, 1, 44100, false);
        SetPrivateField("addTool", clip);

        SoundEvents.ToolPickup();
        yield return null;

        var sources = testObject.GetComponents<AudioSource>();
        Assert.IsTrue(sources[1].isPlaying);
    }

    // Call PlaySFX(null) and confirm no playback
    [UnityTest]
    public IEnumerator SFX_NullClipDoesNotPlay()
    {
        SetPrivateField("playerJump", null);
        SoundEvents.PlayerJump();
        yield return null;

        var sources = testObject.GetComponents<AudioSource>();
        Assert.IsFalse(sources[1].isPlaying);
    }

    // Play jump, tool-add, tool-use in sequence and confirm all play
    [UnityTest]
    public IEnumerator SFX_PlayMultipleClipsSequentially()
    {
        var jump = AudioClip.Create("Jump", 44100, 1, 44100, false);
        var land = AudioClip.Create("Land", 44100, 1, 44100, false);
        var tool = AudioClip.Create("addTool", 44100, 1, 44100, false);

        SetPrivateField("playerJump", jump);
        SetPrivateField("addTool", land);
        SetPrivateField("useTool", tool);

        SoundEvents.PlayerJump();
        yield return null;
        var sources = testObject.GetComponents<AudioSource>();
        Assert.IsTrue(sources[1].isPlaying, "Jump should be playing");

        SoundEvents.ToolPickup();
        yield return null;
        sources = testObject.GetComponents<AudioSource>();
        Assert.IsTrue(sources[1].isPlaying, "Add-Tool should be playing");

        SoundEvents.ToolUse();
        yield return null;
        sources = testObject.GetComponents<AudioSource>();
        Assert.IsTrue(sources[1].isPlaying, "Tool-use should be playing");
    }

    // Play multiple clips rapidly and confirm overlapping playback works
    // Note: PlayOneShot mixes internally; AudioSource.clip may be null or unchanged. isPlaying is the reliable indicator.

    /* Bug 3: We actually dont want the same clip to overlap a bunch it sounds horrific
    Fix: Control the times that certain SoundEvents are broadcasted in PlayerController
    Have cooldown for footsteps and jump
    */
    [UnityTest]
    public IEnumerator SFX_PlayMultipleClipsOverlap()
    {
        var jump = AudioClip.Create("Jump", 44100, 1, 44100, false);
        var tool = AudioClip.Create("ToolUse", 44100, 1, 44100, false);

        SetPrivateField("playerJump", jump);
        SetPrivateField("toolUse", tool);

        SoundEvents.PlayerJump();
        SoundEvents.ToolUse();
        SoundEvents.PlayerJump();
        SoundEvents.ToolUse();

        yield return null;

        var sources = testObject.GetComponents<AudioSource>();
        Assert.IsTrue(sources[1].isPlaying, "SFX AudioSource should be playing overlapping clips");

    }

    /* ---------------- Boundary & Edge Cases ---------------- */

    // Set volume = 0 and confirm muted playback
    // i.e. make sure clip is still playing but at zero volume
    [UnityTest]
    public IEnumerator Boundary_BackgroundVolumeZero()
    {
        var clip = AudioClip.Create("BG", 44100, 1, 44100, false);
        SetPrivateField("background_clip", clip);
        SetPrivateField("background_volume", 0f);

        soundManager.BackgroundMusic();
        yield return null;

        var sources = testObject.GetComponents<AudioSource>();
        Assert.IsTrue(sources[0].isPlaying, "Background should play even at zero volume");
        Assert.AreEqual(0f, sources[0].volume, "Volume should be exactly zero");
    }

    // Set volume = 0.5 and confirm max allowed playback
    [UnityTest]
    public IEnumerator Boundary_BackgroundVolumeHalf()
    {
        var clip = AudioClip.Create("BG", 44100, 1, 44100, false);
        SetPrivateField("background_clip", clip);
        SetPrivateField("background_volume", 0.5f);

        soundManager.BackgroundMusic();
        yield return null;

        var sources = testObject.GetComponents<AudioSource>();
        Assert.IsTrue(sources[0].isPlaying);
        Assert.AreEqual(0.5f, sources[0].volume, "Volume should be clamped to 0.5 max");
    }

    // Ensure no crash when footstep array is emptys
    // Give it an Audio Clip with no items
    [UnityTest]
    public IEnumerator Boundary_FootstepArrayEmpty()
    {
        SetPrivateField("footStep", new AudioClip[0]);
        yield return null;

        var managers = Object.FindObjectsByType<SoundManager>(FindObjectsSortMode.None);
        Assert.AreEqual(1, managers.Length, "Manager should be fine with empty footstep array");
    }

    // Ensure no crash when footstep array is null.
    [UnityTest]
    public IEnumerator Boundary_FootstepArrayNull()
    {
        SetPrivateField("footStep", null);
        yield return null;

        var managers = Object.FindObjectsByType<SoundManager>(FindObjectsSortMode.None);
        Assert.AreEqual(1, managers.Length, "Manager should be fine with null footstep array");
    }

    /* ---------------- Stress & Performance Tests ---------------- */

    // Call BackgroundMusic() 50 times rapidly and confirm only one AudioSource is used
    // Technically two expected AudioSources exist (background + sfx)s
    [UnityTest]
    public IEnumerator Stress_BackgroundMusicRapidRestart()
    {
        var clip = AudioClip.Create("BG", 44100, 1, 44100, false);
        SetPrivateField("background_clip", clip);
        SetPrivateField("background_volume", 0.3f);

        for (int i = 0; i < 50; i++)
            soundManager.BackgroundMusic();

        yield return null;

        var sources = testObject.GetComponents<AudioSource>();
        Assert.AreEqual(2, sources.Length, "Should only have background and sfx AudioSources");
        Assert.IsTrue(sources[0].isPlaying, "Background should be playing after rapid restarts");
        Assert.IsTrue(sources[0].loop, "Background should remain looped");
    }

    // Call PlaySFX() 100 times with a valid clip and confirm no crash
    [UnityTest]
    public IEnumerator Stress_SFXRapidCalls()
    {
        var clip = AudioClip.Create("Jump", 44100, 1, 44100, false);
        SetPrivateField("playerJump", clip);

        for (int i = 0; i < 100; i++)
            SoundEvents.PlayerJump();

        yield return null;

        var sources = testObject.GetComponents<AudioSource>();
        Assert.AreEqual(2, sources.Length, "Should only have background and sfx AudioSources");
        Assert.IsTrue(sources[1].isPlaying, "SFX should be playing after rapid PlayOneShot calls");
    }

    // Invoke SoundEvents.PlayerJump() 100 times rapidly and confirm manager still singleton + subscribed
    [UnityTest]
    public IEnumerator Stress_EventFlooding()
    {
        var clip = AudioClip.Create("Jump", 44100, 1, 44100, false);
        SetPrivateField("playerJump", clip);

        for (int i = 0; i < 100; i++)
            SoundEvents.PlayerJump();

        yield return null;

        var sources = testObject.GetComponents<AudioSource>();
        Assert.IsTrue(sources[1].isPlaying, "SFX should be playing after event flooding");
        Assert.AreEqual(soundManager, SoundManager.Instance, "Singleton should remain stable under flooding");
    }

    [TearDown]
    public void Cleanup()
    {
        // Make sure no duplicate singletons pile up after tests
        if (SoundManager.Instance != null)
        {
            UnityEngine.Object.DestroyImmediate(SoundManager.Instance.gameObject);
        }
    }
    
}
    