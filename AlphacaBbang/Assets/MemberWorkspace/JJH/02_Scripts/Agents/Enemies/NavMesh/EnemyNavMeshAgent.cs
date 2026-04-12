using UnityEngine;
using UnityEngine.AI;

namespace MemberWorkspace.JJH._02_Scripts.Agents.Enemies.NavMesh
{
    public class EnemyNavMeshAgent : MonoBehaviour, IModule, INavMeshAgent
    {
        private ModuleOwner _owner;
        private NavMeshAgent _navMeshAgent;

        public void Initialize(ModuleOwner owner)
        {
            _owner = owner;
            _navMeshAgent = GetComponentInParent<NavMeshAgent>();

            _navMeshAgent.updateRotation = false;
        }

        public void MoveTo(Vector3 targetPosition)
        {
            _navMeshAgent.SetDestination(targetPosition);
        }

        public void KeepChase(bool value)
        {
            _navMeshAgent.isStopped = !value;
        }
    }
}