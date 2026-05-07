using MemberWorkspace.JJG._02_Scripts.Item.Data;
using UnityEngine.UI;
using UnityEngine;

public class InventoryContextMenu : MonoSingleton<InventoryContextMenu>
{
    [SerializeField] private GameObject rootPanel;

    [SerializeField] private Button useButton;
    [SerializeField] private Button equipButton;
    [SerializeField] private Button dropButton;
    [SerializeField] private Button quickSlotButton;

    private ItemContainer _container;
    private int _slotIndex;
    
    public void Open(ItemContainer container, int slotIndex, Vector3 position)
    {
        _container = container;
        _slotIndex = slotIndex;

        rootPanel.transform.position = position;
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
        quickSlotButton.onClick.RemoveAllListeners();

        useButton.onClick.AddListener(OnClickUse);
        equipButton.onClick.AddListener(OnClickEquip);
        dropButton.onClick.AddListener(OnClickDrop);
        quickSlotButton.onClick.AddListener(OnClickQuickSlot);
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

        useButton.gameObject.SetActive(item is FoodItemData || item is MedicineItemData);
        //equipButton.gameObject.SetActive(item is WeaponItemData || item is ArmorItemData);
        quickSlotButton.gameObject.SetActive(item is FoodItemData || item is MedicineItemData);
        dropButton.gameObject.SetActive(true);
    }

    private void OnClickUse()
    {
        _container.UseItem(_slotIndex, null);
        Close();
    }

    private void OnClickEquip()
    {
        Debug.Log("장착");
        Close();
    }

    private void OnClickDrop()
    {
        _container.ClearSlot(_slotIndex);
        Close();
    }

    private void OnClickQuickSlot()
    {
        Debug.Log("퀵슬롯 등록");
        Close();
    }
}
