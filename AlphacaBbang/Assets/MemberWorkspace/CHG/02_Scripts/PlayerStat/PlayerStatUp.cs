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

        [SerializeField] private ItemData[] needItem;
        [SerializeField] private EventChannelSO playerStatChannel;
        [SerializeField] private EventChannelSO systemChannel;
        [SerializeField] private TextMeshProUGUI needItemText;
        [SerializeField] private PlayerStatStruct[] _statViews;

        [SerializeField] private InventoryContainer inventory;

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
                _statViews[i].needItem = needItem[i];
            }

            var sb = new StringBuilder("필요한 재료\n");
            foreach (PlayerStatStruct pStruct in _statViews)
            {
                int idx = (int)pStruct.StatType;
                int cur = (int)GetCurrentStat(pStruct.StatType);
                pStruct.statUpUi.StatTextChange(cur.ToString(), (cur + pStruct.statUpValue).ToString());
                sb.AppendLine($"{StatLabels[idx]}: {pStruct.needItem.ItemName}X{pStruct.needItemCount}");
            }
            needItemText.text = sb.ToString();

            foreach (PlayerStatStruct pStruct in _statViews)
            {

                int curStat = (int)GetCurrentStat(pStruct.StatType);
                pStruct.statUpUi.StatTextChange(curStat.ToString(), (curStat + pStruct.statUpValue).ToString());
            }
        }

        public void AddStat(int index)
        {
            PlayerStatType type = (PlayerStatType)index;
            PlayerStatStruct pStruct = _statViews[index];

            if (inventory.GetItemCount(pStruct.needItem) < pStruct.needItemCount)
            {
                return;
            }
            int idx = (int)type;
            int cur = (int)GetCurrentStat(type);
            int next = cur + _statViews[idx].statUpValue;

            switch (type)
            {
                case PlayerStatType.Health:
                    playerStatChannel.RaiseEvent(new AddMaxHealth().Init(next));
                    break;
                case PlayerStatType.Stamina:
                    systemChannel.RaiseEvent(new AddMaxStamina().Init(next));
                    break;
                case PlayerStatType.AimStamina:
                    systemChannel.RaiseEvent(new AddMaxAimStamina().Init(next));
                    break;
                case PlayerStatType.Gold:
                    systemChannel.RaiseEvent(new AddGold().Init(next));
                    break;
                default:
                    Debug.LogWarning("NOoo");
                    return;
            }

            int updated = (int)GetCurrentStat(type);
            _statViews[idx].statUpUi.StatTextChange(updated.ToString(), (updated + _statViews[idx].statUpValue).ToString());
            _statViews[idx].needItemCount += _statViews[idx].needValueIncrease;

            inventory.ConsumeBulletByName(pStruct.needItem.Id, pStruct.needItemCount);
        }

        private float GetCurrentStat(PlayerStatType type) => type switch
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
            transform.GetComponent<RectTransform>().position = _slidePanelController.HiddenPosition;

        }
    }


}