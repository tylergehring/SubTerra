//JackKroll
using UnityEngine;
public class ReusableToolClass : ToolSystem
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
    }
}