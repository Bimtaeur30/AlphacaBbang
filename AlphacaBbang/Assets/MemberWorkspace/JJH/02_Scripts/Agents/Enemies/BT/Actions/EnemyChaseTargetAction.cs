using JJH._02_Scripts.Agents.Enemies.NavMeshs;
using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using UnityEngine.AI;
using Action = Unity.Behavior.Action;

namespace JJH._02_Scripts.Agents.Enemies.BT.Actions
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(name: "EnemyChaseTarget", story: "[Enemy] chase [Target]", category: "Action/Transform", id: "9b645b3decf4371eed59eff87eb6faa7")]
    public partial class EnemyChaseTargetAction : Action
    {
        [SerializeReference] public BlackboardVariable<AbstractEnemy> Enemy;
        [SerializeReference] public BlackboardVariable<GameObject> Target;

        private INavMeshAgent _navMeshAgent;
        private ISensor _sensor;

        private Vector3 _enemyPos;
        private Vector3 _targetPos;

        protected override Status OnStart()
        {
            if (Enemy.Value == null || Enemy.Value.EnemyNavMeshAgent == null || Enemy.Value.Sensor == null || Target.Value == null)
                return Status.Failure;

            _navMeshAgent = Enemy.Value.EnemyNavMeshAgent;
            _sensor = Enemy.Value.Sensor;

            return Status.Running;
        }

        protected override Status OnUpdate()
        {
            if (Target.Value == null)
                return Status.Failure;

            _enemyPos = Enemy.Value.transform.position;
            _targetPos = Target.Value.transform.position;
            if (NavMesh.SamplePosition(_targetPos, out NavMeshHit hit, 2f, NavMesh.AllAreas))
            {
                _targetPos = hit.position;
            }

            float distance = Vector3.Distance(_targetPos, _enemyPos);

            if (_sensor.IsTargetInSight(_enemyPos, Target.Value.transform) &&
                distance <= Enemy.Value.EnemyData.AttackRange)
            {
                _navMeshAgent.KeepChase(false);
                return Status.Success;
            }

            _navMeshAgent.KeepChase(true);
            _navMeshAgent.MoveTo(_targetPos);

            return Status.Running;
        }
    }
}


