using UnityEngine;
using System.Collections;

public class Rasor : UtilityTool
{
    private AudioSource _audioSource;
    private Transform player;

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();

        // Find player if not assigned
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }
    }

    private void Update()
    {
        
        if (player != null && gameObject.activeSelf)
        {
            // Keep Rasor positioned relative to player
            Vector3 pos = transform.position;
            pos.x = player.position.x + 0.6f;
            pos.y = player.position.y + 0.1f;
            pos.z = -1f; // fixed Z position
            transform.position = pos;

            // Trigger the tool when left mouse button is pressed
            if ((Input.GetMouseButton(0) && !Input.GetKeyDown(KeyCode.F)) || ((!Input.GetMouseButton(0) && Input.GetKeyDown(KeyCode.F)))) // Left mouse button held down
            {
                UseTool();
            }
        }
    }

    // Dynamic binding: called when the tool is used
    public override void UseTool(GameObject target = null)
    {
        StartCoroutine(PlaySoundForTwoSeconds());
    }

    private IEnumerator PlaySoundForTwoSeconds()
    {
        if (_audioSource != null)
        {
            _audioSource.Play();
            yield return new WaitForSeconds(0.8f);
            _audioSource.Stop();
        }
    }
}