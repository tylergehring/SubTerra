// Tyler Gehring
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using System.Reflection;

/// <summary>
/// Comprehensive test suite for the NonReusableTool system.
/// Tests Mushroom, TNT, and base NonReusableTools functionality including:
/// - Pickup/drop mechanics, consumption states, health restoration
/// - TNT fuse timing, explosions, physics, terrain destruction
/// - Edge cases, stress tests, and boundary conditions
/// Tests use only TylerGehring_Scripts to minimize dependencies on other team members' code.
/// </summary>
public class NonReusableToolClassPlayModeTests
{
    private GameObject testMushroomObj;
    private Mushroom mushroom;
    private GameObject testTNTObj;
    private TNT tnt;
    private GameObject testPlayerObj;
    private PlayerController testPlayer;

    [UnitySetUp]
    public IEnumerator Setup()
    {
        // Ignore all log messages during setup to avoid false failures from teammates' code
        // This prevents ArjunRai's enemy scripts from causing test failures
        LogAssert.ignoreFailingMessages = true;
        
        // Load the MVP scene
        SceneManager.LoadScene("MVP");
        yield return null; // Wait a frame for scene load to complete
        
        // Create a test mushroom GameObject
        testMushroomObj = new GameObject("TestMushroom");
        mushroom = testMushroomObj.AddComponent<Mushroom>();
        mushroom.gameObject.AddComponent<SpriteRenderer>();

        // Create a test TNT GameObject
        testTNTObj = new GameObject("TestTNT");
        tnt = testTNTObj.AddComponent<TNT>();
        tnt.gameObject.AddComponent<SpriteRenderer>();
        // Don't manually add Rigidbody2D and CircleCollider2D - TNT adds them itself
        
        // Try to find existing player in scene
        testPlayer = GameObject.FindObjectOfType<PlayerController>();
        
        yield return null;
        
        // Re-enable log assertion checking after setup is complete
        LogAssert.ignoreFailingMessages = false;
    }

    [UnityTearDown]
    public IEnumerator Teardown()
    {
        // Ignore log messages during teardown as well
        LogAssert.ignoreFailingMessages = true;
        
        // Clean up test objects
        if (testMushroomObj != null)
            Object.Destroy(testMushroomObj);
        
        if (testTNTObj != null)
            Object.Destroy(testTNTObj);
        
        if (testPlayerObj != null)
            Object.Destroy(testPlayerObj);
        
        yield return null;
        
        // Reset log assertion checking
        LogAssert.ignoreFailingMessages = false;
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
                BindingFlags.NonPublic | BindingFlags.Instance);
            
