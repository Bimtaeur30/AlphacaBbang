using TMPro;
using UnityEngine;
using UnityEngine.UI;
using MemberWorkspace.JJG._02_Scripts.Item;

public class ItemSlotUI : MonoBehaviour
{
    [SerializeField] private Image _iconImage;
    [SerializeField] private TextMeshProUGUI _amountText;
    
    private int _slotIndex;
    
    public void SetSlotIndex(int index)
    {
        _slotIndex = index;
    }
    
    public void SetSlot(ItemSlot slot)
    {
        if (slot == null || slot.IsEmpty || slot.ItemData == null)
        {
            ClearSlot();
            return;
        }

        _iconImage.sprite = slot.ItemData.Icon;
        _iconImage.enabled = slot.ItemData.Icon != null;

        if (slot.Amount > 1)
        {
            _amountText.text = slot.Amount.ToString();
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
        _iconImage.enabled = itemSprite != null;
    }
}