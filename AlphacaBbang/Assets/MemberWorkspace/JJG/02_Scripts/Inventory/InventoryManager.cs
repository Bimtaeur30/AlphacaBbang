using MemberWorkspace.JJG._02_Scripts;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    [field: SerializeField] public InventoryContainer Inventory { get; private set; }
    //[field: SerializeField] public StorageContainer Storage { get; private set; }
    [field: SerializeField] public EquipmentContainer Equipment { get; private set; }
    [field: SerializeField] public QuickSlotContainer QuickSlot { get; private set; }
    

    private void Awake()
    {
        //ItemMoveService = new ItemMoveService();
        //EquipmentMoveService = new EquipmentMoveService();
    }
}
