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
            SlidePanelController.SlideOut();
    }
 
    /// <summary>
    /// 상자 오브젝트의 LootBox 컴포넌트를 넘겨주세요.
    /// 거기 붙어있는 LootTableSO로 아이템을 생성하고 열기 루틴을 실행합니다.
    /// </summary>
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
 
        // LootBoxContainer에 이 상자의 LootTable을 주입
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