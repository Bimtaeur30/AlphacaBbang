using System;
using System.Collections.Generic;
using JJH._02_Scripts_Systems.EventSystems;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace MemberWorkspace.CHG._02_Scripts.PawnShop
{
    
    public class PawnShop : MonoBehaviour
    {
        [SerializeField] private EventChannelSO AddGoldChannel;
        
        [SerializeField] private Transform layOut;
        [SerializeField] private ItemDatabase itemDatabase;
        [SerializeField] private GameObject itemChoiceBtnPrefab;

        [SerializeField] private Image itemImage;
        [SerializeField] private TextMeshProUGUI itemNameText;
        [SerializeField] private TextMeshProUGUI itemGradeText;
        [SerializeField] private TextMeshProUGUI itemPriceText;
        [SerializeField] private TextMeshProUGUI itemCountText;
        [SerializeField] private Slider itemCountSlider;
        
        [SerializeField] private InventoryContainer inventory;
        private SaleItemDataSO _curItemData;
        
        private List<PawnItemUI> itemChoiceBtns =  new List<PawnItemUI>();
        private void Awake()
        {
            foreach (SaleItemDataSO itemData in itemDatabase.Items)
            {
                GameObject itemChoiceBtn = Instantiate(itemChoiceBtnPrefab, layOut);
                PawnItemUI itemUI = itemChoiceBtn.GetComponent<PawnItemUI>();
                itemUI.SaleItemDataSO = itemData;
                itemUI.itemImage.sprite = itemData.Icon;
                itemUI.itemName.text = itemData.ItemName;
                itemChoiceBtns.Add(itemUI);
                itemUI.btn.onClick.AddListener(() => ChangeContent(itemData));
            }
            //inventory = FindFirstObjectByType<InventoryContainer>();
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

        public void SaleItem()
        {
            if (inventory.GetItemCount(_curItemData) < itemCountSlider.value) return;
            
            inventory.ConsumeBulletByName(_curItemData.ItemName, (int)itemCountSlider.value);
            AddGold evt = new AddGold();
            evt.Init(_curItemData.Price * (int)itemCountSlider.value);
            AddGoldChannel.RaiseEvent(evt);
            ChangeContent(_curItemData);
        }

        [ContextMenu("ADDItem")]
        private void AddItem()
        {
            inventory.AddItem(_curItemData);
            inventory.AddItem(_curItemData);
            inventory.AddItem(_curItemData);
            inventory.AddItem(_curItemData);
            inventory.AddItem(_curItemData);
            inventory.AddItem(_curItemData);
            inventory.AddItem(_curItemData);
            inventory.AddItem(_curItemData);
            inventory.AddItem(_curItemData);
            inventory.AddItem(_curItemData);
            inventory.AddItem(_curItemData);
            inventory.AddItem(_curItemData);
            inventory.AddItem(_curItemData);
        }
        
        [ContextMenu("CheckSlots")]
        private void CheckSlots()
        {
            for (int i = 0; i < inventory.SlotCount; i++)
            {
                var slot = inventory.GetSlot(i);
                if (slot != null && !slot.IsEmpty)
                    Debug.Log($"슬롯 {i}: {slot.ItemData.name} x{slot.Amount}");
            }
        }
    }
}