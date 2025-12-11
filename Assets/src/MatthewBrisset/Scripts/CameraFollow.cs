using UnityEngine;

// Pixel-perfect camera follow
public class CameraFollow : MonoBehaviour
{
    [SerializeField]
    private int _pixelsPerUnit = 8;

    private Transform _player;

    private void Start()
    {
        transform.parent = null;
    }

    void Update()
    {
        // Keep searching for camera if not in scene yet
        if (_player == null)
            _player = GameObject.FindGameObjectWithTag("Player").transform;
        else
        {
            float roundedX = Mathf.Round(_player.transform.position.x * _pixelsPerUnit) / _pixelsPerUnit;
            float roundedY = Mathf.Round(_player.transform.position.y * _pixelsPerUnit) / _pixelsPerUnit;

            Vector3 newPos = new Vector3(roundedX, roundedY, -10);
            transform.position = newPos;
        }
    }
}
