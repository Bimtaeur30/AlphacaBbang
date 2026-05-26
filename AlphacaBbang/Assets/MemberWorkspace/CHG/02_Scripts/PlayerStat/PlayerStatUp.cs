using JJH._02_Scripts_Systems.EventSystems;
using System;
using System.Collections;
using UnityEngine;

namespace MemberWorkspace.CHG._02_Scripts.PlayerStat
{
    [Serializable]
    public struct PlayerStatStruct
    {
        public PlayerStatType statType;
        public PlayerStatUpUI statUpUi;
        public ItemData needItem;
        public int statUpValue;
        public int needItemCount;
        public int needValueIncrease;
    }

    public class PlayerStatUp : MonoBehaviour
    {
        [SerializeField] private EventChannelSO playerStatChannel;
        [SerializeField] private PlayerStatStruct[] _statViews;
        [SerializeField] private InventoryContainer inventory;
        [SerializeField] private int needGold;

        private PlayerSaveData _playerSaveData;
        private SlidePanelController _slidePanelController;
        private PlayerStatUpUIRefresher _uiRefresher;

        private void Awake()
        {
            _slidePanelController = GetComponent<SlidePanelController>();
            _uiRefresher = GetComponent<PlayerStatUpUIRefresher>();
            _playerSaveData = PlayerStatSystem.Instance.SaveData;
            
            for (int i = 0; i < _statViews.Length; i++)
                _statViews[i].statType = (PlayerStatType)i;
        }

        public void UIRefresh()
        {
            _uiRefresher.RefreshAll(_statViews, inventory, _playerSaveData, needGold);
        }

        public void AddStat(int index)
        {
            PlayerStatType type = (PlayerStatType)index;
            PlayerStatStruct pStruct = _statViews[index];

            if (inventory.GetItemCount(pStruct.needItem) < pStruct.needItemCount)
            {
                Debug.Log("Item이 부족합니다.");
                return;
            }
            if (_playerSaveData.Gold < needGold) return;

            int cur = (int)GetCurrentStatValue(type);
            int next = cur + _statViews[index].statUpValue;

            switch (type)
            {
                case PlayerStatType.Health:
                    playerStatChannel.RaiseEvent(new AddMaxHealth().Init(next - cur));
                    break;
                case PlayerStatType.Stamina:
                    playerStatChannel.RaiseEvent(new AddMaxStamina().Init(next - cur));
                    break;
                case PlayerStatType.AimStamina:
                    playerStatChannel.RaiseEvent(new AddMaxAimStamina().Init(next - cur));
                    break;
                case PlayerStatType.Gold:
                    playerStatChannel.RaiseEvent(new AddGold().Init(next - cur));
                    break;
                default:
                    Debug.LogWarning("NOoo");
                    return;
            }

            inventory.ConsumeBulletByName(pStruct.needItem.ItemName, pStruct.needItemCount);
            playerStatChannel.RaiseEvent(new AddGold().Init(-needGold));

            _statViews[index].needItemCount += _statViews[index].needValueIncrease;

            _uiRefresher.RefreshAll(_statViews, inventory, _playerSaveData, needGold);
        }

        private float GetCurrentStatValue(PlayerStatType type) => type switch
        {
            PlayerStatType.Health     => _playerSaveData.MaxHealth,
            PlayerStatType.Stamina    => _playerSaveData.MaxStamina,
            PlayerStatType.AimStamina => _playerSaveData.MaxAimStamina,
            PlayerStatType.Gold       => _playerSaveData.Gold,
            _                         => 0f
        };

        [ContextMenu("ddd")]
        private void ddd() => inventory.AddItem(_statViews[0].needItem, 40);
    }
}