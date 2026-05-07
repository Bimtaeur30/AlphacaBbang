namespace MemberWorkspace.JJG._02_Scripts
{
    public interface IItemContainer
    {
        int SlotCount { get; }
        ItemSlot GetSlot(int index);
        bool CanPlaceItem(int index, ItemData itemData);
        bool SetSlot(int index, ItemData itemData, int amount);
        bool ClearSlot(int index);
    }
}