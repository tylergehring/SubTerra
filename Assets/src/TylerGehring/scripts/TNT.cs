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
    [SerializeField] private float _throwForce = 10f;
    [SerializeField] private float _bounciness = 0.5f;
    [SerializeField] private AudioClip _explosionSound;
    [SerializeField] private GameObject _explosionEffect;
    [SerializeField] private Color _litColor = Color.red;
    [SerializeField] private Vector3 _desiredScale = new Vector3(0.1f, 0.1f, 1f);
    
    private bool _isLit = false;
    private SpriteRenderer _spriteRenderer;
    private Rigidbody2D _rigidbody;
    private CircleCollider2D _collider;
    private Color _originalColor;
    private static readonly WaitForSeconds CollisionGraceDelay = new WaitForSeconds(0.2f);
    private Coroutine _fuseCoroutine;
    private float _fuseEndTime;
    private PhysicsMaterial2D _cachedPhysicsMaterial;
    private bool _warnedAboutMissingPhysics;

    protected override void OnEnable()
    {
        base.OnEnable();
        CacheComponents();
        ConfigurePhysics();

        if (_spriteRenderer)
        {
            _originalColor = _spriteRenderer.color;
        }

        if (_isLit)
        {
            BeginFuseCountdown();
        }
    }

    private void OnDisable()
    {
        if (_fuseCoroutine != null)
        {
            StopCoroutine(_fuseCoroutine);
            _fuseCoroutine = null;
        }
    }

    /// Called when the tool enters the player's inventory.
    public override void OnPickup(PlayerController player)
    {
        if (_isLit)
        {
            Debug.LogWarning($"{name}: Lit TNT cannot be picked up again; keeping it active in the world.");
            if (player)
            {
                player.ConsumeCurrentTool(this, false);
                if (!gameObject.activeSelf)
                {
                    gameObject.SetActive(true);
                }
            }
            return;
        }

        base.OnPickup(player);
        
        // Hide the sprite when in inventory
        CacheSpriteRenderer();
        if (_spriteRenderer)
        {
            _spriteRenderer.enabled = false;
        }
        
        // Disable physics when in inventory
        CachePhysicsComponents();
        ConfigurePhysics();
        if (_rigidbody)
        {
            _rigidbody.linearVelocity = Vector2.zero;
            _rigidbody.isKinematic = true;
        }
    }

    /// Called when the tool leaves the player's inventory.
    public override void OnDropped(PlayerController player)
    {
        base.OnDropped(player);
        
        // Show the sprite when dropped
        CacheSpriteRenderer();
        if (_spriteRenderer)
        {
            _spriteRenderer.enabled = true;
        }

        CachePhysicsComponents();
        ConfigurePhysics();
        if (_rigidbody)
        {
            _rigidbody.isKinematic = false;
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

        _fuseEndTime = Time.time + _fuseTime;
        BeginFuseCountdown();
    }

    private IEnumerator ExplodeAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        // Explode!
        Explode();
        _fuseCoroutine = null;
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
        CacheComponents();
        // Don't immediately destroy - the TNT needs to exist for the explosion
        // The explosion will handle destroying the GameObject
        Debug.Log($"INFORMATION: {player?.name ?? "Unknown"} threw TNT.");

        if (player)
        {
            player.ConsumeCurrentTool(this, false);
        }

        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
        }
        
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
        
        // Enable physics and throw the TNT
        if (_rigidbody)
        {
            _rigidbody.isKinematic = false;
            ThrowTNT(player);
        }

        BeginFuseCountdown();
    }
    
    private void ThrowTNT(PlayerController player)
    {
        if (!_rigidbody)
            return;

        Camera mainCamera = Camera.main;
        Vector3 mousePos = mainCamera
            ? mainCamera.ScreenToWorldPoint(Input.mousePosition)
            : transform.position + Vector3.right;
        mousePos.z = transform.position.z;

        Vector2 direction = (mousePos - transform.position);
        if (direction.sqrMagnitude < 0.0001f)
            direction = player ? (Vector2)player.transform.right : Vector2.right;
        direction.Normalize();

        float separation = (_collider ? _collider.radius : 0.25f) + 0.01f;
        transform.position += (Vector3)(direction * separation);

        if (_collider && player)
            StartCoroutine(TemporarilyIgnorePlayer(player));

        _rigidbody.linearVelocity = direction * _throwForce;

        Debug.Log($"INFORMATION: TNT thrown towards {mousePos} with force {_throwForce}");
    }

    private IEnumerator TemporarilyIgnorePlayer(PlayerController player)
    {
        Collider2D[] colliders = player.GetComponentsInChildren<Collider2D>();
        foreach (Collider2D playerCollider in colliders)
            if (playerCollider)
                Physics2D.IgnoreCollision(_collider, playerCollider, true);

        yield return CollisionGraceDelay;

        if (!_collider)
            yield break;

        foreach (Collider2D playerCollider in colliders)
            if (playerCollider)
                Physics2D.IgnoreCollision(_collider, playerCollider, false);
    }

    private void BeginFuseCountdown()
    {
        if (!_isLit || !isActiveAndEnabled)
            return;

        if (_fuseCoroutine != null)
        {
            StopCoroutine(_fuseCoroutine);
        }

        float remainingTime = Mathf.Max(0f, _fuseEndTime - Time.time);
        _fuseCoroutine = StartCoroutine(ExplodeAfterDelay(remainingTime));
    }

    private void OnDrawGizmosSelected()
    {
        // Draw explosion radius in editor
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _explosionRadius);
    }

    private void Update()
    {
        if (!_rigidbody || !_collider || _rigidbody.sharedMaterial == null)
        {
            CacheComponents();
            ConfigurePhysics();
        }
    }

    private void CacheComponents()
    {
        CacheSpriteRenderer();
        CachePhysicsComponents();
    }

    private void CacheSpriteRenderer()
    {
        if (!_spriteRenderer)
        {
            TryGetComponent(out _spriteRenderer);
        }
    }

    private void CachePhysicsComponents()
    {
        // Try to get existing Rigidbody2D first
        if (!_rigidbody)
        {
            _rigidbody = GetComponent<Rigidbody2D>();
        }
        
        // Only add if it doesn't exist
        if (!_rigidbody)
        {
            _rigidbody = gameObject.AddComponent<Rigidbody2D>();
            _rigidbody.gravityScale = 1f;
            _rigidbody.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        }

        // Try to get existing CircleCollider2D first
        if (!_collider)
        {
            _collider = GetComponent<CircleCollider2D>();
        }
        
        // Only add if it doesn't exist
        if (!_collider)
        {
            _collider = gameObject.AddComponent<CircleCollider2D>();
            _collider.radius = Mathf.Max(_collider.radius, 0.5f);
        }

        if (_rigidbody && _collider)
        {
            _warnedAboutMissingPhysics = false;
        }
    }

    private void ConfigurePhysics()
    {
        CachePhysicsComponents();

        if (!_rigidbody)
        {
            return;
        }

        _rigidbody.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        if (_cachedPhysicsMaterial == null)
        {
            _cachedPhysicsMaterial = new PhysicsMaterial2D("TNT_Bounce")
            {
                bounciness = _bounciness,
                friction = 0.3f
            };
        }

        if (_rigidbody.sharedMaterial == null || !Mathf.Approximately(_rigidbody.sharedMaterial.bounciness, _bounciness))
        {
            _rigidbody.sharedMaterial = _cachedPhysicsMaterial;
        }

        if (_collider)
        {
            if (_collider.radius <= 0f)
            {
                _collider.radius = 0.5f;
            }

            if (_collider.sharedMaterial == null)
            {
                _collider.sharedMaterial = _cachedPhysicsMaterial;
            }
        }
        else if (!_warnedAboutMissingPhysics)
        {
            Debug.LogWarning($"{name}: TNT could not obtain a CircleCollider2D.");
            _warnedAboutMissingPhysics = true;
        }
    }

    private void WarnMissingPhysics()
    {
        if (_warnedAboutMissingPhysics)
        {
            return;
        }

        Debug.LogWarning($"{name}: TNT is missing required physics components; behaviour may be limited during tests.");
        _warnedAboutMissingPhysics = true;
    }
}
