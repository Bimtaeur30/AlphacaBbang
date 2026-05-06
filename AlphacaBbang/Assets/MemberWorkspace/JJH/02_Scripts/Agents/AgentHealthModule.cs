using JJH._02_Scripts.Systems.EventSystems;
using JJH._02_Scripts_Systems.EventSystems;
using UnityEngine;
using UnityEngine.UI;

namespace MemberWorkspace.JJH._02_Scripts.Agents
{
    public class AgentHealthModule : MonoBehaviour, IModule, IHealth
    {
        [SerializeField] private Slider slider;
        private EventChannelSO _agentEventChannel;

        private Agent _owner;

        private float _maxHealth;
        private float _health;

        public void Initialize(ModuleOwner owner)
        {
            _owner = owner as Agent;
            _agentEventChannel = _owner.AgentEventChannel;
            _agentEventChannel.AddListener<AgentDeadEvent>(OnAgentDeadEvent);
            slider.gameObject.SetActive(true);
        }

        private void OnDestroy()
        {
            _agentEventChannel.RemoveListener<AgentDeadEvent>(OnAgentDeadEvent);
        }

        private void OnAgentDeadEvent(AgentDeadEvent evt)
        {
            if (evt.Agent == _owner)
                slider.gameObject.SetActive(false);
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
            _agentEventChannel.RaiseEvent(AgentEvents.AgentHealthChangeEvent);

            if (_health <= 0)
            {
                _agentEventChannel.RaiseEvent(AgentEvents.AgentDeadEvent.Init(_owner));
                return;
            }

            ChangeHealthText();
        }

        private void ChangeHealthText()
        {
            slider.value = _health / _maxHealth;
        }
    }
}