using UnityEngine;
using System.Collections.Generic;

public class TerrainHandler : MonoBehaviour
{
    public int viewDistance = 30;
    public int worldWidth = 10;
    public int worldHeight = 20;
    public int chunkSize = 10;
    public StaticChunk chunkPrefab;

    private NoiseHandler noiseHandler;
    private Transform viewer; // Find first camera object, use for view culling. This is a good example of low coupling
    public Dictionary<Vector2Int, StaticChunk> loadedChunks = new Dictionary<Vector2Int, StaticChunk>();

    void Start()
    {
        noiseHandler = GetComponent<NoiseHandler>();
        viewer = FindFirstObjectByType<Camera>().transform;
        _LoadChunks();
    }

    void Update()
    {
        if (viewer != null)
        {
            _UpdateChunks();
            DestroyInRadius(viewer.transform.position, 5);
        }
        else
        {
            Debug.Log("No viewer detected. Attempting to find camera object");
            viewer = FindFirstObjectByType<Camera>().transform;
        }
    }

    void _LoadChunks()
    {
        // Generate grid of disabled chunks
        for (int x = 0; x < worldWidth; x++)
        {
            for (int y = 0; y < worldHeight; y++)
            {
                // Initialize each chunk
                StaticChunk chunk = Instantiate(chunkPrefab);
                chunk.transform.position = new Vector3(x * chunkSize, y * chunkSize, 0);
                chunk.transform.parent = transform;
                chunk.name = $"Chunk {x}, {y}";
                chunk.chunkSize = chunkSize;
                chunk.noiseHandler = noiseHandler;
                loadedChunks[new Vector2Int(x, y)] = chunk;
            }
        }
    }

    public void DestroyInRadius(Vector3 position, float radius)
    {
        List<StaticChunk> chunks = GetChunksInRadius(position, radius + chunkSize);

        foreach (var chunk in chunks)
        {
            chunk.DestroyInRadius(position, radius);
        }
    }


    public List<StaticChunk> GetChunksInRadius(Vector3 position, float radius)
    {
        List<StaticChunk> chunksInRadius = new List<StaticChunk>();

        Vector2 center = new Vector2(position.x, position.y);

        foreach (var kvp in loadedChunks)
        {
            // Convert the chunk’s world position to a 2D point
            Vector2 chunkPos = kvp.Value.transform.position;

            // Compute distance between chunk center and position
            float distance = Vector2.Distance(center, chunkPos);

            // Compare with world-space radius (not chunk units)
            if (distance <= radius)
            {
                chunksInRadius.Add(kvp.Value);
            }
        }

        return chunksInRadius;
    }


    void _UpdateChunks()
    {
        List<Vector2Int> points = GetPointsInRadius(GetChunkCoordFromPosition(viewer.position), viewDistance);


        for (int i = 0; i < points.Count; i++)
        {
            if (loadedChunks.ContainsKey(points[i]))
            {
                loadedChunks[points[i]].gameObject.SetActive(true);
            }
        }
    }

    Vector2Int GetChunkCoordFromPosition(Vector3 position)
    {
        int x = Mathf.FloorToInt(position.x / chunkSize);
        int y = Mathf.FloorToInt(position.y / chunkSize);
        return new Vector2Int(x, y);
    }

    List<Vector2Int> GetPointsInRadius(Vector2Int center, int radius)
    {
        List<Vector2Int> points = new List<Vector2Int>();

        for (int x = -radius; x <= radius; x++)
        {
            for (int y = -radius; y <= radius; y++)
            {
                Vector2Int point = new Vector2Int(center.x + x, center.y + y);
                points.Add(point);
            }
        }

        return points;
    }
}
