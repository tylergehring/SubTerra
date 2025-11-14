using UnityEngine;

/// <summary>
/// Child class that overrides parent methods to demonstrate binding behavior.
/// </summary>
public class DetailedHealthPrinter : HealthPrinter
{
    private string playerName;

    public DetailedHealthPrinter(int health, string name) : base(health)
    {
        playerName = name;
    }

    // This method HIDES the parent's non-virtual method
    // It won't be called through a parent reference (static binding)
    public new void PrintHealthStatic()
    {
        Debug.Log($"[STATIC BINDING - Child] {playerName}'s health: {playerHealth}");
    }

    // This method OVERRIDES the parent's virtual method
    // It WILL be called through a parent reference (dynamic binding)
    public override void PrintHealthDynamic()
    {
        Debug.Log($"[DYNAMIC BINDING - Child] {playerName}'s health: {playerHealth}");
    }
}
