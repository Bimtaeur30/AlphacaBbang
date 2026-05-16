using UnityEngine;

namespace MemberWorkspace.JJG._02_Scripts.Item.Data
{
    [CreateAssetMenu(fileName = "HotMedicineItemData", menuName = "JJK/HotMedicineItemData", order = 8)]
    public class HotMedicineItemData : CountableItemData
    {
        [field: SerializeField] public int HealAmountPerTick { get; private set; }
        [field: SerializeField] public int TickCount { get; private set; } = 5;
        [field: SerializeField] public float TickInterval { get; private set; } = 0.5f;
    }
}
