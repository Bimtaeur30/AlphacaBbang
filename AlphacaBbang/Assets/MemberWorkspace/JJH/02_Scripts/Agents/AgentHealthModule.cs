using TMPro;
using UnityEngine;

namespace MemberWorkspace.JJH._02_Scripts.Agents
{
    public class AgentHealthModule : MonoBehaviour, IModule, IHealth
    {
        [SerializeField] private TextMeshPro healthText;

        private Agent _owner;

        private float _maxHealth;
        private float _health;

        public void Initialize(ModuleOwner owner)
        {
            _owner = owner as Agent;
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
            ChangeHealthText();
        }

        private void ChangeHealthText()
        {
            healthText.text = $"{_health}/{_maxHealth}";
        }
    }
}