using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace MemberWorkspace.CHG._02_Scripts.PawnShop
{
    
    public class PawnShop : MonoBehaviour
    {
        [SerializeField] private Transform layOut;
        [SerializeField] private ItemDatabase itemDatabase;
        [SerializeField] private GameObject itemChoiceBtnPrefab;

        [SerializeField] private Image itemImage;
        [SerializeField] private TextMeshProUGUI itemNameText;
        [SerializeField] private TextMeshProUGUI itemGradeText;
        [SerializeField] private TextMeshProUGUI itemPriceText;
        [SerializeField] private TextMeshProUGUI itemCountText;
        [SerializeField] private Slider itemCountSlider;
        
        private InventoryContainer inventory;
        private SaleItemDataSO _curItemData;
        
        private List<PawnItemUI> itemChoiceBtns =  new List<PawnItemUI>();
        private void Awake()
        {
            foreach (SaleItemDataSO itemData in itemDatabase.Items)
            {
                GameObject itemChoiceBtn = Instantiate(itemChoiceBtnPrefab, layOut);
                PawnItemUI itemUI = itemChoiceBtn.GetComponent<PawnItemUI>();
                itemUI.SaleItemDataSO = itemData;
                itemChoiceBtns.Add(itemUI);
                itemUI.btn.onClick.AddListener(() => ChangeContent(itemData));
            }
            inventory = FindFirstObjectByType<InventoryContainer>();
            
            ChangeContent(itemChoiceBtns[0].SaleItemDataSO);
        }

        private void ChangeContent(SaleItemDataSO itemData)
        {
            _curItemData = itemData;
            itemImage.sprite = itemData.Icon;
            itemNameText.text = itemData.ItemName;
            itemGradeText.text = itemData.GradeType.ToString();
            itemPriceText.text = itemData.Price.ToString();
            int itemCount = inventory.GetItemCount(itemData);
            itemCountText.text = itemCount.ToString();
            itemCountSlider.maxValue = itemCount;
            itemCountSlider.value = 0;
        }

        public void SellItem()
        {
            if (inventory.GetItemCount(_curItemData) < itemCountSlider.value) return;
            
            inventory.ConsumeBulletByName(_curItemData.ItemName, (int)itemCountSlider.value);
            ChangeContent(_curItemData);
            //플레이어 골드 추가
        }
    }
}