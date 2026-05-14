using JJH._02_Scripts.Agents.Enemies.Skills;
using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Action = Unity.Behavior.Action;

namespace JJH._02_Scripts.Agents.Enemies.BT.Actions
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(name: "EnemyDashAttack", story: "[Enemy] Dash Attack", category: "Action", id: "7255976bfeb59242f0d5a6bb9526d63b")]
    public partial class EnemyDashAttackAction : Action
    {
        [SerializeReference] public BlackboardVariable<AbstractEnemy> Enemy;

        private float _cooldown = 3f;
        private float _lastUseTime = -999f;

        protected override Status OnStart()
        {
            if (Enemy.Value == null)
                return Status.Failure;

            if (Time.time < _lastUseTime + _cooldown)
                return Status.Failure;

            _lastUseTime = Time.time;
            Enemy.Value.DashAttack();

            return Status.Running;
        }

        protected override Status OnUpdate()
        {
            if (Enemy.Value == null)
                return Status.Failure;

            EnemyDashAttackSkill dashSkill =
                Enemy.Value.EnemySkill.GetSkill<EnemyDashAttackSkill>() as EnemyDashAttackSkill;

            return dashSkill != null && dashSkill.IsDashing ? Status.Running : Status.Success;
        }
    }
}

