using System.Collections.Generic;
using UnityEngine;

public class EquipmentContainer : MonoBehaviour
{
    [SerializeField] private List<EquipmentSlot> equipmentSlots = new();

    public int SlotCount => equipmentSlots.Count;

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
}
