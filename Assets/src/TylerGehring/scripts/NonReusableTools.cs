using UnityEngine;

/// <summary>
/// Base class for single-use tools such as food or explosives.
/// Provides shared inventory hooks and a simple way to spawn test instances in the world.
/// </summary>
public abstract class NonReusableTools : MonoBehaviour
{
    [Header("Tool Setup")]
    [SerializeField] private string _toolName = "Tool";
    [SerializeField] private GameObject _itemHandlerPrefab;
    [SerializeField, Range(0.5f, 5f)] private float _spawnForwardDistance = 1.25f;
    [SerializeField] private Vector2 _randomSpawnRange = new Vector2(3f, 1.5f);

    private bool _consumed;
    private PlayerController _owner;

    // Cached components for toggling world interaction when the tool is in the inventory
    private Collider2D[] _colliders2D;
    private bool[] _colliders2DInitialStates;
    private Collider[] _colliders3D;
    private bool[] _colliders3DInitialStates;
    private Rigidbody2D _rigidbody2D;
    private bool _rigidbody2DSimulatedInitial;
    private Rigidbody _rigidbody3D;
    private bool _rigidbody3DKinematicInitial;
    private SpriteRenderer[] _spriteRenderers;
    private bool[] _spriteRenderersInitialStates;
    private bool _worldInteractionEnabled = true;

    public string ToolName => _toolName;
    public PlayerController Owner => _owner;
    public bool IsConsumed => _consumed;

    protected virtual void Awake()
    {
        _colliders2D = GetComponentsInChildren<Collider2D>(true);
        _colliders2DInitialStates = new bool[_colliders2D.Length];
        for (int i = 0; i < _colliders2D.Length; i++)
        {
            _colliders2DInitialStates[i] = _colliders2D[i] && _colliders2D[i].enabled;
        }

        _colliders3D = GetComponentsInChildren<Collider>(true);
        _colliders3DInitialStates = new bool[_colliders3D.Length];
        for (int i = 0; i < _colliders3D.Length; i++)
        {
            _colliders3DInitialStates[i] = _colliders3D[i] && _colliders3D[i].enabled;
        }

        _rigidbody2D = GetComponentInChildren<Rigidbody2D>(true);
        if (_rigidbody2D)
            _rigidbody2DSimulatedInitial = _rigidbody2D.simulated;

        _rigidbody3D = GetComponentInChildren<Rigidbody>(true);
        if (_rigidbody3D)
            _rigidbody3DKinematicInitial = _rigidbody3D.isKinematic;

        _spriteRenderers = GetComponentsInChildren<SpriteRenderer>(true);
        _spriteRenderersInitialStates = new bool[_spriteRenderers.Length];
        for (int i = 0; i < _spriteRenderers.Length; i++)
        {
            _spriteRenderersInitialStates[i] = _spriteRenderers[i] && _spriteRenderers[i].enabled;
        }
    }

    protected virtual void OnEnable()
    {
        _consumed = false;
    }

    /// <summary>
    /// Called when the tool enters the player's inventory.
    /// </summary>
    public virtual void OnPickup(PlayerController player)
    {
        _owner = player;
        SetWorldInteractionEnabled(false);
    }

    /// <summary>
    /// Called when the tool leaves the player's inventory.
    /// </summary>
    public virtual void OnDropped(PlayerController player)
    {
        if (_owner == player)
        {
            _owner = null;
        }

        if (!_consumed)
        {
            SetWorldInteractionEnabled(true);
        }
    }

