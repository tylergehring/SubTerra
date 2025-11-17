//Jack Kroll
using UnityEngine;

public class GoldPickAxe : UtilityTool
{
    //I use the facade pattern for the pickaxes 
    //varible defnitions
    private PickaxeFacade _facade;
    private Transform _player;
    private AudioSource _audioSource;
    private TerrainHandler _terrain;
    //speed of the pickax movment when being used 
    [SerializeField] private float _rotationSpeed = 50f;
    //The varible that defines the radius that will brake in the destry chunk function
    [SerializeField] private float _breakRadius = 3f;
    //the varible that defins the time that it takes before the destry chunk function can be used.
    [SerializeField] private float _mineDelay = 0.8f;

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        _terrain = FindFirstObjectByType<TerrainHandler>();
        //_player = GameObject.FindWithTag("Player").transform; //old way to get player position 
        //NEW WAY TO GET PLAYER POSITION. 
        _player = PlayerController.Instance.transform;
        // uses facade pattern to reduce the redundent code with helps with errors and keeps things consistent
        _facade = new PickaxeFacade(_audioSource, _terrain);
    }

    private void Update()
    {
        _facade.UpdatePickaxe(transform, _player, Time.deltaTime, _rotationSpeed, _breakRadius, _mineDelay);
    }
}
//old code that i now mustly use a pattern for. 
/*
using UnityEngine;

public class GoldPickAxe : UtilityTool
{
    private float _mineTimer;
    private AudioSource _audioSource;
    private float _breakRadius = 3f; // Radius to destroy chunks
    private TerrainHandler _terrain;
    private Transform player;
    private float _rotationSpeed = 50f; // degrees per second


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
            if (_mineTimer > 0.8f)
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
*/