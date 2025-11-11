using UnityEngine;

public class PickaxeTool : UtilityTool
{
    private float _mineTimer;
    private AudioSource _audioSource;
    private float _breakRadius = 1.6f; // Radius to destroy chunks

    private TerrainHandler _terrain;

    private Transform player;
    private float _rotationSpeed = 40f; // degrees per second


    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();

        _terrain = FindFirstObjectByType<TerrainHandler>();

        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }
    }
   

void Update()
    {
        _mineTimer += Time.deltaTime;
        //cnages position 
        if (gameObject.activeSelf) // checks if the prefab (this GameObject) is active
        {
            transform.position = player.position + player.forward * 1f;



            // Keep Z position fixed at 1
            var pos = transform.position;
            pos.y = player.position.x + -0.5f;
            pos.y = player.position.y + 0.1f;
            pos.z = -1f;
            transform.position = pos;
           
            if ((Input.GetMouseButton(0) && !Input.GetKeyDown(KeyCode.F)) || ((!Input.GetMouseButton(0) && Input.GetKeyDown(KeyCode.F)))) // Left mouse button held down
            {
                // Rotate around the Y axis (you can change to X/Z as needed)
                transform.Rotate(0f, _rotationSpeed * Time.deltaTime, 90f);


                if (!_audioSource.isPlaying)
                {
                    _audioSource.Play();
                }


            }
            else
            {
                if (_audioSource.isPlaying)
                {
                    _audioSource.Stop();
                }
            }
        }





        if ((Input.GetMouseButton(0) && !Input.GetKeyDown(KeyCode.F)) || ((!Input.GetMouseButton(0) && Input.GetKeyDown(KeyCode.F)))) // Left mouse button held down
        {
            if (_mineTimer > 1f)
            {
                _mineTimer = 0;
                DestroyChunksAtPosition();
            }
        }

    }

    

    private void DestroyChunksAtPosition()
    {
        if (_terrain != null)
        {
            _terrain.DestroyInRadius(transform.position, _breakRadius);
        }
    }
}
