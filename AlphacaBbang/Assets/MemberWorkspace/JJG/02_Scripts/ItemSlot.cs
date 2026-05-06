using System;
using MemberWorkspace.JJG._02_Scripts;
using UnityEngine;

[Serializable]
public class ItemSlot
{
    [field: SerializeField]
    public ItemData ItemData { get; set; }

    [field: SerializeField]
    public int Amount { get; set; }

    public bool IsEmpty =>
        ItemData == null || Amount <= 0;

    public void Clear()
    {
        ItemData = null;
        Amount = 0;
    }
}