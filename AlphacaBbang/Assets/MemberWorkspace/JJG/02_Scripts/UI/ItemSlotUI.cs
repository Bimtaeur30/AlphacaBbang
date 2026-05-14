using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ItemSlotUI : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Image _iconImage;
    [SerializeField] private TextMeshProUGUI _amountText;

    private int _slotIndex;
    private ItemContainer _container;
    private ItemSlot _currentSlot;
    
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

        _currentSlot = slot;
        _iconImage.sprite = slot.ItemData.Icon;
        _iconImage.enabled = slot.ItemData.Icon != null;

        if (slot.Amount > 1)
            _amountText.text = slot.Amount.ToString();
        else
            _amountText.text = "";
    }

    public void ClearSlot()
    {
        _currentSlot = null;
        _iconImage.sprite = null;
        _iconImage.enabled = false;
        _amountText.text = "";
    }

    public void SetItem(Sprite itemSprite)
    {
        _iconImage.sprite = itemSprite;
        _iconImage.enabled = itemSprite != null;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_currentSlot != null && !_currentSlot.IsEmpty)
            ItemTooltip.Instance?.Show(_currentSlot.ItemData);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ItemTooltip.Instance?.Hide();
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

            InventoryContextMenu.Open(
                _container,
                _slotIndex,
                transform.position
            );
        }
    }
}