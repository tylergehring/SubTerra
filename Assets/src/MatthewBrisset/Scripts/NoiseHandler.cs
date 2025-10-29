using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class NoiseLayer
{
    [SerializeField]
    [Range(1, 30)]
    public float frequency = 5;

    [SerializeField]
    [Range(0, 1)]
    public float intensity = 0.5f;
}

// Used to generate noise based on a point
public class NoiseHandler : MonoBehaviour
{
    public List<NoiseLayer> noise;
    [Range(0, 1)]
    public float terrainThreshold = 0.5f;
    public int edgeThickness = 10;
    public float seed = 10000;

    private TerrainHandler handler;
    private int chunkSize;
    private int worldWidth;
    private int worldHeight;

    private void Start()
    {
        handler = FindFirstObjectByType<TerrainHandler>();
        chunkSize = handler.chunkSize;
        worldWidth = handler.worldWidth;
        worldHeight = handler.worldHeight;
    }

    public float TerrainNoiseValue(float x, float y)
    {
        float baseNoise = 0;

        foreach (NoiseLayer noiseLayer in noise)
        {
            baseNoise += Mathf.PerlinNoise(x / noiseLayer.frequency + seed, y / noiseLayer.frequency + seed) * noiseLayer.intensity / noise.Count;
        }

        float mapWidth = worldWidth * chunkSize;
        float mapHeight = worldHeight * chunkSize;

        float distLeft = x;
        float distRight = mapWidth - x;
        float distBottom = y;
        float distTop = mapHeight - y;

        float distanceToEdge = Mathf.Min(distLeft, distRight, distBottom, distTop);
        float edgeValue = 1f - Mathf.Clamp01(Mathf.InverseLerp(edgeThickness, 0f, distanceToEdge));

        // optional: sharper edge
        edgeValue = Mathf.Pow(edgeValue, 2f);

        float finalValue = Mathf.Lerp(1, baseNoise, edgeValue);
        return finalValue;
    }
}
