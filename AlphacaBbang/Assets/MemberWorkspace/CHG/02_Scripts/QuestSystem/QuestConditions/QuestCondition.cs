namespace MemberWorkspace.CHG._02_Scripts.QuestSystem
{
    public abstract class QuestCondition
    {
        public string TargetId { get; protected set; }
        public int Progress {get; set;}
        public int Required {get; protected set;}
        public bool IsCompleted => Progress >= Required;
    }
}