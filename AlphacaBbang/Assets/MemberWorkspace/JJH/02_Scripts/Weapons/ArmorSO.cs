using UnityEngine;

namespace JJH._02_Scripts.Weapons
{
    [CreateAssetMenu(fileName = "Armor", menuName = "JJH/SO/Weapon/Armor", order = 0)]
    public class ArmorSO : ScriptableObject
    {
        [field: SerializeField] public int ArmorLevel { get; private set; } = 1;
        [field: SerializeField] public float DamageReductionRate { get; private set; } = 0.1f;
        [field: SerializeField] public float ArmorDurability { get; private set; } = 100f;
        [field: SerializeField] public GameObject ArmorModel { get; private set; }
        [field: SerializeField] public ArmorType ArmorType { get; private set; }
    }

    public enum ArmorType
    {
        Helmet, Body
    }
}