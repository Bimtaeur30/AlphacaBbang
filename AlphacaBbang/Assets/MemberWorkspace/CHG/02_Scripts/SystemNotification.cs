using System;
using System.Collections.Generic;
using DG.Tweening;
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
        private SlidePanelController _slidePanelController;
        
        private void Awake()
        {
            _slidePanelController = GetComponent<SlidePanelController>();
            eventChannel.AddListener<SystemNotificationEvent>(ShowNotification);
        }

        private void ShowNotification(SystemNotificationEvent obj)
        {
            _notifications.Enqueue(obj);

            while (_notifications.Count > 0)
            {
                
            }
        }
    }
}
