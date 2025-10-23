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
    public float noiseThreshold = 0.5f;
    public float seed = 10000;

    public float NoiseValue(float x, float y)
    {
        float value = 0;

        foreach (NoiseLayer noiseLayer in noise)
        {
            value += Mathf.PerlinNoise(x / noiseLayer.frequency + seed, y / noiseLayer.frequency + seed) * noiseLayer.intensity / noise.Count;
        }

        return value;
    }
}
