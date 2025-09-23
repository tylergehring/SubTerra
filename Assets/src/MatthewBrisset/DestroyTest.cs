using UnityEngine;

public class DestroyTest : MonoBehaviour
{
    public StaticChunk chunk;

    void Start()
    {
        
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            chunk.DestroyInRadius(transform.position, 10);
        }
    }
}
