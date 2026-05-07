using System;
using UnityEngine;

[Serializable]
public class InventoryItem
{
    public ItemData itemData;
    public int count;
    
    public InventoryItem(ItemData itemData, int count)
    {
        this.itemData = itemData;
        this.count = count;
    }
    
    public int AddCount(int amount)
    {
        if (itemData is CountableItemData countableItemData)
        {
            int maxAmount = countableItemData.MaxAmount;
            int newCount = Mathf.Min(count + amount, maxAmount);
            int addedAmount = newCount - count;
            count = newCount;
            return addedAmount; 
        }
        return 0;
    }
}
