using MemberWorkspace.JJG._02_Scripts.Item.Data;
using UnityEngine;

public class FoodItem : CountableItem, IUsableItem
{
    public FoodItem(CountableItemData data, int amount = 1) : base(data, amount)
    {
    }

    protected override CountableItem Clone(int amount)
    {
        return new FoodItem(CountableData as FoodItemData, amount);
    }

    public bool Use(GameObject user)
    {
        Amount--;

        return true;
    }
}
