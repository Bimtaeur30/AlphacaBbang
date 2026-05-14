using System;
using MemberWorkspace.JJG._02_Scripts;
using MemberWorkspace.JJG._02_Scripts.Item.Data;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.InputSystem;

public class InventoryContextMenu : MonoBehaviour
{
    // 장비 아이템(EquipType != None)만 처리할지 여부 — 인스턴스별로 Inspector에서 설정
    [SerializeField] private bool _handleEquipmentItems = false;

    [SerializeField] private GameObject rootPanel;

    // ItemSlotUI에서 호출하는 static 진입점
    public static event Action<ItemContainer, int, Vector3> OnOpenRequested;
    public static void Open(ItemContainer container, int slotIndex, Vector3 position)
        => OnOpenRequested?.Invoke(container, slotIndex, position);

    [SerializeField] private Button useButton;
    [SerializeField] private Button equipButton;
    [SerializeField] private Button unequipButton;
    [SerializeField] private Button dropButton;

    [SerializeField] private QuickSlotContainer quickSlotContainer;
    [SerializeField] private EquipmentContainer equipmentContainer;
    [SerializeField] private InventoryContainer inventoryContainer;

    [SerializeField] private int xOffset = 50;

    private ItemContainer _container;
    private int _slotIndex;

    private RectTransform _panelRT;

    private void Awake()
    {
        _panelRT = rootPanel?.GetComponent<RectTransform>();
        rootPanel?.SetActive(false);
    }

    private void OnEnable()  => OnOpenRequested += HandleOpenRequest;
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
        ItemSlot slot = container.GetSlot(slotIndex);
        if (slot == null || slot.IsEmpty) return;

        bool isEquipItem = slot.ItemData.EquipType != EquipType.None;
        if (isEquipItem != _handleEquipmentItems) return;

        _container = container;
        _slotIndex = slotIndex;

        rootPanel.transform.position = position + new Vector3(xOffset, 0, 0);
        //backdrop?.gameObject.SetActive(true);
        rootPanel.SetActive(true);

        BindButtons();
        RefreshVisibleButtons();
    }

    public void Close()
    {
        rootPanel?.SetActive(false);
    }

    private void BindButtons()
    {
        if (useButton != null)     { useButton.onClick.RemoveAllListeners();     useButton.onClick.AddListener(OnClickUse); }
        if (equipButton != null)   { equipButton.onClick.RemoveAllListeners();   equipButton.onClick.AddListener(OnClickEquip); }
        if (unequipButton != null) { unequipButton.onClick.RemoveAllListeners(); unequipButton.onClick.AddListener(OnClickUnequip); }
        if (dropButton != null)    { dropButton.onClick.RemoveAllListeners();    dropButton.onClick.AddListener(OnClickDrop); }
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

        bool isConsumable  = item is FoodItemData || item is MedicineItemData;
        bool isWeapon      = item is WeaponItemData;
        bool isGearEquip   = !isWeapon && item.EquipType != EquipType.None;
        bool isEquippable  = isWeapon || isGearEquip;

        useButton?.gameObject.SetActive(isConsumable);
        equipButton?.gameObject.SetActive(isEquippable || isConsumable);
        unequipButton?.gameObject.SetActive(isGearEquip && HasEquippedItemOfType(item.EquipType));
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
        _container.UseItem(_slotIndex, null);
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

        if (itemData is WeaponItemData)
        {
            TryMoveToQuickSlot(itemData, minIndex: 0, maxIndex: 3);
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
            TryMoveToQuickSlot(itemData, minIndex: 3, maxIndex: quickSlotContainer.SlotCount);
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
        if (quickSlotContainer == null)
        {
            Debug.LogWarning("QuickSlotContainer가 연결되지 않았습니다.");
            return;
        }

        for (int i = minIndex; i < maxIndex; i++)
        {
            if (!quickSlotContainer.CanPlaceItem(i, itemData))
                continue;

            ItemSlot targetSlot = quickSlotContainer.GetSlot(i);
            if (targetSlot == null || !targetSlot.IsEmpty)
                continue;

            _container.MoveItemTo(_slotIndex, quickSlotContainer, i);
            return;
        }

        Debug.LogWarning("빈 슬롯이 없습니다.");
    }

    private void OnClickDrop()
    {
        _container.ClearSlot(_slotIndex);
        Close();
    }
}
