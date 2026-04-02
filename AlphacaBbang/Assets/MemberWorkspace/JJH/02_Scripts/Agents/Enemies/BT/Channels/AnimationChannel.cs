using JJH._02_Scripts_Systems.AnimationSystems;
using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;

namespace JJH._02_Scripts.Agents.Enemies.BT.Channels
{
#if UNITY_EDITOR
    [CreateAssetMenu(menuName = "Behavior/Event Channels/AnimationChannel")]
#endif
    [Serializable, GeneratePropertyBag]
    [EventChannelDescription(name: "AnimationChannel", message: "Set Animation to [Clip]", category: "Events", id: "208c46df98749213ca714c43843c3322")]
    public sealed partial class AnimationChannel : EventChannel<AnimParamSO> { }
}
