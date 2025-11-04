using UnityEngine;

public class PickaxeTool : MonoBehaviour
{
    public float breakRadius = 10f; // Radius to destroy chunks

    private TerrainHandler _terrain;
  
   

    private void Start()
    {
        _terrain = FindFirstObjectByType<TerrainHandler>();
    }
   

void Update()
    {

        //cnages position 
        if (gameObject.activeSelf) // checks if the prefab (this GameObject) is active
        {
            transform.position = new Vector3(115f, 88f, 0f);
        }

        if (Input.GetKeyDown(KeyCode.U))
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
