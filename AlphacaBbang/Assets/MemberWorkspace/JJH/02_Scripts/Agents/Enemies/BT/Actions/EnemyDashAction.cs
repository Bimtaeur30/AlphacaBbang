using JJH._02_Scripts.Agents.Enemies.Skills;
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

        private float _cooldown = 3f;
        private float _lastUseTime = -999f;

        protected override Status OnStart()
        {
            if (Enemy.Value == null)
                return Status.Failure;

            if (Time.time < _lastUseTime + _cooldown)
                return Status.Failure;

            _lastUseTime = Time.time;

            Enemy.Value.Dash();

            return Status.Running;
        }

        protected override Status OnUpdate()
        {
            if (Enemy.Value == null)
                return Status.Failure;

            EnemyDashSkill dashSkill = Enemy.Value.EnemySkill.GetSkill<EnemyDashSkill>() as EnemyDashSkill;

            return dashSkill != null && dashSkill.IsDashing ? Status.Running : Status.Success;
        }
    }
}

