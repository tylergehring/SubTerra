using UnityEngine;

public class Hazard : MonoBehaviour
{
    [SerializeField] private float damage = 20f; // Damage dealt to player
    [SerializeField] private float activationRange = 1f; // Range to trigger hazard
    [SerializeField] private float damageInterval = 1f; // Seconds between damage ticks
    private PlayerController player;
    private float lastDamageTime;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.GetComponent<PlayerController>();
        if (player == null) Debug.LogError("Player not found! Ensure Player has 'Player' tag and PlayerController script.");
        lastDamageTime = Time.time;
    }

    void Update()
    {
        if (player == null) return;

        // Check if player is within range
        if (Vector3.Distance(transform.position, player.transform.position) < activationRange)
        {
            if (Time.time - lastDamageTime >= damageInterval)
            {
                player.ChangeHealth((int)-damage); // Deal damage to player
                Debug.Log($"Hazard dealt {damage} damage to player at {transform.position}");
                lastDamageTime = Time.time;
            }
        }
    }

    // For manual or future random spawning
    public void Spawn(Vector3 position)
    {
        transform.position = position;
        Debug.Log($"Hazard spawned at {position}");
    }

    // Visualize activation range in Scene view
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, activationRange);
    }
}