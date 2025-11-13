using UnityEngine;

public class PickaxeFacade
{
    private readonly AudioSource _audioSource;
    private readonly TerrainHandler _terrain;
    private float _mineTimer;

    public PickaxeFacade(AudioSource audioSource, TerrainHandler terrain)
    {
        _audioSource = audioSource;
        _terrain = terrain;
        _mineTimer = 0f;
    }

    public void UpdatePickaxe(Transform pickaxeTransform, Transform player, float deltaTime, float rotationSpeed, float breakRadius, float mineDelay)
    {
        _mineTimer += deltaTime;

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

    private void RotatePickaxe(Transform pickaxeTransform, float rotationSpeed)
    {
        pickaxeTransform.Rotate(0f, rotationSpeed * Time.deltaTime, 90f);
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

    private void TryMine(Transform pickaxeTransform, float breakRadius, float mineDelay)
    {
        if (_mineTimer >= mineDelay)
        {
            _mineTimer = 0f;
            _terrain?.DestroyInRadius(pickaxeTransform.position, breakRadius);
        }
    }
}

