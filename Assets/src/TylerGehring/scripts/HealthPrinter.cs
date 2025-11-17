using UnityEngine;

/// <summary>
/// Parent class that demonstrates static and dynamic binding.
/// Shows how method binding works at compile-time vs runtime.
/// </summary>
public class HealthPrinter
{
    protected int playerHealth;

    public HealthPrinter(int health)
    {
        playerHealth = health;
    }

    // Non-virtual method - demonstrates STATIC BINDING
    // The method to call is determined at compile-time based on the reference type
    public void PrintHealthStatic()
    {
        Debug.Log($"[STATIC BINDING - Parent] Player health: {playerHealth}");
    }

    // Virtual method - demonstrates DYNAMIC BINDING
    // The method to call is determined at runtime based on the actual object type
    public virtual void PrintHealthDynamic()
    {
        Debug.Log($"[DYNAMIC BINDING - Parent] Player health: {playerHealth}");
    }
}
