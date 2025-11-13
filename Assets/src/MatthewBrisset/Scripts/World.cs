using UnityEngine;

[RequireComponent(typeof(TerrainHandler))]
[RequireComponent(typeof(ItemSpawner))]
[RequireComponent(typeof(NoiseHandler))]
public class World : MonoBehaviour
{
    public static World Instance { get; private set; }
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

        // If another instance exists, destroy this one
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
}
