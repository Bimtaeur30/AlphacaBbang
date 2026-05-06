namespace MemberWorkspace.CHG._02_Scripts.QuestSystem
{
    public class CollectCondition : QuestCondition
    {
        public CollectCondition(string targetId, int required)
        {
            TargetId = targetId;
            Required = required;
        }
    }
}