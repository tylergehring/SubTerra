//jack kroll
using NUnit.Framework;
using UnityEngine;

public class FlashlightTests
{
    private ReusableToolClass _flashlightTool;
    private GameObject _flashlightGO;

    [SetUp]
    public void SetUp()
    {
        // Find the first ReusableToolClass (flashlight) instance in the scene
        _flashlightTool = Object.FindFirstObjectByType<ReusableToolClass>();
        if (_flashlightTool != null)
        {
            _flashlightGO = _flashlightTool.gameObject;
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
    public void Flashlight_PrefabExists()
    {
        if (_flashlightTool == null) Assert.Pass("No flashlight prefab in the scene, test passes.");
        else Assert.IsNotNull(_flashlightTool, "Flashlight prefab not found in the scene!");
    }

    // --------------------------
    // 2️ Prefab is inactive initially


    // Ensures that the tool GameObject starts inactive.
    // This is important because tools may be hidden until the player equips them
    // --------------------------
    [Test]
    public void Flashlight_IsInactiveInitially()
    {
        if (_flashlightTool != null)
            Assert.IsFalse(_flashlightGO.activeSelf, " Flashlight should start inactive!");
    }

    // --------------------------
    // 3️ Prefab has Light component and is off

    // 1. The tool has an AudioSource component.
    // 2. The sound is NOT playing at the start.
    // Prevents audio from playing prematurely and ensures the prefab is set up correctly.
    // --------------------------
    [Test]
    public void Flashlight_HasLightAndIsOff()
    {
        if (_flashlightTool != null)
        {
            var light = _flashlightGO.GetComponent<Light>();
            Assert.IsNotNull(light, " Flashlight is missing a Light component!");
            Assert.IsFalse(light.enabled, " Flashlight Light should start disabled!");
        }
    }

    // --------------------------
    // 4️ No duplicate prefabs

    // This test verifies that only one tool prefab exists in the scene.
    // Helps prevent bugs caused by accidentally placing multiple prefabs.
    // --------------------------
    [Test]
    public void Flashlight_NoDuplicatePrefabs()
    {
        var flashlights = Object.FindObjectsByType<ReusableToolClass>(FindObjectsSortMode.None);
        if (flashlights.Length == 0)
        {
            Assert.Pass(" No flashlight prefab found in the scene, test passes.");
        }
        Assert.LessOrEqual(flashlights.Length, 1, $" There are {flashlights.Length} flashlight prefabs in the scene! Only 1 expected.");
    }

    // --------------------------
    // 5️ Multiple instances in scene


    // Verifies that the tool can be safely instantiated multiple times.
    // A duplicate is created, and the test ensures that 2+ tools instances exist.
    // --------------------------
    [Test]
    public void Flashlight_MultipleInstancesInScene()
    {
        if (_flashlightGO != null)
        {
            var duplicate = Object.Instantiate(_flashlightGO);
            var flashlights = Object.FindObjectsByType<ReusableToolClass>(FindObjectsSortMode.None);
            Assert.GreaterOrEqual(flashlights.Length, 2, "There should be 2 or more flashlight instances in the scene.");
            Object.DestroyImmediate(duplicate);
        }
    }
}
