
using UnityEngine;

public class ABetterGame : Weapon_Class
{
    private float mineTimer;
    private AudioSource audioSource;
    private float rotationSpeed = 40f;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();

        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }
    }

    private void Update()
    {
        mineTimer += Time.deltaTime;

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
        Vector3 pos = player.position + player.forward * 1.2f; // slightly farther
        pos.y = player.position.y + 2.1f;
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
        transform.Rotate(0f, rotationSpeed * Time.deltaTime, 90f);
    }

    private void PlaySound()
    {
        if (!audioSource.isPlaying)
            audioSource.Play();
    }

    private void StopSound()
    {
        if (audioSource.isPlaying)
            audioSource.Stop();
    }

    private void HandleMineTimer()
    {
        if (mineTimer > 1.4f)
        {
            mineTimer = 0;
        }
    }
}
