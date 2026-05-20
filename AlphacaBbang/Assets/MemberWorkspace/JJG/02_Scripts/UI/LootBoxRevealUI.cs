using JJH._02_Scripts_Systems.EventSystems;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LootBoxRevealUI : MonoBehaviour
{
    [SerializeField] private GameObject _root;

    [Header("Buffering")]
    [SerializeField] private GameObject _bufferingPanel;
    [SerializeField] private Image _spinnerImage;
    [SerializeField] private float _bufferDuration = 1.5f;
    [SerializeField] private float _spinnerSpeed = 360f;

    [Header("Content")]
    [SerializeField] private GameObject _contentArea;
    [SerializeField] private InventoryUI _lootInventoryUI;
    [SerializeField] private float _itemRevealInterval = 0.4f;


    private Coroutine _revealCoroutine;

    private void Awake()
    {
        _root.SetActive(false);
    }


    private void Update()
    {
        if (_spinnerImage != null && _bufferingPanel != null && _bufferingPanel.activeSelf)
            _spinnerImage.transform.Rotate(0f, 0f, -_spinnerSpeed * Time.deltaTime);
    }

    public void Show(LootBoxContainer lootBox)
    {
        if (_revealCoroutine != null)
            StopCoroutine(_revealCoroutine);

        if (_lootInventoryUI != null)
            _lootInventoryUI.SetInventory(lootBox);

        _root.SetActive(true);
        _bufferingPanel.SetActive(true);
        _contentArea.SetActive(false);
        _revealCoroutine = StartCoroutine(RevealSequence(lootBox));
    }

    public void Hide()
    {
        if (_revealCoroutine != null)
        {
            StopCoroutine(_revealCoroutine);
            _revealCoroutine = null;
        }
        _contentArea.SetActive(false);
        _root.SetActive(false);
    }

    private IEnumerator RevealSequence(LootBoxContainer lootBox)
    {
        _lootInventoryUI.gameObject.SetActive(true);
        
        yield return new WaitForSeconds(_bufferDuration);

        _bufferingPanel.SetActive(false);
        _contentArea.SetActive(true);       // InventoryUI OnEnable → 슬롯 빌드
        _lootInventoryUI.PrepareForReveal(); // 즉시 아이템 가림 (같은 프레임)

        for (int i = 0; i < lootBox.SlotCount; i++)
        {
            ItemSlot slot = lootBox.GetSlot(i);
            if (slot == null || slot.IsEmpty) continue;

            _lootInventoryUI.RevealSlot(i);
            yield return new WaitForSeconds(_itemRevealInterval);
        }

        _revealCoroutine = null;
    }
}
