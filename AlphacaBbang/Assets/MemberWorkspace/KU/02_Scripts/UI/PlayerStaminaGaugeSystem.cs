using UnityEngine;
using UnityEngine.UI;

public class PlayerStaminaGaugeSystem : MonoBehaviour
{
    private PlayerController _controller;

    private bool _isAiming;
    private bool _canAim = true;
    public bool CanAim => _canAim;

    [SerializeField] private Image _gaugeImage;
    [SerializeField] private Image _parentGaugeImage;

    public float CurrentGauge { get; private set; }

    [SerializeField] private float _gaugeMaxTime = 10f;
    [SerializeField] private float _minAimCooldown = 2f;
    [SerializeField] private float _useSpeed = 1f;
    [SerializeField] private float _chargeSpeed = 1f;
    [SerializeField] private float _aimReleaseCooldown = 1.5f;
    private bool _prevAiming;

    private float _cooldownTimer = 0f;

    private Color _firstColor;
    private Color _parentFirstColor;
    private readonly Color _zeroColor = new Color(0, 0, 0, 0);


    private void Awake()
    {
        _controller = GetComponentInParent<PlayerController>();

        CurrentGauge = _gaugeMaxTime;

        _firstColor = _gaugeImage.color;
        _parentFirstColor = _parentGaugeImage.color;
    }

    private void Update()
    {
        SyncAimState();
        UpdateGauge();
        UpdateCooldown();
        UpdateUI();
    }

    private void SyncAimState()
    {
        if (_controller == null)
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

        CurrentGauge = Mathf.Clamp(CurrentGauge, 0, _gaugeMaxTime);
    }

    private void UpdateCooldown()
    {
        if (_canAim)
            return;

        _cooldownTimer -= Time.deltaTime;

        if (_cooldownTimer <= 0f)
        {
            _canAim = true;
        }
    }

    private void UpdateUI()
    {
        float fill = CurrentGauge / _gaugeMaxTime;

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