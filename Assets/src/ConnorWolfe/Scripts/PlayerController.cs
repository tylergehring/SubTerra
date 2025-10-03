using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
    // using Old Unity Input systems
    public KeyCode mvRightKey;
    public KeyCode mvLeftKey;
    public KeyCode jumpKey;

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


    void Start()
    {
        if (mvRightKey == KeyCode.None)
            mvRightKey = KeyCode.D;
        if (mvLeftKey == KeyCode.None)
            mvLeftKey = KeyCode.A;
        if (jumpKey == KeyCode.None)
            jumpKey = KeyCode.Space;

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
        Debug.Log("_horizontalMovement == " + _horizontalMovement);

        if (Input.GetKey(jumpKey))
            _isJumping = true;
        else
            _isJumping= false;

        Debug.Log("_isJumping == " + _isJumping);

    }

    private void _GoundMove()
    {
        //if (_onWall) return;
        rb.linearVelocityX = _horizontalMovement * _moveSpeed;
        Debug.Log("rb.linearVelocityX == " + rb.linearVelocityX);

        if (_isJumping && _onGround)
            rb.linearVelocityY = _jumpStrength;
        else if (!_isJumping && rb.linearVelocityY > 0)
            rb.linearVelocityY = 0f;
    }

    private void _CheckSurface()
    {
        _onGround = _groundCollider.IsTouchingLayers(LayerMask.GetMask("Environment"));
        Debug.Log("_groundCollider.IsTouchingLayers(LayerMask.GetMask(\"Environment\")) == " + _groundCollider.IsTouchingLayers(LayerMask.GetMask("Environment")));
    }

    // PublicFunctions //
    public void Pause(bool newPause) { _isPaused = newPause; }    
}
