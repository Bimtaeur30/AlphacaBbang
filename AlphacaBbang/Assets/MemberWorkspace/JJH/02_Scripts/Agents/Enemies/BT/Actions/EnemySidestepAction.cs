using JJH._02_Scripts.Agents.Enemies.NavMeshs;
using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using UnityEngine.AI;
using Action = Unity.Behavior.Action;
using Random = UnityEngine.Random;

namespace JJH._02_Scripts.Agents.Enemies.BT.Actions
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(name: "EnemySidestep", story: "[Enemy] Sidestep", category: "Action/Navigation", id: "1798f379016915d45fb30164e405629b")]
    public partial class EnemySidestepAction : Action
    {
        [SerializeReference] public BlackboardVariable<AbstractEnemy> Enemy;

        [SerializeReference] public BlackboardVariable<float> SidestepDistance = new(5f);

        private INavMeshAgent _navMeshAgent;

        private Transform _enemyTrans;
        private Vector3 _targetPosition;

        protected override Status OnStart()
        {
            if (Enemy.Value == null)
                return Status.Failure;

            _navMeshAgent = Enemy.Value.EnemyNavMeshAgent;
            _enemyTrans = Enemy.Value.transform;

            if (!TryGetSideStepPosition(out _targetPosition))
                return Status.Failure;

            _navMeshAgent.MoveTo(_targetPosition);

            return Status.Running;
        }

        protected override Status OnUpdate()
        {
            Vector3 direction = _targetPosition - _enemyTrans.position;
            direction.y = 0f;
            if (direction.sqrMagnitude <= 0.25f)
            {
                _navMeshAgent.KeepChase(false);
                return Status.Success;
            }

            return Status.Running;
        }

        private bool TryGetSideStepPosition(out Vector3 targetPosition)
        {
            Vector3 sideDirection = Random.Range(0, 2) == 0 ? -_enemyTrans.right : _enemyTrans.right;

            if (TryGetValidPosition(sideDirection, out targetPosition))
                return true;

            sideDirection = -sideDirection;

            if (TryGetValidPosition(sideDirection, out targetPosition))
                return true;

            targetPosition = Vector3.zero;

            return false;
        }

        private bool TryGetValidPosition(Vector3 sideDirection, out Vector3 targetPosition)
        {
            Vector3 rawTargetPosition = _enemyTrans.position + sideDirection * SidestepDistance;

            if (NavMesh.SamplePosition(rawTargetPosition, out NavMeshHit hit, 2f, NavMesh.AllAreas))
            {
                NavMeshPath path = new NavMeshPath();

                if (NavMesh.CalculatePath(_enemyTrans.position, hit.position, NavMesh.AllAreas, path)
                    && path.status == NavMeshPathStatus.PathComplete)
                {
                    targetPosition = hit.position;
                    return true;
                }
            }

            targetPosition = Vector3.zero;
            return false;
        }
    }
}
