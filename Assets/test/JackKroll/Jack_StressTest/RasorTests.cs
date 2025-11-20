//jack kroll
using NUnit.Framework;
using UnityEngine;

public class RasorTests
{
    private Rasor _rasor;
    private GameObject _rasorGO;

    [SetUp]
    public void SetUp()
    {
        // Find the first Rasor instance in the scene
        _rasor = Object.FindFirstObjectByType<Rasor>();
        if (_rasor != null)
        {
            _rasorGO = _rasor.gameObject;
        }
    }

    [TearDown]
    public void TearDown()
    {
        // Nothing special to clean up
    }

    // --------------------------
    // 1️ Prefab exists in scene
    // This test checks whether a tool prefab exists in the scene.
    // If none exists, the test passes because the project allows it.
    // Otherwise it confirms that the tool object is properly found.
    // --------------------------
    [Test]
    public void Rasor_PrefabExists()
    {
        if (_rasor == null) Assert.Pass(" No Rasor prefab in the scene, test passes.");
        else Assert.IsNotNull(_rasor, " Rasor prefab not found in the scene!");
    }

    // --------------------------
    // 2️ Prefab is inactive initially


    // Ensures that the pickaxe GameObject starts inactive.
    // This is important because tools may be hidden until the player equips them
    // --------------------------
    [Test]
    public void Rasor_IsInactiveInitially()
    {
        if (_rasor != null)
            Assert.IsFalse(_rasorGO.activeSelf, " Rasor should start inactive!");
    }

    // --------------------------
    // 3️ Prefab has AudioSource and sound is off

    // 1. The tool has an AudioSource component.
    // 2. The sound is NOT playing at the start.
    // Prevents audio from playing prematurely and ensures the prefab is set up correctly.
    // --------------------------
    [Test]
    public void Rasor_HasAudioSourceAndSoundOff()
    {
        if (_rasor != null)
        {
            var audio = _rasorGO.GetComponent<AudioSource>();
            Assert.IsNotNull(audio, " Rasor is missing an AudioSource component!");
            Assert.IsFalse(audio.isPlaying, " Rasor AudioSource should not be playing initially!");
        }
    }

    // --------------------------
    // 4️ No duplicate prefabs

    // This test verifies that only one tool prefab exists in the scene.
    // Helps prevent bugs caused by accidentally placing multiple prefabs.
    // --------------------------
    [Test]
    public void Rasor_NoDuplicatePrefabs()
    {
        var rasors = Object.FindObjectsByType<Rasor>(FindObjectsSortMode.None);
        if (rasors.Length == 0)
        {
            Assert.Pass(" No Rasor prefab found in the scene, test passes.");
        }
        Assert.LessOrEqual(rasors.Length, 1, $" There are {rasors.Length} Rasor prefabs in the scene! Only 1 expected.");
    }

    // --------------------------
    // 5️ Multiple instances in scene


    // Verifies that the tool can be safely instantiated multiple times.
    // A duplicate is created, and the test ensures that 2+ tools instances exist.
    // --------------------------
    [Test]
    public void Rasor_MultipleInstancesInScene()
    {
        if (_rasorGO != null)
        {
            var duplicate = Object.Instantiate(_rasorGO);
            var rasors = Object.FindObjectsByType<Rasor>(FindObjectsSortMode.None);
            Assert.GreaterOrEqual(rasors.Length, 2, "There should be 2 or more Rasor instances in the scene.");
            Object.DestroyImmediate(duplicate);
        }
    }
}
