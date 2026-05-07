namespace MemberWorkspace.CHG._02_Scripts.QuestSystem
{
    public abstract class QuestCondition
    {
        public string TargetId { get; protected set; }
        public int Current {get; set;}
        public int Required {get; protected set;}
        public bool IsCompleted => Current >= Required;
    }
}