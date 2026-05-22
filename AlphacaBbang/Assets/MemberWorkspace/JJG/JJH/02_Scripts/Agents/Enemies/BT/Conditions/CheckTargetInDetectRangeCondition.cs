using System;
using Unity.Behavior;
using UnityEngine;

namespace JJH._02_Scripts.Agents.Enemies.BT.Conditions
{
    [Serializable, Unity.Properties.GeneratePropertyBag]
    [Condition(name: "CheckTargetInDetectRange", story: "Target in [Enemy] Detect Range", category: "Conditions", id: "609ddb9f3c8382807c372549c893ca7d")]
    public partial class CheckTargetInDetectRangeCondition : Condition
    {
        [SerializeReference] public BlackboardVariable<AbstractEnemy> Enemy;

        public override bool IsTrue()
        {
            if (Enemy.Value == null || Enemy.Value.EnemyData == null || Enemy.Value.Sensor == null)
                return false;
            return Enemy.Value.Sensor.IsTargetInRange(Enemy.Value.EnemyData.DetectRange, out Collider hitCollider);
        }
    }
}
