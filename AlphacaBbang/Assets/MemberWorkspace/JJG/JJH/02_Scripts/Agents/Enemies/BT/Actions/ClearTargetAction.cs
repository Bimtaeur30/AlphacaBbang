using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Action = Unity.Behavior.Action;

namespace JJH._02_Scripts.Agents.Enemies.BT.Actions
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(name: "ClearTarget", story: "Clear [Target]", category: "Action/GameObject", id: "ae31bf4768e6f744471bc4502a1fc111")]
    public partial class ClearTargetAction : Action
    {
        [SerializeReference] public BlackboardVariable<GameObject> Target;

        protected override Status OnStart()
        {
            if (Target == null)
                return Status.Failure;

            Target.Value = null;
            return Status.Success;
        }
    }
}

