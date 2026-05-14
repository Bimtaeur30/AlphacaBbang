using System;
using UnityEngine;

public class EquipmentStatSystem : MonoBehaviour
{
    [SerializeField] private EquipmentContainer equipmentContainer;

    private void OnEnable()
    {
        if (equipmentContainer != null)
            equipmentContainer.OnEquipmentChanged += RecCalculateStats;
    }

    private void OnDisable()
    {
        if (equipmentContainer != null)
            equipmentContainer.OnEquipmentChanged -= RecCalculateStats;
    }
    
    private void RecCalculateStats()
    {
        int totalAttack = 0;
        int totalDefense = 0;

        for (int i = 0; i < equipmentContainer.SlotCount; i++)
        {
            // EquipmentSlotDefinition slot = equipmentContainer.GetSlot(i);
            // if (slot == null || slot.Slot.IsEmpty)
            //     continue;
            //
            // if (slot.Slot.Item.Data is EquippableItemDataSO equippable)
            // {
            //     totalAttack += equippable.AttackPower;
            //     totalDefense += equippable.DefensePower;
            // }
        }

        Debug.Log($"공격력: {totalAttack}, 방어력: {totalDefense}");
    }
}
