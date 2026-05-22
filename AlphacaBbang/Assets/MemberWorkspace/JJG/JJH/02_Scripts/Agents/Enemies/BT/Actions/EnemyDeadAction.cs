using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Action = Unity.Behavior.Action;

namespace JJH._02_Scripts.Agents.Enemies.BT.Actions
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(name: "EnemyDead", story: "[Enemy] Dead", category: "Action/GameObject", id: "82a5e348f50e8aea142951dcab7005ef")]
    public partial class EnemyDeadAction : Action
    {
        [SerializeReference] public BlackboardVariable<AbstractEnemy> Enemy;

        protected override Status OnStart()
        {
            if (Enemy.Value == null)
                return Status.Failure;

            Enemy.Value.OnDead();

            return Status.Success;
        }
    }
}

