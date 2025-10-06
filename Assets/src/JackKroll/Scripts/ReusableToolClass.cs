// JackKroll
using UnityEngine;

public class ReusableToolClass : ToolSystem
{
    // Override the abstract UseTool method to define custom behavior
    public override void UseTool(GameObject target = null)
    {
        // Check if tool is usable
        if (!CanUseTool())
        {
            Debug.Log($"{_toolName} cannot be used anymore!");
            return;
        }

        // Perform the shovel action
        Debug.Log($"{_toolName} used successfully!");
        Debug.Log($"{_toolName} digs a hole!");
    }

    // Override Start to name and initialize this tool
    protected override void Start()
    {
        base.Start();
        _toolName = "Shovel";
        _isUsable = true;
        Debug.Log($"{_toolName} is ready to dig!");
    }

    // Optional custom pickup message
    protected override void OnPickedUp(GameObject player)
    {
        base.OnPickedUp(player);
        Debug.Log($"{_toolName} added to {player.name}'s inventory (reusable tool).");
    }

    // Update runs every frame — perfect for detecting player input
    protected override void Update()
    {
        base.Update();

        // Only allow usage if the player has already picked up the shovel
        if (_isPickedUp)
        {
            // When the player presses E, use the shovel
            if (Input.GetKeyDown(KeyCode.E))
            {
                UseTool();
            }
        }
    }
}
