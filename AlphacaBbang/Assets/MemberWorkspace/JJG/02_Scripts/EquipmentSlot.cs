using System;
using UnityEngine;

[Serializable]
public class EquipmentSlot
{
    public EquipType allowedEquipType;
    public ItemSlot slot = new ItemSlot();
}
