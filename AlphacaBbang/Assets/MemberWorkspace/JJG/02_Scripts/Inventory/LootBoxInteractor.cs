using JJH._02_Scripts_Systems.EventSystems;
using System;
using System.Collections;
using UnityEngine;

public class LootBoxInteractor : MonoBehaviour
{
    [SerializeField] private EventChannelSO InventoryChannel;
    [SerializeField] private SlidePanelController SlidePanelController;

    [SerializeField] private PlayerController playerController;
    [SerializeField] private LootBoxOpeningUI openingUI;

    [Header("Post-Open UI")]
    [SerializeField] private GameObject inventoryUIRoot;
    [SerializeField] private LootBoxRevealUI lootBoxRevealUI;

    private bool _isOpening;

    private void Awake()
    {
        InventoryChannel.AddListener<InventoryToggleEvt>(HandleInventoryToggle);
    }

    private void HandleInventoryToggle(InventoryToggleEvt evt)
    {
        if (!evt.Value)
        {
            SlidePanelController.SlideOut();
        }

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

        inventoryUIRoot?.SetActive(true);
        lootBoxRevealUI?.Show(lootBox);

        InventoryChannel.RaiseEvent(InventoryEvents.InventoryToggle.Init(true));
        SlidePanelController.SlideIn();

        _isOpening = false;
    }
}