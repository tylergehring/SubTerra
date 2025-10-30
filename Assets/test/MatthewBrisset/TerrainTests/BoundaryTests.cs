using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using UnityEngine.UIElements;

public class BoundaryTests
{
    const int TERRAIN_TEST_SIZE = 100;

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
    public IEnumerator NoiseThreshold_Extremes_ProduceValidMap()
    {
        // Create chunk *inactive* so Awake() doesn’t run yet
        var chunkGO = new GameObject("Test Chunk");
        chunkGO.SetActive(false);

        var chunk = chunkGO.AddComponent<StaticChunk>();
        chunk.transform.Translate(Vector3.left * TERRAIN_TEST_SIZE / 2);
        chunk.transform.Translate(Vector3.down * TERRAIN_TEST_SIZE / 2);
        chunk.noiseHandler = GameObject.FindFirstObjectByType<NoiseHandler>();
        chunk.chunkSize = TERRAIN_TEST_SIZE;
        Debug.Log("Generating terrain with noise threshold of -100");
        chunk.noiseHandler.noiseThreshold = -100;
        yield return new WaitForSeconds(1);
        chunkGO.SetActive(true);
        yield return new WaitForSeconds(1);
        Assert.Pass();
        yield return null;
    }


    [UnityTest]
    public IEnumerator DestroyOutsideChunk_DoesNothing()
    {
        // Create chunk *inactive* so Awake() doesn’t run yet
        var chunkGO = new GameObject("Test Chunk");
        chunkGO.SetActive(false);

        var chunk = chunkGO.AddComponent<StaticChunk>();
        chunk.transform.Translate(Vector3.left * TERRAIN_TEST_SIZE / 2);
        chunk.transform.Translate(Vector3.down * TERRAIN_TEST_SIZE / 2);
        chunk.noiseHandler = GameObject.FindFirstObjectByType<NoiseHandler>();
        chunk.chunkSize = TERRAIN_TEST_SIZE;
        chunkGO.SetActive(true);

        yield return new WaitForSeconds(1);
        Debug.Log("Attempting to destroy parts of a chunk outside of chunk bounds...");
        chunk.DestroyInRadius(Vector3.up * 1001, 20);
        yield return new WaitForSeconds(1);
        Assert.Pass();
        yield return null;
    }
}
