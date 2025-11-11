using UnityEngine;

public class skeletonEnemy : MonoBehaviour 
{
    private PlayerController player; // Placeholder for the reference of the player controller script 
    private float lastDamageTime;  // The variable that holds when was the last time damage was delt to the player.
    private bool _playerMissingLogged;


    private void Awake()
    {
        TryResolvePlayer();
        lastDamageTime = Time.time;
    }

    private void OnEnable()
    {
        TryResolvePlayer();
    }

    private void Update()
    {
        if (player)
        {
            return;
        }

        TryResolvePlayer();
    }

    private void TryResolvePlayer()
    {
        if (player)
        {
            return;
        }

        var candidate = GameObject.FindGameObjectWithTag("Player");
        player = candidate ? candidate.GetComponent<PlayerController>() : null;

        if (!player && !_playerMissingLogged)
        {
            Debug.LogWarning("skeletonEnemy: Player not found. Enemy will remain idle until a player is available.");
            _playerMissingLogged = true;
        }
        else if (player)
        {
            _playerMissingLogged = false;
        }
    }

}
