using UnityEngine;

/// Simple food item that heals the player when consumed.
public class Mushroom : NonReusableTools
{
    [Header("Mushroom Settings")]
    [SerializeField] private int _healthRestoreAmount = 1;
    [SerializeField] private AudioClip _consumeSound;

    public int HealthRestoreAmount => _healthRestoreAmount;

    private SpriteRenderer _spriteRenderer;

    protected override void OnEnable()
    {
        base.OnEnable();
        CacheSpriteRenderer();
    }

    /// Called when the mushroom enters the player's inventory.
    public override void OnPickup(PlayerController player)
    {
        base.OnPickup(player);
        
        // Hide the sprite when in inventory
        CacheSpriteRenderer();
        if (_spriteRenderer)
        {
            _spriteRenderer.enabled = false;
        }
    }

    /// Called when the mushroom leaves the player's inventory.
    public override void OnDropped(PlayerController player)
    {
        base.OnDropped(player);
        
        // Show the sprite when dropped
        CacheSpriteRenderer();
        if (_spriteRenderer)
        {
            _spriteRenderer.enabled = true;
        }
    }

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
        // Note: ChangeHealth subtracts the amount, so we negate to heal (this is so Arjuns enemies can subract health and my code will just negate that value)
        player.ChangeHealth(-_healthRestoreAmount);
        Debug.Log($"INFORMATION: {player.name} consumed a mushroom and restored {_healthRestoreAmount} health.");

        if (_consumeSound)
        {
            AudioSource.PlayClipAtPoint(_consumeSound, player.transform.position);
        }
    }

    private void CacheSpriteRenderer()
    {
        if (_spriteRenderer)
        {
            return;
        }

        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public override string GetToolSummary()
    {
        return $"{ToolName} restores {_healthRestoreAmount} health when eaten.";
    }

    public new string GetStaticSummary()
    {
        return $"{ToolName} is food with a predictable effect.";
    }
}
