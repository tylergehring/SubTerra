//Jack Kroll
//This class is primarly for the tool systems orgnization from the other sub class of reusabletoolcass
using UnityEngine;


public class UtilityTool : ReusableToolClass
{
    [Header("Utility Settings")]
  //used to define derblity was goring to do more with this in the future but it was low priority 
  //and is somthing i can use later for a version update 
    [SerializeField] private float durability = 100f;
    

      //old code that is used for overiting the tool name for seporation purposes    
    public override void UseTool(GameObject target = null)
    {
        base.UseTool(target);
         Debug.Log($"{_toolName} utility base logic executed.");
        
    }
}
