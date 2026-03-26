using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;

namespace JJH._02_Scripts.Agents.Enemies.BT.Channels
{
#if UNITY_EDITOR
    [CreateAssetMenu(menuName = "Behavior/Event Channels/StateChannel")]
#endif
    [Serializable, GeneratePropertyBag]
    [EventChannelDescription(name: "StateChannel", message: "Set CurrentState To [State]", category: "Events", id: "052d43b2ab7674a5661f9410e02e3f64")]
    public sealed partial class StateChannel : EventChannel<EnemyState> { }
}

