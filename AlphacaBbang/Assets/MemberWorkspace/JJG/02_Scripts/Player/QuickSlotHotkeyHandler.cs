using MemberWorkspace.JJG._02_Scripts;
using MemberWorkspace.JJG._02_Scripts.Item.Data;
using UnityEngine;
using UnityEngine.InputSystem;

public class QuickSlotHotkeyHandler : MonoBehaviour
{
    [SerializeField] private QuickSlotContainer quickSlotContainer;
    [SerializeField] private WeaponHolder weaponHolder;
    [SerializeField] private GameObject itemUser;
    [SerializeField] private AgentAttack agentAttack;
    [SerializeField] private PlayerGunHandleModule playerGunHandleModule;
    [SerializeField] private Transform grenadeHoldParent;

    // 슬롯 0: 무기, 슬롯 1: 수류탄
    private static readonly Key[] WeaponKeys = { Key.Digit1, Key.Digit2 };
    private static readonly Key[] ItemKeys = { Key.Digit3, Key.Digit4, Key.Digit5, Key.Digit6 };

    private int _currentThrowingSlotIndex = -1;
    private GrenadeBehavior _currentGrenade;

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

    private void OnDestroy()
    {
        if (_currentGrenade != null)
            _currentGrenade.OnFired -= HandleOnFired;
    }

    private void Update()
    {
        HandleWeaponKeys();
        HandleItemKeys();
        HandleThrowingInput();
    }

    private void OnThrowingItemChanged(ThrowingItemData throwingData)
    {
        if (_currentGrenade != null)
        {
            _currentGrenade.OnFired -= HandleOnFired;
            Destroy(_currentGrenade.gameObject);
            _currentGrenade = null;
        }

        if (throwingData == null)
        {
            _currentThrowingSlotIndex = -1;
            return;
        }

        if (throwingData.GrenadeBehaviorPrefab == null)
        {
            Debug.LogWarning("[QuickSlotHotkeyHandler] GrenadeBehaviorPrefab이 null입니다.");
            return;
        }

        GrenadeBehavior grenade = Instantiate(
            throwingData.GrenadeBehaviorPrefab,
            grenadeHoldParent.position,
            grenadeHoldParent.rotation,
            grenadeHoldParent
        );

        grenade.Setup(throwingData.Grenade);
        _currentGrenade = grenade;
        _currentGrenade.OnFired += HandleOnFired;
    }

    private void HandleOnFired()
    {
        int slotIndex = _currentThrowingSlotIndex;

        bool success = quickSlotContainer.UseItem(slotIndex, itemUser);

        if (!success)
        {
            weaponHolder.UnequipThrowingItem();
            return;
        }

        ItemSlot slot = quickSlotContainer.GetSlot(slotIndex);
        if (slot == null || slot.IsEmpty)
        {
            weaponHolder.UnequipThrowingItem();
        }
        else
        {
            weaponHolder.UnequipThrowingItem();
            if (slot.ItemData is ThrowingItemData throwingData)
            {
                weaponHolder.EquipThrowingItem(slotIndex, throwingData);
                SetThrowingSlotIndex(slotIndex);
            }
        }
    }

    private void HandleThrowingInput()
    {
        if (_currentGrenade == null) return;

        var mouse = Mouse.current;
        if (mouse == null) return;

        if (mouse.rightButton.isPressed)
        {
            if (!_currentGrenade.IsAiming)
                _currentGrenade.SetAim(true);

            Vector3? targetPos = GetMouseWorldPosition();
            if (targetPos.HasValue)
                _currentGrenade.SetTarget(targetPos.Value);
        }
        else
        {
            if (_currentGrenade.IsAiming)
                _currentGrenade.SetAim(false);
        }

        if (mouse.leftButton.wasPressedThisFrame && _currentGrenade.IsAiming)
        {
            _currentGrenade.StartFire(true);
        }
    }

    private void HandleWeaponKeys()
    {
        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        // 1번키 - 슬롯 0 (총/근접)
        if (keyboard[WeaponKeys[0]].wasPressedThisFrame)
        {
            weaponHolder.UnequipThrowingItem();

            ItemSlot slot = quickSlotContainer.GetSlot(0);
            if (slot == null || slot.IsEmpty)
            {
                weaponHolder.Unequip();
                weaponHolder.UnequipMeleeWeapon();
                return;
            }

            if (slot.ItemData is WeaponItemData weaponData)
            {
                if (weaponData.Gun != null)
                {
                    weaponHolder.UnequipMeleeWeapon();
                    weaponHolder.EquipWeapon(0, weaponData);
                }
                else if (!string.IsNullOrEmpty(weaponData.MeleeWeaponId))
                {
                    weaponHolder.Unequip();
                    MeleeWeaponBase meleeWeapon = weaponHolder.FindMeleeWeapon(weaponData.MeleeWeaponId);
                    weaponHolder.EquipMeleeWeapon(0, weaponData, meleeWeapon);
                }
            }
            return;
        }

        // 2번키 - 슬롯 1 (총/근접/투척)
        if (keyboard[WeaponKeys[1]].wasPressedThisFrame)
        {
            weaponHolder.UnequipThrowingItem();

            ItemSlot slot = quickSlotContainer.GetSlot(1);
            if (slot == null || slot.IsEmpty)
            {
                weaponHolder.Unequip();
                weaponHolder.UnequipMeleeWeapon();
                return;
            }

            if (slot.ItemData is WeaponItemData weaponData)
            {
                if (weaponData.Gun != null)
                {
                    weaponHolder.UnequipMeleeWeapon();
                    weaponHolder.EquipWeapon(1, weaponData);
                }
                else if (!string.IsNullOrEmpty(weaponData.MeleeWeaponId))
                {
                    weaponHolder.Unequip();
                    MeleeWeaponBase meleeWeapon = weaponHolder.FindMeleeWeapon(weaponData.MeleeWeaponId);
                    weaponHolder.EquipMeleeWeapon(1, weaponData, meleeWeapon);
                }
            }
            return;
        }
    }

    public void SetThrowingSlotIndex(int index)
    {
        _currentThrowingSlotIndex = index;
    }

    private void HandleItemKeys()
    {
        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        for (int i = 0; i < ItemKeys.Length; i++)
        {
            if (!keyboard[ItemKeys[i]].wasPressedThisFrame) continue;

            weaponHolder.UnequipMeleeWeapon();

            int slotIndex = 2 + i;
            ItemSlot slot = quickSlotContainer.GetSlot(slotIndex);

            quickSlotContainer.UseItem(slotIndex, itemUser);
            return;
        }
    }

    private Vector3? GetMouseWorldPosition()
    {
        if (_currentGrenade == null) return null;
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, _currentGrenade.layermask))
            return hit.point;
        return null;
    }
}