using UnityEngine;

public class InventoryTester : MonoBehaviour
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
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            inventory.UseItem(0, gameObject);
            PrintInventory();
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            inventory.UseItem(1, gameObject);
            PrintInventory();
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            inventory.UseItem(2, gameObject);
            PrintInventory();
        }

        if (Input.GetKeyDown(KeyCode.I))
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
            Debug.Log($"{i}: {item.itemData.itemName} x{item.count}");
        }

        Debug.Log("===================");
    }
}
