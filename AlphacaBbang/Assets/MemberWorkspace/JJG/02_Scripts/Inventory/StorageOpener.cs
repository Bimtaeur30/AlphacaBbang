using System;
using JJH._02_Scripts_Systems.EventSystems;
using UnityEngine;

public class StorageOpener : MonoBehaviour
{
    [SerializeField] private EventChannelSO inventoryChannel;
    
    private SlidePanelController SlidePanelController;

    private void Awake()
    {
        SlidePanelController = GetComponent<SlidePanelController>();
        inventoryChannel.AddListener<InventoryToggleEvt>(HandleInventoryToggle);
    }

    private void HandleInventoryToggle(InventoryToggleEvt evt)
    {
        if (!evt.Value)
            SlidePanelController.SlideOut();
    }

    public void Open()
    {
        //gameObject.SetActive(true);
        inventoryChannel.RaiseEvent(InventoryEvents.InventoryToggle.Init(true));
        SlidePanelController.SlideIn();
    }
}