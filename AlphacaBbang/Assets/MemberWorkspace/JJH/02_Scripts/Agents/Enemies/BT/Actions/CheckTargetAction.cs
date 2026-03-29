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

        protected override Status OnStart()
        {
            if (Enemy.Value == null || Target.Value == null)
                return Status.Failure;

            AgentSensor sensor = Enemy.Value.Sensor;
            AttackDataSO attackConfig = Enemy.Value.AttackData;

            if (sensor.IsTargetInRange(attackConfig.DetectRange, out Collider hitCollider)
                && sensor.IsTargetInSight(Enemy.Value.transform.position, attackConfig.DetectRange, hitCollider))
            {
                Target.Value = hitCollider.gameObject;
                Debug.Log("Target set");
                return Status.Success;
            }

            return Status.Failure;
        }
    }
}
