using DG.Tweening;
using JJH._02_Scripts.Systems.EventSystems;
using JJH._02_Scripts.Systems.SoundSystems;
using JJH._02_Scripts_Systems.EventSystems;
using Reflex.Attributes;
using System;
using System.Collections;
using UnityEngine;

public class GameEndUI : MonoBehaviour
{
    [SerializeField] private EventChannelSO systemChannel;
    [SerializeField] private EventChannelSO mapEventChannel;
    [SerializeField] private EventChannelSO soundChannel;
    [SerializeField] private SoundClipSO successClip;
    [SerializeField] private SoundClipSO failClip;
    [SerializeField] private CanvasGroup successGroup;
    [SerializeField] private CanvasGroup failGroup;
    [SerializeField] private float TransitionDuration = 2.0f;

    bool isEnd = false;

    private void Awake()
    {
        systemChannel.AddListener<OnGameENd>(HandleOnGameEnd);
        failGroup.alpha = 0;
        successGroup.alpha = 0;
    }
    private void OnDestroy()
    {
        systemChannel.RemoveListener<OnGameENd>(HandleOnGameEnd);
    }

    private void HandleOnGameEnd(OnGameENd nd)
    {
        if (isEnd) return;
        isEnd = true;

        if (nd.IsPlayerAlive)
        {
            successGroup.DOFade(1f, TransitionDuration);
            soundChannel.RaiseEvent(SoundEvents.PlaySoundEvent.Init(successClip));
        }
        else
        {
            failGroup.DOFade(1f, TransitionDuration);
            soundChannel.RaiseEvent(SoundEvents.PlaySoundEvent.Init(failClip));
        }
        StartCoroutine(NextEndEffect());
    }

    IEnumerator NextEndEffect()
    {
        yield return new WaitForSeconds(3f);
        mapEventChannel.RaiseEvent(MapEvents.StartPlayEvent);
    }
}
