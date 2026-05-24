using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MemberWorkspace.CHG._02_Scripts.QuestSystem
{
    public class QuestPanelUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private GameObject conditionTextPrefab;

        public event Action<Quest> OnClaimed;
        
        public Quest Quest => _quest;
        
        private Quest _quest;
        private List<TextMeshProUGUI> _conditionTexts = new List<TextMeshProUGUI>();
        
        public void Initialize(Quest quest)
        {
            _quest = quest;
            nameText.text = quest.Data.Name;
            
            foreach (QuestCondition condition in quest.Conditions)
            {
                GameObject obj = Instantiate(conditionTextPrefab, transform);
                TextMeshProUGUI text = obj.GetComponent<TextMeshProUGUI>();
                text.text = $" {condition.TargetId}: {condition.Progress} / {condition.Required}";
                _conditionTexts.Add(text);
            }
        }
        
        public void UpdateProgress()
        {
            for (int i = 0; i < _quest.Conditions.Count; i++)
            {
                QuestCondition condition = _quest.Conditions[i];
                _conditionTexts[i].text = $"{condition.TargetId}: {condition.Progress} / {condition.Required}";

                if (condition.Progress >= condition.Required)
                    _conditionTexts[i].color = Color.green;
            }

            if (_quest.IsCompleted)
            {
                nameText.color = Color.green;
            }
        }
    }
}
