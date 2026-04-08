using System;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField] private int capacity;
    [SerializeField] private List<InventoryItem> items;
    public IReadOnlyList<InventoryItem> Items => items;
    public event Action OnInventoryChanged;
    
    private InventoryUI _inventoryUI;

    private void Awake()
    {
        _inventoryUI = GetComponent<InventoryUI>();
    }
    
    public bool AddItem(ItemData itemData, int amount = 1)
    {
        if (itemData == null)
        {
            Debug.LogWarning("itemData is null");
            return false;
        }

        if (amount <= 0)
        {
            Debug.LogWarning("amount must be more than 0");
            return false;
        }

        if (itemData is CountableItemData countableItemData)
        {
            for (int i = 0; i < items.Count; i++)
            {
                InventoryItem inventoryItem = items[i];

                if (inventoryItem.itemData == itemData)
                {
                    int added = inventoryItem.AddCount(amount);
                    amount -= added;

                    if (amount <= 0)
                        return true;
                }
            }

            while (amount > 0)
            {
                if (items.Count >= capacity)
                {
                    Debug.LogWarning("Inventory is full");
                    return false;
                }

                int addCount = Mathf.Min(amount, countableItemData.MaxAmount);
                items.Add(new InventoryItem(itemData, addCount));
                amount -= addCount;
            }

            OnInventoryChanged?.Invoke();
            return true;
        }
        else
        {
            while (amount > 0)
            {
                if (items.Count >= capacity)
                {
                    Debug.LogWarning("Inventory is full");
                    return false;
                }

                items.Add(new InventoryItem(itemData, 1));
                amount--;
            }

            OnInventoryChanged?.Invoke();
            return true;
        }
    }

    public bool UseItem(int index, GameObject user)
    {
        if (index < 0 || index >= items.Count)
        {
            Debug.LogWarning("index out of range");
            return false;
        }
        
        InventoryItem inventoryItem = items[index];
        ItemData itemData = inventoryItem.itemData;
        
        if (itemData == null)
        {
            Debug.LogWarning("itemData is null");
            return false;
        }

        OnInventoryChanged?.Invoke();
        return true;
    }
}
