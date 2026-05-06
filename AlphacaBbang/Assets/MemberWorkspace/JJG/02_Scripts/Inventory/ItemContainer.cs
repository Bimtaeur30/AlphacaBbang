using System;
using System.Collections.Generic;
using MemberWorkspace.JJG._02_Scripts;
using MemberWorkspace.JJG._02_Scripts.Item;
using UnityEngine;

public class ItemContainer : MonoBehaviour, IItemContainer
{
    [SerializeField] protected ContainerType containerType;
    [SerializeField] protected int slotCount = 20;
    [SerializeField] protected List<ItemSlot> slots = new();

    public int SlotCount => slots.Count;
    public ContainerType ContainerType => containerType;

    public event Action OnContainerChanged;

    protected virtual void Awake()
    {
        InitializeSlots();
    }

    protected void InitializeSlots()
    {
        if (slots.Count == slotCount)
            return;

        slots.Clear();

        for (int i = 0; i < slotCount; i++)
        {
            slots.Add(new ItemSlot());
        }
    }

    public ItemSlot GetSlot(int index)
    {
        if (index < 0 || index >= slots.Count)
            return null;

        return slots[index];
    }

    public virtual bool CanPlaceItem(int index, ItemData itemData)
    {
        if (index < 0 || index >= slots.Count)
            return false;

        return true;
    }

    public virtual bool SetSlot(int index, ItemData itemData, int amount)
    {
        if (!CanPlaceItem(index, itemData))
            return false;

        if (amount <= 0)
            return false;

        ItemSlot slot = slots[index];
        slot.ItemData = itemData;
        slot.Amount = amount;

        NotifyContainerChanged();
        return true;
    }

    public virtual bool ClearSlot(int index)
    {
        if (index < 0 || index >= slots.Count)
            return false;

        slots[index].Clear();

        NotifyContainerChanged();
        return true;
    }

    public virtual bool AddItem(ItemData itemData, int amount = 1)
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
            for (int i = 0; i < slots.Count; i++)
            {
                ItemSlot slot = slots[i];

                if (slot.IsEmpty)
                    continue;

                if (slot.ItemData != itemData)
                    continue;

                if (slot.Amount >= countableItemData.MaxAmount)
                    continue;

                int canAdd = countableItemData.MaxAmount - slot.Amount;
                int addAmount = Mathf.Min(canAdd, amount);

                slot.Amount += addAmount;
                amount -= addAmount;

                if (amount <= 0)
                {
                    NotifyContainerChanged();
                    return true;
                }
            }

            for (int i = 0; i < slots.Count; i++)
            {
                ItemSlot slot = slots[i];

                if (!slot.IsEmpty)
                    continue;

                if (!CanPlaceItem(i, itemData))
                    continue;

                int addAmount = Mathf.Min(countableItemData.MaxAmount, amount);
                slot.ItemData = itemData;
                slot.Amount = addAmount;
                amount -= addAmount;

                if (amount <= 0)
                {
                    NotifyContainerChanged();
                    return true;
                }
            }

            Debug.LogWarning("Container is full");
            NotifyContainerChanged();
            return false;
        }
        else
        {
            while (amount > 0)
            {
                int emptyIndex = FindEmptySlot(itemData);

                if (emptyIndex < 0)
                {
                    Debug.LogWarning("Container is full");
                    NotifyContainerChanged();
                    return false;
                }

                slots[emptyIndex].ItemData = itemData;
                slots[emptyIndex].Amount = 1;
                amount--;
            }

            NotifyContainerChanged();
            return true;
        }
    }

    public virtual bool RemoveAmount(int index, int amount = 1)
    {
        if (index < 0 || index >= slots.Count)
            return false;

        if (amount <= 0)
            return false;

        ItemSlot slot = slots[index];

        if (slot.IsEmpty)
            return false;

        if (slot.Amount < amount)
            return false;

        slot.Amount -= amount;

        if (slot.Amount <= 0)
            slot.Clear();

        NotifyContainerChanged();
        return true;
    }

    public virtual bool UseItem(int index, GameObject user)
    {
        if (index < 0 || index >= slots.Count)
        {
            Debug.LogWarning("index out of range");
            return false;
        }

        ItemSlot slot = slots[index];

        if (slot == null || slot.IsEmpty)
        {
            Debug.LogWarning("slot is empty");
            return false;
        }

        ItemData itemData = slot.ItemData;

        if (itemData == null)
        {
            Debug.LogWarning("itemData is null");
            return false;
        }

        if (itemData is CountableItemData)
            RemoveAmount(index, 1);
        else
            ClearSlot(index);

        NotifyContainerChanged();
        return true;
    }

    public virtual bool MoveItemTo(int fromIndex, ItemContainer targetContainer, int toIndex)
    {
        if (targetContainer == null)
            return false;

        if (fromIndex < 0 || fromIndex >= slots.Count)
            return false;

        ItemSlot fromSlot = slots[fromIndex];
        if (fromSlot == null || fromSlot.IsEmpty)
            return false;

        ItemSlot toSlot = targetContainer.GetSlot(toIndex);
        if (toSlot == null)
            return false;

        ItemData movingItemData = fromSlot.ItemData;
        int movingAmount = fromSlot.Amount;

        if (!targetContainer.CanPlaceItem(toIndex, movingItemData))
            return false;

        if (toSlot.IsEmpty)
        {
            toSlot.ItemData = movingItemData;
            toSlot.Amount = movingAmount;

            fromSlot.Clear();

            targetContainer.NotifyContainerChanged();
            NotifyContainerChanged();
            return true;
        }

        if (toSlot.ItemData == movingItemData && movingItemData is CountableItemData countableItemData)
        {
            int max = countableItemData.MaxAmount;
            int current = toSlot.Amount;

            if (current >= max)
                return false;

            int canMove = Mathf.Min(max - current, movingAmount);

            toSlot.Amount += canMove;
            fromSlot.Amount -= canMove;

            if (fromSlot.Amount <= 0)
                fromSlot.Clear();

            targetContainer.NotifyContainerChanged();
            NotifyContainerChanged();
            return true;
        }

        if (!CanPlaceItem(fromIndex, toSlot.ItemData))
            return false;

        ItemData tempData = toSlot.ItemData;
        int tempAmount = toSlot.Amount;

        toSlot.ItemData = movingItemData;
        toSlot.Amount = movingAmount;

        fromSlot.ItemData = tempData;
        fromSlot.Amount = tempAmount;

        targetContainer.NotifyContainerChanged();
        NotifyContainerChanged();
        return true;
    }

    protected int FindEmptySlot(ItemData itemData)
    {
        for (int i = 0; i < slots.Count; i++)
        {
            if (!slots[i].IsEmpty)
                continue;

            if (!CanPlaceItem(i, itemData))
                continue;

            return i;
        }

        return -1;
    }
    
    public int GetItemCount(ItemData itemData)
    {
        if (itemData == null)
            return 0;

        int totalCount = 0;

        for (int i = 0; i < slots.Count; i++)
        {
            ItemSlot slot = slots[i];

            if (slot == null || slot.IsEmpty || slot.ItemData != itemData)
                continue;

            totalCount += slot.Amount;
        }

        return totalCount;
    }

    protected void NotifyContainerChanged()
    {
        OnContainerChanged?.Invoke();
    }
}
