using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ItemSlotUI : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Image _iconImage;
    [SerializeField] private TextMeshProUGUI _amountText;

    private int _slotIndex;
    private ItemContainer _container;
    
    public void Initialize(ItemContainer container, int index)
    {
        _container = container;
        _slotIndex = index;
    }

    public void SetSlotIndex(int index)
    {
        _slotIndex = index;
    }

    public void SetContainer(ItemContainer container)
    {
        _container = container;
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
            _amountText.text = slot.Amount.ToString();
        else
            _amountText.text = "";
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

    public void OnPointerClick(PointerEventData eventData)
    {
        if (_container == null)
            return;
        
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            ItemSlot slot = _container.GetSlot(_slotIndex);

            if (slot == null || slot.IsEmpty)
                return;

            InventoryContextMenu.Instance.Open(
                _container,
                _slotIndex,
                transform.position
            );
        }
    }
}