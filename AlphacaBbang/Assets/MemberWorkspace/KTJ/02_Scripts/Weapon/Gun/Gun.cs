using System;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public abstract class Gun : MonoBehaviour
{
    [SerializeField] private PlayerInputSO_KTJ playerInputSO;

    protected void OnEnable()
    {
        playerInputSO.OnAimKeyPressed += HandleAimKeyPressed;
        playerInputSO.OnAimKeyCanceled += HandleAimKeyCanceled;
    }

    protected void OnDisable()
    {
        playerInputSO.OnAimKeyPressed -= HandleAimKeyPressed;
        playerInputSO.OnAimKeyCanceled -= HandleAimKeyCanceled;
    }

    private void HandleAimKeyPressed() => OnAim(true);
    private void HandleAimKeyCanceled() => OnAim(false);

    private void OnAim(bool value)
    {
        if (value)
        {
            // 여기서 이벤트 UI 채널로 크로스헤드 조준 변경
        }
        else
        {
            // 여기서 이벤트 UI 채널로 크로스헤드 해제 변경
        }
    }
}
