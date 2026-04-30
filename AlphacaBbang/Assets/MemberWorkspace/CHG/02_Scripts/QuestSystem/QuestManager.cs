using System;
using System.Collections.Generic;
using UnityEngine;

namespace MemberWorkspace.CHG._02_Scripts.QuestSystem
{
    
    public class QuestManager : MonoSingleton<QuestManager>
    {
        public Dictionary<string, QuestData> MainQuests = new Dictionary<string, QuestData>();
        public Dictionary<string, QuestData> SubQuests = new Dictionary<string, QuestData>();
        
        //Json이용해서 데이터 저장, 실행시 받는건 이벤트 채널 이용, 달성은 ??
        private void Awake()
        {
            LoadQuestDB();
        }

        private void LoadQuestDB()
        {
            var files = Resources.LoadAll<TextAsset>("Quests"); //Quests의 의미
            foreach (var file in files)
            {
                var data = JsonUtility.FromJson<QuestData>(file.text);
                if (data.Importance == "Main")
                    MainQuests[data.Id] = data;
                else if (data.Importance == "Sub")
                    SubQuests[data.Id] = data;
            }
        }

        public void SaveQuestData()
        {
            
        }
        
    }
}
