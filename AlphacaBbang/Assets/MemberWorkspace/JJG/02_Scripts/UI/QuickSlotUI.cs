using System.Collections.Generic;
using MemberWorkspace.JJG._02_Scripts;
using UnityEngine;

public class QuickSlotUI : MonoBehaviour
{
    [SerializeField] private List<ItemSlotUI> slotUIList;
    [SerializeField] private Sprite _emptyIcon;
    [SerializeField] private List<Sprite> _slotEmptyIcons = new();
    
    private QuickSlotContainer _quickSlotContainer;

    private void OnEnable()
    {
        if (_quickSlotContainer != null)
            _quickSlotContainer.OnContainerChanged += RefreshUI;
    }

    private void OnDisable()
    {
        if (_quickSlotContainer != null)
            _quickSlotContainer.OnContainerChanged -= RefreshUI;
    }

    private void Start()
    {
        _quickSlotContainer = GetComponent<QuickSlotContainer>();
        
        RefreshUI();
    }

    private void RefreshUI()
    {
        if (_quickSlotContainer == null)
            return;

        for (int i = 0; i < slotUIList.Count; i++)
        {
            ItemSlot slot = _quickSlotContainer.GetSlot(i);

            if (slot != null && !slot.IsEmpty)
            {
                slotUIList[i].SetSlot(slot);
            }
            else
            {
                Sprite slotIcon = (i < _slotEmptyIcons.Count && _slotEmptyIcons[i] != null) ? _slotEmptyIcons[i] : _emptyIcon;
                slotUIList[i].ClearSlotWithDefault(slotIcon);
            }
        }
    }
}
