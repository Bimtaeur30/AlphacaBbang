using UnityEngine;
using UnityEngine.AI;

namespace JJH._02_Scripts.Agents.Enemies.NavMeshs
{
    public class EnemyNavMeshAgent : MonoBehaviour, IModule, INavMeshAgent
    {
        [field: SerializeField] public NavMeshAgent NavMeshAgent { get; private set; }
        private AbstractEnemy _enemy;

        public void Initialize(ModuleOwner owner)
        {
            _enemy = owner as AbstractEnemy;
            NavMeshAgent = GetComponentInParent<NavMeshAgent>();
            NavMeshAgent.speed = _enemy.EnemyData.EnemySpeed;
            NavMeshAgent.updateRotation = false;
            NavMeshAgent.autoBraking = true;
        }

        public void MoveTo(Vector3 targetPosition)
        {
            NavMeshAgent.SetDestination(targetPosition);
        }

        public void KeepChase(bool value)
        {
            NavMeshAgent.isStopped = !value;


            if (!value)
            {
                NavMeshAgent.ResetPath();
                NavMeshAgent.velocity = Vector3.zero;
            }
        }
    }
}