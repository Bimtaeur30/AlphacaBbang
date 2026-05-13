namespace JJH._02_Scripts.Agents.Enemies.Skills
{
    public interface IEnemySkillModule
    {
        public IEnemySkill GetSkill<T>() where T : IEnemySkill;
        public void UseSkill<T>() where T : IEnemySkill;
    }
}
