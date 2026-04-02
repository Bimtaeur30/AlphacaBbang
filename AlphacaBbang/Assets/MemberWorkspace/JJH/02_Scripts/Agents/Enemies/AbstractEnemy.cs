using UnityEngine;

namespace JJH._02_Scripts.Agents.Enemies
{
    public abstract class AbstractEnemy : Agent
    {
        [field: SerializeField] public AttackDataSO AttackData { get; private set; }
    }
}
