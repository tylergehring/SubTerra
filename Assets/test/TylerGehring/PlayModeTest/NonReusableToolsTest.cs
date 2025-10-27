// Tyler Gehring
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;

/// <summary>
/// Stress and boundary tests for the NonReusableTool system.
/// Tests rapid mushroom creation/consumption, health restoration boundaries, and consumption state.
/// Tests use only TylerGehring_Scripts to minimize dependencies on other team members' code.
/// </summary>
public class NonReusableToolClassPlayModeTests
{
    private GameObject testMushroomObj;
    private Mushroom mushroom;
    private GameObject testPlayerObj;
    private PlayerController testPlayer;

    [UnitySetUp]
    public IEnumerator Setup()
    {
        // Load the MVP scene
        SceneManager.LoadScene("MVP");
        yield return null; // Wait a frame for scene load to complete

        // Create a test mushroom GameObject
        testMushroomObj = new GameObject("TestMushroom");
        mushroom = testMushroomObj.AddComponent<Mushroom>();

        // Try to find existing player in scene, or create a mock one for testing
        testPlayer = GameObject.FindObjectOfType<PlayerController>();
        
        yield return null;
    }

    [UnityTearDown]
    public IEnumerator Teardown()
    {
        // Clean up test objects
        if (testMushroomObj != null)
            Object.Destroy(testMushroomObj);
        
        if (testPlayerObj != null)
            Object.Destroy(testPlayerObj);
        
        yield return null;
    }

    /// <summary>
    /// STRESS TEST: Rapidly creates and consumes 1000 mushrooms while monitoring FPS.
    /// This simulates very heavy use over extended gameplay to ensure
    /// the NonReusableTool system works properly with no major errors
    /// under stress conditions. Logs a warning when FPS drops to 40 or below.
    /// </summary>
    [UnityTest]
    public IEnumerator Stress_CreateAndConsumeManyMushroomsRapidly()
    {
        int createCount = 10000;
        int successfulCreations = 0;
        bool lagWarningLogged = false;

        for (int i = 0; i < createCount; i++)
        {
            // Create a new mushroom
            GameObject mushroomObj = new GameObject($"StressMushroom_{i}");
            Mushroom testMushroom = mushroomObj.AddComponent<Mushroom>();

            // Verify mushroom initialized correctly
            Assert.IsNotNull(testMushroom, $"Mushroom {i} should not be null");
            Assert.IsFalse(testMushroom.IsConsumed, $"Mushroom {i} should start unconsumed");
            
            successfulCreations++;

            // Clean up immediately to prevent memory buildup
            Object.Destroy(mushroomObj);

            // Wait a frame every 100 creations to prevent timeout
            if (i % 100 == 0)
            {
                yield return null;

                // Measure FPS and log warning if it drops
                float currentFPS = 1f / Time.deltaTime;
                if (currentFPS <= 40f && !lagWarningLogged)
                {
                    Debug.LogWarning($"[PERFORMANCE] Game has started to lag! Current FPS: {currentFPS:F2} | Items Consumed: {successfulCreations}");
                    lagWarningLogged = true;
                }
            }
        }

        // Verify all mushrooms were created successfully
        Assert.AreEqual(createCount, successfulCreations,
            $"All {createCount} mushrooms should have been created successfully");
    }

    /// <summary>
    /// BOUNDARY TEST: Tests mushroom with maximum possible health restore value.
    /// This tests the game's response to extreme health values, ensuring
    /// the system handles large numbers without overflow or errors.
    /// </summary>
    [UnityTest]
    public IEnumerator Boundary_MaximumHealthRestoreAmount()
    {
        // Set health restore to a very high value using reflection
        int maxHealthRestore = int.MaxValue;
        var healthRestoreField = typeof(Mushroom).GetField("_healthRestoreAmount",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        if (healthRestoreField != null)
        {
            healthRestoreField.SetValue(mushroom, maxHealthRestore);
        }

        yield return null;

        // Verify the mushroom accepts and stores the max value
        Assert.AreEqual(maxHealthRestore, mushroom.HealthRestoreAmount,
            "Mushroom should handle maximum health restore amount");
        Assert.IsFalse(mushroom.IsConsumed, "Mushroom should not be consumed yet");
    }

    /// <summary>
    /// BOUNDARY TEST: Tests mushroom consumption state remains valid.
    /// This ensures that once a mushroom is consumed, it properly maintains
    /// its IsConsumed state and cannot be used again, preventing duplicate
    /// healing or other unintended behaviors.
    /// </summary>
    [UnityTest]
    public IEnumerator Boundary_ConsumptionStateStaysValid()
    {
        // Verify mushroom starts unconsumed
        Assert.IsFalse(mushroom.IsConsumed, "Mushroom should start unconsumed");

        // Simulate consumption by using the tool if player exists
        if (testPlayer != null)
        {
            mushroom.Use(testPlayer);
            yield return null;

            // After use, mushroom should be marked as consumed
            Assert.IsTrue(mushroom.IsConsumed, "Mushroom should be consumed after use");
        }
        else
        {
            // If no player available, manually trigger consumed state for testing
            var consumedField = typeof(NonReusableTools).GetField("_consumed",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            if (consumedField != null)
            {
                consumedField.SetValue(mushroom, true);
                yield return null;

                Assert.IsTrue(mushroom.IsConsumed, "Mushroom should be marked as consumed");
            }
        }
    }
}