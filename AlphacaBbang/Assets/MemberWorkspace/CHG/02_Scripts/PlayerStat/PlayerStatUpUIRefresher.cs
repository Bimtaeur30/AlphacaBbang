using UnityEngine;
using System.Text;
using TMPro;

namespace MemberWorkspace.CHG._02_Scripts.PlayerStat
{
    public class PlayerStatUpUIRefresher : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI needItemText;
        [SerializeField] private TextMeshProUGUI currentItemText;
        [SerializeField] private TextMeshProUGUI needGoldText;
        [SerializeField] private TextMeshProUGUI currentGoldText;

        private static readonly string[] StatLabels = { "체력", "지구력", "집중력" };

        public void RefreshAll(
            PlayerStatStruct[] statViews,
            InventoryContainer inventory,
            PlayerSaveData saveData,
            int needGold)
        {
            RefreshItemTexts(statViews, inventory);
            RefreshStatUIs(statViews, saveData);
            RefreshGoldTexts(saveData, needGold);
        }

        public void RefreshItemTexts(PlayerStatStruct[] statViews, InventoryContainer inventory)
        {
            var needSb = new StringBuilder();
            var curSb = new StringBuilder();

            foreach (PlayerStatStruct pStruct in statViews)
            {
                int idx = (int)pStruct.statType;
                needSb.AppendLine($"{StatLabels[idx]}: {pStruct.needItem.ItemName}X{pStruct.needItemCount}");
                curSb.AppendLine($"{pStruct.needItem.ItemName}X{inventory.GetItemCount(pStruct.needItem)}");
            }

            needItemText.text = needSb.ToString();
            currentItemText.text = curSb.ToString();
        }

        public void RefreshStatUIs(PlayerStatStruct[] statViews, PlayerSaveData saveData)
        {
            foreach (PlayerStatStruct pStruct in statViews)
            {
                int cur = (int)GetCurrentStatValue(pStruct.statType, saveData);
                pStruct.statUpUi.StatTextChange(cur.ToString(), (cur + pStruct.statUpValue).ToString());
            }
        }

        public void RefreshGoldTexts(PlayerSaveData saveData, int needGold)
        {
            if (needGoldText != null)
                needGoldText.text = $"필요 골드: {needGold}";
            if (currentGoldText != null)
                currentGoldText.text = $"보유 골드: {(int)saveData.Gold}";
        }

        private float GetCurrentStatValue(PlayerStatType type, PlayerSaveData saveData) => type switch
        {
            PlayerStatType.Health     => saveData.MaxHealth,
            PlayerStatType.Stamina    => saveData.MaxStamina,
            PlayerStatType.AimStamina => saveData.MaxAimStamina,
            PlayerStatType.Gold       => saveData.Gold,
            _                         => 0f
        };
    }
}
