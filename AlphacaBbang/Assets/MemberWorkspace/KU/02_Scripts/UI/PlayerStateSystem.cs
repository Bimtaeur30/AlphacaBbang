using JJH._02_Scripts.Systems.EventSystems;
using JJH._02_Scripts_Systems.EventSystems;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatSystem : MonoSingleton<PlayerStatSystem>
{
    [Header("UI")]
    [SerializeField] private Scrollbar _healthBar;
    [SerializeField] private Scrollbar _staminaBar;
    [SerializeField] private TextMeshProUGUI _healthText;
    [SerializeField] private TextMeshProUGUI _staminaText;

    [Header("Event Channel")]
    [SerializeField] private EventChannelSO systemChannel;
    [SerializeField] private EventChannelSO agentChannel;

    [Header("Health")]
    public float MaxHealth = 300f;

    [SerializeField] private float currentHealth;
    public float CurrentHealth => currentHealth;

    [Header("Stamina")]
    public float MaxStamina = 10f;

    [SerializeField] private float currentStamina;
    public float CurrentStamina => currentStamina;

    [SerializeField] private float _staminaDrainSpeed = 2f;
    [SerializeField] private float _staminaRegenSpeed = 1.5f;

    private PlayerController _controller;

    public bool IsRunning { get; private set; }

    public event Action<float> OnStaminaChanged;

    public PlayerSaveData SaveData { get; private set; }

    private bool isDead = false;

    protected override void Awake()
    {
        base.Awake();

        _controller = GetComponentInParent<PlayerController>();
        SaveData = GetComponentInParent<PlayerSaveData>();

        if (SaveData != null)
        {
            //MaxHealth = Mathf.Max(1, SaveData.MaxHealth);
            //MaxStamina = Mathf.Max(1, SaveData.MaxStamina);
        }

        currentHealth = MaxHealth;
        currentStamina = MaxStamina;

        agentChannel.AddListener<AgentDeadEvent>(OnAgentDead);
        agentChannel.AddListener<AgentHealthChangeEvent>(UpdateHealthUI);

        OnStaminaChanged += UpdateStaminaUI;

        UpdateHealthBar();
        UpdateStaminaUI(currentStamina);
    }

    private void OnDestroy()
    {
        agentChannel.RemoveListener<AgentDeadEvent>(OnAgentDead);
        agentChannel.RemoveListener<AgentHealthChangeEvent>(UpdateHealthUI);

        OnStaminaChanged -= UpdateStaminaUI;
    }

    private void Update()
    {
        UpdateStamina();
    }

    private void OnAgentDead(AgentDeadEvent evt)
    {
        if (evt.Agent == _controller && !isDead)
        {
            isDead = true;

            systemChannel.RaiseEvent(SystemEvents.OnGameEnd.Init(false));

            systemChannel.RaiseEvent(
                SystemEvents.SystemNotificationEvent.Init(
                    "사망",
                    "인벤토리가 청산되었습니다."
                )
            );
        }
    }

    public void TakeDamage(float damage)
    {
        _controller.HealthModule.Damage(damage);
    }

    public void Heal(float amount)
    {
        _controller.HealthModule.Heal(amount);
    }

    public void SetRunning(bool isRunning)
    {
        IsRunning = isRunning;
    }

    private void UpdateStamina()
    {
        if (IsRunning && currentStamina > 0f)
        {
            currentStamina -= _staminaDrainSpeed * Time.deltaTime;

            if (currentStamina <= 0f)
            {
                currentStamina = 0f;
                IsRunning = false;

                if (_controller != null)
                    _controller.RefreshMovementSpeed();
            }
        }
        else
        {
            currentStamina += _staminaRegenSpeed * Time.deltaTime;
        }

        currentStamina = Mathf.Clamp(currentStamina, 0f, MaxStamina);

        OnStaminaChanged?.Invoke(currentStamina);
    }

    private void UpdateHealthUI(AgentHealthChangeEvent evt)
    {
        currentHealth = evt.CurrentHealth;

        UpdateHealthBar();
    }

    private void UpdateHealthBar()
    {
        if (_healthBar != null)
        {
            _healthBar.size = currentHealth / MaxHealth;
        }

        if (_healthText != null)
        {
            _healthText.text = $"{currentHealth:F0} / {MaxHealth:F0}";
        }
    }

    private void UpdateStaminaUI(float value)
    {
        if (_staminaBar != null)
        {
            _staminaBar.size = value / MaxStamina;
        }

        if (_staminaText != null)
        {
            _staminaText.text = $"{value:F1} / {MaxStamina:F1}";
        }
    }

    public bool CanRun()
    {
        return currentStamina > 0f;
    }

    [ContextMenu("테스트로 죽이기")]
    private void TEST()
    {
        TakeDamage(10000000);
    }
}