using System;
using JJH._02_Scripts_Systems.EventSystems;
using MemberWorkspace.JJG._02_Scripts;
using MemberWorkspace.JJG._02_Scripts.Item.Data;
using UnityEngine;

public class WeaponHolder : MonoBehaviour
{
    [SerializeField] private EventChannelSO gunChannel;
    [SerializeField] private QuickSlotContainer quickSlotContainer;
    [SerializeField] private EquipmentContainer equipmentContainer;
    [SerializeField] private MeleeWeaponBase[] meleeWeapons;
    public WeaponItemData CurrentWeapon { get; private set; }
    public int CurrentSlotIndex { get; private set; } = -1;

    public ThrowingItemData CurrentThrowingItem { get; private set; }
    public int CurrentThrowingSlotIndex { get; private set; } = -1;
    public event Action OnWeaponRemoved;

    public event Action<WeaponItemData> OnWeaponChanged;
    public event Action<ThrowingItemData> OnThrowingItemChanged;
    public event Action<MeleeWeaponBase> OnMeleeWeaponChanged;

    private readonly WeaponItemData[] _slotCache = new WeaponItemData[2];
    private AgentAttack _agentAttack;

    private void Awake()
    {
        _agentAttack = GetComponentInChildren<AgentAttack>();
    }

    private void OnEnable()
    {
        if (quickSlotContainer != null)
            quickSlotContainer.OnContainerChanged += OnQuickSlotChanged;
    }

    private void OnDisable()
    {
        if (quickSlotContainer != null)
            quickSlotContainer.OnContainerChanged -= OnQuickSlotChanged;
    }

    public MeleeWeaponBase FindMeleeWeapon(string id)
    {
        foreach (var weapon in meleeWeapons)
            if (weapon.name == id) return weapon;
        Debug.LogWarning($"[WeaponHolder] 근접무기 못찾음: {id}");
        return null;
    }

    public void EquipMeleeWeapon(int slotIndex, WeaponItemData weaponData, MeleeWeaponBase meleeWeapon)
    {
        if (CurrentSlotIndex == slotIndex && CurrentWeapon == weaponData)
        {
            UnequipMeleeWeapon();
            return;
        }
        CurrentSlotIndex = slotIndex;
        CurrentWeapon = weaponData;
        OnMeleeWeaponChanged?.Invoke(meleeWeapon);
        Debug.Log($"[WeaponHolder] 근접무기 장착: {weaponData?.ItemName ?? "없음"} (슬롯 {slotIndex})");
    }

    public void UnequipMeleeWeapon()
    {
        CurrentSlotIndex = -1;
        CurrentWeapon = null;
        OnMeleeWeaponChanged?.Invoke(null);
        OnWeaponRemoved?.Invoke();
        Debug.Log("[WeaponHolder] 근접무기 해제");
    }

    private void OnQuickSlotChanged()
    {
        for (int i = 0; i < 2; i++)
        {
            ItemSlot slot = quickSlotContainer.GetSlot(i);
            WeaponItemData weapon = slot?.ItemData as WeaponItemData;

            if (weapon == _slotCache[i]) continue; // ✅ 변경 없으면 완전히 스킵
            _slotCache[i] = weapon;

            WeaponSlotIndex slotIndex = i == 0 ? WeaponSlotIndex.First : WeaponSlotIndex.Second;

            if (weapon != null)
            {
                // ✅ WeaponSlotEquipEvent만 발생, WeaponEquipEvent는 여기서 발생 안 함
                gunChannel.RaiseEvent(GunEvents.WeaponSlotEquipEvent.Init(weapon.Gun, slotIndex));
                equipmentContainer?.TryEquipWeapon(weapon, i);
            }
            else
            {
                gunChannel.RaiseEvent(GunEvents.WeaponSlotEquipEvent.Init(null, slotIndex));
                equipmentContainer?.UnequipWeapon(i);
            }
        }
    }

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

        if (slotIndex == 0)
        {
            gunChannel.RaiseEvent(GunEvents.WeaponSlotEquipEvent_UI.Init(null, WeaponSlotIndex.Second, false));
            gunChannel.RaiseEvent(GunEvents.WeaponEquipEvent.Init(WeaponSlotIndex.First));
            gunChannel.RaiseEvent(GunEvents.WeaponSlotEquipEvent_UI.Init(null, WeaponSlotIndex.First, true));
        }
        else if (slotIndex == 1)
        {
            gunChannel.RaiseEvent(GunEvents.WeaponSlotEquipEvent_UI.Init(null, WeaponSlotIndex.First, false));
            gunChannel.RaiseEvent(GunEvents.WeaponEquipEvent.Init(WeaponSlotIndex.Second));
            gunChannel.RaiseEvent(GunEvents.WeaponSlotEquipEvent_UI.Init(null, WeaponSlotIndex.Second, true));
        }
    }

    public void Unequip()
    {
        // 해제 전에 현재 슬롯 저장
        int prevSlotIndex = CurrentSlotIndex;

        CurrentSlotIndex = -1;
        CurrentWeapon = null;
        OnWeaponChanged?.Invoke(null);
        OnWeaponRemoved?.Invoke();

        // 해제 이벤트 발송
        if (prevSlotIndex == 0)
            gunChannel.RaiseEvent(GunEvents.WeaponSlotEquipEvent_UI.Init(null, WeaponSlotIndex.First, false));
        else if (prevSlotIndex == 1)
            gunChannel.RaiseEvent(GunEvents.WeaponSlotEquipEvent_UI.Init(null, WeaponSlotIndex.Second, false));

        Debug.Log("[WeaponHolder] 무기 해제");
    }

    public void EquipThrowingItem(int slotIndex, ThrowingItemData throwingData)
    {
        if (CurrentThrowingSlotIndex == slotIndex && CurrentThrowingItem == throwingData)
        {
            UnequipThrowingItem();
            return;
        }

        Unequip();

        CurrentThrowingSlotIndex = slotIndex;
        CurrentThrowingItem = throwingData;
        OnThrowingItemChanged?.Invoke(CurrentThrowingItem);

        Debug.Log($"[WeaponHolder] 투척류 장착: {throwingData?.ItemName ?? "없음"} (슬롯 {slotIndex})");
    }

    public void UnequipThrowingItem()
    {
        CurrentThrowingSlotIndex = -1;
        CurrentThrowingItem = null;
        OnThrowingItemChanged?.Invoke(null);
        OnWeaponRemoved?.Invoke();
        Debug.Log("[WeaponHolder] 투척류 해제");
    }
}