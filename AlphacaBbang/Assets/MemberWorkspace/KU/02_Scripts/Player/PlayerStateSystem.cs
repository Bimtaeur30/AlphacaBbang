using System;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatSystem : MonoBehaviour
{
    [SerializeField] private Scrollbar _healthBar;
    [SerializeField] private Scrollbar _staminaBar;

    public float MaxHealth = 100f;
    public float CurrentHealth { get; private set; }

    public float MaxStamina = 10f;
    public float CurrentStamina { get; private set; }

    [SerializeField] private float _staminaDrainSpeed = 2f;
    [SerializeField] private float _staminaRegenSpeed = 1.5f;

    public bool IsRunning { get; private set; }

    public event Action<float> OnHealthChanged;
    public event Action<float> OnStaminaChanged;

    private void Awake()
    {
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
                CurrentStamina = 0f;
                IsRunning = false; // 강제 종료
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
        if (_staminaBar != null)
            _staminaBar.size = value / MaxStamina;
    }

    public bool CanRun()
    {
        return CurrentStamina > 0f;
    }
}