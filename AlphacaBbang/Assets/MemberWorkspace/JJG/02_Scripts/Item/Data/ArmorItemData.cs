using JJH._02_Scripts.Weapons;
using UnityEngine;

namespace MemberWorkspace.JJG._02_Scripts.Item.Data
{
    [CreateAssetMenu(fileName = "ArmorItemData", menuName = "JJK/ArmorItemData", order = 0)]
    public class ArmorItemData : ItemData
    {
        [field: SerializeField] public ArmorSO Armor { get; private set; }
    }
}