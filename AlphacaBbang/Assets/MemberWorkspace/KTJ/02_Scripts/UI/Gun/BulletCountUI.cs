using JJH._02_Scripts_Systems.EventSystems;
using System;
using TMPro;
using UnityEngine;

public class BulletCountUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI bulletText;
    [SerializeField] private GameObject bulletCountLabel; // toggle

    [Header("System")]
    [SerializeField] private EventChannelSO uiChannel;
    [SerializeField] private EventChannelSO gunChannel;
    [SerializeField] private WeaponSlotIndex index;

    private void Awake()
    {
        gunChannel.AddListener<WeaponSlotEquipEvent>(HandleWeaponSlotEquipEvent);
        uiChannel.AddListener<BulletCountHandleEvent>(HandleBulletCountChanged);
        bulletCountLabel.SetActive(false);
    }

    private void HandleWeaponSlotEquipEvent(WeaponSlotEquipEvent @event)
    {
        if (@event.SlotIndex != index) return;
        bulletCountLabel.gameObject.SetActive(@event.IsEquip);
        bulletText.text = "X";
    }
    private void HandleBulletCountChanged(BulletCountHandleEvent @event)
    {
        if (@event.Slot != index) return;
        bulletText.text = @event.Text;
    }
}
