using System.Collections.Generic;
using MemberWorkspace.JJG._02_Scripts;
using UnityEngine;

public class QuickSlotUI : MonoBehaviour
{
    [SerializeField] private QuickSlotContainer quickSlotContainer;
    [SerializeField] private List<ItemSlotUI> slotUIList;

    private void OnEnable()
    {
        if (quickSlotContainer != null)
            quickSlotContainer.OnContainerChanged += RefreshUI;
    }

    private void OnDisable()
    {
        if (quickSlotContainer != null)
            quickSlotContainer.OnContainerChanged -= RefreshUI;
    }

    private void Start()
    {
        RefreshUI();
    }

    private void RefreshUI()
    {
        if (quickSlotContainer == null)
            return;

        for (int i = 0; i < slotUIList.Count; i++)
        {
            ItemSlot slot = quickSlotContainer.GetSlot(i);

            if (slot != null && !slot.IsEmpty)
            {
                slotUIList[i].SetSlot(slot);
            }
            else
            {
                slotUIList[i].ClearSlot();
            }
        }
    }
}
