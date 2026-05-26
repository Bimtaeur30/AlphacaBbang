using JJH._02_Scripts.Systems.EventSystems;
using JJH._02_Scripts.Systems.SoundSystems;
using JJH._02_Scripts.Weapons;
using JJH._02_Scripts_Systems.EventSystems;
using UnityEngine;
using UnityEngine.UI;

namespace JJH._02_Scripts.Agents
{
    public class AgentHealthModule : MonoBehaviour, IModule, IHealth
    {
        [SerializeField] private SoundClipSO hitSound;
        [SerializeField] private SoundClipSO deadSound;
        [SerializeField] private Slider slider;
        [SerializeField] private float sliderSmoothSpeed = 5f;

        public float Health
        {
            get => _health;
            set
            {
                _health = Mathf.Min(value, _maxHealth);

                if (_health <= 0)
                {
                    _health = 0;
                    _agentEventChannel.RaiseEvent(AgentEvents.AgentDeadEvent.Init(_owner));
                    _owner.AgentSoundPlayer.PlaySound(deadSound);
                    return;
                }

                UpdateTargetSliderValue();
            }
        }

        private Agent _owner;
        private ArmorSO[] armors;
        private EventChannelSO _agentEventChannel;

        private float _maxHealth;
        private float _health;

        private float _targetSliderValue;

        public float MaxHealth => _maxHealth;

        public void Initialize(ModuleOwner owner)
        {
            _owner = owner as Agent;
            _agentEventChannel = _owner.AgentEventChannel;

            _agentEventChannel.AddListener<AgentDeadEvent>(OnAgentDeadEvent);
            _agentEventChannel.AddListener<AgentArmorEquip>(OnAgentArmorEquip);

            slider.gameObject.SetActive(true);
        }

        private void Update()
        {
            slider.value = Mathf.Lerp(slider.value, _targetSliderValue, Time.deltaTime * sliderSmoothSpeed);

            if (Mathf.Abs(slider.value - _targetSliderValue) < 0.001f)
                slider.value = _targetSliderValue;
        }

        private void OnDestroy()
        {
            if (_agentEventChannel == null)
                return;

            _agentEventChannel.RemoveListener<AgentDeadEvent>(OnAgentDeadEvent);
            _agentEventChannel.RemoveListener<AgentArmorEquip>(OnAgentArmorEquip);
        }

        private void OnAgentArmorEquip(AgentArmorEquip evt)
        {
            if (evt.Agent == _owner)
                armors = evt.Armors;
        }

        private void OnAgentDeadEvent(AgentDeadEvent evt)
        {
            if (evt.Agent == _owner)
                slider.gameObject.SetActive(false);
        }

        public void InitHealth(float maxHealth)
        {
            _maxHealth = maxHealth;
            Health = _maxHealth;

            slider.value = 1f;
            _targetSliderValue = 1f;
        }

        public void Damage(float value)
        {
            float damage = value;

            if (armors != null)
            {
                foreach (ArmorSO armor in armors)
                {
                    damage *= 1f - armor.DamageReductionRate;
                }
            }

            Health -= damage;

            _agentEventChannel.RaiseEvent(
                AgentEvents.AgentHealthChangeEvent.Init(_owner, Health, damage));

            //_owner.AgentSoundPlayer.PlaySound(hitSound);
        }

        public void Heal(float amount)
        {
            Health = Mathf.Min(Health + amount, _maxHealth);
        }

        private void UpdateTargetSliderValue()
        {
            _targetSliderValue = Health / _maxHealth;
        }

        public void SetHealthBarVisible(bool isVisible)
        {
            slider.gameObject.SetActive(isVisible);
        }
    }
}