using UnityEngine;

public class skeletonEnemy : MonoBehaviour 
{
    private PlayerController player; // Placeholder for the reference of the player controller script 
    private float lastDamageTime;  // The variable that holds when was the last time damage was delt to the player.


    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.GetComponent<PlayerController>();  // Take the reference of the player controller script into player 
        if (player == null) Debug.LogError("Player not found! Ensure Player has 'Player' tag and PlayerController script.");
        lastDamageTime = Time.time;
    }

}
