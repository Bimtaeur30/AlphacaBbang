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
    [SerializeField] private Transform grenadeHoldParent; // 방법 2: 인스펙터에서 직접 연결

    private static readonly Key[] WeaponKeys = { Key.Digit1, Key.Digit2, Key.Digit3 };
    private static readonly Key[] ItemKeys = { Key.Digit4, Key.Digit5, Key.Digit6, Key.Digit7 };

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
        // 기존 수류탄 정리
        if (_currentGrenade != null)
        {
            _currentGrenade.OnFired -= HandleOnFired;
            if (_currentGrenade != null)
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

        // 본체 Instantiate → grenadeHoldParent에 배치
        GrenadeBehavior grenade = Instantiate(
            throwingData.GrenadeBehaviorPrefab,
            grenadeHoldParent.position,
            grenadeHoldParent.rotation,
            grenadeHoldParent
        );

        grenade.Setup(throwingData.Grenade);
        Debug.Log($"[QuickSlotHotkeyHandler] 수류탄 생성: {grenade.name} at {grenade.transform.position}");
        _currentGrenade = grenade;
        Debug.Log($"[QuickSlotHotkeyHandler] 수류탄 설정 완료: {grenade.name} with GrenadeSO {throwingData.Grenade.name}");
        _currentGrenade.OnFired += HandleOnFired;
    }

    private void HandleOnFired()
    {
        bool success = quickSlotContainer.UseItem(_currentThrowingSlotIndex, itemUser);

        if (!success)
        {
            weaponHolder.UnequipThrowingItem();
            return;
        }

        ItemSlot slot = quickSlotContainer.GetSlot(_currentThrowingSlotIndex);
        if (slot == null || slot.IsEmpty)
            weaponHolder.UnequipThrowingItem();
    }

    private void HandleThrowingInput()
    {
        Debug.Log("%%%%  = 1");
        if (_currentGrenade == null) return;
        Debug.Log("%%%%  = 2");
        var mouse = Mouse.current;
        if (mouse == null) return;
        Debug.Log("%%%%  = 3");
        if (mouse.rightButton.isPressed)
        {
            Debug.Log("%%%%  = 4");
            if (!_currentGrenade.IsAiming)
                _currentGrenade.SetAim(true);
            Debug.Log("%%%%  = 5");
            Vector3? targetPos = GetMouseWorldPosition();
            if (targetPos.HasValue)
            {
                Debug.Log($"%%%%  = 6, Target Position: {targetPos.Value}");
                _currentGrenade.SetTarget(targetPos.Value);
            }
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

            if (slot.ItemData is ThrowingItemData throwingData)
            {
                weaponHolder.Unequip();
                weaponHolder.UnequipMeleeWeapon();
                weaponHolder.EquipThrowingItem(i, throwingData);
                SetThrowingSlotIndex(i);
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
                SetThrowingSlotIndex(slotIndex);
                return;
            }

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