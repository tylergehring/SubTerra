using UnityEngine;

public class PickaxeTool : UtilityTool
{
    public float breakRadius = 10f; // Radius to destroy chunks

    private TerrainHandler _terrain;

    public Transform player;
    private float rotationSpeed = 40f; // degrees per second


    private void Start()
    {
        _terrain = FindFirstObjectByType<TerrainHandler>();

        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }
    }
   

void Update()
    {

        //cnages position 
        if (gameObject.activeSelf) // checks if the prefab (this GameObject) is active
        {
            transform.position = player.position + player.forward * 1f;



            // Keep Z position fixed at 1
            var pos = transform.position;
            pos.y = player.position.x + -0.5f;
            pos.y = player.position.y + 0.1f;
            pos.z = -1f;
            transform.position = pos;

            if (Input.GetMouseButton(0)) // Left mouse button held down
            {
                // Rotate around the Y axis (you can change to X/Z as needed)
                transform.Rotate(0f, rotationSpeed * Time.deltaTime, 90f);


            }
        }
              
            

        

        if (Input.GetMouseButtonDown(0))
        {
            DestroyChunksAtPosition();
        }

    }

    // added by Connor
    public void DoThing()
    {
        DestroyChunksAtPosition();
    }

    private void DestroyChunksAtPosition()
    {
        if (_terrain != null)
        {
            _terrain.DestroyInRadius(transform.position, breakRadius);
        }
    }
}
