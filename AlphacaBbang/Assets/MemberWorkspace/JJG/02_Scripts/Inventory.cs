using System;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField] private int capacity;
    [SerializeField] private Item[] _items;
    
    private InventoryUI _inventoryUI;

    private void Awake()
    {
        _inventoryUI = GetComponent<InventoryUI>();
        _items = new Item[capacity];
    }

    public bool AddItem(ItemData itemData, int amount = 1)
    {
        
        
        return amount <= 0;
    }

    public bool UseItem(int index, GameObject user)
    {
        return true;
    }
}
