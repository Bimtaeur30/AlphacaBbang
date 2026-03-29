using JJH._02_Scripts.Agents.Enemies;
using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Action = Unity.Behavior.Action;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "EnemyMove", story: "[Enemy] move between [MinRange] [MaxRange]", category: "Action/Transform", id: "44f5c467c27c787202a6fa8765217d63")]
public partial class EnemyMoveAction : Action
{
    [SerializeReference] public BlackboardVariable<AbstractEnemy> Enemy;
    [SerializeReference] public BlackboardVariable<float> MinRange;
    [SerializeReference] public BlackboardVariable<float> MaxRange;



    protected override Status OnStart()
    {
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

