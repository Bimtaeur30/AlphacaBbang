using JJH._02_Scripts_Systems.EventSystems;
using JJH._02_Scripts.Agents.Enemies;
using JJH._02_Scripts.Systems.EventSystems;
using UnityEngine;

namespace MemberWorkspace.CHG._02_Scripts
{
    public class TestGameEventInvok : MonoBehaviour
    {
        [SerializeField] private EventChannelSO eventChannel;
        [SerializeField] private MeleeEnemy enemy; 

        [ContextMenu("Invoke")]
        private void Invoke()
        {
            AgentDeadEvent evt = new AgentDeadEvent();
            evt.Init(enemy);
            eventChannel.RaiseEvent(evt);
        }
    }
}