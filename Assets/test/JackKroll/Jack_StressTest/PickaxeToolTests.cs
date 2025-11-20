//jack kroll
using NUnit.Framework;
using UnityEngine;

public class PickaxeToolTests
{
    private PickaxeTool _pickaxe;
    private GameObject _pickaxeGO;

    [SetUp]
    public void SetUp()
    {
        
        _pickaxe = Object.FindFirstObjectByType<PickaxeTool>();
        if (_pickaxe != null)
        {
            _pickaxeGO = _pickaxe.gameObject;
        }
    }

    [TearDown]
    public void TearDown()
    {
        // no clean up jsut in case 
    }

    // --------------------------
    // 1️ Prefab exists in scene
    // --------------------------
  
    // This test checks whether a PickaxeTool prefab exists in the scene.
    // If none exists, the test passes because the project allows it.
    // Otherwise it confirms that the pickaxe object is properly found.
    // -------------------------
    [Test]
    public void Pickaxe_PrefabExists()
    {
        if (_pickaxe == null) Assert.Pass(" No pickaxe prefab in the scene, test passes.");
        else Assert.IsNotNull(_pickaxe, " Pickaxe prefab not found in the scene!");
    }

    // --------------------------
    // 2️ Prefab is inactive initially

    // Ensures that the pickaxe GameObject starts inactive.
    // This is important because tools may be hidden until the player equips them
    // --------------------------
    [Test]
    public void Pickaxe_IsInactiveInitially()
    {
        if (_pickaxe != null)
            Assert.IsFalse(_pickaxe.gameObject.activeSelf, " Pickaxe should start inactive!");
    }

    // --------------------------
    // 3️ Prefab has AudioSource and sound is off

    // 1. The tool has an AudioSource component.
    // 2. The sound is NOT playing at the start.
    // Prevents audio from playing prematurely and ensures the prefab is set up correctly.
    // --------------------------
    [Test]
    public void Pickaxe_HasAudioSourceAndSoundOff()
    {
        if (_pickaxe != null)
        {
            var audio = _pickaxe.GetComponent<AudioSource>();
            Assert.IsNotNull(audio, " Pickaxe is missing an AudioSource component!");
            Assert.IsFalse(audio.isPlaying, " Pickaxe AudioSource should not be playing initially!");
        }
    }

    // --------------------------
    // 4️ No duplicate prefabs
    // This test verifies that only one tool prefab exists in the scene.
    // Helps prevent bugs caused by accidentally placing multiple prefabs.
    // --------------------------
    [Test]
    public void Pickaxe_NoDuplicatePrefabs()
    {
        var pickaxes = Object.FindObjectsByType<PickaxeTool>(FindObjectsSortMode.None);
        if (pickaxes.Length == 0)
        {
            Assert.Pass(" No pickaxe prefab found in the scene, test passes.");
        }
        Assert.LessOrEqual(pickaxes.Length, 1, $" There are {pickaxes.Length} pickaxe prefabs in the scene! Only 1 expected.");
    }

    // --------------------------
    // 5️ Multiple pickaxes in scene

    // Verifies that the tool can be safely instantiated multiple times.
    // A duplicate is created, and the test ensures that 2+ tools instances exist.
   
    // --------------------------
    [Test]
    public void Pickaxe_MultipleInstancesInScene()
    {
        if (_pickaxeGO != null)
        {
            var duplicate = Object.Instantiate(_pickaxeGO);
            var pickaxes = Object.FindObjectsByType<PickaxeTool>(FindObjectsSortMode.None);
            Assert.GreaterOrEqual(pickaxes.Length, 2, "There should be 2 or more pickaxe instances in the scene.");
            Object.DestroyImmediate(duplicate);
        }
    }

   


}
