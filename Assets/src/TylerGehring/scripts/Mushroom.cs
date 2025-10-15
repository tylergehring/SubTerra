using UnityEngine;

/// Simple food item that heals the player when consumed.
public class Mushroom : NonReusableTools
{
    [Header("Mushroom Settings")]
    [SerializeField] private int _healthRestoreAmount = 1;
    [SerializeField] private AudioClip _consumeSound;

    public int HealthRestoreAmount => _healthRestoreAmount;

    protected override bool OnUse(PlayerController player)
    {
        if (!player)
        {
            Debug.LogWarning($"{name}: Cannot consume mushroom without a player reference.");
            return false;
        }

        ApplyFoodEffect(player);
        return true; // Return true to consume the item
    }

    protected virtual void ApplyFoodEffect(PlayerController player)
    {
        player.ChangeHealth(_healthRestoreAmount);
        Debug.Log($"INFORMATION: {player.name} consumed a mushroom and restored {_healthRestoreAmount} health.");

        if (_consumeSound)
        {
            AudioSource.PlayClipAtPoint(_consumeSound, player.transform.position);
        }
    }
}
