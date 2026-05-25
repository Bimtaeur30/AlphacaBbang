using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MemberWorkspace.CHG._02_Scripts.PawnShop
{
    public class PawnItemUI : MonoBehaviour
    {
        [field: SerializeField] public Button btn;
        [field: SerializeField] public Image itemImage;
        [field: SerializeField] public TextMeshProUGUI itemName;
        [HideInInspector] public CountableItemData SaleItemDataSO;
    }
}