    /// <summary>
    /// Spawns a copy of this tool inside an ItemHandler in front of the supplied player.
    /// Useful for quick testing or temporary random placement.
    /// </summary>
    /// <param name="player">Player used for positioning and key bindings.</param>
    /// <param name="randomizePosition">When true, uses a random offset instead of fixed forward distance.</param>
    public GameObject PlaceForTesting(PlayerController player, bool randomizePosition = false)
    {
        if (!_itemHandlerPrefab)
        {
            Debug.LogWarning($"{name}: Item handler prefab not assigned; cannot spawn tool for testing.");
            return null;
        }

        if (!player)
        {
            Debug.LogWarning($"{name}: Player reference missing; cannot spawn tool for testing.");
            return null;
        }

        Vector3 spawnPos = player.transform.position;
        if (randomizePosition)
        {
            Vector2 offset = Random.insideUnitCircle;
            spawnPos += new Vector3(offset.x * _randomSpawnRange.x, offset.y * _randomSpawnRange.y, 0f);
        }
        else
        {
            spawnPos += player.transform.right * _spawnForwardDistance;
        }
        spawnPos.z = player.transform.position.z;

        GameObject handlerInstance = Instantiate(_itemHandlerPrefab, spawnPos, Quaternion.identity);
        handlerInstance.SetActive(true);

        ItemHandler handler = handlerInstance.GetComponent<ItemHandler>();
        if (!handler)
        {
            Debug.LogWarning($"{name}: Spawned handler is missing an ItemHandler component.");
            return handlerInstance;
        }

        GameObject toolInstance = Instantiate(gameObject, handler.transform.position, Quaternion.identity);
        toolInstance.transform.SetParent(handler.transform);
        toolInstance.SetActive(false);

        NonReusableTools toolScript = toolInstance.GetComponent<NonReusableTools>();
        if (toolScript)
        {
            toolScript._owner = null;
            toolScript._consumed = false;
        }

        handler.SetInteractKey(player.pickUpKey);
        handler.SetHeldItem(toolInstance);
        handler.UpdateSprite();

        return handlerInstance;
    }

    /// <summary>
    /// Called by the player to trigger the tool's effect.
    /// </summary>
    public void Use(PlayerController player)
    {
        if (_consumed)
        {
            Debug.Log($"{name}: Attempted to use {_toolName}, but it has already been consumed.");
            return;
        }

        PlayerController user = player ? player : _owner;
        if (!user)
        {
            Debug.LogWarning($"{name}: No player associated with {_toolName}; use cancelled.");
            return;
        }

        _owner = user;
        Debug.Log($"INFORMATION: {user.name} is attempting to use {_toolName}.");
        bool consume = OnUse(user);
        if (consume)
        {
            _consumed = true;
            OnConsumed(user);
        }
        else
        {
            Debug.Log($"INFORMATION: {_toolName} was used by {user.name} but was not consumed.");
        }
    }

    /// <summary>
    /// Override to implement tool behaviour. Return true to remove the tool from the inventory.
    /// </summary>
    protected abstract bool OnUse(PlayerController player);

    protected virtual void OnConsumed(PlayerController player)
    {
    Debug.Log($"INFORMATION: {player?.name ?? "Unknown"} consumed {_toolName}.");
    OnDropped(player);
        if (player)
        {
            player.ConsumeCurrentTool(this, true);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void SetWorldInteractionEnabled(bool enabled)
    {
        if (_worldInteractionEnabled == enabled)
            return;

        _worldInteractionEnabled = enabled;

        if (_colliders2D != null)
        {
            for (int i = 0; i < _colliders2D.Length; i++)
            {
                if (!_colliders2D[i]) continue;
                bool targetState = enabled ? _colliders2DInitialStates[i] : false;
                _colliders2D[i].enabled = targetState;
            }
        }

        if (_colliders3D != null)
        {
            for (int i = 0; i < _colliders3D.Length; i++)
            {
                if (!_colliders3D[i]) continue;
                bool targetState = enabled ? _colliders3DInitialStates[i] : false;
                _colliders3D[i].enabled = targetState;
            }
        }

        if (_rigidbody2D)
        {
            _rigidbody2D.simulated = enabled ? _rigidbody2DSimulatedInitial : false;
            if (!enabled)
            {
                _rigidbody2D.linearVelocity = Vector2.zero;
                _rigidbody2D.angularVelocity = 0f;
            }
        }

        if (_rigidbody3D)
        {
            _rigidbody3D.isKinematic = enabled ? _rigidbody3DKinematicInitial : true;
            if (!enabled)
            {
                _rigidbody3D.linearVelocity = Vector3.zero;
                _rigidbody3D.angularVelocity = Vector3.zero;
            }
        }

        if (_spriteRenderers != null)
        {
            for (int i = 0; i < _spriteRenderers.Length; i++)
            {
                if (!_spriteRenderers[i]) continue;
                bool targetState = enabled ? _spriteRenderersInitialStates[i] : false;
                _spriteRenderers[i].enabled = targetState;
            }
        }
    }
}
