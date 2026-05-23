using DG.Tweening;
using JJH._02_Scripts_Systems.EventSystems;
using Reflex.Attributes;
using System;
using System.Collections;
using UnityEngine;

public class GameEndUI : MonoBehaviour
{
    [SerializeField] private EventChannelSO systemChannel;
    [SerializeField] private EventChannelSO mapEventChannel;
    [SerializeField] private CanvasGroup successGroup;
    [SerializeField] private CanvasGroup failGroup;
    [SerializeField] private float TransitionDuration = 2.0f;

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
        if (nd.IsPlayerAlive)
        {
            successGroup.DOFade(1f, TransitionDuration);
        }
        else
        {
            failGroup.DOFade(1f, TransitionDuration);
        }
        StartCoroutine(NextEndEffect());
    }

    IEnumerator NextEndEffect()
    {
        yield return new WaitForSeconds(3f);
        mapEventChannel.RaiseEvent(MapEvents.StartPlayEvent);
    }
}
