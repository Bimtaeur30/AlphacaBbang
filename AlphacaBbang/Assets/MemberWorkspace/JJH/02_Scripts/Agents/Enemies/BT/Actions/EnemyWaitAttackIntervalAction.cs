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

        private float _time;

        protected override Status OnStart()
        {
            if (Enemy == null || Enemy.Value == null || Enemy.Value.EnemyData == null)
                return Status.Failure;

            _time = 0;

            return Status.Running;
        }

        protected override Status OnUpdate()
        {
            _time += Time.deltaTime;

            if (Enemy.Value.EnemyData.AttackInterval <= _time)
                return Status.Success;

            return Status.Running;
        }
    }
}

