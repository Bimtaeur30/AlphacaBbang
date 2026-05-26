using DG.Tweening;
using JJH._02_Scripts_Systems.EventSystems;
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InventoryToggleUI : MonoBehaviour
{
    [SerializeField] private SlidePanelController slidePanel;
    [SerializeField] private CanvasGroup InventoryGroup;
    [SerializeField] private EventChannelSO InventoryChannel;
    [SerializeField] private CanvasGroup bg;
    [SerializeField] private StorageOpener storageOpener;
    [SerializeField] private float AnimDuration = 1f;
    private RectTransform inventory;
    private bool _isOpen = false;

    private void Awake()
    {
        inventory = InventoryGroup.GetComponent<RectTransform>();
        Debug.Assert(inventory != null, "�κ��丮 �׷쿡 RecTransform�� �������� �ʽ��ϴ�.");

        InventoryChannel.AddListener<InventoryToggleEvt>(HandleInventoryToggle);
    }

    private void OnDestroy()
    {
        InventoryChannel.RemoveListener<InventoryToggleEvt>(HandleInventoryToggle);
    }
    private void Start()
    {
        InventoryGroup.alpha = 0f;
        InventoryChannel.RaiseEvent(InventoryEvents.InventoryToggle.Init(false));
    }
    private void Update() // �׽�Ʈ �ڵ�
    {
        if (Keyboard.current.tabKey.wasPressedThisFrame)
        {
            if (!_isOpen)
            {
                InventoryChannel.RaiseEvent(InventoryEvents.InventoryToggle.Init(true));
                _isOpen = true;
            }
            else if (_isOpen)
            {
                InventoryChannel.RaiseEvent(InventoryEvents.InventoryToggle.Init(false));
                _isOpen = false;
            }
        }
    }

    private void HandleInventoryToggle(InventoryToggleEvt evt)
    {
        _isOpen = evt.Value;
        
        if (evt.Value)
        {
            InventoryGroup.DOFade(1f, AnimDuration);
            bg.DOFade(1f, AnimDuration);
            slidePanel.SlideIn();
        }
        else
        {
            InventoryGroup.DOFade(0f, AnimDuration);
            bg.DOFade(0f, AnimDuration);
            slidePanel.SlideOut();
        }
    }
}
