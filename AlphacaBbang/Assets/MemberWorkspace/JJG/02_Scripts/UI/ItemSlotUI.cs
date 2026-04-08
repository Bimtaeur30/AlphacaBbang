using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlotUI : MonoBehaviour
{
    [SerializeField] private Image _iconImage;
    [SerializeField] private TextMeshProUGUI _amountText;
    
    private int _slotIndex;
    
    public void SetSlotIndex(int index)
    {
        _slotIndex = index;
    }
    
    public void SetSlot(InventoryItem inventoryItem)
    {
        if (inventoryItem == null || inventoryItem.itemData == null)
        {
            ClearSlot();
            return;
        }

        _iconImage.sprite = inventoryItem.itemData.Icon;
        _iconImage.enabled = inventoryItem.itemData.Icon != null;

        if (inventoryItem.count > 1)
        {
            _amountText.text = inventoryItem.count.ToString();
        }
        else
        {
            _amountText.text = "";
        }
    }
    
    public void ClearSlot()
    {
        _iconImage.sprite = null;
        _iconImage.enabled = false;
        _amountText.text = "";
    }

    public void SetItem(Sprite itemSprite)
    {
        _iconImage.sprite = itemSprite;
    }
}
