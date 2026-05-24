using System;
using JJH._02_Scripts_Systems.EventSystems;
using MemberWorkspace.JJG._02_Scripts;
using MemberWorkspace.JJG._02_Scripts.Item;
using MemberWorkspace.JJG._02_Scripts.Item.Data;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.InputSystem;

public class InventoryContextMenu : MonoBehaviour
{
    [SerializeField] private bool _handleEquipmentItems = false;
    [SerializeField] private GameObject rootPanel;

    public static bool IsOpen { get; private set; }

    private static InventoryContextMenu _currentOpenMenu;
    private static bool _openHandled;

    public static event Action<ItemContainer, int, Vector3> OnOpenRequested;
    public static void Open(ItemContainer container, int slotIndex, Vector3 position)
    {
        CloseCurrent();
        _openHandled = false;

        try
        {
            OnOpenRequested?.Invoke(container, slotIndex, position);
        }
        finally
        {
            _openHandled = false;
        }
    }

    private static void CloseCurrent()
    {
        if (_currentOpenMenu != null)
        {
            _currentOpenMenu.Close();
            _currentOpenMenu = null;
        }
    }

    [SerializeField] private Button useButton;
    [SerializeField] private Button equipButton;
    [SerializeField] private Button unequipButton;
    [SerializeField] private Button storeButton;
    [SerializeField] private Button retrieveButton;
    [SerializeField] private Button dropButton;

    [SerializeField] private QuickSlotContainer quickSlotContainer;
    [SerializeField] private EquipmentContainer equipmentContainer;
    [SerializeField] private InventoryContainer inventoryContainer;
    [SerializeField] private ItemContainer storageContainer;
    [SerializeField] private GameObject itemUser;
    [SerializeField] private WeaponHolder weaponHolder;
    [SerializeField] private GrenadeFirePos grenadeFirePos;
    [SerializeField] private QuickSlotHotkeyHandler hotkeyHandler;
    [SerializeField] private EventChannelSO gunChannel;

    [SerializeField] private int xOffset = 50;

    private ItemContainer _container;
    private int _slotIndex;
    private RectTransform _panelRT;

    private void Awake()
    {
        _panelRT = rootPanel?.GetComponent<RectTransform>();
        rootPanel?.SetActive(false);
    }

    private void OnEnable() => OnOpenRequested += HandleOpenRequest;
    private void OnDisable() => OnOpenRequested -= HandleOpenRequest;

    private void Update()
    {
        if (rootPanel == null || !rootPanel.activeSelf) return;

        var mouse = Mouse.current;
        if (mouse == null) return;

        Vector2 mousePos = mouse.position.ReadValue();

        if (mouse.leftButton.wasPressedThisFrame &&
            !RectTransformUtility.RectangleContainsScreenPoint(_panelRT, mousePos))
        {
            Close();
        }
    }

    private void HandleOpenRequest(ItemContainer container, int slotIndex, Vector3 position)
    {
        if (_openHandled) return;

        ItemSlot slot = container.GetSlot(slotIndex);
        if (slot == null || slot.IsEmpty) return;

        bool isEquipItem = slot.ItemData.EquipType != EquipType.None;
        bool ignoreEquipFiltering = container is LootBoxContainer || container.ContainerType == ContainerType.Storage;
        if (ignoreEquipFiltering)
            isEquipItem = false;

        if (isEquipItem != _handleEquipmentItems) return;

        _openHandled = true;
        _currentOpenMenu = this;

        _container = container;
        _slotIndex = slotIndex;

        IsOpen = true;
        ItemTooltip.Instance?.Hide();

        rootPanel.transform.position = position + new Vector3(xOffset, 0, 0);
        rootPanel.SetActive(true);

        BindButtons();
        RefreshVisibleButtons();
    }

    public void Close()
    {
        rootPanel?.SetActive(false);
        IsOpen = false;

        if (_currentOpenMenu == this)
            _currentOpenMenu = null;
    }

