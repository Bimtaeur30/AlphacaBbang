using UnityEngine;
namespace MemberWorkspace.JJG._02_Scripts.Item.Data
{
    [CreateAssetMenu(fileName = "ThrowingItemData", menuName = "JJK/ThrowingItemData", order = 0)]
    public class ThrowingItemData : CountableItemData
    {
        [field: SerializeField] public GrenadeSO Grenade { get; private set; }
        [field: SerializeField] public GrenadeBehavior GrenadeBehaviorPrefab { get; private set; }
    }
}