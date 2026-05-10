using JJH._02_Scripts_Systems.EventSystems;
using UnityEngine;

public static class SaveEvents
{
    public static readonly AddTestValue AddTestValue = new AddTestValue();
}

public class AddTestValue : GameEvent
{
    public int val { get; private set; }
    public AddTestValue Init(int val)
    {
        this.val = val;
        return this;
    }
}
