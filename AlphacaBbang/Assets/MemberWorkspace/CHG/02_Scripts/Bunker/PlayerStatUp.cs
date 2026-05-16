using System;
using System.Collections.Generic;
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

        private void Start()
        {
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
                        s = PlayerStatSystem.Instance.MaxHealth.ToString();
                        break;
                    case PlayerStatType.Speed:
                        s = PlayerStatSystem.Instance.MoveSpeed.ToString();
                        break;
                    case PlayerStatType.Stamina:
                        s = PlayerStatSystem.Instance.MaxStamina.ToString();
                        break;
                    case PlayerStatType.AimStamina:
                        s = PlayerStatSystem.Instance.GaugeMaxTime.ToString();
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
                    ChangeText(PlayerStatType.Health ,PlayerStatSystem.Instance.MaxHealth);
                }
                    break;
                case PlayerStatType.Speed:
                {
                    //추가해야함.
                    PlayerStatSystem.Instance.AddStat(type, speedUpValue);
                    ChangeText(PlayerStatType.Speed ,PlayerStatSystem.Instance.MoveSpeed);
                }
                    break;
                case PlayerStatType.Stamina:
                    AddMaxStamina addMaxStamina = new AddMaxStamina().Init(staminaUpValue);
                    systemChannel.RaiseEvent(addMaxStamina);
                    ChangeText(PlayerStatType.Stamina ,PlayerStatSystem.Instance.MaxStamina);
                    break;
                case PlayerStatType.AimStamina:
                    AddMaxAimStamina addAimStamina = new AddMaxAimStamina().Init(aimStaminaUpValue);
                    systemChannel.RaiseEvent(addAimStamina);
                    ChangeText(PlayerStatType.AimStamina ,PlayerStatSystem.Instance.GaugeMaxTime);
                    break;
                default:
                    Debug.LogWarning("NOoo");
                    break;
            }
        }
        
        /*public void StatDown(PlayerStatType type)
        {
            PlayerStateManager.Instance.StateUpPoint++;
            
            switch (type)
            {
                case PlayerStatType.Speed:
                {
                    changeValues[type] -= speedUpValue;
                    ChangeText(type, PlayerStatSystem.Instance.MoveSpeed - changeValues[type]);
                }
                    break;
                case PlayerStatType.Health:
                {
                    changeValues[type] -= healthUpValue;
                    ChangeText(type, PlayerStatSystem.Instance.MaxHealth - changeValues[type]);
                }
                    break;
                case PlayerStatType.Stamina:
                    changeValues[type] -= staminaUpValue;
                    ChangeText(type, PlayerStatSystem.Instance.MaxStamina - changeValues[type]);
                    break;
                case PlayerStatType.Concentration:
                    
                    break;
            }
        }*/

        /*public void SetStat()
        {
            PlayerStatSystem.Instance.MoveSpeed += changeValues[PlayerStatType.Speed];
            PlayerStatSystem.Instance.MaxHealth += changeValues[PlayerStatType.Health];
            PlayerStatSystem.Instance.MaxStamina += changeValues[PlayerStatType.Stamina];
            
        }*/

        private void ChangeText(PlayerStatType type, float value)
        {
            statView[(int)type].Text.text = value.ToString();
        }
    }
}