using UnityEngine;

namespace MemberWorkspace.JJG._02_Scripts.Item.Data
{
    [CreateAssetMenu(fileName = "MedicineItemData", menuName = "JJK/MedicineItemData", order = 7)]
    public class MedicineItemData : CountableItemData
    {
        [field: SerializeField] public int HealAmount { get; private set; }
    }
}