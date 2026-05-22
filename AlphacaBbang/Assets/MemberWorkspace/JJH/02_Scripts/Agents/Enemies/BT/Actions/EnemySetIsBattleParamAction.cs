using JJH._02_Scripts_Systems.AnimationSystems;
using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Action = Unity.Behavior.Action;

namespace JJH._02_Scripts.Agents.Enemies.BT.Actions
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(name: "EnemySetIsBattleParam", story: "[Enemy] Set [IsBattleParam] [Value]", category: "Action/Animation", id: "40e5d2c3e3b54d36bc35c55258b34abe")]
    public partial class EnemySetIsBattleParamAction : Action
    {
        [SerializeReference] public BlackboardVariable<AbstractEnemy> Enemy;
        [SerializeReference] public BlackboardVariable<AnimParamSO> IsBattleParam;
        [SerializeReference] public BlackboardVariable<bool> Value;

        protected override Status OnStart()
        {
            if (Enemy.Value == null || Enemy.Value.Renderer == null || IsBattleParam.Value == null)
                return Status.Failure;

            Enemy.Value.Renderer.Animator.SetBool(IsBattleParam.Value.ParamHash, Value.Value);

            return Status.Success;
        }
    }
}

