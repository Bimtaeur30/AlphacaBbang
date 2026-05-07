namespace MemberWorkspace.CHG._02_Scripts.QuestSystem
{
    public class VisitCondition : QuestCondition
    {
        public VisitCondition(string targetId)
        {
            TargetId = targetId;
            Required = 1;
        }
    }
}