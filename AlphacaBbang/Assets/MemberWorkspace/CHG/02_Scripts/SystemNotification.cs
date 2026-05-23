using System;
using System.Collections.Generic;
using JJH._02_Scripts_Systems.EventSystems;
using TMPro;
using UnityEngine;

namespace MemberWorkspace.CHG._02_Scripts
{
    public class SystemNotification : MonoBehaviour
    {
        [SerializeField] private EventChannelSO eventChannel;
        [SerializeField] private TextMeshProUGUI mainText;
        [SerializeField] private TextMeshProUGUI subText;
        
        private Queue<SystemNotificationEvent> _notifications = new Queue<SystemNotificationEvent>();
        
        private void Awake()
        {
            eventChannel.AddListener<SystemNotificationEvent>(ShowNotification);
        }

        private void ShowNotification(SystemNotificationEvent obj)
        {
            _notifications.Enqueue(obj);
        }
    }
}
