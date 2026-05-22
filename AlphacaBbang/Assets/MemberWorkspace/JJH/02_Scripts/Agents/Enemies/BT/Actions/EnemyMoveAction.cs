using JJH._02_Scripts.Agents.Enemies.NavMeshs;
using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
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

            Vector3 move = Random.insideUnitSphere;
            move.y = 0f;
            move.Normalize();
            _direction = move;


            _navMeshAgent.KeepChase(true);
            _navMeshAgent.MoveTo(Enemy.Value.transform.position + move * 10f);

            return Status.Running;
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

