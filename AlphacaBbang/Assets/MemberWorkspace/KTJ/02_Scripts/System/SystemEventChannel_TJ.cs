using JJH._02_Scripts_Systems.EventSystems;
using UnityEngine;

public static class SystemEventChannel_TJ
{
    public static readonly WeaponEquipDataEvent WeaponEqnupDataEvent = new WeaponEquipDataEvent();
    public static readonly WeaponDropEvent WeaponDropEvent = new WeaponDropEvent();
}

public class WeaponEquipEvent : GameEvent // CurrentGun¿¡ ÀåÂø
{
    public Gun Gun { get; private set; }
    public WeaponEquipEvent Init(Gun gun)
    {
        Gun = gun;
        return this;
    }
}


public enum WeaponSlotType
{
    First,
    Second
}
public class WeaponSlotEquipEvent : GameEvent // WeaponSlot¿¡ ÀåÂø
{
    public Gun Gun { get; private set; }
    public int SlotIndex { get; private set; }
    public WeaponSlotEquipEvent Init(Gun gun, int slotIndex)
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