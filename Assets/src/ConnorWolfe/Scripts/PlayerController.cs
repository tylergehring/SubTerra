using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.PlayerSettings;


/* Example of dynamic binding
 
 */
public class SpeedSuperC
{
    protected float _speed = 0f;

    public void SetSpeed(float newSpeed)
    {
        _speed = newSpeed;
    }
    // dynamic version
    public virtual float GetSpeed()
    {
        return Random.Range(0, 2 * _speed);
        // return Speed
    }
    /* static version
    public float GetSpeed()
    {
        return Random.Range(0, 2 * _speed);
        // return Speed
    }
    */
}

public class Speed : SpeedSuperC
{
    // dynamic version
    public override float GetSpeed()
    {
        return _speed;
    }
    /* static version
    public override float GetSpeed()
    {
        return _speed;
    }
    */
}


public class PlayerController : MonoBehaviour
{
    // this script handles systems for the player (Arjun)

    // public as to be able to change the key binds in other scripts
    // using Old Unity Input systems
    [Header("Keybind settings")]
    public KeyCode mvRightKey = KeyCode.D;
    public KeyCode mvLeftKey = KeyCode.A;
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode pickUpKey = KeyCode.E; // picking up actions moved to ItemHandler script/GameObject
    public KeyCode dropKey = KeyCode.Q;
    public KeyCode tabKey = KeyCode.Tab;
    public KeyCode useKey = KeyCode.F;
    public KeyCode sprintKey = KeyCode.LeftShift; 
    public List<KeyCode> invenHotKeys = new List<KeyCode> { KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, KeyCode.Alpha4 };

    // movement values
    [Header("Movement Settings")]
    [SerializeField] private float _jumpStrength;
    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _climbSpeed;
    // raycast value(s)
    [Header("Raycast Settings")]
    [Tooltip("This is the range that the raycast will look outside the player")]
    [SerializeField] private float _raycastRange;
    // stamina
    [Header("Stamina Settings")]
    [Tooltip("Max Stamina is 100f, This is the drain rate when sprinting")]
    [SerializeField] private float _sprintSRate = 20f;
    [Tooltip("Max Stamina is 100f, This is the drain rate when climbing")]
    [SerializeField] private float _climbSRate = 15f;
  //  [SerializeField] private float _staminaRegenRate = 10f; 
//    [SerializeField] private float _staminaRegenDelay = 3f;
    // player health
    [Header("Health settings")]
    [SerializeField] private byte _health = (byte)3; // Unsigned 8bit integer (0 to 255)
    // player score
    [Header("Score settings")]
    [SerializeField] private uint _playerScore = 0; // serialized so it can be viewed in inspector
    // player components that help the player move
    [Header("Player components")]
    [SerializeField] private Rigidbody2D _rb;
    [SerializeField] private SpriteRenderer _playerSprite;
    [SerializeField] private Animator _playerAnimator;
    [Header("External components")]
    [SerializeField] private GameObject _itemHandlerPrefab; // Lets us drop items into the world
    [SerializeField] private GameObject _deadPlayerBodyPrefab;
    [SerializeField] private GameObject _deadPlayerHelmetPrefab;
    [SerializeField] private GameObject _camera;
    [SerializeField] private GameObject _flashlight;
    [SerializeField] private InventoryHotBarScript _inventoryHotBar;
    [SerializeField] private StaminaWheelScript _staminaWheel;


    // booleans for checking movement and position states
    private bool _onGround = false;
    private bool _onWall = false;
    //private bool _onLeftWall = false;
    //private bool _onRightWall = false;
    private bool _isPaused = false;
    private bool _isJumping = false;
    private bool _isSprinting = false;
    private bool _staminaExhausted = true; // turn off stamina for a moment if stamina reaches 0
    private bool _wasRight = true; // used in animation/flip control
    private bool _playerAlive = true;
    private float _horizontalMovement;
    private float _halfHeight; // used with raycasting to determine bounds
    private float _halfWidth;
    private float _animTimer = 0f;
    private float _currStamina = 100f;
    private float _staminaRegenTimer = 0f;

