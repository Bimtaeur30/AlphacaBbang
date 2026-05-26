using DG.Tweening;
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
        [Header("HP Bar")]
        [SerializeField] private Slider slider;
        [SerializeField] private Image damageEffectImage;
        [SerializeField] private RectTransform fillRect;
        [SerializeField] private RectTransform damageEffectRect;

        [Header("Damage Effect")]
        [SerializeField] private float damageFadeDelay = 0.15f;
        [SerializeField] private float damageFadeDuration = 0.35f;

        [Header("Sound")]
        [SerializeField] private SoundClipSO hitSound;
        [SerializeField] private SoundClipSO deadSound;


        public float MaxHealth => _maxHealth;
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

                ChangeHealthText();
            }
        }

        private Agent _owner;
        private ArmorSO[] armors;
        private EventChannelSO _agentEventChannel;

        private Tween _damageTween;

        private float _maxHealth;
        [SerializeField] private float _health;

        public void Initialize(ModuleOwner owner)
        {
            _owner = owner as Agent;
            _agentEventChannel = _owner.AgentEventChannel;

            _agentEventChannel.AddListener<AgentDeadEvent>(OnAgentDeadEvent);
            _agentEventChannel.AddListener<AgentArmorEquip>(OnAgentArmorEquip);

            slider.gameObject.SetActive(true);

            Color color = damageEffectImage.color;
            color.a = 0f;
            damageEffectImage.color = color;
        }

        private void OnDestroy()
        {
            if (_agentEventChannel != null)
            {
                _agentEventChannel.RemoveListener<AgentDeadEvent>(OnAgentDeadEvent);
                _agentEventChannel.RemoveListener<AgentArmorEquip>(OnAgentArmorEquip);
            }

            _damageTween?.Kill();
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
            ChangeHealthText();

            damageEffectImage.fillAmount = slider.value;
        }

        public void Damage(float value)
        {
            float damage = value;
            if (armors != null)
                foreach (ArmorSO armor in armors)
                {
                    damage *= 1f - armor.DamageReductionRate;
                }

            float prevHealthRatio = Health / _maxHealth;
            Health -= damage;
            float currentHealthRatio = Health / _maxHealth;

            ShowDamageEffect(prevHealthRatio, currentHealthRatio);

            _agentEventChannel.RaiseEvent(AgentEvents.AgentHealthChangeEvent.Init(_owner, Health, damage));
            //_owner.AgentSoundPlayer.PlaySound(hitSound);
        }

        public void Heal(float amount)
        {
            Health = Mathf.Min(Health + amount, _maxHealth);
            ChangeHealthText();
        }

        private void ChangeHealthText()
        {
            slider.value = Health / _maxHealth;
        }

        private void ShowDamageEffect(float prevRatio, float currentRatio)
        {
            _damageTween?.Kill();

            float totalWidth = fillRect.rect.width;
            float damageWidth = (prevRatio - currentRatio) * totalWidth;

            if (damageWidth <= 0f)
                return;

            float currentX = currentRatio * totalWidth;

            damageEffectRect.anchoredPosition = new Vector2(currentX, damageEffectRect.anchoredPosition.y);
            damageEffectRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, damageWidth);

            Color color = damageEffectImage.color;
            color.a = 1f;
            damageEffectImage.color = color;

            _damageTween = damageEffectImage
                                                .DOFade(0f, damageFadeDuration)
                                                .SetDelay(damageFadeDelay);
        }
    }
}