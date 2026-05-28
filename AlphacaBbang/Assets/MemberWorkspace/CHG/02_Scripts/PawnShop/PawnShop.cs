using System;
using System.Collections.Generic;
using System.Diagnostics;
using JJH._02_Scripts_Systems.EventSystems;
using MemberWorkspace.JJG._02_Scripts;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

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
        [SerializeField] private TextMeshProUGUI sliderValueText;
        [SerializeField] private TextMeshProUGUI itemDescriptionText;
        [SerializeField] private TextMeshProUGUI currentGoldText;
        [SerializeField] private Slider itemCountSlider;
        
        [SerializeField] private InventoryContainer inventory;
        private CountableItemData _curItemData;
        
        private List<PawnItemUI> itemChoiceBtns =  new List<PawnItemUI>();
        private void Awake()
        {
            foreach (CountableItemData itemData in itemDatabase.Items)
            {
                GameObject itemChoiceBtn = Instantiate(itemChoiceBtnPrefab, layOut);
                PawnItemUI itemUI = itemChoiceBtn.GetComponent<PawnItemUI>();
                itemUI.SaleItemDataSO = itemData;
                itemUI.itemImage.sprite = itemData.Icon;
                itemUI.itemName.text = itemData.ItemName;
                itemChoiceBtns.Add(itemUI);
                itemUI.btn.onClick.AddListener(() => SetCurrentContent(itemData));
            }
            //inventory = FindFirstObjectByType<InventoryContainer>();
            itemCountSlider.onValueChanged.AddListener(OnSliderValueChange);
            //SetCurrentContent(itemChoiceBtns[0].SaleItemDataSO);
            _curItemData = itemChoiceBtns[0].SaleItemDataSO;
        }

        private void OnDestroy()
        {
            itemCountSlider.onValueChanged.RemoveAllListeners();
        }

        private void OnSliderValueChange(float value)
        {
            sliderValueText.text = ((int)value).ToString();
        }

        private void SetCurrentContent(CountableItemData itemData)
        {
            _curItemData = itemData;
            ChangeContent();
        }
        
        public void ChangeContent()
        {
            itemImage.sprite = _curItemData.Icon;
            itemNameText.text = _curItemData.ItemName;
            itemGradeText.text = _curItemData.GradeType.ToString();
            itemPriceText.text = GetPrice(_curItemData).ToString();
            itemDescriptionText.text = _curItemData.description;
            currentGoldText.text = PlayerStatSystem.Instance.SaveData.Gold.ToString();
            int itemCount = inventory.GetItemCount(_curItemData);
            itemCountText.text = itemCount.ToString();
            itemCountSlider.maxValue = itemCount;
            itemCountSlider.value = 0;
            OnSliderValueChange(itemCountSlider.value);
        }

        public void SaleItem()
        {
            if (inventory.GetItemCount(_curItemData) < itemCountSlider.value) return;
            
            inventory.ConsumeBulletByName(_curItemData.ItemName, (int)itemCountSlider.value);
            AddGold evt = new AddGold();
            evt.Init(GetPrice(_curItemData) * (int)itemCountSlider.value);
            AddGoldChannel.RaiseEvent(evt);
            ChangeContent();
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

        private int GetPrice(CountableItemData itemData)
        {   
            return itemData.GradeType switch
            {
                GradeType.Common => 3,
                GradeType.UnCommon => 5,
                GradeType.Rare => 15,
                GradeType.Epic => 50,
                GradeType.Legendary => 100
            };
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