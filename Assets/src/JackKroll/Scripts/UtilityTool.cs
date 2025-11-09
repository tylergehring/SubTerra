using UnityEngine;

public class UtilityTool : ReusableToolClass
{
    [Header("Utility Settings")]
  
    [SerializeField] private float durability = 100f;
    

   
    public override void UseTool(GameObject target = null)
    {
        base.UseTool(target);
         Debug.Log($"{_toolName} utility base logic executed.");
        
    }
}
