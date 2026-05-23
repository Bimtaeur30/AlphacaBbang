using JJH._02_Scripts.Agents.Enemies.Skills;
using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Action = Unity.Behavior.Action;

namespace JJH._02_Scripts.Agents.Enemies.BT.Actions
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(name: "EnemyDashAttack", story: "[Enemy] DashAttack", category: "Action", id: "2041cff7f812a12eea70b35d7a2152e8")]
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

            Enemy.Value.EnemySkill.UseSkill<EnemyDashAttackSkill>();

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

