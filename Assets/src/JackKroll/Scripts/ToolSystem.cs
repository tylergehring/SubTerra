
//JackKroll
using UnityEngine;

public abstract class ToolSystem : MonoBehaviour
{
    [Header("Tool Settings")]
    protected string _toolName = "Tool";
   // public string _toolName = "Tool";
    [SerializeField] private int _toolDamage = 10;
    [SerializeField] private int _toolTerLeval = 10;
    //[SerializeField] private int _tool = 5;
    // Read-only access for other scripts
    // public string ToolName => _toolName;
    // public int ToolDamage => _toolDamage;
    // public int ToolWallDamage => _toolWallDamage;

    // Provides a Start that derived classes can override without errors
    protected virtual void Start() { }

    // Each tool will define its own behavior
    public abstract void UseTool(GameObject target = null);

    // Optional logging helper
    protected void LogUsage()
    {
        Debug.Log(_toolName + " is being used!");

    }
}
//dynamic and static have to be somthing changing on the screen 

/*

using UnityEngine;

public abstract class ToolSystem : MonoBehaviour
{
[Header("Tool Settings")]
public string _toolName = "Tool";
public int _toolDamage = 10;
public int _toolWallDamage = 5;


// Provides a Start that derived classes can override without errors
protected virtual void Start() { }

// each tool will define its own behavior
public abstract void UseTool(GameObject target = null);

// prints if tool is being used in the command line
protected void LogUsage()
{
   // Debug.Log(_toolName + " is being used!");
}
}

//static 
//dynamic tpye 
*/