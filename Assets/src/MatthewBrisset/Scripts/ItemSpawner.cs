using UnityEngine;
using System.Collections.Generic;
using System.Collections;

[System.Serializable]
public class Item
{
    public GameObject prefab;
    public int numSpawns;
    [Range(0, 1f)]
    public float minDepth = 0; // 1 -> Top of map, 0 -> bottom of map
    [Range(0, 1f)]
    public float maxDepth = 1; // 1 -> Top of map, 0 -> bottom of map
}

[RequireComponent(typeof(TerrainHandler))]
public class ItemSpawner : MonoBehaviour
{
    public List<Item> items;
    private TerrainHandler _terrain;

    void Awake()
    {
        _terrain = GetComponent<TerrainHandler>();
    }
    
    public void SpawnAllItems()
    {
        print("asda");
        foreach (Item item in items)
        {
            for (int i = 0; i<item.numSpawns; i++)
            {
                StartCoroutine(SpawnItem(item.prefab, RandomSpawnPoint(item)));
            }
        }
    }

    Vector3 RandomSpawnPoint(Item item)
    {
        float minY = _terrain.worldHeight * _terrain.chunkSize * item.minDepth;
        float maxY = _terrain.worldHeight * _terrain.chunkSize * item.maxDepth;
        float minX = _terrain.noiseHandler.edgeThickness;
        float maxX = _terrain.worldWidth * _terrain.chunkSize - _terrain.noiseHandler.edgeThickness;

        Vector3 position = new Vector3(Random.Range(minX, maxX), Random.Range(minY, maxY));

        return position;
    }

    IEnumerator SpawnItem(GameObject newItem, Vector3 position)
    {
        GameObject item = GameObject.Instantiate(newItem);
        item.transform.position = position;
        yield return new WaitForSeconds(0.1f); // Wait for 3 seconds
        _terrain.DestroyInRadius(position, 5);
    }
}
