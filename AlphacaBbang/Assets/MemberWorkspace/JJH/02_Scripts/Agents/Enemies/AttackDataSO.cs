using UnityEngine;

namespace JJH._02_Scripts.Agents.Enemies
{
    [CreateAssetMenu(fileName = "Attack Data", menuName = "JJH/SO/Enemy/Attack Data", order = 0)]
    public class AttackDataSO : ScriptableObject
    {
        [field: SerializeField] public float Damage { get; private set; }
        [field: SerializeField] public float DetectRange { get; private set; }
        [field: SerializeField] public float StoppingDistance { get; private set; }
    }
}