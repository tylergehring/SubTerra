//Jack Kroll

using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Collider2D))]
public abstract class ToolSystem : MonoBehaviour
{
    [SerializeField] protected string _toolName = "Unnamed Tool";
    [SerializeField] protected bool _isUsable = true;
    protected bool _isPickedUp = false;  // Track if player has collected this tool

    public abstract void UseTool(GameObject target = null);

    public virtual bool CanUseTool()
    {
        return _isUsable;
    }

    protected virtual void Start()
    {
        Debug.Log($"{_toolName} initialized.");

        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            col.isTrigger = true; // allow trigger detection
        }
        else
        {
            Debug.LogWarning($"{_toolName} needs a Collider2D for pickup detection!");
        }
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (!_isPickedUp && collision.CompareTag("Player"))
        {
            _isPickedUp = true;
            OnPickedUp(collision.gameObject);
        }
    }

    // Called when the tool is picked up
    protected virtual void OnPickedUp(GameObject player)
    {
        Debug.Log($"{_toolName} picked up by {player.name}");

        // Instead of disabling it completely, we can hide it visually
        GetComponent<SpriteRenderer>().enabled = false;

        // Optional: Disable collider so it can’t be picked up again
        GetComponent<Collider2D>().enabled = false;
    }

    public GameObject GetGameObject()
    {
        return gameObject;
    }

    // Allow tools to run their own update logic (like input checks)
    protected virtual void Update()
    {
        // Base class doesn’t do anything yet.
        // Derived classes can override this to check input.
    }
}
