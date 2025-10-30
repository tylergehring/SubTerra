using UnityEngine;

public class PickaxeTool : MonoBehaviour
{
    public float breakRadius = 10f; // Radius to destroy chunks

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.U))
        {
            DestroyChunksAtPosition();
        }
    }

    private void DestroyChunksAtPosition()
    {
        // Find all StaticChunk components in the scene
        StaticChunk[] chunks = FindObjectsOfType<StaticChunk>();

        if (chunks.Length == 0)
        {
            Debug.LogWarning("No chunks found!");
            return;
        }

        foreach (StaticChunk chunk in chunks)
        {
            float distance = Vector3.Distance(transform.position, chunk.transform.position);
            if (distance <= breakRadius)
            {
                chunk.DestroyInRadius(transform.position, breakRadius);
                Debug.Log($"Destroyed chunk: {chunk.name}");
            }
        }
    }
}
