using UnityEngine;
using System.Collections;

/// <summary>
/// TNT explosive that can destroy cave walls in a radius.
/// When thrown, it lights and waits 3 seconds before exploding.
/// </summary>
public class TNT : NonReusableTools
{
    [Header("TNT Settings")]
    [SerializeField] private float _explosionRadius = 30f;
    [SerializeField] private float _fuseTime = 3f;
    [SerializeField] private AudioClip _explosionSound;
    [SerializeField] private GameObject _explosionEffect;
    [SerializeField] private Color _litColor = Color.red;
    
    private bool _isLit = false;
    private SpriteRenderer _spriteRenderer;
    private Color _originalColor;

    protected override void OnEnable()
    {
        base.OnEnable();
        _isLit = false;
        _spriteRenderer = GetComponent<SpriteRenderer>();
        if (_spriteRenderer)
        {
            _originalColor = _spriteRenderer.color;
        }
    }

    /// Called when the tool enters the player's inventory.
    public override void OnPickup(PlayerController player)
    {
        base.OnPickup(player);
        
        // Hide the sprite when in inventory
        if (_spriteRenderer)
        {
            _spriteRenderer.enabled = false;
        }
    }

    /// Called when the tool leaves the player's inventory.
    public override void OnDropped(PlayerController player)
    {
        base.OnDropped(player);
        
        // Show the sprite when dropped
        if (_spriteRenderer)
        {
            _spriteRenderer.enabled = true;
        }
    }

    protected override bool OnUse(PlayerController player)
    {
        if (!player)
        {
            Debug.LogWarning($"{name}: Cannot use TNT without a player reference.");
            return false;
        }

        // Light the TNT and start countdown
        LightTNT(player);
        return true; // Return true to consume the item from inventory
    }

    private void LightTNT(PlayerController player)
    {
        if (_isLit)
        {
            Debug.LogWarning($"{name}: TNT is already lit!");
            return;
        }

        _isLit = true;
        Debug.Log($"INFORMATION: {player.name} lit the TNT! Exploding in {_fuseTime} seconds...");

        // Change color to indicate it's lit
        if (_spriteRenderer)
        {
            _spriteRenderer.color = _litColor;
        }

        // Start the countdown coroutine
        StartCoroutine(ExplodeAfterDelay());
    }

    private IEnumerator ExplodeAfterDelay()
    {
        // Wait for the fuse time
        yield return new WaitForSeconds(_fuseTime);

        // Explode!
        Explode();
    }

    private void Explode()
    {
        Debug.Log($"INFORMATION: TNT exploded at {transform.position} with radius {_explosionRadius}!");

        // Play explosion sound
        if (_explosionSound)
        {
            AudioSource.PlayClipAtPoint(_explosionSound, transform.position);
        }

        // Spawn explosion effect
        if (_explosionEffect)
        {
            Instantiate(_explosionEffect, transform.position, Quaternion.identity);
        }

        // Destroy cave walls in radius
        DestroyTerrainInRadius();

        // Destroy the TNT object
        Destroy(gameObject);
    }

    private void DestroyTerrainInRadius()
    {
        // Find all StaticChunk objects in the scene
        StaticChunk[] chunks = FindObjectsOfType<StaticChunk>();

        if (chunks.Length == 0)
        {
            Debug.LogWarning($"{name}: No StaticChunk objects found in scene to destroy.");
            return;
        }

        // Destroy terrain in each chunk that's within range
        foreach (StaticChunk chunk in chunks)
        {
            chunk.DestroyInRadius(transform.position, _explosionRadius);
        }

        Debug.Log($"INFORMATION: TNT destroyed terrain in {chunks.Length} chunk(s).");
    }

    protected override void OnConsumed(PlayerController player)
    {
        // Don't immediately destroy - the TNT needs to exist for the explosion
        // The explosion will handle destroying the GameObject
        Debug.Log($"INFORMATION: {player?.name ?? "Unknown"} threw TNT.");
        
        // Position TNT at the player's current location
        if (player)
        {
            transform.position = player.transform.position;
        }
        
        // Make sure the sprite is visible when thrown
        if (_spriteRenderer)
        {
            _spriteRenderer.enabled = true;
        }
        
        // Detach from player so it stays in the world
        if (transform.parent != null)
        {
            transform.SetParent(null);
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Draw explosion radius in editor
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _explosionRadius);
    }
}
