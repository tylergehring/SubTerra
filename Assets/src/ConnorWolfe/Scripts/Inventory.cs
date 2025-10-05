using UnityEngine;
using System.Collections.Generic;

public class Inventory
{
    protected List<GameObject> _inventory = new List<GameObject>();
    protected int _maxCapacity = 4;
    protected int _invenIndex = 0;

    // Class Constructor
    public Inventory(int capacity)
    {
        _invenIndex = 0;
        _maxCapacity = capacity > 0 ? capacity : 4;
        _inventory = new List<GameObject>();
        while (_inventory.Count < _maxCapacity)
            _AddItem(null);
    }

    // utility funtions, ie ADD/SET funcitons //
    // adds an item to the end of the list if the list is not full
    private bool _AddItem(GameObject newItem)
    {
        if (_inventory.Count >= _maxCapacity)
        {
            Debug.LogWarning($"WARNING: inventory is full, {newItem.name} was not added too inventory!");
            return false;
        }

        if (newItem)
        {
            newItem.SetActive(false);
            _inventory.Add(newItem);
            Debug.Log($"INFORMATION: {newItem.name} added successfully to inventory!");
            return true;
        }

        _inventory.Add(newItem);
        return true;
    }

    // this funciton is helpful for setting a certain item at an index or removing an item at an index (ie, use arguments (indexNumber, null))
    public GameObject SetItem(int index, GameObject newItem)
    {
        if (index >= _maxCapacity || index < 0)
        {
            Debug.LogWarning($"WARNING: tried to add {newItem.name} at slot {index}, but failed since max capacity is {_maxCapacity}!");
            return null;
        }

        // if inventory is not filled out to where the index is inserting, then fill inventory to index position
        while (_inventory.Count < index)
        {
            _AddItem(null);
        }

        GameObject temp = _inventory[index];
        _inventory[index] = newItem;
        return temp;

    }

    // Game Inventory functions, ie: tabbing through inventory//
    public void Tab()
    {
        if (_invenIndex < 0 || _invenIndex >= _maxCapacity)
        {
            _invenIndex = 0;
            if (_inventory[_invenIndex])
                _inventory[_invenIndex].SetActive(true);
            return;
        }
        if (_inventory[_invenIndex])
            _inventory[_invenIndex].SetActive(false);
        _invenIndex = (_invenIndex + 1) % _maxCapacity; // wrap around the inventory
        if (_inventory[_invenIndex])
            _inventory[_invenIndex].SetActive(true);
    }

}
