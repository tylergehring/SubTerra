/*using UnityEngine;

public class DemoManager : MonoBehaviour
{
    [Header("Player")]
    public PlayerController player;

    [Header("Demo Paths")]
    public Transform[] successPath;
    public Transform[] failurePath;

    [Header("Settings")]
    public bool randomPath = true;
    public float waypointTolerance = 0.2f;

    private Transform[] currentPath;
    private int waypointIndex = 0;
    private bool demoActive = false;

    void Start()
    {
        StartDemo();
    }

    void Update()
    {
        // Stop demo instantly on input
        if (demoActive && Input.anyKeyDown)
        {
            StopDemo();
            return;
        }

        if (demoActive)
        {
            FollowPath();
        }
    }

    // ---------------- CONTROL ----------------

    public void StartDemo()
    {
        if (!player)
            return;

        demoActive = true;
        player.useAI = true;

        waypointIndex = 0;

        // Pick success or failure path
        if (randomPath)
            currentPath = (Random.value > 0.5f) ? successPath : failurePath;
        else
            currentPath = successPath;

        Debug.Log("Demo started (AI playing)");
    }

    public void StopDemo()
    {
        demoActive = false;

        if (!player)
            return;

        player.useAI = false;

        // Reset AI movement
        player.SetAIMove(0);
        player.SetAIJump(false);

        Debug.Log("Demo stopped (User now in control)");
    }

    // ---------------- AI PATHING ----------------

    private void FollowPath()
    {
        if (!player || currentPath == null || currentPath.Length == 0)
            return;

        if (waypointIndex >= currentPath.Length)
        {
            StartDemo();   // loop demo
            return;
        }

        Transform target = currentPath[waypointIndex];

        float dx = target.position.x - player.transform.position.x;
        float dy = target.position.y - player.transform.position.y;

        // Horizontal input feed
        player.SetAIMove(Mathf.Sign(dx));

        // Stop drift near waypoints
        if (Mathf.Abs(dx) < waypointTolerance)
            player.SetAIMove(0);

        // Trigger jump only when target is above
        player.SetAIJump(dy > 0.25f);

        // Advance waypoint
        if (Vector2.Distance(player.transform.position, target.position) <= waypointTolerance)
            waypointIndex++;
    }
}
*/