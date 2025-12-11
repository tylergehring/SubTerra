using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;
using UnityEngine.UIElements;

#if UNITY_EDITOR
using static UnityEditor.PlayerSettings;
#endif

/* Example of dynamic binding
 
 */
public class SpeedSuperC
{
    protected float _speed = 0f;

    public void SetSpeed(float newSpeed)
    {
        _speed = newSpeed;
    }

    public virtual float GetSpeed()
    {
        return 3 * _speed;
    }
}

public class Speed : SpeedSuperC
{
    public override float GetSpeed()
    {
        return _speed;
    }
}

public class PlayerController : MonoBehaviour
{
    [HideInInspector] public static PlayerController Instance { get; private set; }

    [Header("BC mode toggle")]
    [SerializeField] public bool isBCMode = false;

    [Header("Keybind settings")]
    public KeyCode mvRightKey = KeyCode.D;
    public KeyCode mvLeftKey = KeyCode.A;
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode pickUpKey = KeyCode.E;
    public KeyCode dropKey = KeyCode.Q;
    public KeyCode tabKey = KeyCode.Tab;
    public KeyCode useKey = KeyCode.F;
    public KeyCode sprintKey = KeyCode.LeftShift;
    public List<KeyCode> invenHotKeys = new List<KeyCode> { KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, KeyCode.Alpha4 };

    [Header("Movement Settings")]
    [SerializeField] private float _jumpStrength;
    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _climbSpeed;

    [Header("Raycast Settings")]
    [Tooltip("This is the range that the raycast will look outside the player")]
    [SerializeField] private float _raycastRange;

    [Header("Stamina Settings")]
    [SerializeField] private float _sprintSRate = 20f;
    [SerializeField] private float _climbSRate = 15f;

    [Header("Health settings")]
    [SerializeField] private byte _health = (byte)3;

    [Header("Score settings")]
    [SerializeField] private uint _playerScore = 0;

    [Header("Player components")]
    [SerializeField] private Rigidbody2D _rb;
    [SerializeField] private SpriteRenderer _playerSprite;
    [SerializeField] private Animator _playerAnimator;

    [Header("External components")]
    [SerializeField] private GameObject _itemHandlerPrefab;
    [SerializeField] private GameObject _deadPlayerBodyPrefab;
    [SerializeField] private GameObject _deadPlayerHelmetPrefab;
    [SerializeField] private GameObject _camera;
    [SerializeField] private GameObject _flashlight;
    [SerializeField] private InventoryHotBarScript _inventoryHotBar;
    [SerializeField] private StaminaWheelScript _staminaWheel;

    private bool _onGround = false;
    private bool _onWall = false;
    private bool _isPaused = false;
    private bool _isJumping = false;
    private bool _isSprinting = false;
    private bool _staminaExhausted = true;
    private bool _wasRight = true;
    private bool _playerAlive = true;

    private float _horizontalMovement;
    private float _halfHeight;
    private float _halfWidth;
    private float _animTimer = 0f;
    private float _currStamina = 100f;
    private float _staminaRegenTimer = 0f;

    private QuickAccess _inventory = new QuickAccess();
    private SpeedSuperC _movementSpeed = new Speed();

    private float footstepCooldown = 0.3f;
    private float lastFootstepTime = 0f;
    private float jumpCooldown = 0.5f;
    private float lastJumpTime = 0f;

    public HealthBar healthBar;
    public bool useAI = false;

    void Start()
    {
        _movementSpeed.SetSpeed(_moveSpeed);

        _health = 100;

        if (mvRightKey == KeyCode.None)
            mvRightKey = KeyCode.D;
        if (mvLeftKey == KeyCode.None)
            mvLeftKey = KeyCode.A;
        if (jumpKey == KeyCode.None)
            jumpKey = KeyCode.Space;
        if (pickUpKey == KeyCode.None)
            pickUpKey = KeyCode.E;
        if (dropKey == KeyCode.None)
            dropKey = KeyCode.Q;
        if (tabKey == KeyCode.None)
            tabKey = KeyCode.Tab;
        if (useKey == KeyCode.None)
            useKey = KeyCode.F;
        if (sprintKey == KeyCode.None)
            sprintKey = KeyCode.LeftShift;

        if (!_rb)
            _rb = GetComponent<Rigidbody2D>();

        if (!_playerSprite)
            _playerSprite = GetComponent<SpriteRenderer>();
        if (_playerSprite)
        {
            _halfHeight = _playerSprite.bounds.extents.y;
            _halfWidth = _playerSprite.bounds.extents.x;
        }

        if (!_playerAnimator)
            _playerAnimator = GetComponent<Animator>();

        if (!_inventoryHotBar)
            _inventoryHotBar = GetComponentInChildren<InventoryHotBarScript>();

        if (!_staminaWheel)
            _staminaWheel = GetComponentInChildren<StaminaWheelScript>();

        if (healthBar != null)
            healthBar.SetMaxHealth(_health);
    }

