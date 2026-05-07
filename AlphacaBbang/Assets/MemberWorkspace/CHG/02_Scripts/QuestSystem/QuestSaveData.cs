using System;
using System.Collections.Generic;

namespace MemberWorkspace.CHG._02_Scripts.QuestSystem
{
    [Serializable]
    public class QuestSaveData
    {
        public List<QuestSaveEntry> ActiveQuests;
        public List<string> CompletedQuestIds;
    }

    [Serializable]
    public class QuestSaveEntry
    {
        public string QuestId;
        public List<ConditionProgress> ConditionProgress;
    }

    [Serializable]
    public class ConditionProgress
    {
        public int Current;
    }
    
}