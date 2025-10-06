//Jack Kroll
using UnityEngine;

// The TestTool class inherits from ToolSystem
// This makes it a specific type of tool with custom behavior
public class TestTool : ToolSystem
{
    // Override the abstract UseTool method to define custom tool behavior
    public override void UseTool(GameObject target = null)
    {
        // Check if the tool is still usable
        if (!CanUseTool())
        {
            // If not, print a message and exit early
            Debug.Log($"{_toolName} cannot be used anymore!");
            return;
        }

        // Print a message showing the tool was used
        Debug.Log($"{_toolName} used for testing!");

        // If a target GameObject was passed, display its name
        if (target != null)
        {
            Debug.Log($"{_toolName} targeted {target.name}");
        }

        // Mark the tool as no longer usable
        _isUsable = false;
    }

    // Override Start to customize setup for this specific tool
    protected override void Start()
    {
        // Call the base class Start() to preserve its initialization behavior
        base.Start();

        // Give this tool a proper name for debugging and identification
        _toolName = "TestTool";
    }

    // Override what happens when this tool is picked up
    protected override void OnPickedUp(GameObject player)
    {
        // Run the default pickup logic (disables the object)
        base.OnPickedUp(player);

        // Add custom behavior — simulate adding to an inventory
        Debug.Log($"{_toolName} added to {player.name}'s inventory (simulation).");
    }
}
