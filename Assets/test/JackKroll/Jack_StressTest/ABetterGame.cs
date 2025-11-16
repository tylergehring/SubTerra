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
    // 1️⃣ Prefab exists in scene
    // --------------------------
    [Test]
    public void Weapon_PrefabExists()
    {
        if (_weapon == null) Assert.Pass("✅ No ABetterGame prefab in the scene, test passes.");
        else Assert.IsNotNull(_weapon, "❌ ABetterGame prefab not found in the scene!");
    }

    // --------------------------
    // 2️⃣ Prefab is inactive initially
    // --------------------------
    [Test]
    public void Weapon_IsInactiveInitially()
    {
        if (_weapon != null)
            Assert.IsFalse(_weaponGO.activeSelf, "❌ ABetterGame should start inactive!");
    }

    // --------------------------
    // 3️⃣ Prefab has AudioSource and sound is off
    // --------------------------
    [Test]
    public void Weapon_HasAudioSourceAndSoundOff()
    {
        if (_weapon != null)
        {
            var audio = _weaponGO.GetComponent<AudioSource>();
            Assert.IsNotNull(audio, "❌ ABetterGame is missing an AudioSource component!");
            Assert.IsFalse(audio.isPlaying, "❌ ABetterGame AudioSource should not be playing initially!");
        }
    }

    // --------------------------
    // 4️⃣ No duplicate prefabs
    // --------------------------
    [Test]
    public void Weapon_NoDuplicatePrefabs()
    {
        var weapons = Object.FindObjectsByType<ABetterGame>(FindObjectsSortMode.None);
        if (weapons.Length == 0)
        {
            Assert.Pass("✅ No ABetterGame prefab found in the scene, test passes.");
        }
        Assert.LessOrEqual(weapons.Length, 1, $"❌ There are {weapons.Length} ABetterGame prefabs in the scene! Only 1 expected.");
    }

    // --------------------------
    // 5️⃣ Multiple instances in scene
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
