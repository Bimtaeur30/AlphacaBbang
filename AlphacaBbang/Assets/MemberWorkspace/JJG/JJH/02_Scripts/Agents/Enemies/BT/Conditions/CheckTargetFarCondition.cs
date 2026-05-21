using System;
using Unity.Behavior;
using UnityEngine;

namespace JJH._02_Scripts.Agents.Enemies.BT.Conditions
{
    [Serializable, Unity.Properties.GeneratePropertyBag]
    [Condition(name: "CheckTargetFar", story: "[Target] is Far from [Enemy]", category: "Conditions", id: "a4cd70b6a8c2f9d9b6b7da8b74b44791")]
    public partial class CheckTargetFarCondition : Condition
    {
        [SerializeReference] public BlackboardVariable<GameObject> Target;
        [SerializeReference] public BlackboardVariable<AbstractEnemy> Enemy;

        public override bool IsTrue()
        {
            if (Target.Value == null || Enemy.Value == null || Enemy.Value.EnemyData == null || Enemy.Value.Sensor == null)
                return false;

            return Enemy.Value.Sensor.IsTargetInRange(Enemy.Value.EnemyData.DetectRange, out Collider hitCollider);
        }
    }
}
