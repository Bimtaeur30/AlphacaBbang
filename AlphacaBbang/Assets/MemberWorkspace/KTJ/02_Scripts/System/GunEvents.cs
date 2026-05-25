using JJH._02_Scripts_Systems.EventSystems;
using UnityEngine;

public static class GunEvents
{
    public static readonly WeaponEquipDataEvent WeaponEquipDataEvent = new WeaponEquipDataEvent();
    public static readonly WeaponEquipEvent WeaponEquipEvent = new WeaponEquipEvent();
    public static readonly WeaponSlotEquipEvent WeaponSlotEquipEvent = new WeaponSlotEquipEvent();
    public static readonly WeaponDropEvent WeaponDropEvent = new WeaponDropEvent();
    public static readonly WeaponReloadEvent WeaponReloadEvent = new WeaponReloadEvent();
}

public class WeaponEquipEvent : GameEvent // CurrentGun에 장착(1,2번 중 선택)
{
    public WeaponSlotIndex SlotIndex { get; private set; }
    public bool IsEquip { get; private set; } // true : 장착, false  : 해제
    public WeaponEquipEvent Init(WeaponSlotIndex index, bool IsEquip = true)
    {
        this.SlotIndex = index;
        this.IsEquip = IsEquip;
        return this;
    }
}
    

public enum WeaponSlotIndex
{
    First,
    Second
}
public class WeaponSlotEquipEvent : GameEvent // WeaponSlot에 장착(1,2번 중 선택)
{
    public GameObject Gun { get; private set; }
    public WeaponSlotIndex SlotIndex { get; private set; }
    public bool IsEquip { get; private set; } // true : 장착, false  : 해제
    public WeaponSlotEquipEvent Init(GameObject gun, WeaponSlotIndex slotIndex, bool IsEquip = true)
    {
        Gun = gun;
        SlotIndex = slotIndex;
        this.IsEquip = IsEquip;
        return this;
    }
}
public class WeaponEquipDataEvent : GameEvent
{
    public GunDataSO GunData { get; private set; }

    public WeaponEquipDataEvent Init(GunDataSO gunData)
    {
        GunData = gunData;
        return this;
    }
}

public class WeaponDropEvent : GameEvent
{
    public WeaponSlotIndex Index { get; private set; }

    public WeaponDropEvent Init(WeaponSlotIndex index)
    {
        Index = index;
        return this;
    }
}

public class WeaponReloadEvent : GameEvent
{
    public bool IsRealoading { get; private set; }

    public WeaponReloadEvent Init(bool isRealoading)
    {
        IsRealoading = isRealoading;
        return this;
    }
}