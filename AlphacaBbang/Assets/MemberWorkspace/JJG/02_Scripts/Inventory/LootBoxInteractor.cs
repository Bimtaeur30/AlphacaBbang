using JJH._02_Scripts_Systems.EventSystems;
using System.Collections;
using UnityEngine;
 
public class LootBoxInteractor : MonoBehaviour
{
    [SerializeField] private EventChannelSO InventoryChannel;
    [SerializeField] private SlidePanelController SlidePanelController;
 
    [SerializeField] private PlayerController playerController;
    [SerializeField] private EventChannelSO systemChannel;
    
    private LootBoxContainer lootBoxContainer;
    private LootBox _currentLootBox;
 
    [Header("Post-Open UI")]
    [SerializeField] private GameObject inventoryUIRoot;
    [SerializeField] private LootBoxRevealUI lootBoxRevealUI;
 
    private bool _isOpening;
 
    private void Awake()
    {
        systemChannel.AddListener<LootboxDataSendEvent>(HandleLootboxDataSendEvent);
        lootBoxContainer = GetComponent<LootBoxContainer>();
        InventoryChannel.AddListener<InventoryToggleEvt>(HandleInventoryToggle);
    }

    private void HandleLootboxDataSendEvent(LootboxDataSendEvent obj)
    {
        StartOpen(obj.LootBox);
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
            _currentLootBox?.MarkAsClosed();
            _currentLootBox = null;
        }
    }
    
    public void StartOpen(LootBox lootBox)
    {
        if (_isOpening) return;
 
        if (lootBox == null)
        {
            Debug.LogWarning("[LootBoxInteractor] LootBox가 null입니다.");
            return;
        }
 
        if (lootBox.IsOpened)
        {
            Debug.Log($"[LootBoxInteractor] '{lootBox.BoxDisplayName}'은 이미 열린 상자입니다.");
            return;
        }
 
        if (lootBox.LootTable == null)
        {
            Debug.LogWarning($"[LootBoxInteractor] '{lootBox.BoxDisplayName}'에 LootTable SO가 없습니다.");
            return;
        }
        
        _currentLootBox = lootBox;
        lootBoxContainer.InitializeWithLootTable(lootBox.LootTable);
        lootBox.MarkAsOpened();
 
        StartCoroutine(OpenRoutine(lootBoxContainer));
    }
 
    private IEnumerator OpenRoutine(LootBoxContainer lootBox)
    {
        _isOpening = true;
        inventoryUIRoot?.SetActive(false);
        InventoryChannel.RaiseEvent(InventoryEvents.InventoryToggle.Init(true));
        SlidePanelController.SlideIn();
 
        yield return new WaitForSeconds(0.2f);
 
        inventoryUIRoot?.SetActive(true);
        lootBoxRevealUI?.Show(lootBox);
        _isOpening = false;
    }
}