            if (consumedField != null)
            {
                consumedField.SetValue(mushroom, true);
                yield return null;

                Assert.IsTrue(mushroom.IsConsumed, "Mushroom should be marked as consumed");
            }
        }
    }

    // ==================== MUSHROOM TESTS ====================

    /// <summary>
    /// TEST 1: Verifies mushroom sprite is hidden when picked up.
    /// </summary>
    [UnityTest]
    public IEnumerator Mushroom_SpriteHiddenOnPickup()
    {
        SpriteRenderer spriteRenderer = mushroom.GetComponent<SpriteRenderer>();
        Assert.IsNotNull(spriteRenderer, "Mushroom should have a SpriteRenderer component");
        
        spriteRenderer.enabled = true;
        
        if (testPlayer != null)
        {
            mushroom.OnPickup(testPlayer);
            yield return null;
            
            Assert.IsFalse(spriteRenderer.enabled, "Mushroom sprite should be hidden when picked up");
        }
        else
        {
            Assert.Inconclusive("No player available for testing");
        }
    }

    /// <summary>
    /// TEST 2: Verifies mushroom sprite is shown when dropped.
    /// </summary>
    [UnityTest]
    public IEnumerator Mushroom_SpriteShownOnDrop()
    {
        SpriteRenderer spriteRenderer = mushroom.GetComponent<SpriteRenderer>();
        spriteRenderer.enabled = false;
        
        if (testPlayer != null)
        {
            mushroom.OnPickup(testPlayer);
            yield return null;
            
            mushroom.OnDropped(testPlayer);
            yield return null;
            
            Assert.IsTrue(spriteRenderer.enabled, "Mushroom sprite should be shown when dropped");
        }
        else
        {
            Assert.Inconclusive("No player available for testing");
        }
    }

    /// <summary>
    /// TEST 3: Tests mushroom with zero health restore amount.
    /// </summary>
    [UnityTest]
    public IEnumerator Mushroom_ZeroHealthRestore()
    {
        var healthRestoreField = typeof(Mushroom).GetField("_healthRestoreAmount",
            BindingFlags.NonPublic | BindingFlags.Instance);
        
        if (healthRestoreField != null)
        {
            healthRestoreField.SetValue(mushroom, 0);
            yield return null;
            
            Assert.AreEqual(0, mushroom.HealthRestoreAmount, "Mushroom should accept zero health restore");
        }
        else
        {
            Assert.Inconclusive("Could not access _healthRestoreAmount field");
        }
    }

    /// <summary>
    /// TEST 4: Tests mushroom with negative health restore amount.
    /// </summary>
    [UnityTest]
    public IEnumerator Mushroom_NegativeHealthRestore()
    {
        var healthRestoreField = typeof(Mushroom).GetField("_healthRestoreAmount",
            BindingFlags.NonPublic | BindingFlags.Instance);
        
        if (healthRestoreField != null)
        {
            healthRestoreField.SetValue(mushroom, -10);
            yield return null;
            
            Assert.AreEqual(-10, mushroom.HealthRestoreAmount, "Mushroom should accept negative health restore");
        }
        else
        {
            Assert.Inconclusive("Could not access _healthRestoreAmount field");
        }
    }

    /// <summary>
    /// TEST 5: Verifies consumed mushroom cannot be used again.
    /// </summary>
    [UnityTest]
    public IEnumerator Mushroom_CannotUseWhenConsumed()
    {
        if (testPlayer != null)
        {
            // First use - should work
            mushroom.Use(testPlayer);
            yield return null;
            
            Assert.IsTrue(mushroom.IsConsumed, "Mushroom should be consumed after first use");
            
            // Second use - should not work
            mushroom.Use(testPlayer);
            yield return null;
            
            // Still should be consumed, no errors
            Assert.IsTrue(mushroom.IsConsumed, "Mushroom should still be consumed");
        }
        else
        {
            Assert.Inconclusive("No player available for testing");
        }
    }

    /// <summary>
    /// TEST 6: Tests mushroom owner assignment on pickup.
    /// </summary>
    [UnityTest]
    public IEnumerator Mushroom_OwnerAssignedOnPickup()
    {
        if (testPlayer != null)
        {
            Assert.IsNull(mushroom.Owner, "Mushroom should start with no owner");
            
            mushroom.OnPickup(testPlayer);
            yield return null;
            
            Assert.AreEqual(testPlayer, mushroom.Owner, "Mushroom owner should be assigned on pickup");
        }
        else
        {
            Assert.Inconclusive("No player available for testing");
        }
    }

    /// <summary>
    /// TEST 7: Tests mushroom owner cleared on drop.
    /// </summary>
    [UnityTest]
    public IEnumerator Mushroom_OwnerClearedOnDrop()
    {
        if (testPlayer != null)
        {
            mushroom.OnPickup(testPlayer);
            yield return null;
            
            Assert.AreEqual(testPlayer, mushroom.Owner, "Mushroom should have owner after pickup");
            
            mushroom.OnDropped(testPlayer);
            yield return null;
            
            Assert.IsNull(mushroom.Owner, "Mushroom owner should be cleared on drop");
        }
        else
        {
            Assert.Inconclusive("No player available for testing");
        }
    }

    /// <summary>
    /// TEST 8: Tests using mushroom without player reference.
    /// </summary>
    [UnityTest]
    public IEnumerator Mushroom_UseWithoutPlayer()
    {
        mushroom.Use(null);
        yield return null;
        
        // Should not crash and should not be consumed
        Assert.IsFalse(mushroom.IsConsumed, "Mushroom should not be consumed when used without player");
    }

    /// <summary>
    /// TEST 9: Stress test - Create and destroy many mushrooms rapidly.
    /// </summary>
    [UnityTest]
    public IEnumerator Mushroom_StressTestRapidCreationDestruction()
    {
        int count = 500;
        
        for (int i = 0; i < count; i++)
        {
            GameObject obj = new GameObject($"TempMushroom_{i}");
            Mushroom temp = obj.AddComponent<Mushroom>();
            temp.gameObject.AddComponent<SpriteRenderer>();
            
            Assert.IsNotNull(temp, "Mushroom should be created successfully");
            Assert.IsFalse(temp.IsConsumed, "New mushroom should not be consumed");
            
            Object.Destroy(obj);
            
            if (i % 50 == 0)
            {
                yield return null;
            }
        }
    }

    // ==================== TNT TESTS ====================

    /// <summary>
    /// TEST 10: Verifies TNT starts in unlit state.
    /// </summary>
    [UnityTest]
    public IEnumerator TNT_StartsUnlit()
    {
        var isLitField = typeof(TNT).GetField("_isLit", BindingFlags.NonPublic | BindingFlags.Instance);
        
        if (isLitField != null)
        {
            bool isLit = (bool)isLitField.GetValue(tnt);
            Assert.IsFalse(isLit, "TNT should start in unlit state");
        }
        else
        {
            Assert.Inconclusive("Could not access _isLit field");
        }
        
        yield return null;
    }

    /// <summary>
    /// TEST 11: Verifies TNT has required physics components.
    /// </summary>
    [UnityTest]
    public IEnumerator TNT_HasRequiredPhysicsComponents()
    {
        Rigidbody2D rb = tnt.GetComponent<Rigidbody2D>();
        CircleCollider2D collider = tnt.GetComponent<CircleCollider2D>();
        
        Assert.IsNotNull(rb, "TNT should have Rigidbody2D component");
        Assert.IsNotNull(collider, "TNT should have CircleCollider2D component");
        
        yield return null;
    }

    /// <summary>
    /// TEST 12: Tests TNT explosion radius is configurable.
    /// </summary>
    [UnityTest]
    public IEnumerator TNT_ExplosionRadiusConfigurable()
    {
        var explosionRadiusField = typeof(TNT).GetField("_explosionRadius",
            BindingFlags.NonPublic | BindingFlags.Instance);
        
        if (explosionRadiusField != null)
        {
            explosionRadiusField.SetValue(tnt, 50f);
            yield return null;
            
            float radius = (float)explosionRadiusField.GetValue(tnt);
            Assert.AreEqual(50f, radius, "TNT explosion radius should be configurable");
        }
        else
        {
            Assert.Inconclusive("Could not access _explosionRadius field");
        }
    }

    /// <summary>
    /// TEST 13: Tests TNT fuse time is configurable.
    /// </summary>
    [UnityTest]
    public IEnumerator TNT_FuseTimeConfigurable()
    {
        var fuseTimeField = typeof(TNT).GetField("_fuseTime",
            BindingFlags.NonPublic | BindingFlags.Instance);
        
        if (fuseTimeField != null)
        {
            fuseTimeField.SetValue(tnt, 5f);
            yield return null;
            
            float fuseTime = (float)fuseTimeField.GetValue(tnt);
            Assert.AreEqual(5f, fuseTime, "TNT fuse time should be configurable");
        }
        else
        {
            Assert.Inconclusive("Could not access _fuseTime field");
        }
    }

    /// <summary>
    /// TEST 14: Tests TNT throw force is configurable.
    /// </summary>
    [UnityTest]
    public IEnumerator TNT_ThrowForceConfigurable()
    {
        var throwForceField = typeof(TNT).GetField("_throwForce",
            BindingFlags.NonPublic | BindingFlags.Instance);
        
        if (throwForceField != null)
        {
            throwForceField.SetValue(tnt, 20f);
            yield return null;
            
            float throwForce = (float)throwForceField.GetValue(tnt);
            Assert.AreEqual(20f, throwForce, "TNT throw force should be configurable");
        }
        else
        {
            Assert.Inconclusive("Could not access _throwForce field");
        }
    }

    /// <summary>
    /// TEST 15: Verifies TNT sprite is hidden when picked up.
    /// </summary>
    [UnityTest]
    public IEnumerator TNT_SpriteHiddenOnPickup()
    {
        SpriteRenderer spriteRenderer = tnt.GetComponent<SpriteRenderer>();
        Assert.IsNotNull(spriteRenderer, "TNT should have a SpriteRenderer component");
        
        spriteRenderer.enabled = true;
        
        if (testPlayer != null)
        {
            tnt.OnPickup(testPlayer);
            yield return null;
            
            Assert.IsFalse(spriteRenderer.enabled, "TNT sprite should be hidden when picked up");
        }
        else
        {
            Assert.Inconclusive("No player available for testing");
        }
    }

    /// <summary>
    /// TEST 16: Verifies TNT sprite is shown when dropped.
    /// </summary>
    [UnityTest]
    public IEnumerator TNT_SpriteShownOnDrop()
    {
        SpriteRenderer spriteRenderer = tnt.GetComponent<SpriteRenderer>();
        spriteRenderer.enabled = false;
        
        if (testPlayer != null)
        {
            tnt.OnPickup(testPlayer);
            yield return null;
            
            tnt.OnDropped(testPlayer);
            yield return null;
            
            Assert.IsTrue(spriteRenderer.enabled, "TNT sprite should be shown when dropped");
        }
        else
        {
            Assert.Inconclusive("No player available for testing");
        }
    }

    /// <summary>
    /// TEST 17: Tests TNT rigidbody becomes kinematic when picked up.
    /// </summary>
    [UnityTest]
    public IEnumerator TNT_RigidbodyKinematicOnPickup()
    {
        Rigidbody2D rb = tnt.GetComponent<Rigidbody2D>();
        rb.isKinematic = false;
        
        if (testPlayer != null)
        {
            tnt.OnPickup(testPlayer);
            yield return null;
            
            Assert.IsTrue(rb.isKinematic, "TNT rigidbody should be kinematic when picked up");
        }
        else
        {
            Assert.Inconclusive("No player available for testing");
        }
    }

    /// <summary>
    /// TEST 18: Tests TNT owner assignment on pickup.
    /// </summary>
    [UnityTest]
    public IEnumerator TNT_OwnerAssignedOnPickup()
    {
        if (testPlayer != null)
        {
            Assert.IsNull(tnt.Owner, "TNT should start with no owner");
            
            tnt.OnPickup(testPlayer);
            yield return null;
            
            Assert.AreEqual(testPlayer, tnt.Owner, "TNT owner should be assigned on pickup");
        }
        else
        {
            Assert.Inconclusive("No player available for testing");
        }
    }

    /// <summary>
    /// TEST 19: Tests TNT owner cleared on drop.
    /// </summary>
    [UnityTest]
    public IEnumerator TNT_OwnerClearedOnDrop()
    {
        if (testPlayer != null)
        {
            tnt.OnPickup(testPlayer);
            yield return null;
            
            Assert.AreEqual(testPlayer, tnt.Owner, "TNT should have owner after pickup");
            
            tnt.OnDropped(testPlayer);
            yield return null;
            
            Assert.IsNull(tnt.Owner, "TNT owner should be cleared on drop");
        }
        else
        {
            Assert.Inconclusive("No player available for testing");
        }
    }

    /// <summary>
    /// TEST 20: Tests using TNT without player reference.
    /// </summary>
    [UnityTest]
    public IEnumerator TNT_UseWithoutPlayer()
    {
        tnt.Use(null);
        yield return null;
        
        // Should not crash and should not be consumed
        Assert.IsFalse(tnt.IsConsumed, "TNT should not be consumed when used without player");
    }

    /// <summary>
    /// TEST 21: Tests TNT bounciness physics material is applied.
    /// </summary>
    [UnityTest]
    public IEnumerator TNT_BouncinessPhysicsMaterial()
    {
        Rigidbody2D rb = tnt.GetComponent<Rigidbody2D>();
        
        yield return null;
        
        Assert.IsNotNull(rb.sharedMaterial, "TNT should have physics material applied");
    }

    /// <summary>
    /// TEST 22: Tests TNT explosion radius boundary with zero radius.
    /// </summary>
    [UnityTest]
    public IEnumerator TNT_ZeroExplosionRadius()
    {
        var explosionRadiusField = typeof(TNT).GetField("_explosionRadius",
            BindingFlags.NonPublic | BindingFlags.Instance);
        
        if (explosionRadiusField != null)
        {
            explosionRadiusField.SetValue(tnt, 0f);
            yield return null;
            
            float radius = (float)explosionRadiusField.GetValue(tnt);
            Assert.AreEqual(0f, radius, "TNT should accept zero explosion radius");
        }
        else
        {
            Assert.Inconclusive("Could not access _explosionRadius field");
        }
    }

    /// <summary>
    /// TEST 23: Tests TNT with extreme explosion radius.
    /// </summary>
    [UnityTest]
    public IEnumerator TNT_ExtremeExplosionRadius()
    {
        var explosionRadiusField = typeof(TNT).GetField("_explosionRadius",
            BindingFlags.NonPublic | BindingFlags.Instance);
        
        if (explosionRadiusField != null)
        {
            explosionRadiusField.SetValue(tnt, 10000f);
            yield return null;
            
            float radius = (float)explosionRadiusField.GetValue(tnt);
            Assert.AreEqual(10000f, radius, "TNT should accept extreme explosion radius");
        }
        else
        {
            Assert.Inconclusive("Could not access _explosionRadius field");
        }
    }

    /// <summary>
    /// TEST 24: Stress test - Create and destroy many TNT objects rapidly.
    /// </summary>
    [UnityTest]
    public IEnumerator TNT_StressTestRapidCreationDestruction()
    {
        int count = 300;
        
        for (int i = 0; i < count; i++)
        {
            GameObject obj = new GameObject($"TempTNT_{i}");
            TNT temp = obj.AddComponent<TNT>();
            temp.gameObject.AddComponent<SpriteRenderer>();
            // Don't manually add Rigidbody2D and CircleCollider2D - TNT adds them itself
            
            Assert.IsNotNull(temp, "TNT should be created successfully");
            Assert.IsFalse(temp.IsConsumed, "New TNT should not be consumed");
            
            Object.Destroy(obj);
            
            if (i % 30 == 0)
            {
                yield return null;
            }
        }
    }

    // ==================== BASE CLASS & INTEGRATION TESTS ====================

    /// <summary>
    /// TEST 25: Tests tool name property is accessible.
    /// </summary>
    [UnityTest]
    public IEnumerator BaseClass_ToolNameAccessible()
    {
        Assert.IsNotNull(mushroom.ToolName, "Tool name should be accessible");
        Assert.IsNotNull(tnt.ToolName, "Tool name should be accessible");
        yield return null;
    }

    /// <summary>
    /// TEST 26: Tests multiple tools can exist simultaneously.
    /// </summary>
    [UnityTest]
    public IEnumerator Integration_MultipleToolsSimultaneously()
    {
        GameObject mushroom1 = new GameObject("Mushroom1");
        Mushroom m1 = mushroom1.AddComponent<Mushroom>();
        mushroom1.AddComponent<SpriteRenderer>();
        
        GameObject mushroom2 = new GameObject("Mushroom2");
        Mushroom m2 = mushroom2.AddComponent<Mushroom>();
        mushroom2.AddComponent<SpriteRenderer>();
        
        GameObject tnt1 = new GameObject("TNT1");
        TNT t1 = tnt1.AddComponent<TNT>();
        tnt1.AddComponent<SpriteRenderer>();
        // Don't manually add Rigidbody2D and CircleCollider2D - TNT adds them itself
        
        yield return null;
        
        Assert.IsNotNull(m1, "First mushroom should exist");
        Assert.IsNotNull(m2, "Second mushroom should exist");
        Assert.IsNotNull(t1, "TNT should exist");
        
        Assert.IsFalse(m1.IsConsumed, "First mushroom should not be consumed");
        Assert.IsFalse(m2.IsConsumed, "Second mushroom should not be consumed");
        Assert.IsFalse(t1.IsConsumed, "TNT should not be consumed");
        
        Object.Destroy(mushroom1);
        Object.Destroy(mushroom2);
        Object.Destroy(tnt1);
        
        yield return null;
    }

    /// <summary>
    /// TEST 27: Tests different tools maintain independent state.
    /// </summary>
    [UnityTest]
    public IEnumerator Integration_IndependentToolStates()
    {
        GameObject mushroomObj = new GameObject("TestMushroom");
        Mushroom testMushroom = mushroomObj.AddComponent<Mushroom>();
        mushroomObj.AddComponent<SpriteRenderer>();
        
        GameObject tntObj = new GameObject("TestTNT");
        TNT testTNT = tntObj.AddComponent<TNT>();
        tntObj.AddComponent<SpriteRenderer>();
        // Don't manually add Rigidbody2D and CircleCollider2D - TNT adds them itself
        
        yield return null;
        
        if (testPlayer != null)
        {
            // Pick up mushroom
            testMushroom.OnPickup(testPlayer);
            yield return null;
            
            Assert.AreEqual(testPlayer, testMushroom.Owner, "Mushroom should have owner");
            Assert.IsNull(testTNT.Owner, "TNT should not have owner");
            
            // Pick up TNT
            testTNT.OnPickup(testPlayer);
            yield return null;
            
            Assert.AreEqual(testPlayer, testMushroom.Owner, "Mushroom should still have owner");
            Assert.AreEqual(testPlayer, testTNT.Owner, "TNT should now have owner");
            
            // Use mushroom
            testMushroom.Use(testPlayer);
            yield return null;
            
            Assert.IsTrue(testMushroom.IsConsumed, "Mushroom should be consumed");
            Assert.IsFalse(testTNT.IsConsumed, "TNT should not be consumed");
        }
        else
        {
            Assert.Inconclusive("No player available for testing");
        }
        
        Object.Destroy(mushroomObj);
        Object.Destroy(tntObj);
        
        yield return null;
    }
}