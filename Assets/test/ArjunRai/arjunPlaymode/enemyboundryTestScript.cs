using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using UnityEngine.SceneManagement;

public class HazardBoundaryTest
{
    private GameObject enemyObject;
    private Hazard hazard;
    private PlayerController PC;
    private GameObject playerObject;

    [OneTimeSetUp]
    public void Setup()
    {
        // Load the test scene once before running the tests
        SceneManager.LoadScene("MyTestSceneCopy");
    }

    [UnityTest]
    public IEnumerator DamageBoundaryTest()
    {
        // Wait one frame for scene to load
        yield return null;

        // Find objects in scene
        enemyObject = GameObject.Find("Stone");
        playerObject = GameObject.Find("LegacyPlayerV2");

        hazard = enemyObject.GetComponent<Hazard>();
        PC = playerObject.GetComponent<PlayerController>();

        // --- Test 1: Positive Damage (health increases in your case) ---
        int previousHealth = PC.getHealth();
        hazard.setDamage(2f);                      // Apply positive damage
        yield return new WaitForSeconds(1.1f);     // Wait enough time for damage to apply
        Assert.Less(previousHealth, PC.getHealth(), "Player health did not increase with positive damage!");

        // --- Test 2: Zero Damage (health stays the same) ---
        previousHealth = PC.getHealth();
        hazard.setDamage(0f);
        yield return new WaitForSeconds(1.1f);
        Assert.AreEqual(previousHealth, PC.getHealth(), "Player health changed when damage = 0!");

        // --- Test 3: Negative Damage (health decreases in your case) ---
        previousHealth = PC.getHealth();
        hazard.setDamage(-2f);
        yield return new WaitForSeconds(1.1f);
        Assert.Greater(previousHealth, PC.getHealth(), "Player health did not decrease with negative damage!");
    }
}
