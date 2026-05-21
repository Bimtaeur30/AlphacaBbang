using System;
using System.Collections.Generic;

namespace MemberWorkspace.CHG._02_Scripts.QuestSystem
{
    [Serializable]
    public class QuestData
    {
        public string Id;
        public string Name;
        public string Description;
        public QuestImportanceType Importance;
        public List<ConditionData> Conditions;
        public List<string> RewardIds;
    }

    [Serializable]
    public class ConditionData
    {
        public QuestType Type;
        public string TargetId;
        public int Required;
    }
    
}