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
    [NodeDescription(name: "EnemyMove", story: "[Enemy] move", category: "Action", id: "cc5e290088f824b2d52ffb47f06a8ce0")]
    public partial class EnemyMoveAction : Action
    {
        [SerializeReference] public BlackboardVariable<AbstractEnemy> Enemy;

        private INavMeshAgent _navMeshAgent;

        private Vector3 _direction;
        private float _moveDuration;
        private float _moveTime;

        protected override Status OnStart()
        {
            if (Enemy.Value == null || Enemy.Value.EnemyNavMeshAgent == null)
                return Status.Failure;

            _navMeshAgent = Enemy.Value.EnemyNavMeshAgent;

            _moveTime = 0;
            _moveDuration = Random.Range(1f, 2f);

            Vector3 randomDirection = Random.insideUnitSphere * 10f;
            randomDirection += Enemy.Value.transform.position;

            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomDirection, out hit, 10f, NavMesh.AllAreas))
            {
                NavMeshPath path = new NavMeshPath();

                if (NavMesh.CalculatePath(Enemy.Value.transform.position, hit.position, NavMesh.AllAreas, path)
                    && path.status == NavMeshPathStatus.PathComplete)
                {
                    _direction = (hit.position - Enemy.Value.transform.position).normalized;
                    _direction.y = 0f;

                    _navMeshAgent.KeepChase(true);
                    _navMeshAgent.MoveTo(hit.position);

                    return Status.Running;
                }
            }

            return Status.Failure;
        }

        protected override Status OnUpdate()
        {
            _moveTime += Time.deltaTime;
            Quaternion targetRotation = Quaternion.LookRotation(_direction);
            Enemy.Value.transform.rotation =
                Quaternion.Slerp(Enemy.Value.transform.rotation, targetRotation, 5f * Time.deltaTime);
            if (_moveTime > _moveDuration)
            {
                _navMeshAgent.KeepChase(false);
                return Status.Success;
            }

            return Status.Running;
        }
    }
}

