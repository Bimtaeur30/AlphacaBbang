using System.Collections.Generic;
using MemberWorkspace.JJG._02_Scripts.Item;
using UnityEngine;

namespace MemberWorkspace.JJG._02_Scripts
{
    public class QuickSlotContainer : InventoryContainer
    {
        [SerializeField] private ContainerType containerType;
        [SerializeField] private int slotCount = 20;
        [SerializeField] private List<ItemSlot> slots = new();

        public int SlotCount => slots.Count;

        private void Awake()
        {
            InitializeSlots();
        }

        private void InitializeSlots()
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

            ItemSlot slot = slots[index];
            slot.ItemData = itemData;
            slot.Amount = amount;
            return true;
        }

        public virtual bool ClearSlot(int index)
        {
            if (index < 0 || index >= slots.Count)
                return false;

            slots[index].ItemData = null;
            slots[index].Amount = 0;
            return true;
        }
    }
}