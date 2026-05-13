namespace JJH._02_Scripts.Agents.Enemies.Skills
{
    public interface IEnemySkillModule
    {
        public void UseSkill<T>() where T : IEnemySkill;
    }
}
