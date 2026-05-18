using System.Collections.Generic;
using MemberWorkspace.CHG._02_Scripts.QuestSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestboardUI : MonoBehaviour
{
    [SerializeField] private GameObject questChoiceLayout;
    [SerializeField] private GameObject questChoiceBtnPrefab;

    [SerializeField] private TextMeshProUGUI QuestNameText;
    [SerializeField] private TextMeshProUGUI DescriptionText;
    [SerializeField] private TextMeshProUGUI ConditionText;

    private List<QuestData> choiceQuestDatas  = new List<QuestData>();
    private string _currentQuestId = "";
    
    
    private void TestAddQuest(string questId)
    {
        QuestData questData = QuestManager.Instance.FindQuestData(questId);
        if (questData == null) return;
        choiceQuestDatas.Add(questData);
    }

    private void AddQuestBtn()
    {
        foreach (QuestData questData in choiceQuestDatas)
        {
            GameObject obj =  Instantiate(questChoiceBtnPrefab, questChoiceLayout.transform);
            obj.GetComponentInChildren<TextMeshProUGUI>().text = questData.Name;
            obj.GetComponent<Button>().onClick.AddListener(() => SetContent(questData));
        }
    }

    private void SetContent(QuestData questData)
    {
        _currentQuestId = questData.Id;
        QuestNameText.text = questData.Name;
        DescriptionText.text = questData.Description;
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
        ConditionText.text = conditionString;
        
    }

    public void AcceptanceQuest()
    {
        QuestManager.Instance.QuestAccept(_currentQuestId);
        _currentQuestId = "";
    }
}
