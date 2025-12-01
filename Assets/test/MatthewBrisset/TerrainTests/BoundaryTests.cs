using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using UnityEngine.UIElements;

public class BoundaryTests
{
    const int TERRAIN_TEST_SIZE = 5;

    private TerrainHandler world;

    [UnitySetUp]
    public IEnumerator Setup()
    {
        var loadOp = SceneManager.LoadSceneAsync("MultiChunkTest", LoadSceneMode.Single);
        yield return loadOp;

        world = World.Instance.GetTerrainHandler();
        Assert.IsNotNull(world, "Could not find TerrainHandler in the scene!");

        world.gameObject.SetActive(false);

        yield return null;
    }

    [UnityTest]
    public IEnumerator BoundaryTest_NoiseThreshold_Extremes()
    {
        Debug.Log("Running test for extreme noise values");

        world.SetWorldHeight(TERRAIN_TEST_SIZE);
        world.SetWorldWidth(TERRAIN_TEST_SIZE);
        world.SetViewDistance(5);
        world.GetComponent<NoiseHandler>().SetTerrainThreshold(1000);
        world.gameObject.SetActive(true);
        world.GenerateTerrain();
        Debug.Log($"Created terrain with terrain threshold {world.GetComponent<NoiseHandler>().GetTerrainThreshold()}");

        yield return new WaitForSeconds(1.0f);
        Assert.Pass();
        yield return null;
    }

    [UnityTest]
    public IEnumerator BoundaryTest_DestroyOutsideChunk()
    {
        Debug.Log("Running test for destroying terrain outside chunks");

        world.SetWorldHeight(TERRAIN_TEST_SIZE);
        world.SetWorldWidth(TERRAIN_TEST_SIZE);
        world.SetViewDistance(5);
        world.gameObject.SetActive(true);
        world.GenerateTerrain();
        Debug.Log("Created terrain");

        yield return new WaitForSeconds(1.0f);
        world.DestroyInRadius(Vector3.one * 100000, 100);
        Debug.Log($"Attempted to destroy terrain out of bounds");
        yield return new WaitForSeconds(1.0f);

        Assert.Pass();
        yield return null;
    }

    [UnityTest]
    public IEnumerator BoundaryTest_DestroyAllTerrain()
    {
        Debug.Log("Running test for destroying terrain outside chunks");

        world.SetWorldHeight(TERRAIN_TEST_SIZE);
        world.SetWorldWidth(TERRAIN_TEST_SIZE);
        world.SetViewDistance(5);
        world.gameObject.SetActive(true);
        world.GenerateTerrain();
        Debug.Log("Created terrain");

        yield return new WaitForSeconds(1.0f);
        world.DestroyInRadius(Vector3.zero, 1000);
        Debug.Log($"Attempted to destroy entirety of terrain");
        yield return new WaitForSeconds(1.0f);

        Assert.Pass();
        yield return null;
    }
}
