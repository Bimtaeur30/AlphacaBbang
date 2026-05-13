using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Action = Unity.Behavior.Action;

namespace JJH._02_Scripts.Agents.Enemies.BT.Actions
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(name: "EnemyFind", story: "[Enemy] Find", category: "Action", id: "71a434bfab67dd38c5bbdca399c0605d")]
    public partial class EnemyFindAction : Action
    {
        [SerializeReference] public BlackboardVariable<AbstractEnemy> Enemy;

        protected override Status OnStart()
        {
            if (Enemy.Value == null || Enemy.Value.EnemyInterface == null)
                return Status.Failure;

            Enemy.Value.EnemyInterface.EnemyTalk.ShowText();

            return Status.Success;
        }
    }
}

