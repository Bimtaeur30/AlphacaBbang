using JJH._02_Scripts.Agents.Enemies.NavMeshs;
using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Action = Unity.Behavior.Action;

namespace JJH._02_Scripts.Agents.Enemies.BT.Actions
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(name: "EnemyDash", story: "[Enemy] Dash", category: "Action/Navigation", id: "922c97a6ce65f391d8d47921c29f74bf")]
    public partial class EnemyDashAction : Action
    {
        [SerializeReference] public BlackboardVariable<AbstractEnemy> Enemy;

        [SerializeReference] public BlackboardVariable<float> DashDistance = new(10f);
        [SerializeReference] public BlackboardVariable<float> DashSpeed = new(20f);

        private INavMeshAgent _navMeshAgent;

        private Vector3 _targetPosition;
        Transform _enemyTrans;

        protected override Status OnStart()
        {
            if (Enemy.Value == null || Enemy.Value.EnemyNavMeshAgent == null)
                return Status.Failure;

            _navMeshAgent = Enemy.Value.EnemyNavMeshAgent;
            _enemyTrans = Enemy.Value.transform;

            Vector3 dashDirection = _enemyTrans.forward;
            dashDirection.y = 0;
            dashDirection.Normalize();

            _targetPosition = _enemyTrans.position + dashDirection * DashDistance.Value;

            _navMeshAgent.KeepChase(false);

            return Status.Running;
        }

        protected override Status OnUpdate()
        {
            Vector3 direction = (_targetPosition - _enemyTrans.position).normalized;
            _enemyTrans.position += direction * DashSpeed.Value * Time.deltaTime;

            float distance = Vector3.Distance(_enemyTrans.position, _targetPosition);
            if (distance <= 0.2f)
            {
                _enemyTrans.position = _targetPosition;
                return Status.Success;
            }

            return Status.Running;
        }
    }
}

