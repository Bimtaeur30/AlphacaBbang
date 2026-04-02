using JJH._02_Scripts_Systems.EventSystems;
using UnityEngine;

public static class SystemEventChannel
{
    public static readonly WeaponEquipEvent WeaponEqnupEventChannel = new WeaponEquipEvent();
    public static readonly WeaponDropEvent WeaponDropEventChannel = new WeaponDropEvent();
}

public class WeaponEquipEvent : GameEvent
{
    public GunDataSO GunData { get; private set; }

    public WeaponEquipEvent Init(GunDataSO gunData)
    {
        GunData = gunData;
        return this;
    }
}

public class WeaponDropEvent : GameEvent
{
}