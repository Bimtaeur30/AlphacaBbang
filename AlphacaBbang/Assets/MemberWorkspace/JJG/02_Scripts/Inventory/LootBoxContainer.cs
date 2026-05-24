using MemberWorkspace.JJG._02_Scripts;
using UnityEngine;
 
public class LootBoxContainer : ItemContainer
{
    [SerializeField] private LootTable lootTable;
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
 
        // 인스펙터에서 직접 LootTable을 할당한 경우 Awake에서 바로 초기화
        if (lootTable != null)
        {
            GenerateLoot(lootTable);
            RequiredOpenTime = CalculateOpenTime();
        }
    }
 
    /// <summary>
    /// LootBoxInteractor가 LootBox 컴포넌트를 통해 동적으로 초기화할 때 호출.
    /// Awake 이후에 호출되므로 슬롯은 이미 준비된 상태입니다.
    /// </summary>
    public void InitializeWithLootTable(LootTable table)
    {
        if (table == null)
        {
            Debug.LogWarning("[LootBoxContainer] InitializeWithLootTable에 null이 전달됐습니다.");
            return;
        }
 
        lootTable = table;
        GenerateLoot(lootTable);
        RequiredOpenTime = CalculateOpenTime();
    }
 
    private void GenerateLoot(LootTable table)
    {
        if (table == null)
        {
            Debug.LogWarning($"[LootBoxContainer] {name}에 LootTable이 없습니다.");
            return;
        }
 
        table.GenerateLoot(this);
    }
 
    private float CalculateOpenTime()
    {
        float totalTime = baseOpenTime;
 
        for (int i = 0; i < SlotCount; i++)
        {
            ItemSlot slot = GetSlot(i);
            if (slot == null || slot.IsEmpty) continue;
            totalTime += CalculateSlotOpenTime(slot);
        }
 
        return totalTime;
    }
 
    private float CalculateSlotOpenTime(ItemSlot slot)
    {
        ItemData itemData = slot.ItemData;
        if (itemData == null) return 0f;
 
        float gradeValue = itemData.GradeType switch
        {
            GradeType.Common    => commonOpenTime,
            GradeType.UnCommon  => unCommonOpenTime,
            GradeType.Rare      => rareOpenTime,
            GradeType.Epic      => epicOpenTime,
            GradeType.Legendary => legendaryOpenTime,
            _                   => 0f
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
            if (slot == null || slot.IsEmpty) continue;
            if (slot.ItemData.GradeType >= highGradeStandard)
                return true;
        }
 
        return false;
    }
}