    // The inventory for the player
    private QuickAccess _inventory = new QuickAccess();

    // Speed class (used to showcase static vs. dynamic binding to meet class requirements)
    // dynamic version
    private SpeedSuperC _movementSpeed = new Speed();
    // static version
    // private SpeedSuperC _movementSpeed;


    void Start()
    {
        _movementSpeed.SetSpeed(_moveSpeed);

        _health = 100;  // Player health start form 100
        // if the key's are not set / set in inspector, I set them manually here
        if (mvRightKey == KeyCode.None) // right: D
            mvRightKey = KeyCode.D;
        if (mvLeftKey == KeyCode.None) // left: A
            mvLeftKey = KeyCode.A;
        if (jumpKey == KeyCode.None) // jump: SPACE
            jumpKey = KeyCode.Space;
        if (pickUpKey == KeyCode.None) // pickUp: E
            pickUpKey = KeyCode.E;
        if (dropKey == KeyCode.None) // drop: Q
            dropKey = KeyCode.Q;
        if (tabKey == KeyCode.None) // tab: TAB
            tabKey = KeyCode.Tab;
        if (useKey == KeyCode.None) // use tool: F
            useKey = KeyCode.F;
         if (sprintKey == KeyCode.None)
            sprintKey = KeyCode.LeftShift;


        // getting the rigibody manually if is not set in inspector
        if (!_rb)
            _rb = GetComponent<Rigidbody2D>();

        if (!_playerSprite)
            _playerSprite = GetComponent<SpriteRenderer>();
        if (_playerSprite) {
            _halfHeight = _playerSprite.bounds.extents.y;
            _halfWidth = _playerSprite.bounds.extents.x;
        }

        if (!_playerAnimator)
            _playerAnimator = GetComponent<Animator>();

        if (!_inventoryHotBar)
            _inventoryHotBar = GetComponentInChildren<InventoryHotBarScript>();

        if (!_staminaWheel)
            _staminaWheel = GetComponentInChildren<StaminaWheelScript>();

        if (!_camera)
        {
//            _camera = ();
        }
    }


    /* in Update:
        - We get input
        - We check if the player is alive
    */
    void Update()
    {
        _inventoryHotBar.UpdateSlotSelect(_inventory.GetIndex());
        _GetInput();

        if (_health == 0)
            Pause(true);
    }

    /* in Fixed Update
      - We check if the player is paused
      - check the surface the player is touching
      - Move the player
    */
    void FixedUpdate()
    {
        _CheckHealth();
        if (_isPaused) return;
        _CheckSurface(); // raycasting is expensive, now only runs when we intend to use it
        _GoundMove();
        _WallMove();
        _UpdateAnimatorAndFlip();
    }

    // _privateFuntions //
    // get input, apply inventory actions
    private void _GetInput()
    {
        // horizontal movement
        float rightMv = 0f;
        float leftMv = 0f;
        if (Input.GetKey(mvRightKey))
            rightMv = 1f;
        if (Input.GetKey(mvLeftKey))
            leftMv = 1f;

        _isSprinting = Input.GetKey(sprintKey);

        _horizontalMovement = rightMv - leftMv;

        _isJumping = Input.GetKey(jumpKey);

        // inventory actions
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

    // move the player if its on the ground
    private void _GoundMove()
    {
        // to meet the requirements of the class I must use the defined class in code to showcase the differences in static vs. dynamic
        float setSpeed = _movementSpeed.GetSpeed();

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

            _rb.linearVelocityX = !_isSprinting ? _horizontalMovement * setSpeed: _horizontalMovement * setSpeed * 2f;

        if (_isJumping && _onGround)
        {
            if (!_staminaWheel.IsExhausted())
                _rb.linearVelocityY = _jumpStrength;
            else
                _rb.linearVelocityY = _jumpStrength * 0.5f;
        }
        else if (!_isJumping && _rb.linearVelocityY > 0)
            _rb.linearVelocityY = 0f;
    }

    private void _WallMove()
    {
        if (_onGround)
            return;

        if (_isJumping && _onWall && !_staminaWheel.IsExhausted())
        {
            _rb.linearVelocityY = _climbSpeed;
            _staminaWheel.ChangeStamina((-1f * _climbSRate) * Time.deltaTime);
        }
     

    }

    // check if the player is on the ground or on a wall
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
        if (nrTool) {
            nrTool.Use(this);
            return;
        }
    }

