using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
    // public as to be able to change the key binds in other scripts
    // using Old Unity Input systems
    public KeyCode mvRightKey = KeyCode.D;
    public KeyCode mvLeftKey = KeyCode.A;
    public KeyCode jumpKey = KeyCode.Space;
    //    public KeyCode pickUpKey = KeyCode.E; // picking up actions moved to ItemHandler script/GameObject
    public KeyCode dropKey = KeyCode.Q;
    public KeyCode tabKey = KeyCode.Tab;
    public List<KeyCode> invenHotKeys = new List<KeyCode> { KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, KeyCode.Alpha4 };

    [SerializeField] private float _jumpStrength;
    [SerializeField] private float _moveSpeed;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private BoxCollider2D _groundCollider;
    [SerializeField] private BoxCollider2D _leftWallCol;
    [SerializeField] private BoxCollider2D _rightWallCol;


    private bool _onGround = false;
    private bool _onWall = false;
    private bool _isPaused = false;
    private bool _isJumping = false;
    private float _horizontalMovement;
    private QuickAccess _inventory = new QuickAccess();

    void Start()
    {

        if (mvRightKey == KeyCode.None) // right: D
            mvRightKey = KeyCode.D;
        if (mvLeftKey == KeyCode.None) // left: A
            mvLeftKey = KeyCode.A;
        if (jumpKey == KeyCode.None) // jump: SPACE
            jumpKey = KeyCode.Space;
        //      if (pickUpKey == KeyCode.None) // pickUp: E
        //          pickUpKey = KeyCode.E;
        if (dropKey == KeyCode.None) // drop: Q
            dropKey = KeyCode.Q;
        if (tabKey == KeyCode.None) // tab: TAB
            tabKey = KeyCode.Tab;

        if (!rb)
            rb = GetComponent<Rigidbody2D>();

        if (!_groundCollider) {
            GameObject obj = GameObject.Find("GroundCollider");
            _groundCollider = obj.GetComponent<BoxCollider2D>();
        }
           
   
    }

    void Update()
    {
        _GetInput();
    }

    void FixedUpdate()
    {
        if (_isPaused) return;
        _CheckSurface();
        _GoundMove();
    }

    // _privateFuntions //
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
            int _tempIndex = 0;
            foreach (KeyCode key in invenHotKeys)
            {
                if (Input.GetKeyDown(key))
                {
                    _inventory.Tab(_tempIndex);
                    break;
                }
                _tempIndex++;
            }
        }
    }

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

    private void _CheckSurface()
    {
        _onGround = _groundCollider.IsTouchingLayers(LayerMask.GetMask("Environment"));
      //  Debug.Log("_groundCollider.IsTouchingLayers(LayerMask.GetMask(\"Environment\")) == " + _groundCollider.IsTouchingLayers(LayerMask.GetMask("Environment")));
    }

    private void _Drop()
    {
        /*
         * code for dropping item
         */
        _inventory.SetItem(null);
    }

    // PublicFunctions //
    public void Pause(bool newPause) { _isPaused = newPause; }    

    public GameObject PickUp(GameObject newItem)
    {
        return _inventory.SetItem(newItem);
    }
}
