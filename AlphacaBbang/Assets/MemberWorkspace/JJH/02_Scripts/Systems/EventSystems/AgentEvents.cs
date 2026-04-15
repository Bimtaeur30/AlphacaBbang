using JJH._02_Scripts_Systems.EventSystems;

namespace JJH._02_Scripts.Systems.EventSystems
{
    public static class AgentEvents
    {
        public static readonly AgentDeadEvent AgentDeadEvent = new AgentDeadEvent();
    }

    public class AgentDeadEvent : GameEvent { }
}
