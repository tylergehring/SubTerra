using UnityEngine;

public class ToolSpawnTester : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private PlayerController player;
    [SerializeField] private NonReusableTools toolPrefab;
    [SerializeField] private KeyCode spawnKey = KeyCode.P;
    [SerializeField] private bool randomizePosition = false;

    private void Update()
    {
        if (Input.GetKeyDown(spawnKey))
        {
            SpawnTool();
        }
    }

    private void SpawnTool()
    {
        if (!player || !toolPrefab)
        {
            Debug.LogWarning($"{name}: Missing player or tool prefab reference—cannot spawn.");
            return;
        }

        toolPrefab.PlaceForTesting(player, randomizePosition);
    }
}