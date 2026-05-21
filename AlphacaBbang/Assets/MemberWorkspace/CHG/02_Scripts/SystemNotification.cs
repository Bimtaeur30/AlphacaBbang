using System;
using JJH._02_Scripts_Systems.EventSystems;
using UnityEngine;

namespace MemberWorkspace.CHG._02_Scripts
{
    public class SystemNotification : MonoBehaviour
    {
        [SerializeField] private EventChannelSO eventChannel;

        private void Awake()
        {
            //eventChannel.AddListener<SystemNotificationEvent>(ShowNotification);
        }

        public void ShowNotification(string main, string sub)
        {
            
        }
    }
}
