using UnityEngine;

/// Utility component to demonstrate static vs dynamic binding on the tool hierarchy.
public class ToolBindingDemonstrator : MonoBehaviour
{
    [SerializeField] private NonReusableTools[] sampleTools;

    [ContextMenu("Demonstrate Binding Behaviour")]
    public void DemonstrateBinding()
    {
        if (sampleTools == null || sampleTools.Length == 0)
        {
            Debug.LogWarning($"{name}: Provide at least one sample tool to demonstrate binding.");
            return;
        }

        foreach (NonReusableTools tool in sampleTools)
        {
            if (!tool)
            {
                continue;
            }

            NonReusableTools baseReference = tool;
            string dynamicResult = baseReference.GetToolSummary();
            string staticResult = baseReference.GetStaticSummary();
            string derivedStaticResult = _GetDerivedStaticSummary(tool);

            Debug.Log($"Dynamic binding ({tool.GetType().Name}): {dynamicResult}");
            Debug.Log($"Static binding via base ({tool.GetType().Name}): {staticResult}");
            Debug.Log($"Static binding via derived ({tool.GetType().Name}): {derivedStaticResult}");
        }
    }

    private string _GetDerivedStaticSummary(NonReusableTools tool)
    {
        // Casting to the concrete type lets the compiler pick the hidden method, demonstrating static binding.
        // (Tells the compiler to use the derived types mehtod.)
        return tool switch
        {
            Mushroom mushroom => mushroom.GetStaticSummary(),
            TNT tnt => tnt.GetStaticSummary(),
            _ => tool.GetStaticSummary()
        };
    }
}
