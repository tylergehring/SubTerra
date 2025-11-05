//Jack Kroll
using UnityEngine;

public class GoldPickAxe : UtilityTool
{
    public float breakRadius = 10f; // Radius to destroy chunks

    private TerrainHandler _terrain;

    public Transform player;



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
            pos.y = player.position.y + 0.4f;
            pos.z = -1f;
            transform.position = pos;


        }





        if (Input.GetMouseButtonDown(0))
        {
            DestroyChunksAtPosition();
        }
    }

    private void DestroyChunksAtPosition()
    {
        if (_terrain != null)
        {
            _terrain.DestroyInRadius(transform.position, breakRadius);
        }
    }
}
