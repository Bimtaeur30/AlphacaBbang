namespace JJH._02_Scripts.Agents.Enemies.Skills
{
    public interface IEnemySkill
    {
        void Initialize(AbstractEnemy owner);
        void UseSkill();
    }
}
