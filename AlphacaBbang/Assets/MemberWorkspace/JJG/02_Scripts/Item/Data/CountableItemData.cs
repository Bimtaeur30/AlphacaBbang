using UnityEngine;

[CreateAssetMenu(fileName = "CountableItemData", menuName = "Scriptable Objects/CountableItemData")]
public class CountableItemData : ItemData
{
    [field: SerializeField, Min(1)]
    public int MaxAmount { get; private set; } = 1;
}
