using MemberWorkspace.JJH._02_Scripts.Agents.Enemies.NavMesh;
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

        private float _moveDuration;
        private float _moveTime;

        protected override Status OnStart()
        {
            if (Enemy.Value == null || Enemy.Value.NavMeshAgent == null)
                return Status.Failure;

            _navMeshAgent = Enemy.Value.NavMeshAgent;

            _moveTime = 0;
            _moveDuration = Random.Range(1f, 2f);

            Vector3 move = Random.insideUnitSphere;
            move.y = 0f;
            move.Normalize();

            _navMeshAgent.KeepChase(true);
            _navMeshAgent.MoveTo(Enemy.Value.transform.position + move * 10f);

            return Status.Running;
        }

        protected override Status OnUpdate()
        {
            _moveTime += Time.deltaTime;
            if (_moveTime > _moveDuration)
            {
                _navMeshAgent.KeepChase(false);
                return Status.Success;
            }

            return Status.Running;
        }
    }
}

