namespace JJH._02_Scripts.Agents.Enemies.Skills
{
    public interface ISkillModule
    {
        public void UseSkill<T>() where T : IEnemySkill;
    }
}
