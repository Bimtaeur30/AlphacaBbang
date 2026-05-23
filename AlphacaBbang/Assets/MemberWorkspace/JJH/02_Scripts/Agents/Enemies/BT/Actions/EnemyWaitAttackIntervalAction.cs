using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Action = Unity.Behavior.Action;

namespace JJH._02_Scripts.Agents.Enemies.BT.Actions
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(name: "EnemyWaitAttackInterval", story: "[Enemy] Wait for AttackInterval", category: "Action/Delay", id: "73cdfaccb90e4b7b6393b45c8e0f8c8b")]
    public partial class EnemyWaitAttackIntervalAction : Action
    {
        [SerializeReference] public BlackboardVariable<AbstractEnemy> Enemy;
        [SerializeReference] public BlackboardVariable<float> AttackTimer;

        protected override Status OnStart()
        {
            if (Enemy.Value == null || Enemy.Value.EnemyData == null)
                return Status.Failure;

            AttackTimer.Value = 0;

            return Status.Running;
        }

        protected override Status OnUpdate()
        {
            AttackTimer.Value += Time.deltaTime;

            if (AttackTimer.Value >= Enemy.Value.EnemyData.AttackInterval)
            {
                AttackTimer.Value = 0f;
                return Status.Success;
            }

            return Status.Running;
        }
    }
}

