using System;
using MemberWorkspace.JJG._02_Scripts;
using UnityEngine;

// [Serializable]
// public class ItemSlot
// {
//     [field: SerializeField]
//     public ItemData ItemData { get; set; }
//
//     [field: SerializeField]
//     public int Amount { get; set; }
//
//     public bool IsEmpty =>
//         ItemData == null || Amount <= 0;
//
//     public void Clear()
//     {
//         ItemData = null;
//         Amount = 0;
//     }
// }

[Serializable]
public class ItemSlot
{
    // Before
    // [field: SerializeField]
    // public ItemData ItemData { get; set; }

    // After
    [SerializeField] private ItemData itemData;
    [SerializeField] private int amount;

    public ItemData ItemData
    {
        get => itemData;
        set => itemData = value;
    }

    public int Amount
    {
        get => amount;
        set => amount = value;
    }

    public bool IsEmpty => itemData == null || amount <= 0;

    public void Clear()
    {
        itemData = null;
        amount = 0;
    }
}