using MemberWorkspace.JJG._02_Scripts.Item.Data;
using UnityEngine;

namespace MemberWorkspace.JJG._02_Scripts
{
    public class QuickSlotContainer : InventoryContainer
    {
        public override bool CanPlaceItem(int index, ItemData itemData)
        {
            if (!base.CanPlaceItem(index, itemData))
                return false;

            if (itemData == null)
                return false;

            if (itemData is WeaponItemData)
                return index < 3;

            if (itemData is FoodItemData || itemData is MedicineItemData)
                return index >= 3;

            return false;
        }
    }
}
