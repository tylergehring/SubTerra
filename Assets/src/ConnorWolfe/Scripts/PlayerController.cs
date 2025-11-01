using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UIElements;
using System.Text;
using System.Runtime.InteropServices;

public class PlayerController : MonoBehaviour
{
    // this script handles systems for the player (Arjun)

    // public as to be able to change the key binds in other scripts
    // using Old Unity Input systems
    public KeyCode mvRightKey = KeyCode.D;
    public KeyCode mvLeftKey = KeyCode.A;
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode pickUpKey = KeyCode.E; // picking up actions moved to ItemHandler script/GameObject
    public KeyCode dropKey = KeyCode.Q;
    public KeyCode tabKey = KeyCode.Tab;
    public KeyCode useKey = KeyCode.F;
    public List<KeyCode> invenHotKeys = new List<KeyCode> { KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, KeyCode.Alpha4 };

    // movement values
    [SerializeField] private float _jumpStrength;
    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _climbSpeed;
    // raycast value(s)
    [SerializeField] private float _raycastRange;
    // player health
    [SerializeField] private byte _health = (byte)3; // Unsigned 8bit integer (0 to 255)
    // player score
    [SerializeField] private uint _playerScore = 0;
    // player components that help the player move
    [SerializeField] private Rigidbody2D _rb;
    [SerializeField] private SpriteRenderer _playerSprite;
    [SerializeField] private Animator _playerAnimator;
    // Lets us drop items into the world
    [SerializeField] private GameObject _itemHandlerPrefab;

    // booleans for checking movement and position states
    private bool _onGround = false;
    private bool _onWall = false;
    //private bool _onLeftWall = false;
    //private bool _onRightWall = false;
    private bool _isPaused = false;
    private bool _isJumping = false;
    private float _horizontalMovement;
    private float _halfHeight; // used with raycasting to determine bounds
    private float _halfWidth;
    private float _animTimer = 0f;
    // The inventory for the player
    private QuickAccess _inventory = new QuickAccess();

    void Start()
    {
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
   
    }

    /* in Update:
        - We get input
        - We check if the player is alive
    */
    void Update()
    {
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
        _horizontalMovement = rightMv - leftMv;
        //   Debug.Log("_horizontalMovement == " + _horizontalMovement);



        if (Input.GetKey(jumpKey))
            _isJumping = true;
        else
            _isJumping= false;

        //    Debug.Log("_isJumping == " + _isJumping);

        // inventory actions
        if (Input.GetKeyDown(dropKey))
        {
            _Drop();
        }
        else if (Input.GetKeyDown(tabKey))
        {
            _inventory.Tab();
        }
        else
        {
            int tempIndex = 0;
            foreach (KeyCode key in invenHotKeys)
            {
                if (Input.GetKeyDown(key))
                {
                    _inventory.Tab(tempIndex);
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


        _rb.linearVelocityX = _horizontalMovement * _moveSpeed;

        if (_isJumping && _onGround)
            _rb.linearVelocityY = _jumpStrength;
        else if (!_isJumping && _rb.linearVelocityY > 0)
            _rb.linearVelocityY = 0f;
    }

    private void _WallMove()
    {
        if (_onGround)
            return;

        if (_isJumping && _onWall)
        {
            _rb.linearVelocityY = _climbSpeed;
        }
     

    }

    // check if the player is on the ground or on a wall
    private void _CheckSurface()
    {
        _onGround = Physics2D.Raycast(transform.position, Vector2.down, _halfHeight + _raycastRange, LayerMask.GetMask("Environment"));
        _onWall = (Physics2D.Raycast(transform.position, Vector2.left, _halfWidth + _raycastRange, LayerMask.GetMask("Environment")) ||
            Physics2D.Raycast(transform.position, Vector2.right, _halfWidth + _raycastRange, LayerMask.GetMask("Environment")));
//        _onLeftWall = Physics2D.Raycast(transform.position, Vector2.left, _halfWidth + 0.1f, LayerMask.GetMask("Environment"));
  //      _onRightWall = Physics2D.Raycast(transform.position, Vector2.right, _halfWidth + 0.1f, LayerMask.GetMask("Environment"));
    }

    private void _UseCurrentItem()
    {
        GameObject current = _inventory.GetItem();
        if (!current)
            return;

        NonReusableTools tool = current.GetComponent<NonReusableTools>();
        if (!tool)
        {
            Debug.LogWarning("WARNING: Current inventory slot does not contain a usable tool.");
            return;
        }

        tool.Use(this);
    }

    // drop an item from the inventory
    private void _Drop()
    {
        if (!_itemHandlerPrefab)
            return;

        GameObject temp = _inventory.SetItem(null);

        if (!temp)
            return;

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


    }

    private void _UpdateAnimatorAndFlip()
    {
        if (!_playerSprite)
            return;
        if (_horizontalMovement != 0)
            _playerSprite.flipX = (_horizontalMovement < 0);

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
                {
                    _animTimer += Time.deltaTime;
                } else
                {
                    _playerAnimator.SetBool("doIdleC" ,true);
                }

                int randNum = Random.Range(-1000, 1000);
                if (randNum == 0)
                {
                    _playerAnimator.SetBool("doIdleB", true);
                }

            }
            else // running
            {
                _animTimer = 0;
                _playerAnimator.SetBool("isRunning", true);
            }
        }
        else if (_onWall)
        {
            _playerAnimator.SetBool("isClimbing", true);
        } 
        else /*if (_isJumping)*/ // in air
        {
            if (_rb.linearVelocityY > 0)
            {
                _playerAnimator.SetBool("isJumping", true);
            }
            else if (_rb.linearVelocityY < 0)
            {
                _playerAnimator.SetBool("isFalling", true);
            }

        }


    }

    // PublicFunctions //
    // pause/unpause the player
    public void Pause(bool newPause) {
        _isPaused = newPause;
    }

    // change the player health by a given amount
    public void ChangeHealth(int amount)
    {

        if (_health + amount < byte.MinValue)
        {
            _health = byte.MinValue;
            return;
        }
        else if (_health + amount > byte.MaxValue) {
            _health = byte.MaxValue;
            return;
        }

        _health += (byte)amount;
        int previousHealth = _health;
        int newHealth = _health + amount;

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
            Destroy(removed);
        else if (destroyInstance && !removed && tool)
            Destroy(tool.gameObject);
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

        return previous;
    }
}
