using UnityEngine;

/// <summary>
/// Simple food item that heals the player when consumed.
/// </summary>
public class Mushroom : Food
{
    [Header("Mushroom Settings")]
    [SerializeField] private AudioClip _consumeSound;

    public override void OnPickup(PlayerController player)
    {
        base.OnPickup(player);
        if (player)
        {
            Debug.Log($"INFORMATION: {player.name} picked up a mushroom.");
        }
    }

    protected override void ApplyFoodEffect(PlayerController player)
    {
        base.ApplyFoodEffect(player);
        Debug.Log($"INFORMATION: {player.name} consumed a mushroom and restored {HealthRestoreAmount} health.");

        if (_consumeSound)
        {
            AudioSource.PlayClipAtPoint(_consumeSound, player.transform.position);
        }
    }
}
