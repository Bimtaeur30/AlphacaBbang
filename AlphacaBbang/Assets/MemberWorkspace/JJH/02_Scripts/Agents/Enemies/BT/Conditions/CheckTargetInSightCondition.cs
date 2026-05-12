using System;
using Unity.Behavior;
using UnityEngine;

namespace JJH._02_Scripts.Agents.Enemies.BT.Conditions
{
    [Serializable, Unity.Properties.GeneratePropertyBag]
    [Condition(name: "CheckTargetInSight", story: "[Target] in [Enemy] Sight", category: "Conditions", id: "ec30690beb689bb7b18a961b43403bf0")]
    public partial class CheckTargetInSightCondition : Condition
    {
        [SerializeReference] public BlackboardVariable<GameObject> Target;
        [SerializeReference] public BlackboardVariable<AbstractEnemy> Enemy;

        public override bool IsTrue()
        {
            if (Target.Value == null || Enemy.Value == null || Enemy.Value.Sensor == null)
                return false;

            return Enemy.Value.Sensor.IsTargetInSight(Enemy.Value.transform.position, Target.Value.transform);
        }
    }
}
