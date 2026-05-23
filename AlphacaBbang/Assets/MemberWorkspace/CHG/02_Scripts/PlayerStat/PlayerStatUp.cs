using System;
using System.Text;
using DG.Tweening;
using JJH._02_Scripts_Systems.EventSystems;
using TMPro;
using UnityEngine;

namespace MemberWorkspace.CHG._02_Scripts.PlayerStat
{
    [Serializable]
    struct PlayerStatViewUI
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
        [SerializeField] private PlayerStatViewUI[] _statViews;

        private CanvasGroup _canvasGroup;
        private Tween _tween;
        private PlayerSaveData _playerSaveData;

        private void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            _playerSaveData = PlayerStatSystem.Instance.SaveData;
        }

        private void Start()
        {
            _tween = _canvasGroup.DOFade(0, 0f);
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;

            for (int i = 0; i < _statViews.Length; i++)
            {
                _statViews[i].StatType = (PlayerStatType)i;
                _statViews[i].needItem = needItem[i];
            }

            var sb = new StringBuilder("필요한 재료\n");
            foreach (PlayerStatViewUI statView in _statViews)
            {
                int idx = (int)statView.StatType;
                int cur = (int)GetCurrentStat(statView.StatType);
                statView.statUpUi.StatTextChange(cur.ToString(), (cur + statView.statUpValue).ToString());
                sb.AppendLine($"{StatLabels[idx]}: {statView.needItem.ItemName}X{statView.needItemCount}");
            }
            needItemText.text = sb.ToString();
        }

        public void AddStat(PlayerStatType type)
        {
            //아이템 없으면 취소 추가
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
                default:
                    Debug.LogWarning("NOoo");
                    return;
            }

            int updated = (int)GetCurrentStat(type);
            _statViews[idx].statUpUi.StatTextChange(updated.ToString(), (updated + _statViews[idx].statUpValue).ToString());
            _statViews[idx].needItemCount +=  _statViews[idx].needValueIncrease;
            //아이템 감소 추가
        }

        private float GetCurrentStat(PlayerStatType type) => type switch
        {
            PlayerStatType.Health     => _playerSaveData.MaxHealth,
            PlayerStatType.Stamina    => _playerSaveData.MaxStamina,
            PlayerStatType.AimStamina => _playerSaveData.MaxAimStamina,
            _                         => 0f
        };

        public void ShowUI()
        {
            _tween?.Kill();
            _tween = _canvasGroup.DOFade(1, 0.5f);
            _canvasGroup.interactable = true;
            _canvasGroup.blocksRaycasts = true;
        }

        public void HideUI()
        {
            _tween?.Kill();
            _tween = _canvasGroup.DOFade(0, 0.5f);
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;
        }
    }
}