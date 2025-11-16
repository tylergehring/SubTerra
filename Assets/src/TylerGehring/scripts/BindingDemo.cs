using UnityEngine;

/// Demo script to show the difference between static and dynamic binding.
/// Attach this to any GameObject to see the output in the console.
/// "virtual" enables runtime method resolution. Instead of the compiler deciding which
/// method to call at compile-time the decision is mae at runtime based on the actual object type.
public class BindingDemo : MonoBehaviour
{
    void Start()
    {
        Debug.Log("=== DEMONSTRATING STATIC vs DYNAMIC BINDING ===\n");

        // Create a child object but store it in a parent reference
        HealthPrinter printer = new DetailedHealthPrinter(100, "Tyler");

        Debug.Log("--- Using Parent Reference to Child Object ---");

        // STATIC BINDING: Calls the PARENT version
        // Method is determined at compile-time by the reference type (HealthPrinter)
        printer.PrintHealthStatic();

        // DYNAMIC BINDING: Calls the CHILD version
        // Method is determined at runtime by the actual object type (DetailedHealthPrinter)
        printer.PrintHealthDynamic();

        Debug.Log("\n--- Using Child Reference to Child Object ---");

        // Now use the actual child reference
        DetailedHealthPrinter detailedPrinter = new DetailedHealthPrinter(75, "Arjun");

        // Both call the child versions because the reference type matches the object type
        detailedPrinter.PrintHealthStatic();
        detailedPrinter.PrintHealthDynamic();

        Debug.Log("\n=== KEY TAKEAWAY ===");
        Debug.Log("Static Binding: Method call resolved at COMPILE-TIME based on reference type");
        Debug.Log("Dynamic Binding: Method call resolved at RUNTIME based on actual object type");
    }
}
