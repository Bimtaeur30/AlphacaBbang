using JJH._02_Scripts.Agents.Enemies;
using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "CheckEnemyInSmoke", story: "[Enemy] in smoke", category: "Conditions", id: "13031a14595e0b07489c92c44185766e")]
public partial class CheckEnemyInSmokeCondition : Condition
{
    [SerializeReference] public BlackboardVariable<AbstractEnemy> Enemy;

    public override bool IsTrue()
    {
        if (Enemy.Value == null)
            return false;

        return Enemy.Value.Sensor.CheckAgentInSmoke();
    }
}
