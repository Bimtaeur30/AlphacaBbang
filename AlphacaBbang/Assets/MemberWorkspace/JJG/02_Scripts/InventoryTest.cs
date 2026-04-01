using UnityEngine;
using UnityEngine.InputSystem;

public class InventoryTest : MonoBehaviour
{
    [SerializeField] private Inventory inventory;
    [SerializeField] private ItemData potionItem;
    [SerializeField] private ItemData swordItem;
    [SerializeField] private ItemData woodItem;

    private void Start()
    {
        inventory.AddItem(potionItem, 5);
        inventory.AddItem(swordItem, 1);
        inventory.AddItem(woodItem, 12);

        PrintInventory();
    }

    private void Update()
    {
        if (Keyboard.current.digit1Key.wasPressedThisFrame)
        {
            inventory.UseItem(0, gameObject);
            PrintInventory();
        }

        if (Keyboard.current.digit2Key.wasPressedThisFrame)
        {
            inventory.UseItem(1, gameObject);
            PrintInventory();
        }

        if (Keyboard.current.digit3Key.wasPressedThisFrame)
        {
            inventory.AddItem(potionItem, 5);
            PrintInventory();
        }

        if (Keyboard.current.iKey.wasPressedThisFrame)
        {
            PrintInventory();
        }
    }

    private void PrintInventory()
    {
        Debug.Log("===== 인벤토리 =====");

        var items = inventory.Items;
        for (int i = 0; i < items.Count; i++)
        {
            InventoryItem item = items[i];
            Debug.Log($"{i}: {item.itemData.ItemName} x{item.count}");
        }

        Debug.Log("===================");
    }
}
