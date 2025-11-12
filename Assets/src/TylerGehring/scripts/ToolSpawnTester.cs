using UnityEngine;
using UnityEngine.SceneManagement;

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
    [SerializeField] private bool autoBindPlayer = true;
    [SerializeField, Min(0.05f)] private float autoBindRetryInterval = 0.35f;

    private float _nextAutoBindAttempt;
    private bool _loggedMissingPlayer;

    private void Awake()
    {
        // Try to grab an instance immediately so we work in scenes where the player is already present.
        if (autoBindPlayer)
        {
            _TryAutoBindPlayer(logOnBind: false);
        }
    }

    private void Update()
    {
        if (!_IsPlayerReferenceReady())
        {
            return;
        }

        if (Input.GetKeyDown(spawnKey))
        {
            _SpawnTool();
        }
    }

    private void _SpawnTool()
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

    private bool _IsPlayerReferenceReady()
    {
        if (_HasValidPlayer())
        {
            return true;
        }

        if (autoBindPlayer && Time.time >= _nextAutoBindAttempt)
        {
            _nextAutoBindAttempt = Time.time + autoBindRetryInterval;
            _TryAutoBindPlayer();
        }

        if (_HasValidPlayer())
        {
            return true;
        }

        if (!_loggedMissingPlayer)
        {
            Debug.LogWarning($"{name}: Player reference is missing—enable auto bind or assign a runtime PlayerController manually.");
            _loggedMissingPlayer = true;
        }

        return false;
    }

    private bool _HasValidPlayer()
    {
        if (!player)
        {
            return false;
        }

        if (!player.isActiveAndEnabled)
        {
            player = null;
            return false;
        }

        if (!player.gameObject.scene.IsValid())
        {
            player = null;
            return false;
        }

        return true;
    }

    private void _TryAutoBindPlayer(bool logOnBind = true)
    {
        PlayerController candidate = FindObjectOfType<PlayerController>();
        if (!candidate || !candidate.gameObject.scene.IsValid())
        {
            return;
        }

        player = candidate;
        _loggedMissingPlayer = false;

        if (logOnBind)
        {
            Debug.Log($"{name}: Auto-bound to PlayerController '{player.name}'.");
        }
    }
}