using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Action = Unity.Behavior.Action;

namespace JJH._02_Scripts.Agents.Enemies.BT.Actions
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(name: "EnemyDashAttackStandby", story: "[Enemy] DashAttack standby [Time]", category: "Action/Animation", id: "19b81acb12632dcbf4b30d74ffecae10")]
    public partial class EnemyDashAttackStandbyAction : Action
    {
        [SerializeReference] public BlackboardVariable<AbstractEnemy> Enemy;
        [SerializeReference] public BlackboardVariable<float> Time;

        private float _nowTime = 0f;

        protected override Status OnStart()
        {
            if (Time.Value < 0 || Enemy.Value == null)
                return Status.Failure;

            Enemy.Value.DashAttackStandBy();

            return Status.Running;
        }

        protected override Status OnUpdate()
        {
            _nowTime += UnityEngine.Time.time;
            if (_nowTime > Time)
            {
                return Status.Success;
            }
            return Status.Running;
        }
    }
}


