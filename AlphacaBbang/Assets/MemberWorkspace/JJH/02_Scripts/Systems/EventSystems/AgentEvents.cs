using JJH._02_Scripts.Agents.Enemies;
using JJH._02_Scripts_Systems.EventSystems;

namespace JJH._02_Scripts.Systems.EventSystems
{
    public static class AgentEvents
    {
        public static readonly AgentDeadEvent AgentDeadEvent = new AgentDeadEvent();
        public static readonly AgentHealthChangeEvent AgentHealthChangeEvent = new AgentHealthChangeEvent();
        public static readonly AgentInventoryDropEvent AgentInventoryDropEvent = new AgentInventoryDropEvent();
    }

    public class AgentDeadEvent : GameEvent
    {
        public Agent Agent { get; private set; }
        public string EnemyName { get; private set; }

        public AgentDeadEvent Init(Agent agent)
        {
            Agent = agent;

            if (Agent is AbstractEnemy enemy)
                EnemyName = enemy.EnemyData.EnemyName;

            return this;
        }
    }

    public class AgentHealthChangeEvent : GameEvent { }

    public class AgentInventoryDropEvent : GameEvent
    {
        public Agent Agent { get; private set; }

        public AgentInventoryDropEvent Init(Agent agent)
        {
            Agent = agent;
            return this;
        }
    }
}
