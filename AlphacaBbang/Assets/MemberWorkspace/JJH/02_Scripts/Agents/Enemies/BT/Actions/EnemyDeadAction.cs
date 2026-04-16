using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Action = Unity.Behavior.Action;

namespace JJH._02_Scripts.Agents.Enemies.BT.Actions
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(name: "EnemyDeadAction", story: "[Enemy] Dead", category: "Action/GameObject", id: "313e0e8be66b53f1a4e0c123bb19880b")]
    public partial class EnemyDeadAction : Action
    {
        [SerializeReference] public BlackboardVariable<AbstractEnemy> Enemy;

        protected override Status OnStart()
        {
            if (Enemy == null || Enemy.Value == null)
                return Status.Failure;

            Enemy.Value.OnDead();

            return Status.Success;
        }
    }
}

