using MemberWorkspace.CHG._02_Scripts.QuestSystem;
using TMPro;
using UnityEngine;

public class TestQustUI : MonoBehaviour
{
    [SerializeField] private GameObject layOutGroup;
    [SerializeField] private GameObject prefab;

    private void Awake()
    {
        QuestManager.Instance.OnQuestAccepted += AcceptQuest;
    }

    public void AcceptQuest(Quest quest)
    {
        GameObject obj = Instantiate(prefab, layOutGroup.transform);
        TextMeshProUGUI[] texts = obj.GetComponentsInChildren<TextMeshProUGUI>();
        texts[0].text = quest.Data.Name;
        texts[1].text = $"{quest.Conditions[0].Current} / {quest.Conditions[0].Required}";
        
    }
}
