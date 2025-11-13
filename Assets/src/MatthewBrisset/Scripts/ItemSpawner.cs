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
    public float spacing = 3;
}

[RequireComponent(typeof(TerrainHandler))]
public class ItemSpawner : MonoBehaviour
{
    public List<Item> items; // MUST be public or else I cannot modify these in the editor. Lists are weird that way.
    private TerrainHandler _terrain;
    private NoiseHandler _noise;

    void Awake()
    {
        _terrain = GetComponent<TerrainHandler>();
        _noise = GetComponent<NoiseHandler>();
    }
    
    public void SpawnAllItems()
    {
        foreach (Item item in items)
        {
            for (int i = 0; i<item.numSpawns; i++)
            {
                StartCoroutine(SpawnItem(item, RandomSpawnPoint(item)));
            }
        }
    }

    Vector3 RandomSpawnPoint(Item item)
    {
        float minY = _terrain.GetWorldHeight() * _terrain.GetChunkSize() * item.minDepth;
        float maxY = _terrain.GetWorldHeight() * _terrain.GetChunkSize() * item.maxDepth;
        float minX = _noise.GetEdgeThickness();
        float maxX = _terrain.GetWorldWidth() * _terrain.GetChunkSize() - _noise.GetEdgeThickness();

        Vector3 position = new Vector3(Random.Range(minX, maxX), Random.Range(minY, maxY));

        return position;
    }

    IEnumerator SpawnItem(Item newItem, Vector3 position)
    {
        GameObject item = GameObject.Instantiate(newItem.prefab);
        item.transform.position = position;
        yield return new WaitForSeconds(0.1f); // Wait for 3 seconds
        _terrain.DestroyInRadius(position, newItem.spacing);
    }
}
