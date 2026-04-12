using UnityEngine;

namespace JJH._02_Scripts.Agents.Enemies
{
    [CreateAssetMenu(fileName = "Enemy Data", menuName = "JJH/SO/Enemy/Enemy Data", order = 0)]
    public class EnemyDataSO : ScriptableObject
    {
        [Header("Information")]
        [field: SerializeField] public string EnemyName { get; private set; }
        [field: SerializeField] public float EnemyHealth { get; private set; }


        [Header("Attacl")]
        [field: SerializeField] public float Damage { get; private set; }
        [field: SerializeField] public float DetectRange { get; private set; }
        [field: SerializeField] public float UnDetectRange { get; private set; }
        [field: SerializeField] public float StoppingDistance { get; private set; }
        [field: SerializeField] public float AttackTime { get; private set; }
    }
}