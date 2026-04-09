using UnityEngine;

public class ItemSlot
{
    public ItemData ItemData;
    public int Amount;

    public bool IsEmpty => ItemData == null;
    public bool CanStack => !IsEmpty;
    
    public void Clear()
    {
        ItemData = null;
        Amount = 0;
    }
}
