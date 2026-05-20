using Assets.MemberWorkspace.HJH._02_Scripts.Grenade;
using MemberWorkspace.JJG._02_Scripts;
using MemberWorkspace.JJG._02_Scripts.Item.Data;
using UnityEngine;
using UnityEngine.InputSystem;

public class QuickSlotHotkeyHandler : MonoBehaviour
{
    [SerializeField] private QuickSlotContainer quickSlotContainer;
    [SerializeField] private WeaponHolder weaponHolder;
    [SerializeField] private GameObject itemUser;
    [SerializeField] private GrenadeFirePos throwingWeapon;
    [SerializeField] private LayerMask groundLayerMask;
    [SerializeField] private AgentAttack agentAttack;

    private IThrowingWeapon _throwingController;
    private bool _isAiming;

    private static readonly Key[] WeaponKeys = { Key.Digit1, Key.Digit2, Key.Digit3 };
    private static readonly Key[] ItemKeys = { Key.Digit4, Key.Digit5, Key.Digit6, Key.Digit7 };

    private int _currentThrowingSlotIndex = -1;
    private void Awake()
    {
        _throwingController = throwingWeapon as IThrowingWeapon;
        if (_throwingController == null)
            Debug.LogError("[QuickSlotHotkeyHandler] throwingWeapon이 IThrowingWeapon을 구현하지 않습니다.");
    }

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

    private void Update()
    {
        HandleWeaponKeys();
        HandleItemKeys();
        HandleThrowingInput();
    }


    private void OnThrowingItemChanged(ThrowingItemData throwingData)
    {
        _isAiming = throwingData != null;
        _throwingController?.SetAim(_isAiming);

        if (throwingData == null)
            _currentThrowingSlotIndex = -1;
    }

    private void HandleWeaponKeys()
    {
        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        for (int i = 0; i < WeaponKeys.Length; i++)
        {
            if (!keyboard[WeaponKeys[i]].wasPressedThisFrame) continue;

            weaponHolder.UnequipThrowingItem();

            ItemSlot slot = quickSlotContainer.GetSlot(i);
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
                    weaponHolder.EquipWeapon(i, weaponData);
                }
                else if (!string.IsNullOrEmpty(weaponData.MeleeWeaponId))
                {
                    weaponHolder.Unequip();
                    MeleeWeaponBase meleeWeapon = weaponHolder.FindMeleeWeapon(weaponData.MeleeWeaponId);
                    weaponHolder.EquipMeleeWeapon(i, weaponData, meleeWeapon);
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
        if (_throwingController == null) return;
        if (!_isAiming) return;

        var mouse = Mouse.current;
        if (mouse == null) return;

        if (mouse.leftButton.isPressed)
        {
            Vector3? targetPos = GetMouseWorldPosition();
            if (targetPos.HasValue)
                _throwingController.SetTarget(targetPos.Value);
        }
        else
        {
            _throwingController.SetAim(false);
        }

        if (mouse.rightButton.wasPressedThisFrame)
        {
            quickSlotContainer.UseItem(_currentThrowingSlotIndex, itemUser);
        }
    }

    private Vector3? GetMouseWorldPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, groundLayerMask))
            return hit.point;
        return null;
    }
}