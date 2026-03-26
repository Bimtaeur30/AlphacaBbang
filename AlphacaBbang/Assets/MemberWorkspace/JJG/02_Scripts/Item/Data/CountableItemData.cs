using UnityEngine;

[CreateAssetMenu(fileName = "CountableItemData", menuName = "Scriptable Objects/CountableItemData")]
public class CountableItemData : ItemData
{
    [field: SerializeField]
    public int MaxAmount { get; private set; }
}
