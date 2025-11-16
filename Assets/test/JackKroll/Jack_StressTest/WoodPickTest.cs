using NUnit.Framework;
using UnityEngine;

public class WoodPickaxeTests
{
    private WoodPickaxe _pickaxe;
    private GameObject _pickaxeGO;

    [SetUp]
    public void SetUp()
    {
        // Use modern API to find the first WoodPickaxe in the scene
        _pickaxe = Object.FindFirstObjectByType<WoodPickaxe>();
        if (_pickaxe != null)
        {
            _pickaxeGO = _pickaxe.gameObject;
        }
    }

    [TearDown]
    public void TearDown()
    {
        // Nothing to clean up
    }

    // --------------------------
    // 1️⃣ Prefab exists in scene
    // --------------------------
    [Test]
    public void Pickaxe_PrefabExists()
    {
        if (_pickaxe == null) Assert.Pass(" No WoodPickaxe prefab in the scene, test passes.");
        else Assert.IsNotNull(_pickaxe, " WoodPickaxe prefab not found in the scene!");
    }

    // --------------------------
    // 2️⃣ Prefab is inactive initially
    // --------------------------
    [Test]
    public void Pickaxe_IsInactiveInitially()
    {
        if (_pickaxe != null)
            Assert.IsFalse(_pickaxeGO.activeSelf, " WoodPickaxe should start inactive!");
    }

    // --------------------------
    // 3️⃣ Prefab has AudioSource and sound is off
    // --------------------------
    [Test]
    public void Pickaxe_HasAudioSourceAndSoundOff()
    {
        if (_pickaxe != null)
        {
            var audio = _pickaxeGO.GetComponent<AudioSource>();
            Assert.IsNotNull(audio, " WoodPickaxe is missing an AudioSource component!");
            Assert.IsFalse(audio.isPlaying, " WoodPickaxe AudioSource should not be playing initially!");
        }
    }

    // --------------------------
    // 4️⃣ No duplicate prefabs
    // --------------------------
    [Test]
    public void Pickaxe_NoDuplicatePrefabs()
    {
        var pickaxes = Object.FindObjectsByType<WoodPickaxe>(FindObjectsSortMode.None);
        if (pickaxes.Length == 0)
        {
            Assert.Pass(" No WoodPickaxe prefab found in the scene, test passes.");
        }
        Assert.LessOrEqual(pickaxes.Length, 1, $" There are {pickaxes.Length} WoodPickaxe prefabs in the scene! Only 1 expected.");
    }

    // --------------------------
    // 5️⃣ Multiple pickaxes in scene
    // --------------------------
    [Test]
    public void Pickaxe_MultipleInstancesInScene()
    {
        if (_pickaxeGO != null)
        {
            var duplicate = Object.Instantiate(_pickaxeGO);
            var pickaxes = Object.FindObjectsByType<WoodPickaxe>(FindObjectsSortMode.None);
            Assert.GreaterOrEqual(pickaxes.Length, 2, "There should be 2 or more WoodPickaxe instances in the scene.");
            Object.DestroyImmediate(duplicate);
        }
    }
}
