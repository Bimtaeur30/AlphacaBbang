using JJH._02_Scripts_Systems.EventSystems;
using System;
using System.Collections;
using UnityEngine;

public class LootBoxInteractor : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;
    [SerializeField] private LootBoxOpeningUI openingUI;

    [Header("Post-Open UI")]
    //[SerializeField] private GameObject inventoryUIRoot;
    //[SerializeField] private LootBoxRevealUI lootBoxRevealUI;
    [SerializeField] private SlidePanelController slidePanelController;

    [Header("Events")]
    [SerializeField] private EventChannelSO InventoryChannel;
    private bool _isOpening;
    private void Awake()
    {
        InventoryChannel.AddListener<InventoryToggleEvt>(HandleLootBoxOff);
    }

    private void HandleLootBoxOff(InventoryToggleEvt evt)
    {
        if (!evt.Value)
            slidePanelController.SlideOut();
    }

    public void StartOpen(LootBoxContainer lootBox)
    {
        if (_isOpening) return;

        StartCoroutine(OpenRoutine(lootBox));
    }

    private IEnumerator OpenRoutine(LootBoxContainer lootBox)
    {
        _isOpening = true;
        openingUI?.Show();

        float timer = 0f;
        float totalTime = lootBox.RequiredOpenTime;

        while (timer < totalTime)
        {
            timer += Time.deltaTime;
            openingUI?.SetProgress(timer, totalTime);
            yield return null;
        }

        openingUI?.Hide();

        //inventoryUIRoot?.SetActive(true);
        //lootBoxRevealUI?.Show(lootBox);

        slidePanelController.SlideIn();
        InventoryChannel.RaiseEvent(InventoryEvents.InventoryToggle.Init(true));

        _isOpening = false;
    }
}