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

        protected override Status OnStart()
        {
            if (Enemy.Value == null)
                return Status.Failure;

            Enemy.Value.DashAttack();

            return Status.Success;
        }
    }
}

