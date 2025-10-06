//Jack Kroll
using UnityEngine;

// Require both a SpriteRenderer (for visuals) and a Collider2D (for pickup detection)
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Collider2D))]
public abstract class ToolSystem : MonoBehaviour
{
    // Tool name (visible in Inspector)
    [SerializeField] protected string _reusableToolName = "Unnamed Tool";

    // Determines whether the tool can still be used
    [SerializeField] protected bool _isUsable = true;

    // Tracks if the tool has already been picked up by the player
    protected bool _isPickedUp = false;

    // Abstract method — must be implemented by child classes
    // Defines what happens when the tool is used
    public abstract void UseTool(GameObject target = null);

    // Virtual method that can be overridden by child classes
    // Returns whether the tool is still usable
    public virtual bool CanUseTool()
    {
        return _isUsable;
    }

    // Called when the GameObject is first created or enabled in Unity
    protected virtual void Start()
    {
        // Print a message in the Unity Console confirming initialization
        Debug.Log($"{_reusableToolName} initialized.");

        // Ensure the tool has a Collider2D for detecting pickups
        Collider2D col = GetComponent<Collider2D>();

        // If a collider exists, make it a trigger (so it doesn't physically collide)
        if (col != null)
        {
            col.isTrigger = true;
        }
        else
        {
            // Warn the developer if no Collider2D is attached
            Debug.LogWarning($"{_reusableToolName} needs a Collider2D for pickup detection!");
        }
    }

    // Unity built-in method called when another collider enters this trigger
    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        // Only allow pickup if the tool hasn’t been picked up yet
        // and the object entering the trigger has the "Player" tag
        if (!_isPickedUp && collision.CompareTag("Player"))
        {
            // Mark the tool as picked up to prevent duplicate triggers
            _isPickedUp = true;

            // Call the OnPickedUp method, passing the player GameObject
            OnPickedUp(collision.gameObject);
        }
    }

    // Virtual method for handling what happens when the tool is picked up
    // Can be customized in derived classes
    protected virtual void OnPickedUp(GameObject player)
    {
        // Log the pickup event to the Console
        Debug.Log($"{_reusableToolName} picked up by {player.name}");

        // Disable the GameObject to simulate being picked up
        gameObject.SetActive(false);
    }

    // Returns a reference to this GameObject
    // Useful for inventory systems or other external scripts
    public GameObject GetGameObject()
    {
        return gameObject;
    }
}
