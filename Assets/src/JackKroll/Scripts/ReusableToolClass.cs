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

        if (_isOn && Input.GetKeyDown(KeyCode.A))
        {
            transform.localRotation = Quaternion.Euler(180f, 90f, 0f);

        }

        if (_isOn && Input.GetKeyDown(KeyCode.D))
        {
            transform.localRotation = Quaternion.Euler(0f, 90f, 0f);

        }
        if (_isOn && Input.GetKeyDown(KeyCode.P))
        {
            transform.Rotate(10f, 5f, 0f);

        }
        // F to use the flashlight tool
        if (Input.GetKeyDown(KeyCode.L))
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
        /* if (Input.GetKeyDown(KeyCode.A))
         {
             m_LocalRotation = m_LocalRotation + 90;
         }*/
        


    }
}
