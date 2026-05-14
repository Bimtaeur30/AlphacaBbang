using System;
using UnityEngine;

public class WeaponHolder : MonoBehaviour
{
    public WeaponItemData CurrentWeapon { get; private set; }
    public int CurrentSlotIndex { get; private set; } = -1;

    public event Action<WeaponItemData> OnWeaponChanged;

    public void EquipWeapon(int slotIndex, WeaponItemData weaponData)
    {
        if (CurrentSlotIndex == slotIndex && CurrentWeapon == weaponData)
        {
            Unequip();
            return;
        }

        CurrentSlotIndex = slotIndex;
        CurrentWeapon = weaponData;
        OnWeaponChanged?.Invoke(CurrentWeapon);

        Debug.Log($"[WeaponHolder] 장착: {weaponData?.ItemName ?? "없음"} (슬롯 {slotIndex})");
    }

    public void Unequip()
    {
        CurrentSlotIndex = -1;
        CurrentWeapon = null;
        OnWeaponChanged?.Invoke(null);

        Debug.Log("[WeaponHolder] 무기 해제");
    }
}
