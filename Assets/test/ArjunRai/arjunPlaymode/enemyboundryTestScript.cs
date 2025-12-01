using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;
using NUnit.Framework;
using System.Collections;

public class HazardBoundaryTest
{
    private Hazard hazard;
    private PlayerController player;

    private IEnumerator PrepareScene()
    {
        SceneManager.LoadScene("MyTestSceneCopy");
        yield return null;

        hazard = Object.FindFirstObjectByType<Hazard>();
        player = Object.FindFirstObjectByType<PlayerController>();

        Assert.NotNull(hazard, "Hazard not found in scene.");
        Assert.NotNull(player, "PlayerController not found in scene.");

        // Freeze movement
        hazard.SetActivationRange(999f);
        hazard.SetFollowRange(0f);

        hazard.transform.position = player.transform.position;
    }

    [UnityTest]
    public IEnumerator Hazard_PositiveDamage_DecreasesHealth()
    {
        yield return PrepareScene();

        int before = player.getHealth();
        hazard.setDamage(10);

        yield return new WaitForSeconds(1.05f);

        Assert.Less(player.getHealth(), before, "Positive damage did NOT reduce player health!");
    }

    [UnityTest]
    public IEnumerator Hazard_ZeroDamage_NoChange()
    {
        yield return PrepareScene();

        int before = player.getHealth();
        hazard.setDamage(0);

        yield return new WaitForSeconds(1.05f);

        Assert.AreEqual(before, player.getHealth(), "Zero damage should not change health!");
    }

    [UnityTest]
    public IEnumerator Hazard_NegativeDamage_IncreasesHealth()
    {
        yield return PrepareScene();

        int before = player.getHealth();
        hazard.setDamage(-10);

        yield return new WaitForSeconds(1.05f);

        Assert.Greater(player.getHealth(), before, "Negative damage should HEAL player!");
    }
}