    private void Awake()
    {
        if (Instance != null && Instance != null)
            Destroy(this.gameObject);

        Instance = this;
        Debug.Log($"instance == {Instance.name}");
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    void Update()
    {
        _inventoryHotBar.UpdateSlotSelect(_inventory.GetIndex());
        _GetInput();

        if (_health == 0)
            Pause(true);
    }

    void FixedUpdate()
    {
        _CheckHealth();
        if (_isPaused) return;
        _CheckSurface();
        _GoundMove();
        _WallMove();
        _UpdateAnimatorAndFlip();
    }

    private void _GetInput()
    {
#if UNITY_ANDROID || UNITY_IOS
        if (FindObjectOfType<MobileControlsUI>() != null)
            return;
#endif

        if (useAI) return;

        float rightMv = 0f;
        float leftMv = 0f;
        if (Input.GetKey(mvRightKey))
            rightMv = 1f;
        if (Input.GetKey(mvLeftKey))
            leftMv = 1f;

        _isSprinting = Input.GetKey(sprintKey);

        _horizontalMovement = rightMv - leftMv;

        _isJumping = Input.GetKey(jumpKey);

        if (Input.GetKeyDown(dropKey))
        {
            _Drop();
        }
        else if (Input.GetKeyDown(tabKey))
        {
            _inventory.Tab();
            _inventoryHotBar.UpdateSlotSelect(_inventory.GetIndex());
        }
        else
        {
            int tempIndex = 0;
            foreach (KeyCode key in invenHotKeys)
            {
                if (Input.GetKeyDown(key))
                {
                    _inventory.Tab(tempIndex);
                    _inventoryHotBar.UpdateSlotSelect(_inventory.GetIndex());
                    break;
                }
                tempIndex++;
            }
        }

        if (Input.GetKeyDown(useKey))
        {
            _UseCurrentItem();
        }
    }

    private void _GoundMove()
    {
        float setSpeed = _movementSpeed.GetSpeed();
        if (isBCMode)
        {
            _rb.linearVelocityX = !_isSprinting ? _horizontalMovement * setSpeed : _horizontalMovement * setSpeed * 2f;
            if (_isJumping)
                _rb.linearVelocityY = _jumpStrength;
            return;
        }

        if (!_staminaWheel.IsExhausted())
        {
            _rb.linearVelocityX = !_isSprinting ? _horizontalMovement * setSpeed : _horizontalMovement * setSpeed * 2f;
            if (_isSprinting && _horizontalMovement != 0)
                _staminaWheel.ChangeStamina((-1f * _sprintSRate) * Time.deltaTime);
        }
        else
        {
            _rb.linearVelocityX = _horizontalMovement * setSpeed;
            _isSprinting = false;
        }

        _rb.linearVelocityX = !_isSprinting ? _horizontalMovement * setSpeed : _horizontalMovement * setSpeed * 2f;

        if (_isJumping && _onGround)
        {
            if (!_staminaWheel.IsExhausted())
                _rb.linearVelocityY = _jumpStrength;
            else
                _rb.linearVelocityY = _jumpStrength * 0.5f;

            if (Time.time - lastJumpTime >= jumpCooldown)
            {
                SoundEvents.PlayerJump();
                lastJumpTime = Time.time;
            }
        }
        else if (!_isJumping && _rb.linearVelocityY > 0)
            _rb.linearVelocityY = 0f;

        if (_onGround && Mathf.Abs(_horizontalMovement) > 0.1f)
        {
            if (Time.time - lastFootstepTime >= footstepCooldown)
            {
                SoundEvents.Footstep();
                lastFootstepTime = Time.time;
            }
        }
    }

    private void _WallMove()
    {
        if (_onGround)
            return;

        if (_isJumping && _onWall && (!_staminaWheel.IsExhausted() || isBCMode))
        {
            _rb.linearVelocityY = _climbSpeed;
            if (!isBCMode)
                _staminaWheel.ChangeStamina((-1f * _climbSRate) * Time.deltaTime);
        }
    }

    private void _CheckSurface()
    {
        _onGround = Physics2D.Raycast(transform.position, Vector2.down, _halfHeight + _raycastRange, LayerMask.GetMask("Environment"));
        _onWall = (Physics2D.Raycast(transform.position, Vector2.left, _halfWidth + _raycastRange, LayerMask.GetMask("Environment")) ||
            Physics2D.Raycast(transform.position, Vector2.right, _halfWidth + _raycastRange, LayerMask.GetMask("Environment")));
    }

    private void _UseCurrentItem()
    {
        GameObject current = _inventory.GetItem();
        if (!current)
            return;

        NonReusableTools nrTool = current.GetComponent<NonReusableTools>();
        if (nrTool)
        {
            nrTool.Use(this);
            SoundEvents.ToolUse();
            return;
        }
    }

    private GameObject _Drop()
    {
        if (!_itemHandlerPrefab)
            return null;

        GameObject temp = _inventory.SetItem(null);

        if (!temp)
            return null;

        NonReusableTools toolComp = temp.GetComponent<NonReusableTools>();
        if (toolComp)
            toolComp.OnDropped(this);

        temp.SetActive(false);

        Vector3 pos = transform.position;
        GameObject newHandler = Instantiate(_itemHandlerPrefab, pos, Quaternion.identity);
        newHandler.SetActive(true);
        ItemHandler handler = newHandler.GetComponent<ItemHandler>();
        if (handler)
        {
            handler.SetInteractKey(pickUpKey);
            handler.SetHeldItem(temp);
            handler.SetPickupCooldown(1f);
            handler.UpdateSprite();
        }

        SoundEvents.ToolPickup();
        _inventoryHotBar.UpdateSlotItem(_inventory.GetIndex(), null);
        return newHandler;
    }

    private void _UpdateAnimatorAndFlip()
    {
        if (!_playerSprite)
            return;

        if (_horizontalMovement != 0)
        {
            if ((_horizontalMovement < 0 && _wasRight) || (_horizontalMovement > 0 && !_wasRight))
                _playerAnimator.Play("Player Turn Around");

            _wasRight = (_horizontalMovement > 0);
            _playerSprite.flipX = (_horizontalMovement < 0);
        }

        if (!_playerAnimator)
            return;

        foreach (AnimatorControllerParameter parameter in _playerAnimator.parameters)
        {
            if (parameter.type == AnimatorControllerParameterType.Bool)
                _playerAnimator.SetBool(parameter.name, false);
        }

        if (_onGround)
        {
            if (_horizontalMovement == 0)
            {
                if (_animTimer <= 25)
                    _animTimer += Time.deltaTime;
                else
                    _playerAnimator.SetBool("doIdleC", true);

                int randNum = Random.Range(-1000, 1000);
                if (randNum == 0)
                    _playerAnimator.SetBool("doIdleB", true);

            }
            else
            {
                _animTimer = 0f;
                _playerAnimator.SetBool("isRunning", true);
                _playerAnimator.SetBool("isSprinting", _isSprinting);
            }
        }
        else if (_onWall)
        {
            _playerAnimator.SetBool("isClimbing", true);
        }
        else
        {
            if (_rb.linearVelocityY > 0)
                _playerAnimator.SetBool("isJumping", true);
            else if (_rb.linearVelocityY < 0)
                _playerAnimator.SetBool("isFalling", true);
        }
    }

    private void _CheckHealth()
    {
        if (isBCMode)
            return;

        if (_health == 0 && _playerAlive)
        {
            if (_deadPlayerBodyPrefab && _camera)
            {
                GameObject tempBody = Instantiate(_deadPlayerBodyPrefab, transform.position, Quaternion.identity);
                _camera.transform.SetParent(tempBody.transform, false);
                Rigidbody2D tempRBB = tempBody.GetComponent<Rigidbody2D>();
                if (tempRBB)
                    tempRBB.linearVelocity = new Vector2(Random.Range(0f, 3f), Random.Range(0f, 3f));
            }
            if (_deadPlayerHelmetPrefab && _flashlight)
            {
                GameObject tempHelm = Instantiate(_deadPlayerHelmetPrefab, _flashlight.transform.position, Quaternion.identity);
                _flashlight.transform.SetParent(tempHelm.transform, false);
                Rigidbody2D tempRBH = tempHelm.GetComponent<Rigidbody2D>();
                if (tempRBH)
                    tempRBH.linearVelocity = new Vector2(Random.Range(0f, 3f), Random.Range(0f, 3f));
            }
            for (int i = 0; i < 4; i++)
            {
                _inventory.Tab(i);
                GameObject item = _Drop();
                Rigidbody2D itemRb = null;
                if (item)
                    itemRb = item.GetComponent<Rigidbody2D>();
                if (itemRb)
                    itemRb.linearVelocity = new Vector2(Random.Range(0f, 5f), Random.Range(0f, 5f));
            }

            _playerAlive = false;
            Pause(true);
            this.gameObject.SetActive(false);
        }
    }

    public void Pause(bool newPause)
    {
        _isPaused = newPause;
    }

    public void ChangeHealth(int amount)
    {
        int previousHealth = _health;
        int newHealth = _health - amount;
        newHealth = Mathf.Clamp(newHealth, byte.MinValue, byte.MaxValue);

        _health = (byte)newHealth;

        if (healthBar != null)
            healthBar.SetHealth(_health);

        if (_health < previousHealth)
        {
            SoundEvents.EnemyDamage();
        }
    }

    public byte getHealth()
    {
        return _health;
    }

    public void ConsumeCurrentTool(NonReusableTools tool, bool destroyInstance)
    {
        GameObject current = _inventory.GetItem();

        if (current && tool && current != tool.gameObject)
            return;

        GameObject removed = _inventory.SetItem(null);
        if (destroyInstance && removed)
        {
            Destroy(removed);
        }

        _inventoryHotBar.UpdateSlotItem(_inventory.GetIndex(), null);
    }

    public float GetHealth()
    {
        return _health;
    }

    public void ChangeScore(int change)
    {
        long newScore = (long)_playerScore + change;
        newScore = (uint)Mathf.Clamp(_playerScore, uint.MinValue, uint.MaxValue);
        _playerScore = (uint)newScore;
    }

    public uint GetScore()
    {
        return _playerScore;
    }

    public GameObject PickUp(GameObject newItem)
    {
        if (newItem == null) return null;

        GameObject item = ItemFactory.CreateItem(newItem);

        item.transform.SetParent(null);
        item.SetActive(false);

        var tool = item.GetComponent<NonReusableTools>();
        tool?.OnPickup(this);

        GameObject previous = _inventory.SetItem(item);

        if (previous)
        {
            var prevTool = previous.GetComponent<NonReusableTools>();
            prevTool?.OnDropped(this);
        }

        var sr = item.GetComponent<SpriteRenderer>();
        if (sr)
            _inventoryHotBar.UpdateSlotItem(_inventory.GetIndex(), sr.sprite);

        SoundEvents.ToolPickup();

        return previous;
    }

    public void Victory()
    {
        int scoreMultiplier = 1;
        for (int i = 0; i < 4; i++)
        {
            _inventory.Tab(i);
            GameObject item = _inventory.GetItem();
            if (item)
                scoreMultiplier += 1;
        }

        ChangeScore((int)(_playerScore * scoreMultiplier));
        Pause(true);
    }

    public void ChangeStamina(float amount)
    {
        _staminaWheel.ChangeStamina(amount);
    }

    public GameObject GetObject()
    {
        return this.gameObject;
    }

    // ✅ ADDED METHOD #1
    public void SetMobileInput(float horizontal, bool jumpPressed)
    {
        _horizontalMovement = horizontal;

        if (jumpPressed)
            _isJumping = true;
        else if (horizontal == 0f)
            _isJumping = false;
    }

    // ✅ ADDED METHOD #2
    public void MobileUseCurrentItem()
    {
        _UseCurrentItem();
    }
}
