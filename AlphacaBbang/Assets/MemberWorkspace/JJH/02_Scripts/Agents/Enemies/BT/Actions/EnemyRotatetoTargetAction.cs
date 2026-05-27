using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Action = Unity.Behavior.Action;

namespace JJH._02_Scripts.Agents.Enemies.BT.Actions
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(name: "EnemyRotatetoTarget", story: "[Enemy] roate to [Target]", category: "Action/Physics", id: "82bae88539c84a07b314f5783f1c44bd")]
    public partial class EnemyRotatetoTargetAction : Action
    {
        [SerializeReference] public BlackboardVariable<AbstractEnemy> Enemy;
        [SerializeReference] public BlackboardVariable<GameObject> Target;

        [SerializeReference] public BlackboardVariable<float> rotateSpeed = new(360f);
        [SerializeReference] public BlackboardVariable<float> minRotateDistance = new(0.5f);

        protected override Status OnUpdate()
        {
            if (Enemy.Value == null || Target.Value == null)
                return Status.Failure;

            Transform enemyTrm = Enemy.Value.transform;

            Vector3 direction = Target.Value.transform.position - enemyTrm.position;
            direction.y = 0f;
            float sqrDistance = direction.sqrMagnitude;
            if (sqrDistance < minRotateDistance * minRotateDistance)
                return Status.Running;

            Quaternion targetRotation = Quaternion.LookRotation(direction.normalized);
            enemyTrm.rotation = Quaternion.RotateTowards(enemyTrm.rotation, targetRotation, rotateSpeed * Time.deltaTime);

            return Status.Running;
        }
    }
}

