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
        Debug.Log("[WeaponHolder] 근접무기 해제");
    }
    private void OnQuickSlotChanged()
    {
        for (int i = 0; i < 2; i++)
        {
            ItemSlot slot = quickSlotContainer.GetSlot(i);
            WeaponItemData weapon = slot?.ItemData as WeaponItemData;

            if (weapon == _slotCache[i]) continue;
            _slotCache[i] = weapon;

            WeaponSlotIndex slotIndex = i == 0 ? WeaponSlotIndex.First : WeaponSlotIndex.Second;

            if (weapon != null)
            {
                gunChannel.RaiseEvent(GunEvents.WeaponSlotEquipEvent.Init(weapon.Gun, slotIndex));
                equipmentContainer?.TryEquipWeapon(weapon, i);
                Debug.Log($"[WeaponHolder] 슬롯 등록: {weapon.ItemName} → {slotIndex}");
            }
            else
            {
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
            gunChannel.RaiseEvent(GunEvents.WeaponEquipEvent.Init(WeaponSlotIndex.First));
        else if (slotIndex == 1)
            gunChannel.RaiseEvent(GunEvents.WeaponEquipEvent.Init(WeaponSlotIndex.Second));
    }

    public void Unequip()
    {
        CurrentSlotIndex = -1;
        CurrentWeapon = null;
        OnWeaponChanged?.Invoke(null);

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

        Debug.Log("[WeaponHolder] 투척류 해제");
    }
}
