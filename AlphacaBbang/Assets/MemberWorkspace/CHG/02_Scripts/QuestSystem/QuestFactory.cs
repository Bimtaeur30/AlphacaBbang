using System;
using System.Linq;

namespace MemberWorkspace.CHG._02_Scripts.QuestSystem
{
    public static class QuestFactory
    {
        public static Quest Create(QuestData data)
        {
            return new Quest(data);
        }

        public static Quest CreateWithSave(QuestData data, QuestSaveEntry save)
        {
            return new Quest(data, save);
        }
        
        public static QuestCondition CreateCondition(ConditionData data)
        {
            return data.Type switch
            {
                QuestType.Kill => new KillCondition(data.TargetId, data.Required),
                QuestType.Collect => new CollectCondition(data.TargetId, data.Required),
                QuestType.Visit => new VisitCondition(data.TargetId),
                _ => null
            };
        }
    }
}