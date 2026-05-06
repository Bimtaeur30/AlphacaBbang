using JJH._02_Scripts_Systems.EventSystems;
using UnityEngine;

public static class SystemEventChannel_TJ
{
    public static readonly WeaponEquipDataEvent WeaponEquipDataEvent = new WeaponEquipDataEvent();
    public static readonly WeaponEquipEvent WeaponEquipEvent = new WeaponEquipEvent();
    public static readonly WeaponSlotEquipEvent WeaponSlotEquipEvent = new WeaponSlotEquipEvent();
    public static readonly WeaponDropEvent WeaponDropEvent = new WeaponDropEvent();
}

public class WeaponEquipEvent : GameEvent // CurrentGun에 장착(1,2번 중 선택)
{
    public WeaponSlotIndex SlotIndex { get; private set; }
    public WeaponEquipEvent Init(WeaponSlotIndex index)
    {
        this.SlotIndex = index;
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
    public Gun Gun { get; private set; }
    public WeaponSlotIndex SlotIndex { get; private set; }
    public WeaponSlotEquipEvent Init(Gun gun, WeaponSlotIndex slotIndex)
    {
        Gun = gun;
        SlotIndex = slotIndex;
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
}