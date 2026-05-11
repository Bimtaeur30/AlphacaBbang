using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStateManager : MonoSingleton<PlayerStateManager>
{
    [Header("EXP UI")]
    [SerializeField] private Scrollbar _expBar;
    [SerializeField] private TMP_Text _levelText;

    [Header("EXP Value")]
    [SerializeField] private float _startMaxExp = 100f;

    [SerializeField] private float _uiSmoothSpeed = 10f;

    public int CurrentLevel { get; private set; } = 1;

    public float CurrentExp { get; private set; }
    public float MaxExp { get; private set; }

    public event Action<float> OnExpChanged;
    public event Action<int> OnLevelChanged;

    private float _targetFill;

    protected override void Awake()
    {
        base.Awake();

        MaxExp = _startMaxExp;

        OnExpChanged += UpdateExpUI;
        OnLevelChanged += UpdateLevelUI;

        UpdateExpUI(CurrentExp);
        UpdateLevelUI(CurrentLevel);
    }

    private void Update()
    {
        if (_expBar != null)
        {
            _expBar.size = Mathf.Lerp(
                _expBar.size,
                _targetFill,
                _uiSmoothSpeed * Time.deltaTime);
        }
    }

    public void AddExp(float amount)
    {
        CurrentExp += amount;

        while (CurrentExp >= MaxExp)
        {
            CurrentExp -= MaxExp;
            LevelUp();
        }

        CurrentExp = Mathf.Clamp(CurrentExp, 0, MaxExp);

        OnExpChanged?.Invoke(CurrentExp);

        Debug.Log($"최대EXP: {MaxExp}, 현재EXP: {CurrentExp}");
    }

    private void LevelUp()
    {
        CurrentLevel++;

        float increaseAmount = GetLevelIncreaseValue();

        MaxExp += increaseAmount;

        OnLevelChanged?.Invoke(CurrentLevel);
    }

    private float GetLevelIncreaseValue()
    {
        int step = ((CurrentLevel - 1) / 5) + 1;

        return step * 10f;
    }

    private void UpdateExpUI(float value)
    {
        _targetFill = value / MaxExp;
    }

    private void UpdateLevelUI(int level)
    {
        if (_levelText != null)
            _levelText.text = $"Lv.{level}";
    }
}