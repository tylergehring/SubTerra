using UnityEngine;

/// Base class for consumable tools that restore health or apply buffs.
public abstract class Food : NonReusableTools
{
    [Header("Food Settings")]
    [SerializeField] private int _healthRestoreAmount = 1;

    public int HealthRestoreAmount => _healthRestoreAmount;

    protected override bool OnUse(PlayerController player)
    {
        if (!player)
        {
            Debug.LogWarning($"{name}: Cannot consume food without a player reference.");
            return false;
        }

        ApplyFoodEffect(player);
        return true;
    }

    protected virtual void ApplyFoodEffect(PlayerController player)
    {
        player.ChangeHealth(_healthRestoreAmount);
    }
}
