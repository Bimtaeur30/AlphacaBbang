using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Action = Unity.Behavior.Action;

namespace JJH._02_Scripts.Agents.Enemies.BT.Actions
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(name: "EnemyChaseTarget", story: "[Enemy] chase [Target]", category: "Action/Transform", id: "9b645b3decf4371eed59eff87eb6faa7")]
    public partial class EnemyChaseTargetAction : Action
    {
        [SerializeReference] public BlackboardVariable<AbstractEnemy> Enemy;
        [SerializeReference] public BlackboardVariable<GameObject> Target;

        private IControllerMovement _movement;
        private Transform _enemyTrans;
        private Transform _targetTrans;

        protected override Status OnStart()
        {
            if (Enemy.Value == null || Enemy.Value.Movement == null || Target.Value == null)
                return Status.Failure;

            _movement = Enemy.Value.Movement;
            _enemyTrans = Enemy.Value.transform;
            _targetTrans = Target.Value.transform;

            if (IsTargetInStoppingDistance())
                return Status.Success;

            return Status.Running;
        }

        protected override Status OnUpdate()
        {
            Vector2 direction = _targetTrans.position - _enemyTrans.position;
            _movement.SetMovementDirection(direction.normalized);

            if (IsTargetInStoppingDistance())
                return Status.Success;

            return Status.Running;
        }

        private bool IsTargetInStoppingDistance()
        {
            return Vector2.Distance(_enemyTrans.position, _targetTrans.position) < Enemy.Value.AttackData.StoppingDistance;
        }
    }
}


