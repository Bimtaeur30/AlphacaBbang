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
    [SerializeField] private float AnimDuration = 1f;
    private RectTransform inventory;

    private void Awake()
    {
        inventory = InventoryGroup.GetComponent<RectTransform>();
        Debug.Assert(inventory != null, "인벤토리 그룹에 RecTransform이 존재하지 않습니다.");

        InventoryChannel.AddListener<InventoryToggleEvt>(HandleInventoryToggle);
    }
    private void Start()
    {
        InventoryGroup.alpha = 0f;
        slidePanel.SlideOut();
    }
    private void Update() // 테스트 코드
    {
        if (Keyboard.current.iKey.wasPressedThisFrame)
            InventoryChannel.RaiseEvent(InventoryEvents.InventoryToggle.Init(true));
        if (Keyboard.current.oKey.wasPressedThisFrame)
            InventoryChannel.RaiseEvent(InventoryEvents.InventoryToggle.Init(false));
    }

    private void HandleInventoryToggle(InventoryToggleEvt evt)
    {
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
