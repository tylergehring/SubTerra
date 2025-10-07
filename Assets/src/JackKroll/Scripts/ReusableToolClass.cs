using UnityEngine;

// Jack Kroll
public class ReusableToolClass : ToolSystem
{
    [Header("Flashlight Settings")]
    public Light flashlight;  
    private bool _isOn = false;   

    void Start()
    {
        
        if (flashlight == null)
        {
            flashlight = GetComponent<Light>();
        }

        // Start with flashlight off
        if (flashlight != null)
        {
            flashlight.enabled = _isOn;
        }
    }

    void Update()
    {
        // F to use the flashlight tool
        if (Input.GetKeyDown(KeyCode.F))
        {
            UseTool();
        }
    }

    public override void UseTool(GameObject target = null)
    {
        LogUsage();       
        ToggleFlashlight();
    }

    //this will be updated 
    private void ToggleFlashlight()
    {
        if (flashlight == null)
        {
            Debug.LogWarning("No Light assigned to " + _toolName);
            return;
        }

        _isOn = !_isOn;
        flashlight.enabled = _isOn;

        Debug.Log(_toolName + " turned " + (_isOn ? "ON" : "OFF"));
    }
}
