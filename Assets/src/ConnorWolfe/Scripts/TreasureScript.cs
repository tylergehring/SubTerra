using System.Runtime.InteropServices;
using UnityEngine;


// this script handles making the value of treasure for the game as well as assigning an opropriate sprite
public class TreasureScript : MonoBehaviour
{

    public int value;
    
    [SerializeField] private SpriteRenderer _renderer;
    [SerializeField] private Sprite _lowTier;
    [SerializeField] private Sprite _midTier;
    [SerializeField] private Sprite _highTier;

    /*
        void Start()
        {

        }
    */

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerController playerController = collision.GetComponent<PlayerController>();
            playerController.ChangeScore(value);
            Destroy(this);
        }
    }
}
