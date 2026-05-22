using MemberWorkspace.CHG._02_Scripts.TalkSystem;

namespace JJH._02_Scripts.Agents.Enemies
{
    public interface IEnemyInterface
    {
        EnemyTalkSystem EnemyTalk { get; }
        void SetInterfaceShow(bool value);
    }
}