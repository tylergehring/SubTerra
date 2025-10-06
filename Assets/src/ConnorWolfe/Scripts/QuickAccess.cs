using System;
using UnityEngine;

public class QuickAccess : Inventory
{
    // QuickAccess is a subclass of Inventory that implements systems that are more specific to what the playerController needs
    //   * ie: QuickAccess is an extension of Inventory that adds some extra features

    // constructor that constructs this as an inventory with four slots
    public QuickAccess() : base(4) { }

    // quickly go to an inventory slot
    public void Tab(int slot)
    {
        if (slot < 0 || slot >= _maxCapacity)
        {
            Debug.LogWarning($"WARNING: tried to move to slot {slot}, but failed since max capacity is {_maxCapacity}!");
            return;
        }

        if (_invenIndex < 0 || _invenIndex >= _maxCapacity)
        {
            _invenIndex = slot;
            if (_inventory[_invenIndex])
                _inventory[_invenIndex].SetActive(true);
            return;
        }

        if (_inventory[_invenIndex])
            _inventory[_invenIndex].SetActive(false);
        _invenIndex = slot;
        if (_inventory[_invenIndex])
            _inventory[_invenIndex].SetActive(true);
    }

    // sets the item to the current slot
    public GameObject SetItem(GameObject newItem)
    {
        // safety check the _invenIndex is in a valid slot
        _invenIndex = _invenIndex < 0 || _invenIndex >= _maxCapacity ? 0 : _invenIndex;

       
        GameObject temp = _inventory[_invenIndex];
        _inventory[_invenIndex] = newItem;
        if (newItem)
            Debug.Log($"INFORMATION: Added {newItem.name} to inventory at slot {_invenIndex}");
        if (temp)
            temp.SetActive(false);
        if (newItem)
            newItem.SetActive(true);
        return temp;
    }

    // get the current open inventory item
    public GameObject GetItem()
    {
        _invenIndex = _invenIndex < 0 || _invenIndex >= _maxCapacity ? 0 : _invenIndex;

        return _inventory[_invenIndex];
    }

    // get how big the inventory is
    public int GetCapacity()
    {
        return _maxCapacity;
    }
}

