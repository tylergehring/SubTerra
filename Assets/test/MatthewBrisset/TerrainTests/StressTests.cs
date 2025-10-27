using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

public class StressTests
{
    const int TERRAIN_TEST_SIZE = 100;
    const float DESTROY_INTERVAL = 0.02f;

    [UnitySetUp]
    public IEnumerator Setup()
    {
        // Load test scene asynchronously
        var loadOp = SceneManager.LoadSceneAsync("TerrainDestructionTest");
        yield return new WaitUntil(() => loadOp.isDone);
        yield return null; // wait one frame so scene Awake/Start complete
        GameObject.Destroy(GameObject.FindFirstObjectByType<StaticChunk>().gameObject);
    }

    [UnityTest]
    public IEnumerator StressTest_LargeMapGeneration()
    {
        int iterations = 0;
        yield return new WaitForSeconds(0.1f);
        Debug.Log("Running test for large map generation");

        // Create chunk *inactive* so Awake() doesn’t run yet
        var chunkGO = new GameObject("Test Chunk");
        chunkGO.SetActive(false);

        var chunk = chunkGO.AddComponent<StaticChunk>();
        chunk.transform.Translate(Vector3.left * TERRAIN_TEST_SIZE / 2);
        chunk.transform.Translate(Vector3.down * TERRAIN_TEST_SIZE / 2);
        chunk.noiseHandler = GameObject.FindFirstObjectByType<NoiseHandler>();
        chunk.noiseHandler.noiseThreshold = 0;
        chunk.chunkSize = TERRAIN_TEST_SIZE;

        // Now activate (Awake() will run using your pre-set values)
        chunkGO.SetActive(true);

        yield return new WaitForSeconds(0.1f);
        Debug.Log("Created terrain with size: " + chunk.chunkSize);

        // Destroy test loop
        Stopwatch sw = Stopwatch.StartNew();
        float endTime = 30f; // safety limit – run for 5 seconds

        while (sw.Elapsed.TotalSeconds < endTime)
        {
            iterations++;
            chunk.DestroyInRadius(Random.insideUnitSphere * TERRAIN_TEST_SIZE, 5);
            float fps = 1f / Time.unscaledDeltaTime;
            Debug.Log($"[{sw.ElapsedMilliseconds}ms] FPS: {fps:F1} Iteration: {iterations}");
            yield return new WaitForSeconds(DESTROY_INTERVAL);
        }

        sw.Stop();
        Debug.Log($"Stress test ran for {sw.Elapsed.TotalSeconds:F2}s without crash.");
        Assert.Pass();
        yield return null; // must yield once for UnityTest
    }
}
