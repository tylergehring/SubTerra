using UnityEngine;

/// <summary>
/// Simplified test spawner for spawning tools using the existing ItemHandler system.
/// In unity you need: PlayerController, ItemHandler prefab, tool prefab, and spawn key.
/// </summary>
public class ToolSpawnTester : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private PlayerController player;
    [SerializeField] private GameObject itemHandlerPrefab;
    [SerializeField] private NonReusableTools toolPrefab;
    [SerializeField] private KeyCode spawnKey = KeyCode.P;
    [SerializeField] private bool randomizePosition = false;
    [SerializeField, Range(0.5f, 5f)] private float spawnDistance = 1.25f;
    [SerializeField] private Vector2 randomSpawnRange = new Vector2(3f, 1.5f);

    private void Update()
    {
        if (Input.GetKeyDown(spawnKey))
        {
            SpawnTool();
        }
    }

    private void SpawnTool()
    {
        if (!player || !toolPrefab || !itemHandlerPrefab)
        {
            Debug.LogWarning($"{name}: Missing player, tool prefab, or item handler prefab reference—cannot spawn.");
            return;
        }

        // Calculate spawn position
        Vector3 spawnPos = player.transform.position;
        if (randomizePosition)
        {
            Vector2 offset = Random.insideUnitCircle;
            spawnPos += new Vector3(offset.x * randomSpawnRange.x, offset.y * randomSpawnRange.y, 0f);
        }
        else
        {
            spawnPos += player.transform.right * spawnDistance;
        }
        spawnPos.z = player.transform.position.z;

        // Create the ItemHandler
        GameObject handlerInstance = Instantiate(itemHandlerPrefab, spawnPos, Quaternion.identity);
        handlerInstance.SetActive(true);

        ItemHandler handler = handlerInstance.GetComponent<ItemHandler>();
        if (!handler)
        {
            Debug.LogWarning($"{name}: ItemHandler prefab is missing an ItemHandler component.");
            return;
        }

        // Create the tool and assign it to the handler
        GameObject toolInstance = Instantiate(toolPrefab.gameObject, handler.transform.position, Quaternion.identity);
        toolInstance.transform.SetParent(handler.transform);
        toolInstance.SetActive(false); // ItemHandler will manage activation

        handler.SetInteractKey(player.pickUpKey);
        handler.SetHeldItem(toolInstance);
        handler.UpdateSprite();

        Debug.Log($"INFORMATION: Spawned {toolPrefab.ToolName} in ItemHandler at {spawnPos}");
    }
}