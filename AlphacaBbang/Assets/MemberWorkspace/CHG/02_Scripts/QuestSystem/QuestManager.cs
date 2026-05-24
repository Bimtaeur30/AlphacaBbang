using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using JJH._02_Scripts_Systems.EventSystems;
using JJH._02_Scripts.Systems.EventSystems;
using UnityEngine;

namespace MemberWorkspace.CHG._02_Scripts.QuestSystem
{
    public class QuestManager : MonoSingleton<QuestManager>
    {
        [SerializeField] private EventChannelSO _agentDeadEvent;
        [SerializeField] private InventoryContainer inventory;
        [SerializeField] private ItemDatabase[] itemDatabase;
        
        //in game all quest
        private Dictionary<string, QuestData> _mainQuests = new Dictionary<string, QuestData>();
        private Dictionary<string, QuestData> _subQuests = new Dictionary<string, QuestData>();
        
        public event Action<Quest> OnQuestAccepted;
        public event Action<Quest> OnUpdateQuestProgress;
        public event Action<Quest> OnQuestCompleted;
        public event Action OnClearAllQuests;
        // now active/completed quest
        private List<Quest> _activeQuests = new List<Quest>();
        private List<string> _completedQuestIds = new List<string>();
        
        
        private string _savePath => Application.persistentDataPath + "/QuestsSave.json";

        protected override void Awake()
        {
            //base.Awake();
            LoadQuestDB();
            _agentDeadEvent.AddListener<AgentDeadEvent>(QuestProgressUpdate);
            //보유중인 아이템?
            
        }

        private void LoadQuestDB()
        {
            var files = Resources.LoadAll<TextAsset>("Quests"); 
            foreach (var file in files)
            {
                var data = JsonUtility.FromJson<QuestData>(file.text);
                if (data.Importance == QuestImportanceType.Main)
                    _mainQuests[data.Id] = data;
                else if (data.Importance == QuestImportanceType.Sub)
                    _subQuests[data.Id] = data;
            }

            foreach (var v in _mainQuests)
            {
                Debug.Log(v.Key + " : " + v.Value);
            }
        }
        
        

        public void SaveQuestData()
        {
            // save active quests and completedQuests
            QuestSaveData save = new QuestSaveData
            {
                ActiveQuests = _activeQuests.Select(q => new QuestSaveEntry
                {
                    QuestId = q.Data.Id,
                    ConditionProgress = q.Conditions.Select(c => new ConditionProgress
                    {
                        Current = c.Progress
                    }).ToList()
                }).ToList(),

                CompletedQuestIds = _completedQuestIds
            };
            
            File.WriteAllText(_savePath, JsonUtility.ToJson(save, true));
        }

        public void LoadSaveData()
        {
            if (!File.Exists(_savePath)) return; 
            
            QuestSaveData save = JsonUtility.FromJson<QuestSaveData>(File.ReadAllText(_savePath)); 
            _completedQuestIds = save.CompletedQuestIds ?? new List<string>();

            foreach (var quest in save.ActiveQuests)
            {
                if(!TryGetQuestData(quest.QuestId,out QuestData data))
                    continue;   
                
                _activeQuests.Add(new Quest(data, quest));
            }
        }

        public void QuestAccept(string questId)
        {
            if (_completedQuestIds.Contains(questId)) 
            {
                Debug.LogError($"Quest {questId} is completed");
                return;
            }
            if (_activeQuests.Any(q => q.Data.Id == questId))
            {
                Debug.LogError($"Quest {questId} is active");
                return;
            }
            
            if (!TryGetQuestData(questId, out QuestData data))
            {
                Debug.LogError($"Quest {questId} not found");
                return;
            }

            Debug.Log(data.Name + " : " + data.Id);
            Quest newQuest = QuestFactory.Create(data);
            _activeQuests.Add(newQuest);
            OnQuestAccepted?.Invoke(newQuest);
        }
        
        public void QuestProgressUpdate(string targetId, int value)
        {
            foreach (Quest quest in _activeQuests)
                foreach (QuestCondition condition in quest.Conditions)
                    if (condition.TargetId == targetId)
                    {
                        condition.Progress += value;
                        OnUpdateQuestProgress?.Invoke(quest);
                    }
        }
        
        public void QuestProgressUpdate(AgentDeadEvent evt)
        {
            foreach (Quest quest in _activeQuests)
                foreach (QuestCondition condition in quest.Conditions)
                    if (condition.TargetId == evt.EnemyName)
                    {
                        condition.Progress++;
                        OnUpdateQuestProgress?.Invoke(quest);
                    }
        }
        
        public bool IsCompleted(string questId)
            => _completedQuestIds.Contains(questId);

        public bool TryGetQuestData(string questId, out QuestData quest)
        {
            if (_mainQuests.TryGetValue(questId, out var main))
            {
                quest = main;
                return true;
            }

            if (_subQuests.TryGetValue(questId, out var sub))
            {
                quest = sub;
                return true;
            }
            quest = null;
            return false;
        }

        public void CompleteQuest(Quest quest)
        {
            if (!_activeQuests.Contains(quest)) return;
            
            _activeQuests.Remove(quest);
            _completedQuestIds.Add(quest.Data.Id);
            OnQuestCompleted?.Invoke(quest);
            if (quest.Data.RewardIds != null && quest.Data.RewardIds.Count > 0)
            {
                foreach (var rewardId in quest.Data.RewardIds)
                {
                    foreach (ItemDatabase itemDatabase in itemDatabase)
                    {
                        if (itemDatabase.TryGetItem(rewardId, out var item))
                        {
                            inventory.AddItem(item);
                            break;
                        }
                        
                    }
                }
            }
        }

        public void ClearAllActiveQuests()
        {
            _activeQuests.Clear();
            OnClearAllQuests?.Invoke(); 
        }
        
#region QuestDataUpdateTest

        [ContextMenu("AcceptQuestTest")]
        private void AcceptQuestTest()
        {
            QuestAccept("quest_001");
            QuestAccept("quest_003");
        }

        /*[ContextMenu("EnemyKillTest")]
        private void QuestProgressUpdate()
        {
            foreach (Quest quest in _activeQuests)
                foreach (QuestCondition condition in quest.Conditions)
                    if (condition.TargetId == "Zombie")
                    {
                        condition.Progress++;
                        OnUpdateQuestProgress?.Invoke(quest);
                    }
        }*/
        
        

#endregion


    }
}
