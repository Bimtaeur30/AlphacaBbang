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

        bool isConsumable = item is FoodItemData || item is MedicineItemData;
        bool isThrowing = item is ThrowingItemData;
        bool isWeapon = item is WeaponItemData;
        bool isGearEquip = !isWeapon && item.EquipType != EquipType.None;
        bool isEquippable = isWeapon || isGearEquip;

        bool isStorage = _container.ContainerType == ContainerType.Storage;
        bool isQuickSlot = _container is MemberWorkspace.JJG._02_Scripts.QuickSlotContainer;
        bool isLootBox = _container is LootBoxContainer;
        bool isExternal = isStorage || isQuickSlot;

        useButton?.gameObject.SetActive(isConsumable && !isExternal);
        equipButton?.gameObject.SetActive((isEquippable || isConsumable || isThrowing) && !isExternal);
        unequipButton?.gameObject.SetActive(isGearEquip && !isExternal && HasEquippedItemOfType(item.EquipType));
        storeButton?.gameObject.SetActive(!isExternal && !isLootBox && storageContainer != null);
        retrieveButton?.gameObject.SetActive(isExternal || isLootBox);
        dropButton?.gameObject.SetActive(true);
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
        if (slot == null || slot.IsEmpty) { Close(); return; }

        ItemData itemData = slot.ItemData;

        if (itemData is WeaponItemData weaponData)
        {
            if (weaponData.Gun != null)
            {
                ItemSlot slot0 = quickSlotContainer.GetSlot(0);
                if (slot0 != null && !slot0.IsEmpty)
                {
                    Debug.Log("장착이 안됨 - 무기 슬롯이 가득 찼습니다.");
                    Close();
                    return;
                }

                _container.MoveItemTo(_slotIndex, quickSlotContainer, 0);
                gunChannel?.RaiseEvent(GunEvents.WeaponSlotEquipEvent.Init(null, WeaponSlotIndex.First, false));
            }
            else if (!string.IsNullOrEmpty(weaponData.MeleeWeaponId))
            {
                if (weaponData.Gun != null)
                {
                    ItemSlot slot0 = quickSlotContainer.GetSlot(0);
                    if (slot0 != null && !slot0.IsEmpty)
                    {
                        Debug.Log("장착이 안됨 - 무기 슬롯이 가득 찼습니다.");
                        Close();
                        return;
                    }

                    _container.MoveItemTo(_slotIndex, quickSlotContainer, 0);

                    gunChannel?.RaiseEvent(GunEvents.WeaponSlotEquipEvent.Init(weaponData.Gun, WeaponSlotIndex.First));
                    gunChannel?.RaiseEvent(GunEvents.WeaponEquipEvent.Init(WeaponSlotIndex.First));
                }

                _container.MoveItemTo(_slotIndex, quickSlotContainer, 0);
                if (weaponHolder != null)
                {
                    MeleeWeaponBase meleeWeapon = weaponHolder.FindMeleeWeapon(weaponData.MeleeWeaponId);
                    weaponHolder.EquipMeleeWeapon(0, weaponData, meleeWeapon);
                }
            }
        }
        else if (itemData is ThrowingItemData throwingData)
        {
            ItemSlot slot1 = quickSlotContainer.GetSlot(1);
            if (slot1 != null && !slot1.IsEmpty)
            {
                Debug.Log("장착이 안됨 - 수류탄 슬롯이 가득 찼습니다.");
                Close();
                return;
            }

            _container.MoveItemTo(_slotIndex, quickSlotContainer, 1);
            if (weaponHolder != null)
            {
                weaponHolder.UnequipThrowingItem();
                weaponHolder.EquipThrowingItem(1, throwingData);
                hotkeyHandler.SetThrowingSlotIndex(1);
            }
        }
        else if (itemData is ArmorItemData)
        {
            if (equipmentContainer == null)
            {
                Debug.LogWarning("EquipmentContainer가 연결되지 않았습니다.");
                Close();
                return;
            }
            equipmentContainer.TryEquipFromContainer(_container, _slotIndex);
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
        else if (itemData is FoodItemData || itemData is MedicineItemData)
        {
            int targetSlotIndex = TryMoveToQuickSlotAndGetIndex(itemData, minIndex: 2, maxIndex: quickSlotContainer.SlotCount);
            if (targetSlotIndex < 0)
            {
                Debug.Log("장착이 안됨 - 슬롯이 가득 찼습니다.");
                Close();
                return;
            }
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

            ItemData unequipItem = eqSlot.slot.ItemData;
            equipmentContainer.UnequipArmor(i, inventoryContainer);

            if (inventoryContainer != null)
                inventoryContainer.AddItem(unequipItem, 1);

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

            if (!canPlace) continue;
            if (targetSlot == null || !targetSlot.IsEmpty) continue;

            _container.MoveItemTo(_slotIndex, quickSlotContainer, i);
            return i;
        }

        Debug.Log("장착이 안됨 - 슬롯이 가득 찼습니다.");
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

        if (storageContainer == null) { Close(); return; }

        ItemData itemData = slot.ItemData;
        int slotIndex = _slotIndex;

        if (storageContainer.AddItem(itemData, slot.Amount))
        {
            _container.ClearSlot(_slotIndex);

            if (itemData is WeaponItemData && slotIndex < 2)
            {
                WeaponSlotIndex weaponSlot = slotIndex == 0 ? WeaponSlotIndex.First : WeaponSlotIndex.Second;
                gunChannel?.RaiseEvent(GunEvents.WeaponEquipEvent.Init(weaponSlot, false));
                gunChannel?.RaiseEvent(GunEvents.WeaponSlotEquipEvent.Init(null, weaponSlot, false));
            }
        }
        else
            Debug.LogWarning("창고가 가득 찼습니다.");

        Close();
    }

    private void OnClickRetrieve()
    {
        ItemSlot slot = _container.GetSlot(_slotIndex);
        if (slot == null || slot.IsEmpty || inventoryContainer == null) { Close(); return; }

        ItemData itemData = slot.ItemData;

        if (inventoryContainer.AddItem(itemData, slot.Amount))
        {
            _container.ClearSlot(_slotIndex);

            if (_container is MemberWorkspace.JJG._02_Scripts.QuickSlotContainer && _slotIndex < 2)
            {
                WeaponSlotIndex weaponSlot = _slotIndex == 0 ? WeaponSlotIndex.First : WeaponSlotIndex.Second;
                if (itemData is WeaponItemData)
                    gunChannel?.RaiseEvent(GunEvents.WeaponEquipEvent.Init(weaponSlot, false));
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

        _container.ClearSlot(_slotIndex);

        if (slotIndex == 0)
        {
            if (itemData is WeaponItemData)
                gunChannel?.RaiseEvent(GunEvents.WeaponEquipEvent.Init(WeaponSlotIndex.First, false));
            gunChannel?.RaiseEvent(GunEvents.WeaponSlotEquipEvent.Init(null, WeaponSlotIndex.First, false));
        }

        Close();
    }
}