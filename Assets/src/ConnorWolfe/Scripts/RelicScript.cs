using UnityEngine;

public class RelicScript : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // without Sigleton pattern
            //            collision.gameObject.GetComponent<PlayerController>().Victory();
            // with singleton pattern
            PlayerController.Instance.Victory();
        }
    }
}
