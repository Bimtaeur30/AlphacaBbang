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
    
    
}