    private void BindButtons()
    {
        if (useButton != null) { useButton.onClick.RemoveAllListeners(); useButton.onClick.AddListener(OnClickUse); }
        if (equipButton != null) { equipButton.onClick.RemoveAllListeners(); equipButton.onClick.AddListener(OnClickEquip); }
        if (unequipButton != null) { unequipButton.onClick.RemoveAllListeners(); unequipButton.onClick.AddListener(OnClickUnequip); }
        if (storeButton != null) { storeButton.onClick.RemoveAllListeners(); storeButton.onClick.AddListener(OnClickStore); }
        if (retrieveButton != null) { retrieveButton.onClick.RemoveAllListeners(); retrieveButton.onClick.AddListener(OnClickRetrieve); }
        if (dropButton != null) { dropButton.onClick.RemoveAllListeners(); dropButton.onClick.AddListener(OnClickDrop); }
    }

    private void RefreshVisibleButtons()
    {
        ItemSlot slot = _container.GetSlot(_slotIndex);

        if (slot == null || slot.IsEmpty)
        {
            Close();
            return;
        }

        ItemData item = slot.ItemData;

        bool isConsumable = item is FoodItemData || item is MedicineItemData || item is ThrowingItemData;
        bool isWeapon = item is WeaponItemData;
        bool isGearEquip = !isWeapon && item.EquipType != EquipType.None;
        bool isEquippable = isWeapon || isGearEquip;

        bool isStorage = _container.ContainerType == ContainerType.Storage;
        bool isQuickSlot = _container is MemberWorkspace.JJG._02_Scripts.QuickSlotContainer;
        bool isLootBox = _container is LootBoxContainer;
        bool isExternal = isStorage || isQuickSlot;

        useButton?.gameObject.SetActive(isConsumable && !isExternal);
        equipButton?.gameObject.SetActive((isEquippable || isConsumable) && !isExternal);
        unequipButton?.gameObject.SetActive(isGearEquip && !isExternal && HasEquippedItemOfType(item.EquipType));
        storeButton?.gameObject.SetActive(!isExternal && !isLootBox && storageContainer != null);
        retrieveButton?.gameObject.SetActive(isExternal || isLootBox);
        dropButton?.gameObject.SetActive(!isLootBox && !isStorage);
    }

    private bool HasEquippedItemOfType(EquipType equipType)
    {
        if (equipmentContainer == null) return false;

        for (int i = 0; i < equipmentContainer.SlotCount; i++)
        {
            EquipmentSlot eqSlot = equipmentContainer.GetEquipmentSlot(i);
            if (eqSlot == null) continue;
            if (eqSlot.allowedEquipType == equipType && !eqSlot.slot.IsEmpty)
                return true;
        }

        return false;
    }

    private void OnClickUse()
    {
        _container.UseItem(_slotIndex, itemUser);
        Close();
    }

