using MemberWorkspace.JJG._02_Scripts;
using UnityEngine;
using UnityEngine.InputSystem;

public class QuickSlotHotkeyHandler : MonoBehaviour
{
    [SerializeField] private QuickSlotContainer quickSlotContainer;
    [SerializeField] private WeaponHolder weaponHolder;
    [SerializeField] private GameObject itemUser;

    private static readonly Key[] WeaponKeys =
    {
        Key.Digit1, Key.Digit2, Key.Digit3
    };

    private static readonly Key[] ItemKeys =
    {
        Key.Digit4, Key.Digit5, Key.Digit6, Key.Digit7
    };

    private void Update()
    {
        HandleWeaponKeys();
        HandleItemKeys();
    }

    private void HandleWeaponKeys()
    {
        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        for (int i = 0; i < WeaponKeys.Length; i++)
        {
            if (!keyboard[WeaponKeys[i]].wasPressedThisFrame)
                continue;

            ItemSlot slot = quickSlotContainer.GetSlot(i);

            if (slot == null || slot.IsEmpty)
            {
                weaponHolder.Unequip();
                return;
            }

            if (slot.ItemData is WeaponItemData weaponData)
                weaponHolder.EquipWeapon(i, weaponData);

            return;
        }
    }

    private void HandleItemKeys()
    {
        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        for (int i = 0; i < ItemKeys.Length; i++)
        {
            if (!keyboard[ItemKeys[i]].wasPressedThisFrame)
                continue;

            int slotIndex = 3 + i;
            quickSlotContainer.UseItem(slotIndex, itemUser);
            return;
        }
    }
}