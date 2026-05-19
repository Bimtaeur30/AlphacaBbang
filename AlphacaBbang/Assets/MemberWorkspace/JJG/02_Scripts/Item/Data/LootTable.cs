// using System;
// using System.Collections.Generic;
// using MemberWorkspace.JJG._02_Scripts;
// using UnityEngine;
// using Random = UnityEngine.Random;
//
// [CreateAssetMenu(fileName = "LootTable", menuName = "JJK/Loot/LootTable")]
// public class LootTable : ScriptableObject
// {
//     [SerializeField] private int minDropCount = 1;
//     [SerializeField] private int maxDropCount = 4;
//
//     [SerializeField] private List<LootEntry> lootEntries = new();
//
//     public bool GenerateLoot(ItemContainer container)
//     {
//         if (container == null) return false;
//
//         int dropCount = Random.Range(minDropCount, maxDropCount + 1);
//
//         int addedCount = 0;
//
//         for (int i = 0; i < dropCount; i++)
//         {
//             LootEntry entry = PickRandomEntry();
//
//             if (entry == null || entry.ItemData == null)
//                 continue;
//
//             int amount = entry.GetRandomAmount();
//
//             bool added = container.AddItem(entry.ItemData, amount);
//         
//             if (added) addedCount++;
//         }
//
//         return addedCount > 0;
//     }
//
//     private LootEntry PickRandomEntry()
//     {
//         int totalWeight = 0;
//
//         foreach (LootEntry entry in lootEntries)
//         {
//             if (entry == null || entry.ItemData == null)
//                 continue;
//
//             totalWeight += entry.Weight;
//         }
//
//         if (totalWeight <= 0)
//             return null;
//
//         int randomValue = Random.Range(0, totalWeight);
//
//         int currentWeight = 0;
//
//         foreach (LootEntry entry in lootEntries)
//         {
//             if (entry == null || entry.ItemData == null)
//                 continue;
//
//             currentWeight += entry.Weight;
//
//             if (randomValue < currentWeight)
//                 return entry;
//         }
//
//         return null;
//     }
// }
//
// [Serializable]
// public class LootEntry
// {
//     [field: SerializeField] public ItemData ItemData { get; private set; }
//
//     [field: SerializeField, Min(1)] 
//     public int Weight { get; private set; } = 1;
//
//     [field: SerializeField, Min(1)] 
//     public int MinAmount { get; private set; } = 1;
//
//     [field: SerializeField, Min(1)] 
//     public int MaxAmount { get; private set; } = 1;
//
//     public int GetRandomAmount()
//     {
//         return Random.Range(MinAmount, MaxAmount + 1);
//     }
// }

using System;
using System.Collections.Generic;
using MemberWorkspace.JJG._02_Scripts;
using UnityEngine;
using Random = UnityEngine.Random;

[CreateAssetMenu(fileName = "LootTable", menuName = "JJK/Loot/LootTable")]
public class LootTable : ScriptableObject
{
    [SerializeField] private List<LootEntry> lootEntries = new();

    public bool GenerateLoot(ItemContainer container)
    {
        if (container == null) return false;

        int addedCount = 0;

        foreach (LootEntry entry in lootEntries)
        {
            if (entry == null || entry.ItemData == null)
                continue;

            // 각 아이템마다 독립적으로 확률 체크
            if (!entry.RollDrop())
                continue;

            int amount = entry.GetRandomAmount();

            if (container.AddItem(entry.ItemData, amount))
                addedCount++;
        }

        return addedCount > 0;
    }
}

[Serializable]
public class LootEntry
{
    [field: SerializeField] 
    public ItemData ItemData { get; private set; }

    [field: SerializeField, Range(0f, 100f)] 
    public float DropChance { get; private set; } = 50f; // 인스펙터에서 0~100% 슬라이더로 표시

    [field: SerializeField, Min(1)] 
    public int MinAmount { get; private set; } = 1;

    [field: SerializeField, Min(1)] 
    public int MaxAmount { get; private set; } = 1;

    public bool RollDrop()
    {
        return Random.Range(0f, 100f) < DropChance;
    }

    public int GetRandomAmount()
    {
        return Random.Range(MinAmount, MaxAmount + 1);
    }
}