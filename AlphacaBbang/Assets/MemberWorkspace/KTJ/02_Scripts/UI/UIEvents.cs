using JJH._02_Scripts.Systems.EventSystems;
using JJH._02_Scripts_Systems.EventSystems;
using UnityEngine;

public static class UIEvents 
{
    public static readonly BulletCountHandleEvent BulletCountHandleEvent = new BulletCountHandleEvent();
}

public class BulletCountHandleEvent : GameEvent
{
    public WeaponSlotIndex Slot { get; private set; }
    public string Text { get; private set; }

    public BulletCountHandleEvent Init(WeaponSlotIndex slot, string text)
    {
        Slot = slot;
        Text = text;
        return this;
    }
}
