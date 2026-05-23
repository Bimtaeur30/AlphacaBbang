using System;
using System.Collections.Generic;
using MemberWorkspace.CHG._02_Scripts.QuestSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MemberWorkspace.CHG._02_Scripts.QuestSystem
{
    public class QuestBoardUI : MonoBehaviour
    {
        [SerializeField] private GameObject questChoiceLayout;
        [SerializeField] private GameObject questChoiceBtnPrefab;

        [SerializeField] private TextMeshProUGUI questNameText;
        [SerializeField] private TextMeshProUGUI descriptionText;
        [SerializeField] private TextMeshProUGUI conditionText;
        [SerializeField] private TextMeshProUGUI rewardText;
        [SerializeField] private ItemDatabase itemDatabase;

        private List<QuestData> _choiceQuestDatas = new();
        private Dictionary<string, GameObject> _questChoiceBtnDict = new();
        private string _currentQuestId = "";

        private void Start()
        {
            EmptyingContent();
        }

        private void AddQuestData(string questId)
        {
            if (!QuestManager.Instance.TryGetQuestData(questId, out QuestData questData))
                return;
            _choiceQuestDatas.Add(questData);
        }

        private void AddQuestBtn()
        {
            foreach (QuestData questData in _choiceQuestDatas)
            {
                if (_questChoiceBtnDict.ContainsKey(questData.Id)) return;
                
                GameObject obj = Instantiate(questChoiceBtnPrefab, questChoiceLayout.transform);
                obj.GetComponentInChildren<TextMeshProUGUI>().text = questData.Name;
                obj.GetComponent<Button>().onClick.AddListener(() => SetContent(questData));

                _questChoiceBtnDict[questData.Id] = obj;    

            }
        }

        private void SetContent(QuestData questData)
        {
            _currentQuestId = questData.Id;
            questNameText.text = questData.Name;
            descriptionText.text = questData.Description;
            string conditionString = "";
            foreach (ConditionData conditionData in questData.Conditions)
            {
                conditionString += conditionData.TargetId + ": " + conditionData.Required;
                switch (conditionData.Type)
                {
                    case QuestType.Kill:
                        conditionString += "마리 처치\n";
                        break;
                    case QuestType.Collect:
                        conditionString += "개 획득\n";
                        break;
                    case QuestType.Visit:
                        conditionString += "방문\n";
                        break;
                }
            }
            
            conditionText.text = conditionString;
            string rewardString = "보상: ";
            foreach (string rewardId in questData.RewardIds)
            {
                Debug.Log(rewardId);
                if (itemDatabase.TryGetItem(rewardId, out ItemData item))
                    rewardString += item.ItemName + ", ";
            }

            rewardString = rewardString.TrimEnd(',', ' ');
            rewardText.text = rewardString;
        }

        public void AcceptanceQuest()
        {
            if (string.IsNullOrEmpty(_currentQuestId)) return;
    
            QuestManager.Instance.QuestAccept(_currentQuestId);
            Destroy(_questChoiceBtnDict[_currentQuestId]);
            _questChoiceBtnDict.Remove(_currentQuestId);  
            _choiceQuestDatas.RemoveAll(q => q.Id == _currentQuestId);
            _currentQuestId = "";

            if (_choiceQuestDatas.Count == 0)
                EmptyingContent();
            else
                SetContent(_choiceQuestDatas[0]);
        }

        private void EmptyingContent()
        {
            questNameText.text = "선택된 퀘스트가 없습니다.";
            descriptionText.text = "";
            conditionText.text = "";
            rewardText.text = "";

        }

        #region Test

        [ContextMenu("TestQuestAdd")]
        public void TestQuestAdd()
        {
            AddQuestData("quest_001");
            AddQuestData("quest_002");
            AddQuestBtn();
        }

        #endregion
    }
}
