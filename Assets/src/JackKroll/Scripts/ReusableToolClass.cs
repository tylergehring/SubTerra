using UnityEngine;

// Jack Kroll
public class ReusableToolClass : ToolSystem
{
    [Header("Flashlight Settings")]
    //public Light flashlight;
    [SerializeField] private Light _flashlight;


    private bool _isOn = false;   

    void Start()
    {
        
        if (_flashlight == null)
        {
            _flashlight = GetComponent<Light>();
        }

        // Start with flashlight off
        if (_flashlight != null)
        {
            _flashlight.enabled = _isOn;
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
        

       



        // F to use the flashlight tool
        if (Input.GetKeyDown(KeyCode.L) || Input.GetKeyDown(KeyCode.Mouse1))
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
        if (_flashlight == null)
        {
           
           Debug.LogWarning("No Light assigned to " + _toolName);
            return;
        }

        _isOn = !_isOn;
        _flashlight.enabled = _isOn;


    }
}
