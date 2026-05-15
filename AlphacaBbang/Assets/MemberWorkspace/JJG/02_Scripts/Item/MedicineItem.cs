using MemberWorkspace.JJG._02_Scripts.Item.Data;
using UnityEngine;

public class MedicineItem : CountableItem, IUsableItem
{
    public MedicineItem(MedicineItemData data, int amount = 1) : base(data, amount)
    {
        
    }
    public bool Use(GameObject user)
    {
        Amount--;
        return true;
    }

    protected override CountableItem Clone(int amount)
    {
        return new MedicineItem(CountableData as MedicineItemData, amount);
    }
}
