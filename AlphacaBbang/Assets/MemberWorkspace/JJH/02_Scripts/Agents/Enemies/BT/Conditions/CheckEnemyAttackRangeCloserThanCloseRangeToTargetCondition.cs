using JJH._02_Scripts.Agents.Enemies;
using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "CheckEnemyAttackRangeCloserThanCloseRangeToTarget", story: "[Enemy] attackrange closer than [CloseRange] to [Target]", category: "Conditions", id: "a77bb287e3d3543d9f54de2aa65f9323")]
public partial class CheckEnemyAttackRangeCloserThanCloseRangeToTargetCondition : Condition
{
    [SerializeReference] public BlackboardVariable<AbstractEnemy> Enemy;
    [SerializeReference] public BlackboardVariable<float> CloseRange;
    [SerializeReference] public BlackboardVariable<GameObject> Target;

    public override bool IsTrue()
    {
        if (Enemy.Value == null || CloseRange.Value <= 0 || Target.Value == null)
            return false;

        float distance = Vector3.Distance(Enemy.Value.transform.position, Target.Value.transform.position);
        float distanceToAttackRange = Mathf.Abs(distance - Enemy.Value.EnemyData.AttackRange);
        float distanceToCloseRange = Mathf.Abs(distance - CloseRange.Value);

        return distanceToAttackRange < distanceToCloseRange;
    }
}
