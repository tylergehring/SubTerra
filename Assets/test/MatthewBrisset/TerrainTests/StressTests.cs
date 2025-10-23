using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using UnityEngine.UIElements;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

public class StressTests
{
    [Test]
    public void StressTest_LargeMapGeneration()
    {
        Debug.Log("Running test for large map generation");
        // Generate noise
        var noise = new GameObject("Noise Handler").AddComponent<NoiseHandler>();
        NoiseLayer n_layer = new NoiseLayer();
        n_layer.frequency = 10;
        n_layer.intensity = 10;
        noise.noise.Add(n_layer);
        Debug.Log("Created noise object");

        // Generate chunk
        StaticChunk chunk = new GameObject("Test Chunk").AddComponent<StaticChunk>();
        chunk.noiseHandler = noise;
        Assert.IsNotNull(chunk.noiseHandler);

        int size = 500; // large test
        Debug.Log("Created terrain with size: " + size);
        Stopwatch sw = Stopwatch.StartNew();
        var terrain = chunk.chunkSize = size;

        Assert.NotNull(terrain);
        Assert.Less(sw.ElapsedMilliseconds, 5000, "Generation too slow!");
        Debug.Log($"Generated {size}x{size} terrain in {sw.ElapsedMilliseconds} ms");
    }
}
