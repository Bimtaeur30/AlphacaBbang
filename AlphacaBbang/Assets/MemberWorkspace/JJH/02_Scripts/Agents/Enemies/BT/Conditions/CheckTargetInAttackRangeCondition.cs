using System;
using Unity.Behavior;
using UnityEngine;

namespace JJH._02_Scripts.Agents.Enemies.BT.Conditions
{
    [Serializable, Unity.Properties.GeneratePropertyBag]
    [Condition(name: "CheckTargetInAttackRange", story: "[Target] in [Enemy] Attack Range", category: "Conditions", id: "40b5cad510dca641dd9198775fb9b335")]
    public partial class CheckTargetInAttackRangeCondition : Condition
    {
        [SerializeReference] public BlackboardVariable<GameObject> Target;
        [SerializeReference] public BlackboardVariable<AbstractEnemy> Enemy;

        public override bool IsTrue()
        {
            if (Target == null || Target.Value == null ||
               Enemy == null || Enemy.Value == null || Enemy.Value.EnemyData == null || Enemy.Value.Sensor == null)
                return false;

            return Enemy.Value.Sensor.IsTargetInRange(Enemy.Value.EnemyData.AttackRange, out Collider hitCollider);
        }
    }
}

