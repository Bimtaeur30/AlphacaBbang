using System;
using Reflex.Core;
using System.Collections.Generic;

public class InventoryContainer : ItemContainer, IInstaller
{
    private void Start()
    {
        //slots
    }

    public override bool ClearSlot(int index)
    {
        bool result = base.ClearSlot(index);
        if (result) Compact();
        return result;
    }

    public void InstallBindings(ContainerBuilder containerBuilder)
    {
        containerBuilder.RegisterValue(this);
    }

    public override bool RemoveAmount(int index, int amount = 1)
    {
        bool result = base.RemoveAmount(index, amount);
        ItemSlot slot = GetSlot(index);
        if (result && (slot == null || slot.IsEmpty))
            Compact();
        return result;
    }

    protected virtual void Compact()
    {
        var items = new List<(ItemData data, int amount)>();
        for (int i = 0; i < slots.Count; i++)
        {
            if (!slots[i].IsEmpty)
                items.Add((slots[i].ItemData, slots[i].Amount));
        }

        for (int i = 0; i < slots.Count; i++)
            slots[i].Clear();

        for (int i = 0; i < items.Count; i++)
        {
            slots[i].ItemData = items[i].data;
            slots[i].Amount = items[i].amount;
        }

        NotifyContainerChanged();
    }
}
