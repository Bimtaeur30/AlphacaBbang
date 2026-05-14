using JJH._02_Scripts.Systems.EventSystems;
using JJH._02_Scripts.Weapons;
using JJH._02_Scripts_Systems.EventSystems;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace JJH._02_Scripts.Agents
{
    public class AgentArmorModule : MonoBehaviour, IModule, IAgentArmor
    {
        public Dictionary<ArmorType, ArmorSO> armors { get; private set; }
        public bool HeadArmorEquiped { get; private set; } = false;
        public bool BodyArmorEquiped { get; private set; } = false;

        private Agent _owner;
        private EventChannelSO _agentEventChannel;

        public void Initialize(ModuleOwner owner)
        {
            _owner = owner as Agent;
            _agentEventChannel = _owner.AgentEventChannel;
        }

        public void ArmorEquip(bool value, ArmorType armorType, ArmorSO armorSO)
        {
            if (value)
                armors.Add(armorType, armorSO);
            else
                armors.Remove(armorType);

            _agentEventChannel.RaiseEvent(AgentEvents.AgentArmorEquip.Init(_owner, armors.Values.ToArray()));
        }
    }
}