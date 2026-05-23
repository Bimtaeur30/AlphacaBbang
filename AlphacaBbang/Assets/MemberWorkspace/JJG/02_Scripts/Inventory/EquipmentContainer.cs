using System;
using System.Collections.Generic;
using MemberWorkspace.JJG._02_Scripts.Item.Data;
using UnityEngine;

public class EquipmentContainer : ItemContainer
{
    [SerializeField] private List<EquipmentSlot> equipmentSlots = new();

    public int SlotCount => equipmentSlots.Count;

    public event Action OnEquipmentChanged;

    protected override void Awake()
    {
        base.Awake();
        SyncEquipmentSlots();
    }

    private void OnValidate()
    {
        SyncEquipmentSlots();
    }

    private void SyncEquipmentSlots()
    {
        if (equipmentSlots == null)
            equipmentSlots = new List<EquipmentSlot>();

        if (slots == null)
            slots = new List<ItemSlot>();

        slots.Clear();

        foreach (EquipmentSlot equipmentSlot in equipmentSlots)
        {
            if (equipmentSlot == null)
                continue;

            if (equipmentSlot.slot == null)
                equipmentSlot.slot = new ItemSlot();

            slots.Add(equipmentSlot.slot);
        }
    }

    public override ItemSlot GetSlot(int index)
    {
        if (index < 0 || index >= equipmentSlots.Count) return null;
        return equipmentSlots[index].slot;
    }

    public EquipmentSlot GetEquipmentSlot(int index)
    {
        if (index < 0 || index >= equipmentSlots.Count)
            return null;

        return equipmentSlots[index];
    }

    public override bool CanPlaceItem(int index, ItemData itemData)
    {
        if (itemData == null)
            return false;

        if (index < 0 || index >= equipmentSlots.Count)
            return false;

        // ArmorItemData만 들어올 수 있음
        return itemData is ArmorItemData;
    }

    public bool CanEquip(int index, ItemData itemData)
    {
        if (itemData == null)
            return false;

        EquipmentSlot equipmentSlot = GetEquipmentSlot(index);
        if (equipmentSlot == null)
            return false;

        return itemData is ArmorItemData;
    }

    public bool Equip(int index, ItemData itemData)
    {
        if (!CanEquip(index, itemData))
            return false;

        equipmentSlots[index].slot.ItemData = itemData;
        equipmentSlots[index].slot.Amount = 1;
        NotifyEquipmentChanged();
        return true;
    }

    public bool Unequip(int index)
    {
        EquipmentSlot equipmentSlot = GetEquipmentSlot(index);
        if (equipmentSlot == null)
            return false;

        equipmentSlot.slot.ItemData = null;
        equipmentSlot.slot.Amount = 0;
        NotifyEquipmentChanged();
        return true;
    }

    public bool TryEquipWeapon(WeaponItemData weaponData, int weaponSlotIndex)
    {
        int weaponCount = 0;
        for (int i = 0; i < equipmentSlots.Count; i++)
        {
            EquipType t = equipmentSlots[i].allowedEquipType;
            if (t != EquipType.MainWeapon && t != EquipType.SubWeapon) continue;

            if (weaponCount == weaponSlotIndex)
            {
                equipmentSlots[i].slot.ItemData = weaponData;
                equipmentSlots[i].slot.Amount = 1;
                NotifyEquipmentChanged();
                return true;
            }
            weaponCount++;
        }
        return false;
    }

    public bool UnequipWeapon(int weaponSlotIndex)
    {
        int weaponCount = 0;
        for (int i = 0; i < equipmentSlots.Count; i++)
        {
            EquipType t = equipmentSlots[i].allowedEquipType;
            if (t != EquipType.MainWeapon && t != EquipType.SubWeapon) continue;

            if (weaponCount == weaponSlotIndex)
            {
                equipmentSlots[i].slot.Clear();
                NotifyEquipmentChanged();
                return true;
            }
            weaponCount++;
        }
        return false;
    }

    public bool TryEquipFromContainer(ItemContainer sourceContainer, int sourceIndex)
    {
        ItemSlot sourceSlot = sourceContainer.GetSlot(sourceIndex);
        if (sourceSlot == null || sourceSlot.IsEmpty)
            return false;

        ItemData itemData = sourceSlot.ItemData;

        for (int i = 0; i < equipmentSlots.Count; i++)
        {
            if (!CanEquip(i, itemData))
                continue;

            if (!equipmentSlots[i].slot.IsEmpty)
                continue;

            Equip(i, itemData);
            sourceContainer.ClearSlot(sourceIndex);
            NotifyEquipmentChanged();
            return true;
        }

        Debug.LogWarning("장착 가능한 빈 슬롯이 없습니다.");
        return false;
    }

    public void NotifyEquipmentChanged()
    {
        NotifyContainerChanged();
        OnEquipmentChanged?.Invoke();
    }
}
