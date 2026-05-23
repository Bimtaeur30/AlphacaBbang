using JJH._02_Scripts_Systems.EventSystems;
using System;
using System.Collections;
using UnityEngine;

public class LootBoxInteractor : MonoBehaviour
{
    [SerializeField] private EventChannelSO InventoryChannel;
    [SerializeField] private SlidePanelController SlidePanelController;

    [SerializeField] private PlayerController playerController;
    //[SerializeField] private LootBoxOpeningUI openingUI;

    [Header("Post-Open UI")]
    [SerializeField] private GameObject inventoryUIRoot;
    [SerializeField] private LootBoxRevealUI lootBoxRevealUI;

    private bool _isOpening;

    private void Awake()
    {
        InventoryChannel.AddListener<InventoryToggleEvt>(HandleInventoryToggle);
    }

    private void OnDestroy()
    {
        InventoryChannel.RemoveListener<InventoryToggleEvt>(HandleInventoryToggle);
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
        inventoryUIRoot?.SetActive(false);
        InventoryChannel.RaiseEvent(InventoryEvents.InventoryToggle.Init(true));
        SlidePanelController.SlideIn();

        //openingUI?.Show();

        yield return new WaitForSeconds(0.2f);

        //openingUI?.Hide();

        inventoryUIRoot?.SetActive(true);
        lootBoxRevealUI?.Show(lootBox);
        _isOpening = false;
    }
}