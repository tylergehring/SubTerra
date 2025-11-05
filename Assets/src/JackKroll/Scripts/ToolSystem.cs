
//JackKroll

    

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
