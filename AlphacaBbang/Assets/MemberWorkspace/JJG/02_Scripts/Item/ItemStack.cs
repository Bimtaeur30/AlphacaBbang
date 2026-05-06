using UnityEngine;

public class ItemStack
{
    public ItemData ItemData { get; private set; }
    public int Count { get; private set; }

    public bool IsCountable => ItemData is CountableItemData;

    public int MaxAmount
    {
        get
        {
            if (ItemData is CountableItemData countableData)
                return countableData.MaxAmount;

            return 1;
        }
    }

    public ItemStack(ItemData itemData, int count)
    {
        ItemData = itemData;
        Count = Mathf.Clamp(count, 1, MaxAmount);
    }

    public bool CanStackWith(ItemStack other)
    {
        if (other == null) return false;
        if (ItemData != other.ItemData) return false;
        if (!IsCountable) return false;

        return Count < MaxAmount;
    }

    public int AddCount(int amount)
    {
        if (!IsCountable)
            return amount;

        int space = MaxAmount - Count;
        int addAmount = Mathf.Min(space, amount);

        Count += addAmount;

        return amount - addAmount;
    }

    public void RemoveCount(int amount)
    {
        Count = Mathf.Max(0, Count - amount);
    }
}