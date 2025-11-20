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

    // Virtual function — can be overridden by subclasses
    public virtual void OnSpawned(Vector3 position)
    {
        Debug.Log("An item was spawned at " + position);
    }
}

public class HealingItem : Item
{
    public override void OnSpawned(Vector3 position)
    {
        Debug.Log("Healing item spawned! Restores health.");
    }
}

public class ExplosiveItem : Item
{
    public override void OnSpawned(Vector3 position)
    {
        Debug.Log("Explosive item spawned! Watch out!");
    }
}

[RequireComponent(typeof(TerrainHandler))]
public class ItemSpawner : MonoBehaviour
{
    public List<Item> items; // MUST be public or else I cannot modify these in the editor. Lists are weird that way.
    private TerrainHandler _terrain;
    private NoiseHandler _noise;

    void Start()
    {
        _terrain = World.Instance.GetTerrainHandler();
        _noise = World.Instance.GetNoiseHandler();
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
        if (_terrain == null)
            _terrain = World.Instance.GetTerrainHandler();
        if (_noise == null)
            _noise = World.Instance.GetNoiseHandler();

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
