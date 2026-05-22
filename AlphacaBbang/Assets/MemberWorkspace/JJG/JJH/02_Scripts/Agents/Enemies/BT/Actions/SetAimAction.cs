using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Action = Unity.Behavior.Action;

namespace JJH._02_Scripts.Agents.Enemies.BT.Actions
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(name: "SetAim", story: "[Enemy] set aim [Value]", category: "Action/GameObject", id: "0802b5156a7482f5fbba1f3360e76517")]
    public partial class SetAimAction : Action
    {
        [SerializeReference] public BlackboardVariable<AbstractEnemy> Enemy;
        [SerializeReference] public BlackboardVariable<bool> Value;

        protected override Status OnStart()
        {
            if (Enemy == null || Enemy.Value == null || Enemy.Value.Weapon == null)
                return Status.Failure;

            Enemy.Value.Weapon.SetAim(Value);
            return Status.Success;
        }
    }
}

