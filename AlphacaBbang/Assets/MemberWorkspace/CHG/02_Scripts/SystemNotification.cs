using System.Collections;
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
        [Tooltip("알림이 표시되는 유지 시간")]
        [SerializeField] private float showDuration = 2f;

        private SlidePanelController _slidePanelController;
        private bool _isMoving = false;
        private Coroutine _currentCoroutine;

        private void Awake()
        {
            _slidePanelController = GetComponent<SlidePanelController>();
            eventChannel.AddListener<SystemNotificationEvent>(OnShowNotification);
            _slidePanelController.OnEndMoving += OnEndMoving;
        }

        private void OnDestroy()
        {
            eventChannel.RemoveListener<SystemNotificationEvent>(OnShowNotification);
            if (_slidePanelController != null)
                _slidePanelController.OnEndMoving -= OnEndMoving;
        }

        private IEnumerator Start()
        {
            yield return null;
            _slidePanelController.SlideOut();
        }

        private void OnShowNotification(SystemNotificationEvent notification)
        {
            if (_currentCoroutine != null)
                StopCoroutine(_currentCoroutine);

            _currentCoroutine = StartCoroutine(ShowingNotification(notification));
        }

        private IEnumerator ShowingNotification(SystemNotificationEvent notification)
        {
            mainText.text = notification.MainMessage;
            subText.text = notification.SubMessage;

            yield return null;

            if (!_slidePanelController.IsHidden)
            {
                _isMoving = true;
                _slidePanelController.SlideOut();
                yield return new WaitUntil(() => !_isMoving);
            }

            if (_slidePanelController.IsHidden)
            {
                _isMoving = true;
                _slidePanelController.SlideIn();
                yield return new WaitUntil(() => !_isMoving);
            }

            yield return new WaitForSeconds(showDuration);

            if (!_slidePanelController.IsHidden)
            {
                _isMoving = true;
                _slidePanelController.SlideOut();
                yield return new WaitUntil(() => !_isMoving);
            }

            _currentCoroutine = null;
        }

        private void OnEndMoving()
        {
            _isMoving = false;
        }

#if UNITY_EDITOR
        [ContextMenu("TestNotification")]
        private void TestNotification()
        {
            SystemNotificationEvent notification = new SystemNotificationEvent();
            notification.Init("testMainMessage", "testSubMessage");
            OnShowNotification(notification);
        }

        [ContextMenu("TestSlideIn")]
        private void TestSlideIn()
        {
            _slidePanelController.SlideIn();
        }

        [ContextMenu("TestSlideOut")]
        private void TestSlideOut()
        {
            _slidePanelController.SlideOut();
        }
#endif
    }
}