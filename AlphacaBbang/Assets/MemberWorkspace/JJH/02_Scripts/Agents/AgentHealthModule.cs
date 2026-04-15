using JJH._02_Scripts.Systems.EventSystems;
using JJH._02_Scripts_Systems.EventSystems;
using TMPro;
using UnityEngine;

namespace MemberWorkspace.JJH._02_Scripts.Agents
{
    public class AgentHealthModule : MonoBehaviour, IModule, IHealth
    {
        [SerializeField] private TextMeshPro healthText;
        private EventChannelSO _agentEventChannel;

        private Agent _owner;

        private float _maxHealth;
        private float _health;

        public void Initialize(ModuleOwner owner)
        {
            _owner = owner as Agent;
            _agentEventChannel = _owner.AgentEventChannel;
        }

        public void InitHealth(float maxHealth)
        {
            _maxHealth = maxHealth;
            _health = _maxHealth;
            ChangeHealthText();
        }

        public void SetHealth(float health)
        {
            _health -= health;

            if (_health <= 0)
            {
                _agentEventChannel.RaiseEvent(AgentEvents.AgentDeadEvent);
                return;
            }

            ChangeHealthText();
        }

        private void ChangeHealthText()
        {
            healthText.text = $"{_health}/{_maxHealth}";
        }
    }
}