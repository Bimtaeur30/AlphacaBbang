using JJH._02_Scripts.Agents.Enemies;
using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "CheckEnemyInSmoke", story: "[Enemy] [In] smoke", category: "Conditions", id: "13031a14595e0b07489c92c44185766e")]
public partial class CheckEnemyInSmokeCondition : Condition
{
    [SerializeReference] public BlackboardVariable<AbstractEnemy> Enemy;
    [SerializeReference] public BlackboardVariable<bool> In;

    public override bool IsTrue()
    {
        if (Enemy.Value == null)
            return false;

        bool isInSmoke = Enemy.Value.Sensor.CheckAgentInSmoke();

        return In.Value ? isInSmoke : !isInSmoke;
    }
}
