using System.Collections.Generic;
using UnityEngine;

public class EquipmentUI : MonoBehaviour
{
    [SerializeField] private List<ItemSlotUI> _slotUIs = new();
    [SerializeField] private Sprite _emptyIcon;
    [SerializeField] private List<Sprite> _slotEmptyIcons = new();
    
    private EquipmentContainer _equipmentContainer;

    private void OnEnable()
    {
        if (_equipmentContainer != null)
            _equipmentContainer.OnEquipmentChanged += RefreshUI;

        RefreshUI();
    }

    private void OnDisable()
    {
        if (_equipmentContainer != null)
            _equipmentContainer.OnEquipmentChanged -= RefreshUI;
    }

    private void Start()
    {
        _equipmentContainer = GetComponent<EquipmentContainer>();
        
        for (int i = 0; i < _slotUIs.Count; i++)
            _slotUIs[i].Initialize(_equipmentContainer, i);

        RefreshUI();
    }

    private void RefreshUI()
    {
        if (_equipmentContainer == null) return;

        for (int i = 0; i < _slotUIs.Count; i++)
        {
            EquipmentSlot eqSlot = _equipmentContainer.GetEquipmentSlot(i);
            if (eqSlot != null && !eqSlot.slot.IsEmpty)
                _slotUIs[i].SetSlot(eqSlot.slot);
            else
            {
                Sprite slotIcon = (i < _slotEmptyIcons.Count && _slotEmptyIcons[i] != null) ? _slotEmptyIcons[i] : _emptyIcon;
                _slotUIs[i].ClearSlotWithDefault(slotIcon);
            }
        }
    }
}
