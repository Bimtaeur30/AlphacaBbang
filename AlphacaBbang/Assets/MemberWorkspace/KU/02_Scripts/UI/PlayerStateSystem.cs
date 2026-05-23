using JJH._02_Scripts_Systems.EventSystems;
using System;
using UnityEngine;
using UnityEngine.UI;
public class PlayerStatSystem : MonoSingleton<PlayerStatSystem>
{
    [SerializeField] private Scrollbar _healthBar;
    [SerializeField] private Scrollbar _staminaBar;
    [SerializeField] private EventChannelSO systemChannel;

    public float MaxHealth = 100f; 
    public float CurrentHealth { get; private set; }

    public float MaxStamina = 10f; 
    public float CurrentStamina { get; private set; }

    [SerializeField] private float _staminaDrainSpeed = 2f;
    [SerializeField] private float _staminaRegenSpeed = 1.5f;
    private PlayerController _controller; 
    public bool IsRunning { get; private set; }
    public event Action<float> OnHealthChanged;
    public event Action<float> OnStaminaChanged;

    public PlayerSaveData SaveData {get; private set;}

    protected override void Awake()
    {
        _controller = GetComponentInParent<PlayerController>();
        SaveData = GetComponentInParent<PlayerSaveData>();

        if (SaveData != null)
        {
            MaxHealth = Mathf.Max(1, SaveData.MaxHealth);
            MaxStamina = Mathf.Max(1, SaveData.MaxStamina);
        }

        CurrentHealth = MaxHealth;
        CurrentStamina = MaxStamina;

        OnHealthChanged += UpdateHealthUI;
        OnStaminaChanged += UpdateStaminaUI;
    }
    private void Update()
    {
        UpdateStamina();
    }
    public void TakeDamage(float damage)
    {
        CurrentHealth -= damage;
        CurrentHealth = Mathf.Clamp(CurrentHealth, 0, MaxHealth);
        OnHealthChanged?.Invoke(CurrentHealth);
        if (CurrentHealth <= 0)
        {
            Die();
        }
    }
    public void Heal(float amount)
    {
        CurrentHealth += amount;
        CurrentHealth = Mathf.Clamp(CurrentHealth, 0, MaxHealth);
        OnHealthChanged?.Invoke(CurrentHealth);
    }
    private void Die()
    {
        Debug.Log("플레이어 사망");
        systemChannel.RaiseEvent(SystemEvents.OnGameEnd.Init(false));
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
    private void UpdateHealthUI(float value)
    {
        if (_healthBar != null)
            _healthBar.size = value / MaxHealth;
    }
    private void UpdateStaminaUI(float value)
    {
        if (_staminaBar != null) _staminaBar.size = value / MaxStamina;
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