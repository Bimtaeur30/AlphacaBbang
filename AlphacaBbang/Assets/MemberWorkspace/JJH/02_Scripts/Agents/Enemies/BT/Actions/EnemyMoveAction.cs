using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Random = UnityEngine.Random;

namespace JJH._02_Scripts.Agents.Enemies.BT.Actions
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(name: "EnemyMove", story: "[Enemy] move", category: "Action", id: "cc5e290088f824b2d52ffb47f06a8ce0")]
    public partial class EnemyMoveAction : Action
    {
        [SerializeReference] public BlackboardVariable<AbstractEnemy> Enemy;

        private IControllerMovement _movement;

        private float _moveDuration;
        private float _moveTime;

        protected override Status OnStart()
        {
            if (Enemy.Value == null || Enemy.Value.Movement == null)
                return Status.Failure;

            _movement = Enemy.Value.Movement;

            _moveTime = 0;
            _moveDuration = Random.Range(1f, 2f);

            Vector3 move = Random.insideUnitCircle.normalized;
            _movement.SetMovementDirection(move);

            return Status.Running;
        }

        protected override Status OnUpdate()
        {
            _moveTime += Time.deltaTime;
            if (_moveTime > _moveDuration)
            {
                _movement.SetMovementDirection(Vector3.zero);
                return Status.Success;
            }

            return Status.Running;
        }
    }
}

