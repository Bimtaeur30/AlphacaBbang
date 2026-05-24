using JJH._02_Scripts_Systems.EventSystems;
using UnityEngine;

namespace MemberWorkspace.CHG._02_Scripts.Test
{
    public class TestGameEventInvok : MonoBehaviour
    {
        [SerializeField] private EventChannelSO eventChannel;

        [ContextMenu("Invoke")]
        private void Invoke()
        {
            SystemNotificationEvent evt  = new SystemNotificationEvent();
            SystemNotificationEvent evt2  = new SystemNotificationEvent();
            SystemNotificationEvent evt3  = new SystemNotificationEvent();
            evt.Init("mainTest", "subTest");
            eventChannel.RaiseEvent(evt);
            evt2.Init("mainTest2", "subTest2");
            eventChannel.RaiseEvent(evt2);
            evt3.Init("mainTest3", "subTest3");
            eventChannel.RaiseEvent(evt3);
        }
    }
}