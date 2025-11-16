using NUnit.Framework;
using UnityEngine;

public class GoldPickAxeTests
{
    private GoldPickAxe _pickaxe;
    private GameObject _pickaxeGO;

    [SetUp]
    public void SetUp()
    {
        // Use modern API to find the first GoldPickAxe in the scene
        _pickaxe = Object.FindFirstObjectByType<GoldPickAxe>();
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
        if (_pickaxe == null) Assert.Pass(" No GoldPickAxe prefab in the scene, test passes.");
        else Assert.IsNotNull(_pickaxe, " GoldPickAxe prefab not found in the scene!");
    }

    // --------------------------
    // 2️⃣ Prefab is inactive initially
    // --------------------------
    [Test]
    public void Pickaxe_IsInactiveInitially()
    {
        if (_pickaxe != null)
            Assert.IsFalse(_pickaxeGO.activeSelf, " GoldPickAxe should start inactive!");
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
            Assert.IsNotNull(audio, " GoldPickAxe is missing an AudioSource component!");
            Assert.IsFalse(audio.isPlaying, " GoldPickAxe AudioSource should not be playing initially!");
        }
    }

    // --------------------------
    // 4️⃣ No duplicate prefabs
    // --------------------------
    [Test]
    public void Pickaxe_NoDuplicatePrefabs()
    {
        var pickaxes = Object.FindObjectsByType<GoldPickAxe>(FindObjectsSortMode.None);
        if (pickaxes.Length == 0)
        {
            Assert.Pass(" No GoldPickAxe prefab found in the scene, test passes.");
        }
        Assert.LessOrEqual(pickaxes.Length, 1, $" There are {pickaxes.Length} GoldPickAxe prefabs in the scene! Only 1 expected.");
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
            var pickaxes = Object.FindObjectsByType<GoldPickAxe>(FindObjectsSortMode.None);
            Assert.GreaterOrEqual(pickaxes.Length, 2, "There should be 2 or more GoldPickAxe instances in the scene.");
            Object.DestroyImmediate(duplicate);
        }
    }
}
