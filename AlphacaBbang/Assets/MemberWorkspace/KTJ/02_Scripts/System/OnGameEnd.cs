using DG.Tweening;
using JJH._02_Scripts.Systems.EventSystems;
using JJH._02_Scripts.Systems.SoundSystems;
using JJH._02_Scripts_Systems.EventSystems;
using MemberWorkspace.JJG._02_Scripts;
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
    [SerializeField] private ItemContainer itemContainer;
    [SerializeField] private QuickSlotContainer quickSlotContainer;
    [SerializeField] private EquipmentContainer equipmentContainer;
    [SerializeField] bool isEnd = false;

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
            ClearAllEquipSlots();
            ClearAllItemSlots();
            ClearAllQuickSlots();
        }
        StartCoroutine(NextEndEffect());
    }
    private void ClearAllItemSlots()
    {
        for (int i = 0; i < itemContainer.SlotCount; i++)
            itemContainer.ClearSlot(i);
    }
    private void ClearAllQuickSlots()
    {
        for (int i = 0; i < quickSlotContainer.SlotCount; i++)
            quickSlotContainer.ClearSlot(i);
    }
    private void ClearAllEquipSlots()
    {
        for (int i = 0; i < equipmentContainer.SlotCount; i++)
            equipmentContainer.ClearSlot(i);
    }
    IEnumerator NextEndEffect()
    {
        yield return new WaitForSeconds(3f);
        mapEventChannel.RaiseEvent(MapEvents.StartPlayEvent);
    }
}
