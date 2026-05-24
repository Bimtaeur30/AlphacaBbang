using JJH._02_Scripts.Systems.SoundSystems;
using JJH._02_Scripts.Weapons;
using UnityEngine;

namespace JJH._02_Scripts.Agents.Enemies
{
    [CreateAssetMenu(fileName = "Enemy Data", menuName = "JJH/SO/Enemy/Enemy Data", order = 0)]
    public class EnemyDataSO : ScriptableObject
    {
        [Header("Information")]
        [field: SerializeField] public string EnemyName { get; private set; }
        [field: SerializeField] public string EnemyID { get; private set; }
        [field: SerializeField] public bool IsBoss { get; private set; }
        [field: SerializeField] public float EnemyHealth { get; private set; }
        [field: SerializeField] public float EnemySpeed { get; private set; }

        [Header("Inventory")]
        [field: SerializeField] public WeaponBase[] Weapons { get; private set; }
        [field: SerializeField] public ArmorSO HelmetArmor { get; private set; }
        [field: SerializeField] public ArmorSO ShieldArmor { get; private set; }
        [field: SerializeField] public GameObject EnemyInventoryPrefab { get; private set; }

        [Header("Attack")]
        [field: SerializeField] public float AttackRange { get; private set; }
        [field: SerializeField] public float DetectRange { get; private set; }
        [field: SerializeField] public float AttackTime { get; private set; }
        [field: SerializeField] public float AttackInterval { get; private set; }

        [Header("Sound")]
        [field: SerializeField] public SoundClipSO FindSound { get; private set; }
    }
}