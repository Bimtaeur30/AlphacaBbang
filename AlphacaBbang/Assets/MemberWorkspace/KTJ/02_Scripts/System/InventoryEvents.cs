using JJH._02_Scripts_Systems.EventSystems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public static class InventoryEvents
{
    public static readonly InventoryToggleEvt InventoryToggle = new InventoryToggleEvt();
}

public class InventoryToggleEvt : GameEvent
{
    public bool Value { get; private set; }
    public InventoryToggleEvt Init(bool value)
    {
        Value = value;
        return this;
    }
}