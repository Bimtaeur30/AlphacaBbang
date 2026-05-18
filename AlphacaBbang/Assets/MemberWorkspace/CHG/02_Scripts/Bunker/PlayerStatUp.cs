using System;
using System.Collections.Generic;
using DG.Tweening;
using JJH._02_Scripts_Systems.EventSystems;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MemberWorkspace.CHG._02_Scripts.Bunker
{
    [Serializable]
    struct PlayerStatViewUI
    {
        public GameObject Obj;
        public TextMeshProUGUI Text;
        public Button Button;
    }
    
    public class PlayerStatUp : MonoBehaviour
    {
        [SerializeField] private EventChannelSO playerStatChannel;
        [SerializeField] private EventChannelSO systemChannel;
        
        [SerializeField] private int healthUpValue;
        [SerializeField] private float speedUpValue;
        [SerializeField] private int staminaUpValue;
        [SerializeField] private int aimStaminaUpValue;

        [SerializeField] private TextMeshProUGUI StatUpPointText;
        [SerializeField] private TextMeshProUGUI LevelText;

        [SerializeField] private PlayerStatViewUI[] statView;

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
            _tween = _canvasGroup.DOFade(0, 0.0f);
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;
            
            StatUpPointText.text = "남은 포인트: " + PlayerStateManager.Instance.StatUpPoint;
            LevelText.text = "현재 레벨: " + PlayerStateManager.Instance.CurrentLevel;
            for (int i = 0; i < statView.Length; i++)
            {
                int index = i;
                statView[i].Button.onClick.AddListener(() => AddStat((PlayerStatType)index));
                string s = "";
                switch ((PlayerStatType)i)
                {
                    case PlayerStatType.Health:
                        s = _playerSaveData.MaxHealth.ToString();
                        break;
                    case PlayerStatType.Stamina:
                        s = _playerSaveData.MaxStamina.ToString();
                        break;
                    case PlayerStatType.AimStamina:
                        s = _playerSaveData.MaxStamina.ToString();
                        break;
                }
                statView[i].Text.text = s;
            }
        }

        public void AddStat(PlayerStatType type)
        {
            if (PlayerStateManager.Instance.StatUpPoint <= 0) return;
            PlayerStateManager.Instance.StatUpPoint--;
            
            switch (type)
            {
                case PlayerStatType.Health:
                {
                    AddMaxHealth addHealth = new AddMaxHealth().Init(healthUpValue);
                    playerStatChannel.RaiseEvent(addHealth);
                    ChangeText(PlayerStatType.Health , _playerSaveData.MaxHealth);
                }
                    break;
                case PlayerStatType.Stamina:
                    AddMaxStamina addMaxStamina = new AddMaxStamina().Init(staminaUpValue);
                    systemChannel.RaiseEvent(addMaxStamina);
                    ChangeText(PlayerStatType.Stamina ,_playerSaveData.MaxStamina);
                    break;
                case PlayerStatType.AimStamina:
                    AddMaxAimStamina addAimStamina = new AddMaxAimStamina().Init(aimStaminaUpValue);
                    systemChannel.RaiseEvent(addAimStamina);
                    ChangeText(PlayerStatType.AimStamina ,_playerSaveData.MaxAimStamina);
                    break;
                default:
                    Debug.LogWarning("NOoo");
                    break;
            }
        }

        private void ChangeText(PlayerStatType type, float value)
        {
            statView[(int)type].Text.text = value.ToString();
        }

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