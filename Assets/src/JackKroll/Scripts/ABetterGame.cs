//Jack Kroll
/*
    This class extends the base Weapon_Class to create a functional tool that rotates,
    plays sound, and responds to player input. It updates its position relative to the player,
    rotates when the player is using it, and manages audio playback and a simple timer.
    The class also overrides the default positioning behavior to adjust how the tool follows
    the player, making it slightly offset and customized for this specific weapon.
*/

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
        //static never changes 
        HandleInput();
    }

    // Override superclass version
    //Comment out for defalt positining.
    
    protected override void UpdatePosition()
    {
     
       
        // Add or modify subclass-specific behavior
        // (for example, rotate slightly, or change offset)
        Vector3 pos = _player.position + _player.forward * 1.2f; 
        pos.y = _player.position.y + 4.1f;
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
        // Rotates weapon on Y-axis over time
        transform.Rotate(0f, _rotationSpeed * Time.deltaTime, 90f);
    }

    private void PlaySound()
    {
        // Only start sound if not already playing
        if (!_audioSource.isPlaying)
            _audioSource.Play();
    }

    private void StopSound()
    {
        // Stops sound when rotation stops.
        if (_audioSource.isPlaying)
            _audioSource.Stop();
    }

    private void HandleMineTimer()
    {
        // Reset the timer after threshold.
        if (_mineTimer > 1.4f)
        {
            _mineTimer = 0;
        }
    }
}
