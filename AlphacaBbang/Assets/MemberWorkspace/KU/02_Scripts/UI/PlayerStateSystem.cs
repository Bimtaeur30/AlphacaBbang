using JJH._02_Scripts.Systems.EventSystems;
using JJH._02_Scripts_Systems.EventSystems;
using System;
using UnityEngine;
using UnityEngine.UI;
public class PlayerStatSystem : MonoSingleton<PlayerStatSystem>
{
    [SerializeField] private Scrollbar _healthBar;
    [SerializeField] private Scrollbar _staminaBar;
    [SerializeField] private EventChannelSO systemChannel;
    [SerializeField] private EventChannelSO agentChannel;

    public float MaxHealth = 100f;
    public float MaxStamina = 10f;
    public float CurrentStamina { get; private set; }

    [SerializeField] private float _staminaDrainSpeed = 2f;
    [SerializeField] private float _staminaRegenSpeed = 1.5f;
    private PlayerController _controller;
    public bool IsRunning { get; private set; }
    public event Action<float> OnStaminaChanged;

    public PlayerSaveData SaveData { get; private set; }

    protected override void Awake()
    {
        _controller = GetComponentInParent<PlayerController>();
        SaveData = GetComponentInParent<PlayerSaveData>();

        if (SaveData != null)
        {
            MaxHealth = Mathf.Max(1, SaveData.MaxHealth);
            MaxStamina = Mathf.Max(1, SaveData.MaxStamina);
        }

        CurrentStamina = MaxStamina;

        agentChannel.AddListener<AgentDeadEvent>(OnAgentDead);
        agentChannel.AddListener<AgentHealthChangeEvent>(UpdateHealthUI);
        OnStaminaChanged += UpdateStaminaUI;
    }

    private void OnDestroy()
    {
        agentChannel.RemoveListener<AgentDeadEvent>(OnAgentDead);
        agentChannel.RemoveListener<AgentHealthChangeEvent>(UpdateHealthUI);
        OnStaminaChanged -= UpdateStaminaUI;
    }

    private void OnAgentDead(AgentDeadEvent evt)
    {
        if (evt.Agent == _controller)
        {
            systemChannel.RaiseEvent(SystemEvents.OnGameEnd.Init(false));
        }
    }

    private void Update()
    {
        UpdateStamina();
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
        if (IsRunning && CurrentStamina > 0f)
        {
            CurrentStamina -= _staminaDrainSpeed * Time.deltaTime;
            if (CurrentStamina <= 0f)
            {
                CurrentStamina = 0f; IsRunning = false;
                if (_controller != null)
                    _controller.RefreshMovementSpeed();
            }
        }
        else
        {
            CurrentStamina += _staminaRegenSpeed * Time.deltaTime;
        }
        CurrentStamina = Mathf.Clamp(CurrentStamina, 0, MaxStamina);
        OnStaminaChanged?.Invoke(CurrentStamina);
    }
    private void UpdateHealthUI(AgentHealthChangeEvent evt)
    {
        if (_healthBar != null)
            _healthBar.size = evt.CurrentHealth / MaxHealth;
    }
    private void UpdateStaminaUI(float value)
    {
        if (_staminaBar != null)
            _staminaBar.size = value / MaxStamina;
    }
    public bool CanRun()
    {
        return CurrentStamina > 0f;
    }

    [ContextMenu("테스트로 죽이기")]
    private void TEST()
    {
        TakeDamage(10000000);
    }
}