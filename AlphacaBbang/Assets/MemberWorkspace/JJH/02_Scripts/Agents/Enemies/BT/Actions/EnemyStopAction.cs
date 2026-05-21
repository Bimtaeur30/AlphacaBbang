using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Action = Unity.Behavior.Action;

namespace JJH._02_Scripts.Agents.Enemies.BT.Actions
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(name: "EnemyStop", story: "[Enemy] stop", category: "Action/Transform", id: "bc204886933df1cd285b0c8db916e1a1")]
    public partial class EnemyStopAction : Action
    {
        [SerializeReference] public BlackboardVariable<AbstractEnemy> Enemy;

        protected override Status OnStart()
        {
            if (Enemy.Value == null || Enemy.Value.EnemyNavMeshAgent == null)
                return Status.Failure;

            Enemy.Value.EnemyNavMeshAgent.StopImmediately();

            return Status.Success;
        }
    }
}
