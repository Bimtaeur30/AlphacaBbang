using System;
using Unity.Behavior;
using UnityEngine;

namespace JJH._02_Scripts.Agents.Enemies.BT.Conditions
{
    [Serializable, Unity.Properties.GeneratePropertyBag]
    [Condition(name: "CheckTargetIsClose", story: "[Target] is close to [Enemy]", category: "Conditions", id: "1185886aa85abd980cfa0c937c00fe51")]
    public partial class CheckTargetIsCloseCondition : Condition
    {
        [SerializeReference] public BlackboardVariable<GameObject> Target;
        [SerializeReference] public BlackboardVariable<AbstractEnemy> Enemy;

        private float _closeDistance = 10f;

        public override bool IsTrue()
        {
            if (Target.Value == null || Enemy.Value == null || Enemy.Value.Sensor == null)
                return false;

            return Enemy.Value.Sensor.IsTargetInRange(_closeDistance, out Collider hitCollider);
        }
    }
}
