using UnityEngine;

[RequireComponent(typeof(TerrainHandler))]
[RequireComponent(typeof(ItemSpawner))]
[RequireComponent(typeof(NoiseHandler))]
public class World : MonoBehaviour
{
    private TerrainHandler _terrainHandler;
    private ItemSpawner _itemSpawner;
    private NoiseHandler _noiseHandler;

    void Start()
    {
        _terrainHandler = GetComponent<TerrainHandler>();
        _itemSpawner = GetComponent<ItemSpawner>();
        _noiseHandler = GetComponent<NoiseHandler>();

        _terrainHandler.GenerateTerrain();
        _itemSpawner.SpawnAllItems();
    }
}
