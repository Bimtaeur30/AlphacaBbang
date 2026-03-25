using JJH._02_Scripts_Systems.AnimationSystems;
using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;

namespace JJH._02_Scripts.Enemies.BT.Channels
{
#if UNITY_EDITOR
    [CreateAssetMenu(menuName = "Behavior/Event Channels/AnimationChannel")]
#endif
    [Serializable, GeneratePropertyBag]
    [EventChannelDescription(name: "AnimationChannel", message: "Set Animation To [Clip]", category: "Events", id: "bd309af26276afa779e263071e06ddc5")]
    public sealed partial class AnimationChannel : EventChannel<AnimParamSO> { }
}

