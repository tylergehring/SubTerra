using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UIElements;
using System.Text;

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
    public List<KeyCode> invenHotKeys = new List<KeyCode> { KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, KeyCode.Alpha4 };

    // movement values
    [SerializeField] private float _jumpStrength;
    [SerializeField] private float _moveSpeed;
    // player health
    [SerializeField] private byte _health = (byte)3; // Unsigned 8bit integer (0 to 255)
    // player components that help the player move
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private BoxCollider2D _groundCollider;
    [SerializeField] private BoxCollider2D _leftWallCol;
    [SerializeField] private BoxCollider2D _rightWallCol;
    // Lets us drop items into the world
    [SerializeField] private GameObject _itemHandlerPrefab;

    // booleans for checking movement and position states
    private bool _onGround = false;
    private bool _onWall = false;
    private bool _isPaused = false;
    private bool _isJumping = false;
    private float _horizontalMovement;
    // The inventory for the player
    private QuickAccess _inventory = new QuickAccess();

    void Start()
    {

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

        // getting the rigibody manually if is not set in inspector
        if (!rb)
            rb = GetComponent<Rigidbody2D>();

        if (!_groundCollider) {
            GameObject obj = GameObject.Find("GroundCollider");
            _groundCollider = obj.GetComponent<BoxCollider2D>();
        }
           
   
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
        _CheckSurface();
        _GoundMove();
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
    }

    // move the player if its on the ground
    private void _GoundMove()
    {
        //if (_onWall) return;
        rb.linearVelocityX = _horizontalMovement * _moveSpeed;
       // Debug.Log("rb.linearVelocityX == " + rb.linearVelocityX);

        if (_isJumping && _onGround)
            rb.linearVelocityY = _jumpStrength;
        else if (!_isJumping && rb.linearVelocityY > 0)
            rb.linearVelocityY = 0f;
    }

    // check if the player is on the ground or on a wall
    private void _CheckSurface()
    {
        _onGround = _groundCollider.IsTouchingLayers(LayerMask.GetMask("Environment"));
      //  Debug.Log("_groundCollider.IsTouchingLayers(LayerMask.GetMask(\"Environment\")) == " + _groundCollider.IsTouchingLayers(LayerMask.GetMask("Environment")));
    }

    // drop an item from the inventory
    private void _Drop()
    {
        if (!_itemHandlerPrefab)
            return;

        GameObject temp = _inventory.SetItem(null);

        if (!temp)
            return;

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

    // PublicFunctions //
    // pause/unpause the player
    public void Pause(bool newPause) { _isPaused = newPause; }

    // change the player health by a given amount
    public void ChangeHealth (int amount)
    {
        if (_health + amount < 0)
            _health = 0;
        if (_health + amount > 255)
            _health = 255;

        _health +=  (byte)amount;


    }

    // pick up an item and add it to the open inventory slot
    public GameObject PickUp(GameObject newItem)
    {
        return _inventory.SetItem(newItem);
    }
}
