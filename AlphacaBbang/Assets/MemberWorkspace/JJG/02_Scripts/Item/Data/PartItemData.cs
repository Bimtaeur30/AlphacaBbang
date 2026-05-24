using UnityEngine;

namespace MemberWorkspace.JJG._02_Scripts.Item.Data
{
    [CreateAssetMenu(fileName = "PartItemData", menuName = "JJK/PartItemData", order = 0)]
    public class PartItemData : ScriptableObject
    {
        [field:SerializeField] public int PartID { get; private set; }
    }
}