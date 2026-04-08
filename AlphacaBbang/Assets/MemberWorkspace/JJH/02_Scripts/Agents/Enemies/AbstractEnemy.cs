using MemberWorkspace.JJH._02_Scripts.Agents.Enemies.NavMesh;
using UnityEngine;

namespace JJH._02_Scripts.Agents.Enemies
{
    public abstract class AbstractEnemy : Agent
    {
        [field: SerializeField] public AttackDataSO AttackData { get; private set; }
        public INavMeshAgent NavMeshAgent { get; private set; }

        protected override void InitializeComponents()
        {
            base.InitializeComponents();
            NavMeshAgent = GetModule<INavMeshAgent>();
        }
    }
}
