using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework.Constraints;
using UnityEngine.UI;

/* This script handles displaying player inventory in the hotbar/UI
 
 */
public class InventoryHotBarScript : MonoBehaviour
{
    [SerializeField] private Color _unselectedColor;
    [SerializeField] private Color _selectedColor;
    [SerializeField] private List<GameObject> _slots = new List<GameObject>();
    [SerializeField] private List<GameObject> _items = new List<GameObject>();

    private void Start()
    {
        if (_unselectedColor == null)
        {
            _unselectedColor = Color.white;
        }
        _unselectedColor.a = 0.8f;


        if (_selectedColor == null)
        {
            _selectedColor = Color.gold;
        }
        _selectedColor.a = 0.9f;
    }

    // Public Functions //

    public void UpdateSlotItem(int itemIndex, Sprite sprite)
    {
        if (itemIndex < 0 || itemIndex >= _items.Capacity)
            return;
        
        Image image = _items[itemIndex].GetComponent<Image>();
        if (!image)
            return;



        if (!sprite)
        {
            Color color = image.color;
            color.a = 0f; // make it transparent
            image.color = color;
        } else
        {
            Color color = image.color;
            color.a = 1f; // make it visable
            image.color = color;
            image.sprite = sprite;
        }
    }

    public void UpdateSlotSelect(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex > _slots.Capacity)
            return;

        foreach (GameObject slot in _slots)
        {
            Image image = slot.GetComponent<Image>();
            if (!image)
            {
                Debug.LogWarning($"Slot \"{slot.name}\" is missing an image component!");
                slot.AddComponent<Image>();
            }

            Color color = image.color;
            color = _unselectedColor;
            image.color = color;
        }

        Image imageSelect = _slots[slotIndex].GetComponent<Image>();        
        Color colorSelect = imageSelect.color;
        colorSelect = _selectedColor;
        imageSelect.color = colorSelect;
    }


}
