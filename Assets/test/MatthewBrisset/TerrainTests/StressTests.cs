using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

public class StressTests
{
    const float DESTROY_INTERVAL = 0.1f;

    private TerrainHandler world;

    [UnitySetUp]
    public IEnumerator Setup()
    {
        var loadOp = SceneManager.LoadSceneAsync("MultiChunkTest", LoadSceneMode.Single);
        yield return loadOp;

        world = GameObject.FindFirstObjectByType<TerrainHandler>();
        Assert.IsNotNull(world, "Could not find TerrainHandler in the scene!");

        world.gameObject.SetActive(false);

        yield return null;
    }

    [UnityTest]
    public IEnumerator StressTest_LargeViewDistance()
    {
        Debug.Log("Running test for large view distance");

        world.worldHeight = 100;
        world.worldWidth = 100;
        world.viewDistance = 1;
        world.gameObject.SetActive(true);
        world.GenerateTerrain();
        Camera _camera = GameObject.FindFirstObjectByType<Camera>();
        int _chunks = 0;

        yield return new WaitForSeconds(1.0f);

        Stopwatch sw = Stopwatch.StartNew();
        float endTime = 30f;

        while (sw.Elapsed.TotalSeconds < endTime)
        {
            _camera.orthographicSize += world.chunkSize;
            world.viewDistance += 1;
            _chunks = world.viewDistance * world.viewDistance;
            float _fps = 1f / Time.unscaledDeltaTime;
            Debug.Log($"FPS: {_fps}, Chunks: {_chunks}");
            yield return new WaitForSeconds(DESTROY_INTERVAL);
        }

        sw.Stop();
        Debug.Log($"Stress test ran for {sw.Elapsed.TotalSeconds:F2}s without crash.");
        Assert.Pass();
        yield return null; // must yield once for UnityTest
    }

    [UnityTest]
    public IEnumerator StressTest_LargeMapGeneration()
    {
        Debug.Log("Running test for large map generation");

        world.worldHeight = 100;
        world.worldWidth = 100;
        world.viewDistance = 20;
        world.gameObject.SetActive(true);
        world.GenerateTerrain();
        Debug.Log($"Generated {world.worldWidth * world.worldHeight} chunks");

        yield return new WaitForSeconds(1.0f);

        Debug.Log("Stress test ran without crash.");
        Assert.Pass();
        yield return null; // must yield once for UnityTest
    }

    [UnityTest]
    public IEnumerator StressTest_Destroy()
    {
        Debug.Log("Running test for extreme destruction");

        world.worldHeight = 100;
        world.worldWidth = 100;
        world.viewDistance = 20;
        world.gameObject.SetActive(true);
        Camera _camera = GameObject.FindFirstObjectByType<Camera>();
        _camera.orthographicSize = 100;
        world.GenerateTerrain();
        Debug.Log($"Generated {world.worldWidth * world.worldHeight} chunks");

        yield return new WaitForSeconds(1.0f);

        Stopwatch sw = Stopwatch.StartNew();
        float endTime = 30f;

        while (sw.Elapsed.TotalSeconds < endTime)
        {
            Debug.Log($"Destroying terrain");
            world.DestroyInRadius(new Vector3(Random.Range(0, 1000), Random.Range(0, 1000), 0), 10);
            float _fps = 1f / Time.unscaledDeltaTime;
            Debug.Log($"FPS: {_fps}");
            yield return new WaitForSeconds(0.02f);
        }

        Debug.Log("Stress test ran without crash.");
        Assert.Pass();
        yield return null; // must yield once for UnityTest
    }
}