    // drop an item from the inventory
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

        // reset parameters
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
            else // running / sprinting
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
        else /*if (_isJumping)*/ // in air
        {
            if (_rb.linearVelocityY > 0)
                _playerAnimator.SetBool("isJumping", true);
            else if (_rb.linearVelocityY < 0)
                _playerAnimator.SetBool("isFalling", true);
        }
    }

    private void _CheckHealth()
    {
        if (_health == 0 && _playerAlive)
        {
            if (_deadPlayerBodyPrefab && _camera)
            {
                GameObject tempBody = Instantiate(_deadPlayerBodyPrefab, transform.position, Quaternion.identity);
                _camera.transform.SetParent(tempBody.transform, false);
                Rigidbody2D tempRBB = tempBody.GetComponent<Rigidbody2D>();
                if (tempRBB)
                    tempRBB.linearVelocity = new Vector2(Random.Range(0f,3f), Random.Range(0f,3f));
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

    //// PublicFunctions ////
    // pause/unpause the player
    public void Pause(bool newPause) {
        _isPaused = newPause;
    }

    // change the player health by a given amount
    public void ChangeHealth(int amount)
    {
        Debug.Log($"PlayerController: ChangeHealth -> amount == {amount}\n" +
                  $"& -> _health == {_health}");
   
        if (_health + amount < byte.MinValue)
        {
            _health = byte.MinValue;
            return;
        }
        else if (_health + amount > byte.MaxValue) {
            _health = byte.MaxValue;
            return;
        }

    
        int previousHealth = _health;
        int newHealth = _health - amount;

        if (newHealth < 0)
            newHealth = 0;
        if (newHealth > 255)
            newHealth = 255;


        _health = (byte)newHealth;            

        Debug.Log($"Player health changed by {amount}. Previous: {previousHealth}, Current: {_health}");

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

    // Returns the player's current health value for testing or display purposes
    public float GetHealth()
    {
        return _health; // _health is the internal variable storing player's health
    }

    public void ChangeScore(int change)
    {
        if (change + _playerScore > uint.MaxValue)
        {
            _playerScore = uint.MaxValue;
            return;
        } else if (change + _playerScore < uint.MinValue)
        {
            _playerScore = uint.MinValue;
            return;
        }
        
        _playerScore += (uint)change;
    }

    public uint GetScore()
    {
        return _playerScore;
    }

    // pick up an item and add it to the open inventory slot
    public GameObject PickUp(GameObject newItem)
    {
        if (newItem)
        {
            newItem.transform.SetParent(null);
        }

        GameObject previous = _inventory.SetItem(newItem);

        if (newItem)
        {
            NonReusableTools newTool = newItem.GetComponent<NonReusableTools>();
            if (newTool)
                newTool.OnPickup(this);
        }

        if (previous)
        {
            NonReusableTools previousTool = previous.GetComponent<NonReusableTools>();
            if (previousTool)
                previousTool.OnDropped(this);
        }

        Sprite tempSprite = newItem.GetComponent<SpriteRenderer>().sprite;
        _inventoryHotBar.UpdateSlotItem(_inventory.GetIndex(), tempSprite);

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

        /* Victory Sceeen stuff*/

        Pause(true);

    }

    public void ChangeStamina(float amount)
    {
        _staminaWheel.ChangeStamina(amount);
    }
}


