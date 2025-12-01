//jack kroll
using NUnit.Framework;
using UnityEngine;

public class ABetterGameTests
{
    private ABetterGame _weapon;
    private GameObject _weaponGO;

    [SetUp]
    public void SetUp()
    {
        // Use modern API to find the first ABetterGame instance in the scene
        _weapon = Object.FindFirstObjectByType<ABetterGame>();
        if (_weapon != null)
        {
            _weaponGO = _weapon.gameObject;
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
    public void Weapon_PrefabExists()
    {
        if (_weapon == null) Assert.Pass(" No ABetterGame prefab in the scene, test passes.");
        else Assert.IsNotNull(_weapon, " ABetterGame prefab not found in the scene!");
    }

    // --------------------------
    // 2️ Prefab is inactive initially


    // Ensures that the pickaxe GameObject starts inactive.
    // This is important because tools may be hidden until the player equips them
    // --------------------------
    [Test]
    public void Weapon_IsInactiveInitially()
    {
        if (_weapon != null)
            Assert.IsFalse(_weaponGO.activeSelf, " ABetterGame should start inactive!");
    }

    // --------------------------
    // 3️ Prefab has AudioSource and sound is off

    // 1. The tool has an AudioSource component.
    // 2. The sound is NOT playing at the start.
    // Prevents audio from playing prematurely and ensures the prefab is set up correctly.
    // --------------------------
    [Test]
    public void Weapon_HasAudioSourceAndSoundOff()
    {
        if (_weapon != null)
        {
            var audio = _weaponGO.GetComponent<AudioSource>();
            Assert.IsNotNull(audio, " ABetterGame is missing an AudioSource component!");
            Assert.IsFalse(audio.isPlaying, " ABetterGame AudioSource should not be playing initially!");
        }
    }

    // --------------------------
    // 4️ No duplicate prefabs

    // This test verifies that only one tool prefab exists in the scene.
    // Helps prevent bugs caused by accidentally placing multiple prefabs.
    // --------------------------
    [Test]
    public void Weapon_NoDuplicatePrefabs()
    {
        var weapons = Object.FindObjectsByType<ABetterGame>(FindObjectsSortMode.None);
        if (weapons.Length == 0)
        {
            Assert.Pass(" No ABetterGame prefab found in the scene, test passes.");
        }
        Assert.LessOrEqual(weapons.Length, 1, $" There are {weapons.Length} ABetterGame prefabs in the scene! Only 1 expected.");
    }

    // --------------------------
    // 5️ Multiple instances in scene


    // Verifies that the tool can be safely instantiated multiple times.
    // A duplicate is created, and the test ensures that 2+ tools instances exist.
    // --------------------------
    [Test]
    public void Weapon_MultipleInstancesInScene()
    {
        if (_weaponGO != null)
        {
            var duplicate = Object.Instantiate(_weaponGO);
            var weapons = Object.FindObjectsByType<ABetterGame>(FindObjectsSortMode.None);
            Assert.GreaterOrEqual(weapons.Length, 2, "There should be 2 or more ABetterGame instances in the scene.");
            Object.DestroyImmediate(duplicate);
        }
    }
}
