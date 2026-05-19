using JJH._02_Scripts_Systems.EventSystems;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerRoutSender : MonoBehaviour
{
    [SerializeField] private EventChannelSO mapEventChannel;

    [SerializeField] private float testRoutRecordTime = 10f; // 테스트로 10초동안 기록함
    private float currentTime = 0f;
    private float recordTime = 0f;
    private bool isRecording = false;

    private void Start()
    {
        isRecording = true;
        mapEventChannel.RaiseEvent(MapEvents.PlayerActionEvent.Init("탐사 시작"));
        SendPlayerPosition();
    }

    private void Update()
    {
        if (isRecording == false) return;

        currentTime += Time.deltaTime;
        recordTime += Time.deltaTime;
        if (recordTime >= testRoutRecordTime)
        {
            mapEventChannel.RaiseEvent(MapEvents.PlayerActionEvent.Init("탐사 종료"));
            SendPlayerPosition();
            mapEventChannel.RaiseEvent(MapEvents.RoutRecordEndEvent.Init(recordTime)); // 기록 종료 이벤트
            isRecording = false;
        }
        else if (currentTime >= MapRoutDataSO.RECORD_INTERVAL)
        {
            currentTime = 0f;
            SendPlayerPosition();
        }

        if (Keyboard.current.qKey.wasPressedThisFrame) // 테스트 액션 전송
        {
            SendPlayerAction();
        }
    }

    private void SendPlayerAction()
    {
        mapEventChannel.RaiseEvent(MapEvents.PlayerActionEvent.Init("테스트 액션이 발동되었습니다."));
        Debug.Log("PlayerRoutSender: 테스트 액션이 발동되었습니다.");
    }

    private void SendPlayerPosition()
    {
        mapEventChannel.RaiseEvent(MapEvents.PlayerPointEvent.Init(transform.position));
        //Debug.Log($"PlayerRoutSender: 현재 위치를 전송했습니다. 위치: {transform.position}");
    }
}
