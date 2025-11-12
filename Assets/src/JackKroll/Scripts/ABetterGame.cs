
using UnityEngine;

public class ABetterGame : Weapon_Class
{
    private float _mineTimer;
    private AudioSource _audioSource;
    private float _rotationSpeed = 40f;

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();

        if (_player == null)
        {
            //NEW WAY TO GET PLAYER POSITION. 
            _player = PlayerController.Instance.transform;
            // _player = GameObject.FindGameObjectWithTag("Player").transform;
        }
    }

    private void Update()
    {
        _mineTimer += Time.deltaTime;

        if (!gameObject.activeSelf) return;
        // Dynamically bounded function call
        UpdatePosition();  
        HandleInput();
    }

    // Override superclass version
    //Comment out for defalt positining.
    protected override void UpdatePosition()
    {
        
        // Add or modify subclass-specific behavior
        // (for example, rotate slightly, or change offset)
        Vector3 pos = _player.position + _player.forward * 1.2f; // slightly farther
        pos.y = _player.position.y + 2.1f;
        pos.z = -1f;
        transform.position = pos;
    }
    
    private void HandleInput()
    {
        bool shouldRotate = ShouldRotate();

        if (shouldRotate)
        {
            RotateWeapon();
            PlaySound();
            HandleMineTimer();
        }
        else
        {
            StopSound();
        }
    }

    private bool ShouldRotate()
    {
        return (Input.GetMouseButton(0) && !Input.GetKeyDown(KeyCode.F))
            || (!Input.GetMouseButton(0) && Input.GetKeyDown(KeyCode.F));
    }

    private void RotateWeapon()
    {
        transform.Rotate(0f, _rotationSpeed * Time.deltaTime, 90f);
    }

    private void PlaySound()
    {
        if (!_audioSource.isPlaying)
            _audioSource.Play();
    }

    private void StopSound()
    {
        if (_audioSource.isPlaying)
            _audioSource.Stop();
    }

    private void HandleMineTimer()
    {
        if (_mineTimer > 1.4f)
        {
            _mineTimer = 0;
        }
    }
}
