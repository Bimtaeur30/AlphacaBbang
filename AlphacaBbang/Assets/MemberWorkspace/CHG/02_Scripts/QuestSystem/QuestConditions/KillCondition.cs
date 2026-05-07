namespace MemberWorkspace.CHG._02_Scripts.QuestSystem
{
    public class KillCondition : QuestCondition
    {
        public KillCondition(string targetId, int required)
        {
            TargetId = targetId;
            Required = required;
        }
    }
}