    private void OnClickEquip()
    {
        ItemSlot slot = _container.GetSlot(_slotIndex);
        if (slot == null || slot.IsEmpty)
        {
            Close();
            return;
        }

        ItemData itemData = slot.ItemData;

        if (itemData is WeaponItemData weaponData)
        {
            if (weaponData.Gun != null)
            {
                int targetSlotIndex = TryMoveToQuickSlotAndGetIndex(itemData, minIndex: 0, maxIndex: 3);
                
                // 기존 퀵슬롯의 다른 무기 제거 이벤트 발송
                for (int i = 0; i < 3; i++)
                {
                    if (i != targetSlotIndex)
                    {
                        ItemSlot otherSlot = quickSlotContainer.GetSlot(i);
                        if (otherSlot != null && otherSlot.ItemData is WeaponItemData)
                        {
                            WeaponSlotIndex slotIndex = i == 0 ? WeaponSlotIndex.First : WeaponSlotIndex.Second;
                            gunChannel?.RaiseEvent(GunEvents.WeaponSlotEquipEvent.Init(null, slotIndex, false));
                        }
                    }
                }
            }
            else if (!string.IsNullOrEmpty(weaponData.MeleeWeaponId))
            {
                int targetSlotIndex = TryMoveToQuickSlotAndGetIndex(itemData, minIndex: 0, maxIndex: 3);
                if (targetSlotIndex >= 0 && weaponHolder != null)
                {
                    MeleeWeaponBase meleeWeapon = weaponHolder.FindMeleeWeapon(weaponData.MeleeWeaponId);
                    weaponHolder.EquipMeleeWeapon(targetSlotIndex, weaponData, meleeWeapon);
                }
            }
        }
        else if (itemData is ArmorItemData armorData)
        {
            if (equipmentContainer == null)
            {
                Debug.LogWarning("EquipmentContainer가 연결되지 않았습니다.");
                Close();
                return;
            }
            
            Debug.Log("Equip Armor");
            
            int targetSlotIndex = -1;
            for (int i = 0; i < equipmentContainer.SlotCount; i++)
            {
                if (!equipmentContainer.CanEquip(i, itemData))
                    continue;
            
                EquipmentSlot equipSlot = equipmentContainer.GetEquipmentSlot(i);
                if (equipSlot == null || !equipSlot.slot.IsEmpty)
                    continue;
            
                targetSlotIndex = i;
                break;
            }

            if (targetSlotIndex < 0)
            {
                Debug.LogWarning("장착 가능한 빈 슬롯이 없습니다.");
                Close();
                return;
            }
            
            if (_container.UseItem(_slotIndex, itemUser))
            {
                equipmentContainer.Equip(targetSlotIndex, itemData);
                equipmentContainer.NotifyEquipmentChanged();
            }
        }
        else if (itemData.EquipType != EquipType.None)
        {
            if (equipmentContainer == null)
            {
                Debug.LogWarning("EquipmentContainer가 연결되지 않았습니다.");
                Close();
                return;
            }
            equipmentContainer.TryEquipFromContainer(_container, _slotIndex);
        }
        else if (itemData is ThrowingItemData throwingData)
        {
            int targetSlotIndex = TryMoveToQuickSlotAndGetIndex(itemData, minIndex: 0, maxIndex: 2);
            if (targetSlotIndex >= 0 && weaponHolder != null)
            {
                weaponHolder.EquipThrowingItem(targetSlotIndex, throwingData);
                hotkeyHandler.SetThrowingSlotIndex(targetSlotIndex);
            }
        }
        else if (itemData is FoodItemData || itemData is MedicineItemData)
        {
            TryMoveToQuickSlot(itemData, minIndex: 2, maxIndex: quickSlotContainer.SlotCount);
        }

        Close();
    }

    private void OnClickUnequip()
    {
        if (equipmentContainer == null) { Close(); return; }

        ItemSlot slot = _container.GetSlot(_slotIndex);
        if (slot == null || slot.IsEmpty) { Close(); return; }

        EquipType targetType = slot.ItemData.EquipType;

        for (int i = 0; i < equipmentContainer.SlotCount; i++)
        {
            EquipmentSlot eqSlot = equipmentContainer.GetEquipmentSlot(i);
            if (eqSlot == null || eqSlot.slot.IsEmpty) continue;
            if (eqSlot.allowedEquipType != targetType) continue;

            if (inventoryContainer != null)
                inventoryContainer.AddItem(eqSlot.slot.ItemData, 1);

            equipmentContainer.Unequip(i);
            break;
        }

        Close();
    }

    private void TryMoveToQuickSlot(ItemData itemData, int minIndex, int maxIndex)
    {
        TryMoveToQuickSlotAndGetIndex(itemData, minIndex, maxIndex);
    }

