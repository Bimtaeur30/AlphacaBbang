    using JJH._02_Scripts_Systems.EventSystems;
using System;
using System.Collections;
using System.Text;
using TMPro;
using UnityEngine;

namespace MemberWorkspace.CHG._02_Scripts.PlayerStat
{
    [Serializable]
    struct PlayerStatStruct
    {
        public PlayerStatType StatType;
        public PlayerStatUpUI statUpUi;
        public ItemData needItem;
        public int statUpValue;
        public int needItemCount;
        public int needValueIncrease;
    }

    public class PlayerStatUp : MonoBehaviour
    {
        private static readonly string[] StatLabels = { "체력", "지구력", "집중력" };

        [SerializeField] private EventChannelSO playerStatChannel;
        [SerializeField] private TextMeshProUGUI needItemText;
        [SerializeField] private PlayerStatStruct[] _statViews;

        [SerializeField] private InventoryContainer inventory;
        [SerializeField] private int needGold;

        private PlayerSaveData _playerSaveData;
        private SlidePanelController _slidePanelController;

        private void Awake()
        {
            _slidePanelController = GetComponent<SlidePanelController>();
            _playerSaveData = PlayerStatSystem.Instance.SaveData;
        }

        private IEnumerator Start()
        {
            yield return null;
            yield return null;


            for (int i = 0; i < _statViews.Length; i++)
            {
                _statViews[i].StatType = (PlayerStatType)i;
            }

            var sb = new StringBuilder("");
            foreach (PlayerStatStruct pStruct in _statViews)
            {
                int idx = (int)pStruct.StatType;
                int cur = (int)GetCurrentStatValue(pStruct.StatType);
                pStruct.statUpUi.StatTextChange(cur.ToString(), (cur + pStruct.statUpValue).ToString());
                sb.AppendLine($"{StatLabels[idx]}: {pStruct.needItem.ItemName}X{pStruct.needItemCount}");
            }
            needItemText.text = sb.ToString();

            foreach (PlayerStatStruct pStruct in _statViews)
            {
                int curStat = (int)GetCurrentStatValue(pStruct.StatType);
                pStruct.statUpUi.StatTextChange(curStat.ToString(), (curStat + pStruct.statUpValue).ToString());
            }
        }

        public void AddStat(int index)
        {
            PlayerStatType type = (PlayerStatType)index;
            PlayerStatStruct pStruct = _statViews[index];

            Debug.Log($"원하는 아이템수: {inventory.GetItemCount(pStruct.needItem)}, 필요한 수: {pStruct.needItemCount}");
            if (inventory.GetItemCount(pStruct.needItem) < pStruct.needItemCount)
            {
                Debug.Log("Item이 부족합니다.");
                return;
            }

            if (_playerSaveData.Gold < needGold) return;
            
            
            int idx = (int)type;
            int cur = (int)GetCurrentStatValue(type);
            int next = cur + _statViews[idx].statUpValue;

            switch (type)
            {
                case PlayerStatType.Health:
                    playerStatChannel.RaiseEvent(new AddMaxHealth().Init(next-cur));
                    break;
                case PlayerStatType.Stamina:
                    playerStatChannel.RaiseEvent(new AddMaxStamina().Init(next-cur));
                    break;
                case PlayerStatType.AimStamina:
                    playerStatChannel.RaiseEvent(new AddMaxAimStamina().Init(next-cur));
                    break;
                case PlayerStatType.Gold:
                    playerStatChannel.RaiseEvent(new AddGold().Init(next-cur));
                    break;
                default:
                    Debug.LogWarning("NOoo");
                    return;
            }

            int updated = (int)GetCurrentStatValue(type);
            _statViews[idx].statUpUi.StatTextChange(updated.ToString(), (updated + _statViews[idx].statUpValue).ToString());
            _statViews[idx].needItemCount += _statViews[idx].needValueIncrease;

            inventory.ConsumeBulletByName(pStruct.needItem.ItemName, pStruct.needItemCount);
            playerStatChannel.RaiseEvent(new AddGold().Init(-needGold));
        }

        private float GetCurrentStatValue(PlayerStatType type) => type switch
        {
            PlayerStatType.Health => _playerSaveData.MaxHealth,
            PlayerStatType.Stamina => _playerSaveData.MaxStamina,
            PlayerStatType.AimStamina => _playerSaveData.MaxAimStamina,
            PlayerStatType.Gold => _playerSaveData.Gold,
            _ => 0f
        };

        [ContextMenu("ddd")]
        private void ddd()
        {
            inventory.AddItem(_statViews[0].needItem, 40);

        }
    }


}