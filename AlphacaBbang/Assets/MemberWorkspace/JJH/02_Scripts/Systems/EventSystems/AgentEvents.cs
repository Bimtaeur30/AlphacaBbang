using JJH._02_Scripts_Systems.EventSystems;

namespace JJH._02_Scripts.Systems.EventSystems
{
    public static class AgentEvents
    {
        public static readonly AgentDeadEvent AgentDeadEvent = new AgentDeadEvent();
        public static readonly AgentHealthChangeEvent AgentHealthChangeEvent = new AgentHealthChangeEvent();
        public static readonly AgentInventoryDropEvent AgentInventoryDropEvent = new AgentInventoryDropEvent();
    }

    public class AgentDeadEvent : GameEvent { }
    public class AgentHealthChangeEvent : GameEvent { }
    public class AgentInventoryDropEvent : GameEvent { }
}