    private int TryMoveToQuickSlotAndGetIndex(ItemData itemData, int minIndex, int maxIndex)
    {
        if (quickSlotContainer == null)
        {
            Debug.LogError("QuickSlotContainer가 null입니다.");
            return -1;
        }

        for (int i = minIndex; i < maxIndex; i++)
        {
            bool canPlace = quickSlotContainer.CanPlaceItem(i, itemData);
            ItemSlot targetSlot = quickSlotContainer.GetSlot(i);
            Debug.Log($"슬롯 {i} | CanPlace: {canPlace} | IsEmpty: {targetSlot?.IsEmpty}");

            if (!canPlace) continue;
            if (targetSlot == null || !targetSlot.IsEmpty) continue;

            _container.MoveItemTo(_slotIndex, quickSlotContainer, i);
            Debug.Log($"MoveItemTo 완료 → return {i}");
            return i;
        }

        Debug.LogWarning("빈 슬롯이 없습니다.");
        return -1;
    }
    private void OnClickStore()
    {
        ItemSlot slot = _container.GetSlot(_slotIndex);
        if (slot == null || slot.IsEmpty) { Close(); return; }

        if (_container is LootBoxContainer)
        {
            if (inventoryContainer == null) { Close(); return; }

            if (inventoryContainer.AddItem(slot.ItemData, slot.Amount))
                _container.ClearSlot(_slotIndex);
            else
                Debug.LogWarning("인벤토리가 가득 찼습니다.");

            Close();
            return;
        }

        if (storageContainer != null && storageContainer.AddItem(slot.ItemData, slot.Amount))
            _container.ClearSlot(_slotIndex);
        else
            Debug.LogWarning("창고가 가득 찼습니다.");

        Close();
    }

    private void OnClickRetrieve()
    {
        ItemSlot slot = _container.GetSlot(_slotIndex);
        if (slot == null || slot.IsEmpty || inventoryContainer == null) { Close(); return; }

        ItemData itemData = slot.ItemData;
        int slotIndex = _slotIndex;

        // if (inventoryContainer.AddItem(itemData, slot.Amount))
        // {
        //     _container.ClearSlot(_slotIndex);
        //     
        //     // 퀵슬롯에서 무기 제거 시 이벤트 발송
        //     if (itemData is WeaponItemData && slotIndex < 2)
        //     {
        //         WeaponSlotIndex weaponSlot = slotIndex == 0 ? WeaponSlotIndex.First : WeaponSlotIndex.Second;
        //         gunChannel?.RaiseEvent(GunEvents.WeaponEquipEvent.Init(weaponSlot, false));
        //         // Also notify slot-level change so holstered gun object is removed
        //         gunChannel?.RaiseEvent(GunEvents.WeaponSlotEquipEvent.Init(null, weaponSlot, false));
        //     }
        // }
        if (inventoryContainer.AddItem(itemData, slot.Amount))
        {
            _container.ClearSlot(_slotIndex);
    
            if (slotIndex < 2)
            {
                WeaponSlotIndex weaponSlot = slotIndex == 0 ? WeaponSlotIndex.First : WeaponSlotIndex.Second;
                if (itemData is WeaponItemData)
                {
                    gunChannel?.RaiseEvent(GunEvents.WeaponEquipEvent.Init(weaponSlot, false));
                }
                gunChannel?.RaiseEvent(GunEvents.WeaponSlotEquipEvent.Init(null, weaponSlot, false));
            }
        }
        else
            Debug.LogWarning("인벤토리가 가득 찼습니다.");

        Close();
    }

    private void OnClickDrop()
    {
        ItemSlot slot = _container.GetSlot(_slotIndex);
        ItemData itemData = slot?.ItemData;
        int slotIndex = _slotIndex;
        
        // _container.ClearSlot(_slotIndex);
        //
        // // 퀵슬롯에서 무기 버릴 때 이벤트 발송
        // if (itemData is WeaponItemData && slotIndex < 2)
        // {
        //     WeaponSlotIndex weaponSlot = slotIndex == 0 ? WeaponSlotIndex.First : WeaponSlotIndex.Second;
        //     gunChannel?.RaiseEvent(GunEvents.WeaponEquipEvent.Init(weaponSlot, false));
        //     // Also remove holstered gun object
        //     gunChannel?.RaiseEvent(GunEvents.WeaponSlotEquipEvent.Init(null, weaponSlot, false));
        // }
        
        _container.ClearSlot(_slotIndex);

        if (slotIndex < 2)
        {
            WeaponSlotIndex weaponSlot = slotIndex == 0 ? WeaponSlotIndex.First : WeaponSlotIndex.Second;
            if (itemData is WeaponItemData)
            {
                gunChannel?.RaiseEvent(GunEvents.WeaponEquipEvent.Init(weaponSlot, false));
            }
            gunChannel?.RaiseEvent(GunEvents.WeaponSlotEquipEvent.Init(null, weaponSlot, false));
        }
        
        Close();
    }
}