using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(NoiseHandler))]
public class TerrainHandler : MonoBehaviour
{
    public int viewDistance = 30;
    public int worldWidth = 10;
    public int worldHeight = 20;
    public int chunkSize = 10;
    public StaticChunk chunkPrefab;
    public TextureAtlas textureAtlas;
    public NoiseHandler noiseHandler;

    private Transform viewer; // Find first camera object, use for view culling. This is a good example of low coupling
    public Dictionary<Vector2Int, StaticChunk> loadedChunks = new Dictionary<Vector2Int, StaticChunk>();
    private bool isLoaded = false;

    void Start()
    {
        noiseHandler = GetComponent<NoiseHandler>();
        if (FindFirstObjectByType<Camera>() != null)
            viewer = FindFirstObjectByType<Camera>().transform;
    }

    public void GenerateTerrain()
    {
        if (!isLoaded)
        {
            _LoadChunks();
            isLoaded = true;
        }
    }

    void LateUpdate()
    {
        if (viewer != null)
        {
            _UpdateChunks();
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
                chunk.terrainHandler = this;
                loadedChunks[new Vector2Int(x, y)] = chunk;
            }
        }
    }

    public void DestroyInRadius(Vector3 position, float radius)
    {
        List<StaticChunk> chunks = GetChunksInRadius(position, radius + chunkSize * 2);

        foreach (var chunk in chunks)
        {
            if (!chunk.gameObject.activeSelf)
                continue; // Skip chunks that aren't initialized

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
                loadedChunks[points[i]].GetComponent<MeshRenderer>().enabled = true;
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

    public int GetTextureAtPoint(Vector2 point)
    {
        int textureIndex = 0;

        // Use different textures based on how close to walls
        float terrainNoiseValue = noiseHandler.TerrainNoiseValue(point.x, point.y);

        if (terrainNoiseValue > noiseHandler.terrainThreshold + (1 - noiseHandler.terrainThreshold) / 3)
            textureIndex = 1;

        if (terrainNoiseValue > noiseHandler.terrainThreshold + (1 - noiseHandler.terrainThreshold) / 2)
            textureIndex = 2;

        if (terrainNoiseValue > noiseHandler.terrainThreshold + (1 - noiseHandler.terrainThreshold) / 1.5f)
            textureIndex = 3;

        return textureIndex;
    }
}
