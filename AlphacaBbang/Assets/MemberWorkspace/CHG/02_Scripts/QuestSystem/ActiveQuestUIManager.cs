using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MemberWorkspace.CHG._02_Scripts.QuestSystem
{ 
    public class ActiveQuestUIManager : MonoBehaviour
    {
        [SerializeField] private GameObject layoutGroup;
        [SerializeField] private GameObject questPanelPrefab;

        private Dictionary<Quest, QuestPanelUI> _activePanels = new();

        private void Awake()
        {
            QuestManager.Instance.OnQuestAccepted += OnQuestAccepted;
            QuestManager.Instance.OnUpdateQuestProgress += OnUpdateQuestProgress;
            QuestManager.Instance.OnQuestCompleted += OnQuestCompleted;
        }

        private void OnQuestCompleted(Quest quest)
        {
            if (!_activePanels.TryGetValue(quest, out QuestPanelUI panelUI)) return;
    
            _activePanels.Remove(quest);
            
        }

        private void OnQuestAccepted(Quest quest)
        {
            GameObject panel = Instantiate(questPanelPrefab, layoutGroup.transform);
            LayoutRebuilder.ForceRebuildLayoutImmediate(panel.GetComponent<RectTransform>());
            QuestPanelUI panelUI = panel.GetComponent<QuestPanelUI>();
            Debug.Log($"{panelUI == null} +  :  + {quest == null}");
            panelUI.Initialize(quest);
            panelUI.OnClaimed += OnPanelClaimed;
            _activePanels.Add(quest, panelUI);
            
        }

        private void OnPanelClaimed(Quest obj)
        {
            _activePanels.Remove(obj);
        }

        private void OnUpdateQuestProgress(Quest quest)
        {
            if (_activePanels.TryGetValue(quest, out QuestPanelUI panelUI))
                panelUI.UpdateProgress();
        }
        
    }
}