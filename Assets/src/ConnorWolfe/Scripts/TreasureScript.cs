using System.Runtime.InteropServices;
using UnityEngine;


// this script handles making the value of treasure for the game as well as assigning an opropriate sprite
public class TreasureScript : MonoBehaviour
{

    [SerializeField] private int value;    

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log($"Collided with {collision.name}");
//            PlayerController playerController = collision.GetComponent<PlayerController>();
            PlayerController.Instance.ChangeScore(value);
            Destroy(this);
            this.gameObject.SetActive(false); // if Destroy doesn't work
        }
    }
}
