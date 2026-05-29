using JJH._02_Scripts_Systems.EventSystems;
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerStaminaGaugeSystem : MonoSingleton<PlayerStaminaGaugeSystem>
{
    private PlayerController _controller;

    private bool _isAiming;
    private bool _canAim = true;
    public bool CanAim => _canAim;

    [SerializeField] private Image _gaugeImage;
    [SerializeField] private Image _parentGaugeImage;
    public float GaugeMaxTime => _saveData.MaxAimStamina;
    public float CurrentGauge { get; private set; }
    private PlayerSaveData _saveData;


    [SerializeField] private float _minAimCooldown = 2f;
    [SerializeField] private float _useSpeed = 1f;
    [SerializeField] private float _chargeSpeed = 1f;
    [SerializeField] private float _aimReleaseCooldown = 1.5f;

    [field: SerializeField] public EventChannelSO playerStatChannel;

    public event Action<float> OnAimStaminaChanged;


    private bool _prevAiming;

    private float _cooldownTimer = 0f;

    private Color _firstColor;
    private Color _parentFirstColor;
    private readonly Color _zeroColor = new Color(0, 0, 0, 0);


    protected override void Awake()
    {
        _controller = GetComponentInParent<PlayerController>();
        _saveData = GetComponentInParent<PlayerSaveData>();

        CurrentGauge = GaugeMaxTime;

        _firstColor = _gaugeImage.color;
        _parentFirstColor = _parentGaugeImage.color;

        OnAimStaminaChanged += UpdateUI;

        Debug.Log(GaugeMaxTime);
    }



    private void Update()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
            playerStatChannel.RaiseEvent(PlayerStateEvents.AddMaxAimStamina.Init(5));
        SyncAimState();
        UpdateGauge();
        UpdateCooldown();
        UpdateUI(GaugeMaxTime);

    }

    private void SyncAimState()
    {
        if (!_controller.IsCanMoving)
            return;

        bool currentAiming = _controller.IsAiming;

        if (_prevAiming && !currentAiming)
        {
            _canAim = false;
            _cooldownTimer = _aimReleaseCooldown;
        }

        _isAiming = currentAiming;
        _prevAiming = currentAiming;
    }

    private void UpdateGauge()
    {
        if (!_controller.IsCanMoving)
            return;
        if (_isAiming)
        {
            CurrentGauge -= _useSpeed * Time.deltaTime;

            if (CurrentGauge <= 0f)
            {
                CurrentGauge = 0f;

                _canAim = false;
                _cooldownTimer = _minAimCooldown;

                if (_controller != null)
                    _controller.ForceStopAim();
            }
        }
        else
        {
            CurrentGauge += _chargeSpeed * Time.deltaTime;
        }

        CurrentGauge = Mathf.Clamp(CurrentGauge, 0, GaugeMaxTime);
    }

    private void UpdateCooldown()
    {
        if (!_controller.IsCanMoving)
            return;
        if (_canAim)
            return;

        _cooldownTimer -= Time.deltaTime;

        if (_cooldownTimer <= 0f)
        {
            _canAim = true;
        }
    }

    private void UpdateUI(float value)
    {
        if (!_controller.IsCanMoving)
            return;
        float fill = GaugeMaxTime <= 0f
            ? 0f
            : CurrentGauge / GaugeMaxTime;

        _gaugeImage.fillAmount = Mathf.Lerp(
            _gaugeImage.fillAmount,
            fill,
            10f * Time.deltaTime);

        if (fill >= 0.999f)
        {
            _gaugeImage.color = _zeroColor;
            _parentGaugeImage.color = _zeroColor;
        }
        else if (!_canAim)
        {
            _gaugeImage.color = Color.red;
            _parentGaugeImage.color = _parentFirstColor;
        }
        else
        {
            _gaugeImage.color = _firstColor;
            _parentGaugeImage.color = _parentFirstColor;
        }
    }
}