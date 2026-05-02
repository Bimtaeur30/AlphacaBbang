using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor.Overlays;
using UnityEngine;

namespace MemberWorkspace.CHG._02_Scripts.QuestSystem
{
    
    public class QuestManager : MonoSingleton<QuestManager>
    {
        //in game all quest
        public Dictionary<string, QuestData> MainQuests = new Dictionary<string, QuestData>();
        public Dictionary<string, QuestData> SubQuests = new Dictionary<string, QuestData>();
        
        // now active/completed quest
        private List<Quest> _activeQuests = new List<Quest>();
        private List<string> _completedQuestIds = new List<string>();
        
        public string SavePath => Application.persistentDataPath + "/QuestsSave.json"; 
        
        private void Awake()
        {
            LoadQuestDB();
        }

        private void LoadQuestDB()
        {
            var files = Resources.LoadAll<TextAsset>("Quests"); 
            foreach (var file in files)
            {
                var data = JsonUtility.FromJson<QuestData>(file.text);
                if (data.Importance == QuestImportanceType.Main)
                    MainQuests[data.Id] = data;
                else if (data.Importance == QuestImportanceType.Sub)
                    SubQuests[data.Id] = data;
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
                        Current = c.Current
                    }).ToList()
                }).ToList(),

                CompletedQuestIds = _completedQuestIds
            };
            
            File.WriteAllText(SavePath, JsonUtility.ToJson(save, true));
        }

        public void LoadSaveData()
        {
            if (!File.Exists(SavePath)) return; 
            
            QuestSaveData save = JsonUtility.FromJson<QuestSaveData>(File.ReadAllText(SavePath)); 
            _completedQuestIds = save.CompletedQuestIds ?? new List<string>();

            foreach (var quest in save.ActiveQuests)
            {
                var data = FindQuestData(quest.QuestId);
                if (data == null) continue;
                
                _activeQuests.Add(new Quest(data, quest));
            }
        }

        public void QuestAccept(string questId)
        {
            if (_completedQuestIds.Contains(questId)) return;
            if (_activeQuests.Any(q => q.Data.Id == questId)) return;
            
            var data = FindQuestData(questId);
            if (data == null) return;
            
            _activeQuests.Add(QuestFactory.Create(data));
        }
        
        public bool IsCompleted(string questId)
            => _completedQuestIds.Contains(questId);

        private QuestData FindQuestData(string questId)
        {
            if (MainQuests.TryGetValue(questId, out var main)) return main;
            if (SubQuests.TryGetValue(questId, out var sub)) return sub;
            return null;
        }
    }
}
