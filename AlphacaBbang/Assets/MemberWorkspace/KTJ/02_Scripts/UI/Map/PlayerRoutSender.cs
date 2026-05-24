using JJH._02_Scripts_Systems.EventSystems;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerRoutSender : MonoBehaviour
{
    [SerializeField] private EventChannelSO mapEventChannel;
    [SerializeField] private RoutRecorder routRecorder; // 직접 참조로 Clear() 호출

    private float _currentTime = 0f;
    private float _recordTime = 0f;
    private bool _isRecording = false;

    private void Awake()
    {
        mapEventChannel.AddListener<StartPlayEvent>(OnEndRecording);
    }

    private void Start()
    {
        routRecorder.Clear(); // 탐사 시작 전 이전 데이터 초기화
        _currentTime = 0f;
        _recordTime = 0f;
        _isRecording = true;

        Debug.Log($"[PlayerRoutSender] 탐사 시작. recordTime 초기화");
        mapEventChannel.RaiseEvent(MapEvents.PlayerActionEvent.Init("탐사 시작"));
        SendPlayerPosition();
    }

    private void OnDestroy()
    {
        mapEventChannel.RemoveListener<StartPlayEvent>(OnEndRecording);
    }

    private void Update()
    {
        if (!_isRecording) return;

        _currentTime += Time.deltaTime;
        _recordTime += Time.deltaTime;

        if (_currentTime >= MapRoutData.RECORD_INTERVAL)
        {
            _currentTime = 0f;
            SendPlayerPosition();
        }

        if (Keyboard.current.qKey.wasPressedThisFrame)
        {
            SendPlayerAction();
        }
    }

    private void SendPlayerAction()
    {
        mapEventChannel.RaiseEvent(MapEvents.PlayerActionEvent.Init("테스트 액션이 발동되었습니다."));
        //Debug.Log("[PlayerRoutSender] 테스트 액션 발송");
    }

    private void SendPlayerPosition()
    {
        mapEventChannel.RaiseEvent(MapEvents.PlayerPointEvent.Init(transform.position));
        //Debug.Log($"[PlayerRoutSender] 포인트 발송 | recordTime: {_recordTime:F2}s");
    }

    public void OnEndRecording(StartPlayEvent @event)
    {
        if (!_isRecording) return; // GameEndUI가 StartPlayEvent를 반복 발행해도 최초 1회만 처리

        _isRecording = false;
        Debug.Log($"[PlayerRoutSender] 탐사 종료 | 최종 recordTime: {_recordTime:F2}s");
        mapEventChannel.RaiseEvent(MapEvents.PlayerActionEvent.Init("탐사 종료"));
        SendPlayerPosition();
        mapEventChannel.RaiseEvent(MapEvents.RoutRecordEndEvent.Init(_recordTime));
    }
}