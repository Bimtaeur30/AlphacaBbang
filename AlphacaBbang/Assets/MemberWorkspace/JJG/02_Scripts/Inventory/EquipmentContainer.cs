using System;
using System.Collections.Generic;
using MemberWorkspace.JJG._02_Scripts.Item.Data;
using UnityEngine;

public class EquipmentContainer : ItemContainer
{
    [SerializeField] private List<EquipmentSlot> equipmentSlots = new();

    public int SlotCount => equipmentSlots.Count;

    public event Action OnEquipmentChanged;

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

    public bool CanEquip(int index, ItemData itemData)
    {
        if (itemData == null)
            return false;

        EquipmentSlot equipmentSlot = GetEquipmentSlot(index);
        if (equipmentSlot == null)
            return false;

        if (itemData is ArmorItemData)
            return equipmentSlot.allowedEquipType == EquipType.Helmet;

        return equipmentSlot.allowedEquipType == itemData.EquipType;
    }

    public bool Equip(int index, ItemData itemData)
    {
        if (!CanEquip(index, itemData))
            return false;

        equipmentSlots[index].slot.ItemData = itemData;
        equipmentSlots[index].slot.Amount = 1;
        return true;
    }

    public bool Unequip(int index)
    {
        EquipmentSlot equipmentSlot = GetEquipmentSlot(index);
        if (equipmentSlot == null)
            return false;

        equipmentSlot.slot.ItemData = null;
        equipmentSlot.slot.Amount = 0;
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
                OnEquipmentChanged?.Invoke();
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
                OnEquipmentChanged?.Invoke();
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
            OnEquipmentChanged?.Invoke();
            return true;
        }

        Debug.LogWarning("장착 가능한 빈 슬롯이 없습니다.");
        return false;
    }
}
