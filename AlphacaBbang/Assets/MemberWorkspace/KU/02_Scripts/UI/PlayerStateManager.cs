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

    private int _currentLevel = 1;
    public int _statUpPoint;
    
    public int CurrentLevel
    {
        get
        {
            return _currentLevel;
        }
        private set
        {
            StatUpPoint += 3;
            _currentLevel = value;
        }
    }

    public int StatUpPoint
    {
        get
        {
            return _statUpPoint;
        }
        set
        {
            _statUpPoint = Mathf.Max(0, value);
        }
    }

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

        Debug.Log($"�ִ�EXP: {MaxExp}, ����EXP: {CurrentExp}");
    }

    [ContextMenu("LevelUp")]
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