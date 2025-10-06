//Jack Kroll
using UnityEngine;

// the TestTool class inherits from ToolSystem this will be used as a basiss for other tool classes 
public class TestTool : ToolSystem
{
    // override the abstract UseTool method to define custom tool behavior
    public override void UseTool(GameObject target = null)
    {
        // check if the tool is still usable
        if (!CanUseTool())
        {
            // if not, print a message and exit early
            Debug.Log($"{_reusableToolName} cannot be used anymore!");
            return;
        }

        // Print a message showing the tool was used
        Debug.Log($"{_reusableToolName} used for testing!");

        // If a target GameObject was passed, display its name
        if (target != null)
        {
            Debug.Log($"{_reusableToolName} targeted {target.name}");
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
        _reusableToolName = "TestTool";
    }

    // Override what happens when this tool is picked up
    protected override void OnPickedUp(GameObject player)
    {
        // Run the default pickup logic (disables the object)
        base.OnPickedUp(player);

        // Add custom behavior — simulate adding to an inventory
        Debug.Log($"{_reusableToolName} added to {player.name}'s inventory (simulation).");
    }
}
