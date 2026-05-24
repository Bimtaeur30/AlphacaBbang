using System;
using System.Collections;
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
        [SerializeField] private float stopDelay;
        
        private Queue<SystemNotificationEvent> _notifications = new Queue<SystemNotificationEvent>();
        private SlidePanelController _slidePanelController;
        private bool _isMoving = false;
        private bool _isShowing = false;
        private bool _loopoing = false;
        
        private void Awake()
        {
            _slidePanelController = GetComponent<SlidePanelController>();
            eventChannel.AddListener<SystemNotificationEvent>(OnShowNotification);
            
            _slidePanelController.OnEndMoving += OnEndMoveing;
        }

        private void OnDestroy()
        {
            eventChannel.RemoveListener<SystemNotificationEvent>(OnShowNotification);
        }

        private IEnumerator Start()
        {
            yield return null; 
            _slidePanelController.SlideOut();
        }

        private void OnShowNotification(SystemNotificationEvent obj)
        {
            _notifications.Enqueue(obj);
            Debug.Log(obj.MainMessage);
            if (!_loopoing)
                StartCoroutine(ShowingNotification());
            
        }

        private IEnumerator ShowingNotification()
        {
            _loopoing = true;
            while (_notifications.Count > 0)
            {
                SystemNotificationEvent text = _notifications.Dequeue();
                _isMoving = true;


                mainText.text = text.MainMessage;
                subText.text = text.SubMessage;

                yield return null;

                _slidePanelController.Toggle();
                yield return new WaitUntil(() => !_isMoving);
                yield return new WaitForSeconds(stopDelay);

                _isMoving = true;
                _slidePanelController.Toggle();
                yield return new WaitUntil(() => !_isMoving);
                yield return new WaitForSeconds(stopDelay);
            }
            _loopoing = false;
            
        }

        private void OnEndMoveing()
        {
            _isMoving = false;
        }
        
        #if UNITY_EDITOR

        [ContextMenu("TestNotification")]
        private void TestNotification()
        {
            SystemNotificationEvent text = new SystemNotificationEvent();
            text.Init("testMainMessage", "testsubstring");
            OnShowNotification(text);
        }

        [ContextMenu("TestToggle")]
        private void TestToggle()
        {
            _slidePanelController.Toggle();
        }
        
        #endif
        
    }
}
