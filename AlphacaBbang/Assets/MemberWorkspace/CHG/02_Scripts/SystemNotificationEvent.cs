using JJH._02_Scripts_Systems.EventSystems;

namespace MemberWorkspace.CHG._02_Scripts
{
    public class SystemNotificationEvent : GameEvent
    {
        public string MainMessage { get; set; }
        public string SubMessage { get; set; }

        public SystemNotificationEvent Init(string mainMessage, string subMessage)
        {
            MainMessage = mainMessage;
            SubMessage = subMessage;
            return this;
        }
    }
}