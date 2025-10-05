using UnityEngine;

public class ItemHandler : MonoBehaviour
{
    // this script handles holding/delivering an item for the player to grab from the world //

    [SerializeField] private KeyCode _interactKey;
    [SerializeField] private GameObject _heldItem;

    // built in / Unity functions
    private void Awake()
    {
        if (_interactKey == KeyCode.None)
            _interactKey = KeyCode.E;

        if (_heldItem)
            _heldItem.SetActive(false);
        else
            this.gameObject.SetActive(false);
    }

    // Private functions
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && Input.GetKey(_interactKey))
        {
            //
        }        
    }

}
