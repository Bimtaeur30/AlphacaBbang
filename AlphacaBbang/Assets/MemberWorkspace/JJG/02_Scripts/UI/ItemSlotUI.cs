using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ItemSlotUI : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Image _iconImage;
    [SerializeField] private TextMeshProUGUI _amountText;
    [SerializeField] private Image _selectedOverlay;
    [SerializeField] private TextMeshProUGUI _slotIndexText;
    [Header("Hover")]
    [SerializeField] private Image _backgroundImage;
    [SerializeField] private Color _hoverColor = new Color(1f, 1f, 1f, 0.15f);
    [Header("Empty Slot")]
    [SerializeField] private Sprite _emptyIcon;
    [SerializeField] private GameObject _emptyIconObject;

    private Image _emptyIconImage;
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

        if (_slotIndexText != null)
            _slotIndexText.text = (index + 1).ToString();

        if (_backgroundImage != null)
            _originalBgColor = _backgroundImage.color;

        if (_emptyIconObject != null)
        {
            _emptyIconImage = _emptyIconObject.GetComponent<Image>();
            if (_emptyIconImage != null && _emptyIcon != null)
                _emptyIconImage.sprite = _emptyIcon;
        }
    }

    public void SetSlotIndex(int index)
    {
        _slotIndex = index;
        
        if (_slotIndexText != null)
            _slotIndexText.text = (index + 1).ToString();
    }

    public void SetContainer(ItemContainer container)
    {
        _container = container;
    }

    public void SetEmptyIcon(Sprite emptyIcon)
    {
        _emptyIcon = emptyIcon;

        if (_emptyIconImage != null)
        {
            _emptyIconImage.sprite = emptyIcon;
            _emptyIconImage.enabled = emptyIcon != null;
        }

        if (_emptyIconObject != null)
            _emptyIconObject.SetActive(emptyIcon != null);
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

        SetEmptyIconVisibility(false);

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

        if (_emptyIcon != null)
        {
            if (_emptyIconObject != null)
                SetEmptyIconVisibility(true);
            else
            {
                _iconImage.sprite = _emptyIcon;
                _iconImage.enabled = true;
            }
        }
        else
        {
            SetEmptyIconVisibility(false);
        }

        _amountText.text = "";
    }

    public void ClearSlotWithDefault(UnityEngine.Sprite defaultIcon)
    {
        _currentSlot = null;
        _iconImage.sprite = defaultIcon;
        _iconImage.enabled = defaultIcon != null;

        if (defaultIcon == null)
            SetEmptyIconVisibility(false);
        else
            SetEmptyIconVisibility(false);

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

        SetEmptyIconVisibility(false);
    }

    private void SetEmptyIconVisibility(bool visible)
    {
        if (_emptyIconObject != null)
            _emptyIconObject.SetActive(visible);

        if (_emptyIconImage != null)
            _emptyIconImage.enabled = visible;
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

            // LootBox면 바로 인벤토리로 가져오기
            if (_container is LootBoxContainer)
            {
                InventoryContextMenu.RetrieveToInventory(_container, _slotIndex);
                return;
            }

            InventoryContextMenu.Open(
                _container,
                _slotIndex,
                transform.position
            );
        }
    }
}