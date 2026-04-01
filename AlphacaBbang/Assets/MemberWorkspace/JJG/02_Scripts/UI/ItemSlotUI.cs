using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class ItemSlotUI : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI amountText;
    
    public void SetSlotIndex(int index)
    {
        
    }

    public void SetItem(Sprite itemSprite)
    {
        iconImage.sprite = itemSprite;
    }
}
