using UnityEngine;
using UnityEngine.InputSystem;
using MemberWorkspace.JJG._02_Scripts.Item;

public class InventoryTest : MonoBehaviour
{
    [Header("Containers")]
    [SerializeField] private ItemContainer inventoryContainer;
    [SerializeField] private ItemContainer storageContainer;
    [SerializeField] private ItemContainer quickSlotContainer;

    [Header("Test Items")]
    [SerializeField] private ItemData normalItem;
    [SerializeField] private ItemData countableItem;
    [SerializeField] private ItemData foodItem;
    [SerializeField] private ItemData medicineItem;
    [SerializeField] private ItemData equipItem;
    [SerializeField] private ItemData equipItem2;
    [SerializeField] private ItemData throwingItem;

    [Header("Options")]
    [SerializeField] private bool printSlotStateOnStart = true;

    private Keyboard _keyboard;

    private void Awake()
    {
        _keyboard = Keyboard.current;
    }

    private void Start()
    {
        if (inventoryContainer == null)
        {
            Debug.LogError("[InventoryTest] inventoryContainer가 비어있음");
            return;
        }

        if (printSlotStateOnStart)
        {
            Debug.Log("=== 시작 시 인벤토리 상태 ===");
            PrintContainerState(inventoryContainer, "Inventory");
        }
    }

    private void Update()
    {
        if (_keyboard == null)
            return;

        if (_keyboard.f1Key.wasPressedThisFrame)
        {
            bool result = inventoryContainer.AddItem(normalItem, 1);
            Debug.Log($"[TEST] 일반 아이템 1개 추가: {result}");
            PrintContainerState(inventoryContainer, "Inventory");
        }

        // 테스트로 이거 비활성화 하고 근접무기 해놔씅
        //if (_keyboard.f2Key.wasPressedThisFrame)
        //{
        //    bool result = inventoryContainer.AddItem(countableItem, 5);
        //    Debug.Log($"[TEST] 스택 아이템 5개 추가: {result}");
        //    PrintContainerState(inventoryContainer, "Inventory");
        //}

        if (_keyboard.f2Key.wasPressedThisFrame)
        {
            bool result = inventoryContainer.AddItem(equipItem2, 1);
            Debug.Log($"근접 아이템 1개 추가: {result}");
            PrintContainerState(inventoryContainer, "Inventory");
        }

        if (_keyboard.f3Key.wasPressedThisFrame)
        {
            bool result = inventoryContainer.AddItem(foodItem, 3);
            Debug.Log($"[TEST] 무기 아이템 3개 추가: {result}");
            PrintContainerState(inventoryContainer, "Inventory");
        }

        if (_keyboard.f4Key.wasPressedThisFrame)
        {
            bool result = inventoryContainer.AddItem(equipItem, 1);
            Debug.Log($"[TEST] 무기 아이템 2개 추가: {result}");
            PrintContainerState(inventoryContainer, "Inventory");
        }

        if (_keyboard.f5Key.wasPressedThisFrame)
        {
            bool result = inventoryContainer.AddItem(throwingItem, 1);
            Debug.Log($"[TEST] 0번 슬롯 아이템 사용: {result}");
            PrintContainerState(inventoryContainer, "Inventory");
        }

        if (_keyboard.f6Key.wasPressedThisFrame)
        {
            bool result = inventoryContainer.MoveItemTo(0, storageContainer, 0);
            Debug.Log($"[TEST] Inventory[0] -> Storage[0] 이동: {result}");
            PrintContainerState(inventoryContainer, "Inventory");
            PrintContainerState(storageContainer, "Storage");
        }

        if (_keyboard.f7Key.wasPressedThisFrame)
        {
            bool result = inventoryContainer.MoveItemTo(1, quickSlotContainer, 0);
            Debug.Log($"[TEST] Inventory[1] -> QuickSlot[0] 이동: {result}");
            PrintContainerState(inventoryContainer, "Inventory");
            PrintContainerState(quickSlotContainer, "QuickSlot");
        }

        if (_keyboard.f8Key.wasPressedThisFrame)
        {
            int count = inventoryContainer.GetItemCount(countableItem);
            Debug.Log($"[TEST] Inventory 안의 {countableItem.name} 총 개수: {count}");
        }

        if (_keyboard.f9Key.wasPressedThisFrame)
        {
            PrintContainerState(inventoryContainer, "Inventory");

            if (storageContainer != null)
                PrintContainerState(storageContainer, "Storage");

            if (quickSlotContainer != null)
                PrintContainerState(quickSlotContainer, "QuickSlot");
        }

        if (_keyboard.f10Key.wasPressedThisFrame)
        {
            ClearAllSlots(inventoryContainer);
            Debug.Log("[TEST] 인벤토리 전체 비움");
            PrintContainerState(inventoryContainer, "Inventory");
        }
    }

    private void ClearAllSlots(ItemContainer container)
    {
        if (container == null)
            return;

        for (int i = 0; i < container.SlotCount; i++)
        {
            ItemSlot slot = container.GetSlot(i);

            if (slot == null || slot.IsEmpty)
                continue;

            container.ClearSlot(i);
        }
    }

    private void PrintContainerState(ItemContainer container, string label)
    {
        if (container == null)
        {
            Debug.LogWarning($"[{label}] container is null");
            return;
        }

        Debug.Log($"==== {label} 상태 ====");

        for (int i = 0; i < container.SlotCount; i++)
        {
            ItemSlot slot = container.GetSlot(i);

            if (slot == null || slot.IsEmpty)
                Debug.Log($"[{label}] Slot {i}: Empty");
            else
                Debug.Log($"[{label}] Slot {i}: {slot.ItemData.name} x {slot.Amount}");
        }
    }
}