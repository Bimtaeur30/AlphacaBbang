using System.Collections.Generic;
using System.Linq;

namespace MemberWorkspace.CHG._02_Scripts.QuestSystem
{
    public class Quest
    {
        public QuestData Data { get; }
        public List<QuestCondition> Conditions { get; }
        
        public bool IsCompleted => Conditions.All(c => c.IsCompleted);
        
        //new quest
        public Quest(QuestData data)
        {
            Data = data;
            Conditions = data.Conditions
                .Select(QuestFactory.CreateCondition)
                .ToList();
        }

        // load save questData
        public Quest(QuestData data, QuestSaveEntry save)
        {
            Data = data;
            Conditions = data.Conditions
                .Select((conData, n) => 
                {
                    var cond = QuestFactory.CreateCondition(conData); 
                    cond.Progress = save.ConditionProgress[n].Current;
                    return cond;
                }).ToList();
        }   
    }
}