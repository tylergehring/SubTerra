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

        // get mouse position in world space
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10f));

        // get direction from flashlight to mouse
        Vector3 direction = mousePos - transform.position;

        // make the flashlight look toward the mouse (affects X and Y rotations)
        transform.rotation = Quaternion.LookRotation(direction);

        //old code for using keys to rotate light
        /*
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
        */

        

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
        //for testing 
       // Debug.Log(_toolName + " turned " + (_isOn ? "ON" : "OFF"));
        /* if (Input.GetKeyDown(KeyCode.A))
         {
             m_LocalRotation = m_LocalRotation + 90;
         }*/
        


    }
}
