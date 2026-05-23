using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ItemSlotUI : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Image _iconImage;
    [SerializeField] private TextMeshProUGUI _amountText;
    [SerializeField] private Image _selectedOverlay;
    [Header("Hover")]
    [SerializeField] private Image _backgroundImage;
    [SerializeField] private Color _hoverColor = new Color(1f, 1f, 1f, 0.15f);
    [Header("Empty Slot")]
    [SerializeField] private Sprite _emptyIcon;

    private int _slotIndex;
    private ItemContainer _container;
    private ItemSlot _currentSlot;
    private Color _originalBgColor = Color.clear;
    
    public void Initialize(ItemContainer container, int index)
    {
        _container = container;
        _slotIndex = index;
        // Cache original background color and try fallback if not assigned
        if (_backgroundImage == null)
            _backgroundImage = GetComponent<Image>();

        if (_backgroundImage != null)
            _originalBgColor = _backgroundImage.color;
    }

    public void SetSlotIndex(int index)
    {
        _slotIndex = index;
    }

    public void SetContainer(ItemContainer container)
    {
        _container = container;
    }

    public void SetEmptyIcon(Sprite emptyIcon)
    {
        _emptyIcon = emptyIcon;
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
        // show configured empty icon if available, otherwise hide
        if (_emptyIcon != null)
        {
            _iconImage.sprite = _emptyIcon;
            _iconImage.enabled = true;
        }
        else
        {
            _iconImage.sprite = null;
            _iconImage.enabled = false;
        }
        _amountText.text = "";
    }

    public void ClearSlotWithDefault(UnityEngine.Sprite defaultIcon)
    {
        _currentSlot = null;
        _iconImage.sprite = defaultIcon;
        _iconImage.enabled = defaultIcon != null;
        _amountText.text = "";
    }

    public void SetSelected(bool selected)
    {
        // if (_selectedOverlay != null)
        //     _selectedOverlay.enabled = selected;
        
        if (_selectedOverlay != null)
        {
            _selectedOverlay.enabled = selected;
        }
    }

    public void SetItem(Sprite itemSprite)
    {
        _iconImage.sprite = itemSprite;
        _iconImage.enabled = itemSprite != null;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // highlight slot background
        SetHover(true);

        if (_currentSlot != null && !_currentSlot.IsEmpty && !InventoryContextMenu.IsOpen)
            ItemTooltip.Instance?.Show(_currentSlot.ItemData);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        SetHover(false);
        ItemTooltip.Instance?.Hide();
    }

    private void SetHover(bool hover)
    {
        if (_backgroundImage == null)
            return;

        _backgroundImage.color = hover ? _hoverColor : _originalBgColor;
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