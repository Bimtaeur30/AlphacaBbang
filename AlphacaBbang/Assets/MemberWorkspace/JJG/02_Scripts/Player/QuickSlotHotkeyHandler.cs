using MemberWorkspace.JJG._02_Scripts;
using MemberWorkspace.JJG._02_Scripts.Item.Data;
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

    private bool _isAiming;

    private void OnEnable()
    {
        if (weaponHolder != null)
            weaponHolder.OnThrowingItemChanged += OnThrowingItemChanged;
    }

    private void OnDisable()
    {
        if (weaponHolder != null)
            weaponHolder.OnThrowingItemChanged -= OnThrowingItemChanged;
    }

    private void OnThrowingItemChanged(ThrowingItemData throwingData)
    {
        if (throwingData != null)
        {
            _isAiming = true;
            Debug.Log($"[투척류] 조준");
        }
        else
        {
            _isAiming = false;
            Debug.Log("[투척류] 조준 해제");
        }
    }

    private void Update()
    {
        HandleWeaponKeys();
        HandleItemKeys();
        HandleThrowingInput();
    }

    private void HandleWeaponKeys()
    {
        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        for (int i = 0; i < WeaponKeys.Length; i++)
        {
            if (!keyboard[WeaponKeys[i]].wasPressedThisFrame)
                continue;

            weaponHolder.UnequipThrowingItem();

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
            ItemSlot slot = quickSlotContainer.GetSlot(slotIndex);

            if (slot != null && slot.ItemData is ThrowingItemData throwingData)
            {
                weaponHolder.EquipThrowingItem(slotIndex, throwingData);
                return;
            }

            quickSlotContainer.UseItem(slotIndex, itemUser);
            return;
        }
    }

    private void HandleThrowingInput()
    {
        if (!_isAiming) return;

        var mouse = Mouse.current;
        if (mouse == null) return;

        if (mouse.leftButton.wasPressedThisFrame)
        {
            ThrowingItemData throwingData = weaponHolder.CurrentThrowingItem;
            Debug.Log($"[투척류] 발사");
        }
    }
}