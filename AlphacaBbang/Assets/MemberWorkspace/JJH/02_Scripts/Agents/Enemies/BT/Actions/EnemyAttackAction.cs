using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Action = Unity.Behavior.Action;

namespace JJH._02_Scripts.Agents.Enemies.BT.Actions
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(name: "EnemyAttack", story: "[Enemy] Attack [Target]", category: "Action/GameObject", id: "4bee01b7c7c99f75f2e1689dcd07bef2")]
    public partial class EnemyAttackAction : Action
    {
        [SerializeReference] public BlackboardVariable<AbstractEnemy> Enemy;
        [SerializeReference] public BlackboardVariable<GameObject> Target;

        private IWeapon _weapon;

        private float _time = 0;

        protected override Status OnStart()
        {
            if (Enemy == null || Enemy.Value == null || Enemy.Value.Weapon == null || Enemy.Value.EnemyData == null ||
                Target == null || Target.Value == null)
                return Status.Failure;

            _weapon = Enemy.Value.Weapon;

            _time = 0;
            _weapon.Attack(Target.Value.transform.position, true);

            return Status.Running;
        }

        protected override Status OnUpdate()
        {
            _time += Time.deltaTime;
            if (_time > Enemy.Value.EnemyData.AttackTime)
            {
                _weapon.Attack(Target.Value.transform.position, false);
                return Status.Success;
            }
            return Status.Running;
        }
    }
}


