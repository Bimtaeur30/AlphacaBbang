using UnityEngine;

[CreateAssetMenu(fileName = "SaleItemDataSO", menuName = "CHG/SaleItemData")]
public class SaleItemDataSO : CountableItemData
{
    [field: SerializeField] public int Price { get; private set; }
}
