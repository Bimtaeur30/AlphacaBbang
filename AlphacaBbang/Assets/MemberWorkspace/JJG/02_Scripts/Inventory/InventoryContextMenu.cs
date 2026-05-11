using MemberWorkspace.JJG._02_Scripts;
using MemberWorkspace.JJG._02_Scripts.Item.Data;
using UnityEngine.UI;
using UnityEngine;

public class InventoryContextMenu : MonoSingleton<InventoryContextMenu>
{
    [SerializeField] private GameObject rootPanel;

    [SerializeField] private Button useButton;
    [SerializeField] private Button equipButton;
    [SerializeField] private Button dropButton;

    [SerializeField] private QuickSlotContainer quickSlotContainer;
    [SerializeField] private EquipmentContainer equipmentContainer;
    
    [SerializeField] private int xOffset = 50;

    private ItemContainer _container;
    private int _slotIndex;

    public void Open(ItemContainer container, int slotIndex, Vector3 position)
    {
        _container = container;
        _slotIndex = slotIndex;

        rootPanel.transform.position = position + new Vector3(xOffset, 0, 0);
        rootPanel.SetActive(true);

        BindButtons();
        RefreshVisibleButtons();
    }

    public void Close()
    {
        rootPanel.SetActive(false);
    }

    private void BindButtons()
    {
        useButton.onClick.RemoveAllListeners();
        equipButton.onClick.RemoveAllListeners();
        dropButton.onClick.RemoveAllListeners();

        useButton.onClick.AddListener(OnClickUse);
        equipButton.onClick.AddListener(OnClickEquip);
        dropButton.onClick.AddListener(OnClickDrop);
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
        bool isEquippable = item is WeaponItemData || item.EquipType != EquipType.None;

        useButton.gameObject.SetActive(isConsumable);
        equipButton.gameObject.SetActive(isEquippable || isConsumable);
        dropButton.gameObject.SetActive(true);
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
