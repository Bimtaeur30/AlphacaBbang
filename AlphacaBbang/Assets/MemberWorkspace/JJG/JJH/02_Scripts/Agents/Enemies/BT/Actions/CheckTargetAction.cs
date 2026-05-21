using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Action = Unity.Behavior.Action;

namespace JJH._02_Scripts.Agents.Enemies.BT.Actions
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(name: "CheckTarget", story: "[Enemy] check [Target]", category: "Action/Find", id: "bf2afb4a94766a16b003a59e6ea59369")]
    public partial class CheckTargetAction : Action
    {
        [SerializeReference] public BlackboardVariable<AbstractEnemy> Enemy;
        [SerializeReference] public BlackboardVariable<GameObject> Target;

        private ISensor _sensor;

        protected override Status OnStart()
        {
            if (Enemy.Value == null || Enemy.Value.Sensor == null || Enemy.Value.EnemyData == null || Target == null)
                return Status.Failure;

            _sensor = Enemy.Value.Sensor;
            EnemyDataSO attackData = Enemy.Value.EnemyData;

            if (_sensor.IsTargetInRange(attackData.DetectRange, out Collider hitCollider))
            {
                Target.Value = hitCollider.gameObject;
                return Status.Success;
            }

            return Status.Failure;
        }
    }
}
