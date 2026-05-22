using MemberWorkspace.JJG._02_Scripts;
using UnityEngine;

public class LootBoxContainer : ItemContainer
{
    public enum LootSelectionMode
    {
        Legacy,         // use single legacy lootTable field
        RandomFromList, // pick a random table from lootTables
        SpecificIndex   // use specific index from lootTables
    }

    [SerializeField] private LootTable lootTable; // legacy single table (kept for backward compatibility)
    [SerializeField] private System.Collections.Generic.List<LootTable> lootTables = new(); // new: multiple tables
    [SerializeField] private LootSelectionMode selectionMode = LootSelectionMode.RandomFromList;
    [SerializeField] private int specificTableIndex = 0;
    [SerializeField] private float baseOpenTime = 1.5f;
    [SerializeField] private GradeType highGradeStandard = GradeType.Rare;

    [Header("Open time by grade")] 
    [SerializeField] private float commonOpenTime = 0.2f;
    [SerializeField] private float unCommonOpenTime = 0.5f;
    [SerializeField] private float rareOpenTime = 1.0f;
    [SerializeField] private float epicOpenTime = 1.5f;
    [SerializeField] private float legendaryOpenTime = 2.5f;

    public float RequiredOpenTime { get; private set; }

    protected override void Awake()
    {
        base.Awake();

        GenerateLoot();
        RequiredOpenTime = CalculateOpenTime();
    }

    private void GenerateLoot()
    {
        LootTable chosenTable = null;

        switch (selectionMode)
        {
            case LootSelectionMode.RandomFromList:
                if (lootTables != null && lootTables.Count > 0)
                {
                    int idx = UnityEngine.Random.Range(0, lootTables.Count);
                    chosenTable = lootTables[idx];
                }
                else if (lootTable != null)
                {
                    chosenTable = lootTable; // fallback
                }
                break;

            case LootSelectionMode.SpecificIndex:
                if (lootTables != null && lootTables.Count > 0 && specificTableIndex >= 0 && specificTableIndex < lootTables.Count)
                {
                    chosenTable = lootTables[specificTableIndex];
                }
                else if (lootTable != null)
                {
                    chosenTable = lootTable; // fallback if list empty
                }
                else
                {
                    Debug.LogWarning($"{name}: SpecificTableIndex {specificTableIndex} is out of range or lootTables is empty.");
                }
                break;

            case LootSelectionMode.Legacy:
            default:
                chosenTable = lootTable;
                break;
        }

        if (chosenTable == null)
        {
            Debug.LogWarning($"{name}에 LootTable이 없습니다.");
            return;
        }

        bool result = chosenTable.GenerateLoot(this);
    }

    private float CalculateOpenTime()
    {
        float totalTime = baseOpenTime;

        for (int i = 0; i < SlotCount; i++)
        {
            ItemSlot slot = GetSlot(i);

            if (slot == null || slot.IsEmpty)
                continue;

            totalTime += CalculateSlotOpenTime(slot);
        }

        return totalTime;
    }

    private float CalculateSlotOpenTime(ItemSlot slot)
    {
        ItemData itemData = slot.ItemData;
        
        if (itemData == null)
            return 0f;

        float gradeValue = itemData.GradeType switch
        {
            GradeType.Common => commonOpenTime,
            GradeType.UnCommon => unCommonOpenTime,
            GradeType.Rare => rareOpenTime,
            GradeType.Epic => epicOpenTime,
            GradeType.Legendary => legendaryOpenTime,
            _ => 0f
        };

        float amountWeight = itemData is CountableItemData
            ? Mathf.Sqrt(slot.Amount)
            : 1f;

        return gradeValue * amountWeight;
    }

    public bool HasHighGradeItem()
    {
        for (int i = 0; i < SlotCount; i++)
        {
            ItemSlot slot = GetSlot(i);

            if (slot == null || slot.IsEmpty)
                continue;

            if (slot.ItemData.GradeType >= highGradeStandard)
                return true;
        }

        return false;
    }
}