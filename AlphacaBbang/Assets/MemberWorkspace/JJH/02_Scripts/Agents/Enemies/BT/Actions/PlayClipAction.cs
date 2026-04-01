using JJH._02_Scripts_Systems.AnimationSystems;
using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Action = Unity.Behavior.Action;

namespace JJH._02_Scripts.Agents.Enemies.BT.Actions
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(name: "PlayClip", story: "[Enemy] play [Clip]", category: "Action/Animation", id: "507ed115817652919bb45ff2f690676e")]
    public partial class PlayClipAction : Action
    {
        [SerializeReference] public BlackboardVariable<AbstractEnemy> Enemy;
        [SerializeReference] public BlackboardVariable<AnimParamSO> Clip;

        protected override Status OnStart()
        {
            if (Enemy.Value == null || Enemy.Value.Renderer == null || Clip.Value == null)
                return Status.Failure;

            Enemy.Value.Renderer.PlayClip(Clip.Value.ParamHash, 0, 0.2f, 0);

            return Status.Success;
        }
    }
}

