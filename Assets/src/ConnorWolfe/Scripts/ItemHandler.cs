using System.Runtime.InteropServices;
using UnityEngine;

public class ItemHandler : MonoBehaviour
{
    // this script handles holding/delivering an item for the player to grab from the world //

    [SerializeField] private KeyCode _interactKey;
    [SerializeField] private GameObject _heldItem;
    [SerializeField] private float _pickUpCooldown = 1f;

    private float _cooldownTime = 0f;
    private bool _onCooldown = false;


    // constructor
    public ItemHandler(KeyCode interactKey, GameObject heldItem, float pickUpCooldown)
    {
        _interactKey = interactKey;
        _heldItem = heldItem;
        _cooldownTime = pickUpCooldown;
    }

    // built in / Unity functions //
    private void Awake()
    {
        if (_interactKey == KeyCode.None)
            _interactKey = KeyCode.E;

        if (_heldItem)
            _heldItem.SetActive(false);
        else
            this.gameObject.SetActive(false);

        UpdateSprite();
    }

    private void Update()
    {
        if (_onCooldown)
        {
            if (_cooldownTime < _pickUpCooldown)
            {
                _cooldownTime += Time.deltaTime;
            }
            else
            {
                _cooldownTime = 0f;
                _onCooldown= false;
            }
        }
    }

    // swap item / give item with/to player
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && Input.GetKey(_interactKey) && !_onCooldown)
        {
            _onCooldown = true;
            // without singleton
            //            _heldItem = collision.GetComponent<PlayerController>().PickUp(_heldItem);
            // with singleton
            _heldItem = PlayerController.Instance.PickUp(_heldItem);
            if (!_heldItem)
                Destroy(this.gameObject);
            else
            {
                this.transform.position = collision.transform.position;
                UpdateSprite();
                Debug.Log($"INFORMATION: {this.name} is now holding {_heldItem.name}");
            }
        }        
    }

    // public functions //
    public void SetInteractKey(KeyCode key) { _interactKey = key; }
    public void SetHeldItem(GameObject item) { _heldItem = item; }
    public void SetPickupCooldown(float cooldown) { _pickUpCooldown = cooldown; }

    public void UpdateSprite()
    {
        if (!_heldItem || !_heldItem.GetComponent<SpriteRenderer>().sprite)
            return;
        SpriteRenderer itemRen = _heldItem.GetComponent<SpriteRenderer>();


            SpriteRenderer spriteRen = this.GetComponent<SpriteRenderer>();
        if (!spriteRen)
        {
            this.gameObject.AddComponent<SpriteRenderer>();
            spriteRen = this.GetComponent<SpriteRenderer>();
        }

        // copy sprite and details
        spriteRen.sprite = itemRen.sprite;
        spriteRen.size = itemRen.size;
        spriteRen.color = itemRen.color;
        spriteRen.flipX = itemRen.flipX;
        spriteRen.flipY = itemRen.flipY;
        if (itemRen.material)
            spriteRen.material = itemRen.material;

    }

}
