using UnityEngine;

/// Base class for single-use tools such as food or explosives.
/// Simplified to work with existing ItemHandler and Inventory systems.

public abstract class NonReusableTools : MonoBehaviour
{
    [Header("Tool Setup")]
    [SerializeField] private string _toolName = "Tool";

    private bool _consumed;
    private PlayerController _owner;

    public string ToolName => _toolName;
    public PlayerController Owner => _owner;
    public bool IsConsumed => _consumed;

    protected virtual void OnEnable()
    {
        _consumed = false;
    }

    /// Called when the tool enters the player's inventory.
    public virtual void OnPickup(PlayerController player)
    {
        _owner = player;
        Debug.Log($"INFORMATION: {player?.name ?? "Unknown"} picked up {_toolName}.");
    }

    /// Called when the tool leaves the player's inventory.
    public virtual void OnDropped(PlayerController player)
    {
        if (_owner == player)
        {
            _owner = null;
        }
        Debug.Log($"INFORMATION: {player?.name ?? "Unknown"} dropped {_toolName}.");
    }

    /// Called by the player to trigger the tool's effect.
    public void Use(PlayerController player)
    {
        if (_consumed)
        {
            Debug.Log($"{name}: Attempted to use {_toolName}, but it has already been consumed.");
            return;
        }

        PlayerController user = player ? player : _owner;
        if (!user)
        {
            Debug.LogWarning($"{name}: No player associated with {_toolName}; use cancelled.");
            return;
        }

        _owner = user;
        Debug.Log($"INFORMATION: {user.name} is attempting to use {_toolName}.");
        bool consume = OnUse(user);
        if (consume)
        {
            _consumed = true;
            OnConsumed(user);
        }
        else
        {
            Debug.Log($"INFORMATION: {_toolName} was used by {user.name} but was not consumed.");
        }
    }

    /// Override to implement tool behaviour. Return true to remove the tool from the inventory.
    protected abstract bool OnUse(PlayerController player);

    protected virtual void OnConsumed(PlayerController player)
    {
        Debug.Log($"INFORMATION: {player?.name ?? "Unknown"} consumed {_toolName}.");
        if (player)
        {
            player.ConsumeCurrentTool(this, true);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
