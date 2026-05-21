using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Action = Unity.Behavior.Action;

namespace JJH._02_Scripts.Agents.Enemies.BT.Actions
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(name: "EnemySuicide", story: "[Enemy] Suicide", category: "Action/GameObject", id: "889eff16acec1d87295edf9e88dde9fd")]
    public partial class EnemySuicideAction : Action
    {
        [SerializeReference] public BlackboardVariable<AbstractEnemy> Enemy;

        protected override Status OnStart()
        {
            if (Enemy.Value == null)
                return Status.Failure;

            Enemy.Value.Suicide();

            return Status.Success;
        }
    }
}

