using JJH._02_Scripts.Agents.Enemies;
using JJH._02_Scripts.Weapons;
using JJH._02_Scripts_Systems.EventSystems;

namespace JJH._02_Scripts.Systems.EventSystems
{
    public static class AgentEvents
    {
        public static readonly AgentDeadEvent AgentDeadEvent = new AgentDeadEvent();
        public static readonly AgentHealthChangeEvent AgentHealthChangeEvent = new AgentHealthChangeEvent();
        public static readonly AgentArmorEquip AgentArmorEquip = new AgentArmorEquip();
    }

    public class AgentDeadEvent : GameEvent
    {
        public Agent Agent { get; private set; }
        public string EnemyID { get; private set; }

        public AgentDeadEvent Init(Agent agent)
        {
            Agent = agent;

            if (Agent is AbstractEnemy enemy)
                EnemyID = enemy.EnemyData.EnemyID;

            return this;
        }
    }

    public class AgentHealthChangeEvent : GameEvent
    {
        public Agent Agent { get; private set; }
        public float CurrentHealth { get; private set; }
        public float Damage { get; private set; }

        public AgentHealthChangeEvent Init(Agent agent, float currentHealth, float damage)
        {
            Agent = agent;
            Damage = damage;
            CurrentHealth = currentHealth;
            return this;
        }
    }

    public class AgentArmorEquip : GameEvent
    {
        public Agent Agent { get; private set; }
        public ArmorSO[] Armors { get; private set; }

        public AgentArmorEquip Init(Agent agent, ArmorSO[] armors)
        {
            Agent = agent;
            Armors = armors;
            return this;
        }
    }
}
