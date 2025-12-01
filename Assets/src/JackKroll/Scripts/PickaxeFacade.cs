//jack kroll

/*
    The PickaxeFacade class manages all behavior related to the pickaxe, including its position,
    rotation, mining actions, and audio playback. It updates the tool each frame by moving it in
    front of the player, checking whether the player is mining, rotating the pickaxe when needed,
    playing or stopping sound effects, and triggering terrain destruction based on a timed delay.
    This class centralizes mining logic into one clean, reusable system, making it easier to
    maintain and extend the pickaxe’s functionality.
*/

using UnityEngine;
// Facade class handling pickaxe behavior, mining logic, and audio
public class PickaxeFacade
{
    // Reference to the audio source
    private readonly AudioSource _audioSource;
    // Reference to terrain handler for destroying terrain
    private readonly TerrainHandler _terrain;
    // Timer to track mining delay
    private float _mineTimer;

    public PickaxeFacade(AudioSource audioSource, TerrainHandler terrain)
    {
        
        _audioSource = audioSource;
        _terrain = terrain;
        _mineTimer = 0f;
    }
    // Updates pickaxe behavior each frame
    public void UpdatePickaxe(Transform pickaxeTransform, Transform player, float deltaTime, float rotationSpeed, float breakRadius, float mineDelay)
    {
        // Accumulate time for mining delay
        _mineTimer += deltaTime;
        // Update pickaxe position based on player
        UpdatePosition(pickaxeTransform, player);

        if (ShouldMine())
        {
            RotatePickaxe(pickaxeTransform, rotationSpeed);
            PlaySound();
            TryMine(pickaxeTransform, breakRadius, mineDelay);
        }
        else
        {
            StopSound();
        }
    }

    private void UpdatePosition(Transform pickaxeTransform, Transform player)
    {
        Vector3 pos = player.position + player.forward * 1f;
        pos.y = player.position.y + 0.1f;
        pos.z = -1f;
        pickaxeTransform.position = pos;
    }

    private bool ShouldMine()
    {
        return (Input.GetMouseButton(0) && !Input.GetKeyDown(KeyCode.F)) ||
               (!Input.GetMouseButton(0) && Input.GetKeyDown(KeyCode.F));
    }
    // Handles pickaxe rotation animation
    private void RotatePickaxe(Transform pickaxeTransform, float rotationSpeed)
    {
        pickaxeTransform.Rotate(0f, rotationSpeed * Time.deltaTime, 90f);
    }
    // Plays mining sound if not already playing
    private void PlaySound()
    {
        if (!_audioSource.isPlaying)
            _audioSource.Play();
    }
    // Stops mining sound if currently playing
    private void StopSound()
    {
        if (_audioSource.isPlaying)
            _audioSource.Stop();
    }
    // Attempts to mine terrain when delay timer allows
    private void TryMine(Transform pickaxeTransform, float breakRadius, float mineDelay)
    {
        // Check if mining delay has passed
        if (_mineTimer >= mineDelay)
        {
            _mineTimer = 0f;
            _terrain?.DestroyInRadius(pickaxeTransform.position, breakRadius);
        }
    }
}

