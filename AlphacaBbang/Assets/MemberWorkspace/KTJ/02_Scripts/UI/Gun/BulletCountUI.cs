using JJH._02_Scripts_Systems.EventSystems;
using System;
using TMPro;
using UnityEngine;

public class BulletCountUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI bulletText;
    [SerializeField] private CanvasGroup bulletCountLabel; // toggle

    [Header("System")]
    [SerializeField] private EventChannelSO uiChannel;
    [SerializeField] private EventChannelSO gunChannel;
    [SerializeField] private WeaponSlotIndex index;

    [SerializeField] private GameObject aa;

    private void Awake()
    {
        gunChannel.AddListener<WeaponSlotEquipEvent_UI>(HandleWeaponEquipEvent);
        uiChannel.AddListener<BulletCountHandleEvent>(HandleBulletCountChanged);
        bulletCountLabel.alpha = 0;
    }

    private void OnDestroy()
    {
        gunChannel.RemoveListener<WeaponSlotEquipEvent_UI>(HandleWeaponEquipEvent);
        uiChannel.RemoveListener<BulletCountHandleEvent>(HandleBulletCountChanged);
    }

    private void HandleWeaponEquipEvent(WeaponSlotEquipEvent_UI @event)
    {
        if (@event.SlotIndex != index) return;
        bulletCountLabel.alpha = @event.IsEquip ? 1 : 0;
        //aa.gameObject.SetActive(false);        //bulletText.text = "X";
    }
    private void HandleBulletCountChanged(BulletCountHandleEvent @event)
    {
        if (@event.Slot != index) return;
        bulletText.text = @event.Text;
    }
}
