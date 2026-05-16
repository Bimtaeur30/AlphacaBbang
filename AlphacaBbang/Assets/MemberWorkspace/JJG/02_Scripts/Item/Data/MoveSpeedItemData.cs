using UnityEngine;

namespace MemberWorkspace.JJG._02_Scripts.Item.Data
{
    [CreateAssetMenu(fileName = "MoveSpeedItemData", menuName = "JJK/MoveSpeedItemData", order = 9)]
    public class MoveSpeedItemData : CountableItemData
    {
        [field: SerializeField] public int SpeedAmount { get; private set; }
        [field: SerializeField] public float Duration { get; private set; }
    }
}
