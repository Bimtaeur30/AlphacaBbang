using MemberWorkspace.JJG._02_Scripts;
using UnityEngine;

public class LootBoxContainer : ItemContainer
{
    [Header("Loot Open Time")]
    [SerializeField] private float baseOpenTime = 1.5f;

    [Header("High Grade Effect")]
    [SerializeField] private GradeType highGradeStandard = GradeType.Rare;

    public float RequiredOpenTime { get; private set; }

    protected override void Awake()
    {
        base.Awake();

        GenerateLoot();
        RequiredOpenTime = CalculateOpenTime();
    }

    private void GenerateLoot()
    {
        // 임시 예시
        // AddItem(itemData, amount);
        // 나중에 LootTable에서 랜덤으로 채우면 됨
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
            GradeType.Common => 0.2f,
            GradeType.UnCommon => 0.5f,
            GradeType.Rare => 1.0f,
            GradeType.Epic => 2.0f,
            GradeType.Legendary => 4.0f,
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