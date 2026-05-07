using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace MemberWorkspace.CHG._02_Scripts.QuestSystem
{ 
    public struct QuestUIStruct
    {
        public GameObject Panel;
        public  TextMeshProUGUI NameText;
        public List<TextMeshProUGUI> ConditionTexts;

        public QuestUIStruct(GameObject panel, TextMeshProUGUI nameText, List<TextMeshProUGUI> conditionTexts)
        {
            Panel = panel;
            NameText = nameText;
            ConditionTexts = conditionTexts;
        }
    }

    public class QuestUIManager : MonoBehaviour
    {
        [SerializeField] private GameObject layOutGroup;
        [SerializeField] private GameObject questPanelPrefab;
        [SerializeField] private GameObject conditionTextPrefab;
    
        private Dictionary<Quest, QuestUIStruct> _activeQuests = new();

        private void Awake()
        {
            QuestManager.Instance.OnQuestAccepted += AcceptQuest;
            QuestManager.Instance.OnUpdateQuestProgress += UpdateEnemyKillProgress;
        }

        private void AcceptQuest(Quest quest)
        {
            GameObject panel = Instantiate(questPanelPrefab, layOutGroup.transform);
            
            TextMeshProUGUI nameText = panel.GetComponentInChildren<TextMeshProUGUI>();
            nameText.text = quest.Data.Name;
            
            Transform conditionLayout = panel.transform.Find("ConditionLayoutGroup");

            List<TextMeshProUGUI> conditionTexts = new List<TextMeshProUGUI>();
            foreach (QuestCondition condition in quest.Conditions)
            {
                GameObject condObj = Instantiate(conditionTextPrefab, conditionLayout);
                TextMeshProUGUI condText = condObj.GetComponent<TextMeshProUGUI>();
                condText.text = $"{condition.TargetId}: {condition.Progress} / {condition.Required}";
                conditionTexts.Add(condText);
            }

            _activeQuests.Add(quest, new QuestUIStruct(panel, nameText, conditionTexts));
        }


        private void UpdateEnemyKillProgress(Quest quest)
        {
            QuestUIStruct questUIs = _activeQuests[quest];

            for (int i = 0; i < quest.Conditions.Count; i++)
            {
                QuestCondition condition = quest.Conditions[i];
                questUIs.ConditionTexts[i].text = $"{condition.TargetId}: {condition.Progress} / {condition.Required}";

                if (condition.Progress >= condition.Required)
                    questUIs.ConditionTexts[i].color = Color.green;
            }
            
            if (quest.IsCompleted)
                questUIs.NameText.color = Color.green;
        }
        
        
        private static string QuestConditionString(Quest quest)
        {
            string questConditionString = "";
            foreach (QuestCondition condition in quest.Conditions)
            {
                questConditionString += $"{condition.TargetId}: {condition.Progress} / {condition.Required}\n";
            }

            return questConditionString;
        }
    }
}