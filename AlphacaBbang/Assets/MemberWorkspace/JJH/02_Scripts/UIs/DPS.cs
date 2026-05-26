using JJH._02_Scripts.Agents;
using TMPro;
using UnityEngine;

namespace JJH._02_Scripts.UIs
{
    public class DPS : MonoBehaviour
    {
        [SerializeField] private AgentHealthModule healthModule;
        [SerializeField] private TextMeshProUGUI dpsText;
        [SerializeField] private float refreshTime = 1f;

        private float _prevHealth;
        private float _timer;
        private float _damageAmount;

        private void Start()
        {
            _prevHealth = healthModule.Health;
        }

        private void Update()
        {
            float currentHealth = healthModule.Health;

            if (currentHealth < _prevHealth)
            {
                _damageAmount += _prevHealth - currentHealth;
            }

            _prevHealth = currentHealth;

            _timer += Time.deltaTime;

            if (_timer >= refreshTime)
            {
                float dps = _damageAmount / refreshTime;

                dpsText.text = $"DPS : {dps:F1}";

                _damageAmount = 0f;
                _timer = 0f;
            }
        }
    }
}