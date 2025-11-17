
//JackKroll
//This supper class defins _toolDamage  _toolName and does a few other 
using UnityEngine;

public abstract class ToolSystem : MonoBehaviour
{
    [Header("Tool Settings")]
    protected string _toolName = "Tool";
   
    [SerializeField] private int _toolDamage = 10;
    [SerializeField] private int _toolTerLeval = 10;
    

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
