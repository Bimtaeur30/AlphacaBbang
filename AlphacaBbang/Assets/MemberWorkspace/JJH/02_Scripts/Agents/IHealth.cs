namespace JJH._02_Scripts.Agents
{
    public interface IHealth
    {
        public void InitHealth(float maxHealth);
        public void Damage(float health);
        public void Heal(float amount);
        public void SetHealthBarVisible(bool isVisible);
    }
}