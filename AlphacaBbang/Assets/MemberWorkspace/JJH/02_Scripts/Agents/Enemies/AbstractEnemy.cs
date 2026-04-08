using MemberWorkspace.JJH._02_Scripts.Agents.Enemies.NavMesh;
using UnityEngine;

namespace JJH._02_Scripts.Agents.Enemies
{
    public abstract class AbstractEnemy : Agent, IDamageable
    {
        [field: SerializeField] public AttackDataSO AttackData { get; private set; }
        public INavMeshAgent NavMeshAgent { get; private set; }

        protected override void InitializeComponents()
        {
            base.InitializeComponents();
            NavMeshAgent = GetModule<INavMeshAgent>();
        }

        public void ApplyBurn(float dps, float duration)
        {
            Debug.Log("적 불탐");
        }

        public void TakeDamage(float damage)
        {
            Debug.Log("적 데미지 받음");
        }
    }
}
